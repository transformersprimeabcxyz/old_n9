using System;
using System.Collections.Generic;

namespace n9.core
{
    public class Parser
    {
        // =====================================================================
        //  Parser state
        // =====================================================================

        Lexer lexer;
        List<Token> readTokens = new List<Token>();

        Dictionary<TokenType, PrefixParselet> prefixParselets = new Dictionary<TokenType, PrefixParselet>();
        Dictionary<TokenType, InfixParselet> infixParselets = new Dictionary<TokenType, InfixParselet>();
        
        // =====================================================================
        //  Constructor 
        // =====================================================================

        public static Parser FromFile(string filename)
        {
            var reader = FileReader.FromFile(filename);
            var lx = new Lexer(reader);
            return new Parser { lexer = lx };
        }

        public static Parser FromString(string pgm, string filename = "default.n9")
        {
            var reader = FileReader.FromString(pgm, filename);
            var lx = new Lexer(reader);
            return new Parser { lexer = lx };
        }

        Parser() // http://javascript.crockford.com/tdop/tdop.html
        {
            // Prefix operators
            Register(TokenType.Id, new NameParselet(), 0);
            Register(TokenType.IntLiteral, new IntLiteralParselet(), 0);
            Register(TokenType.FloatLiteral, new FloatLiteralParselet(), 0);
            Register(TokenType.StringLiteral, new StringLiteralParselet(), 0);
            Register(TokenType.Minus, new PrefixMinusParselet(), 0);
            Register(TokenType.LParen, new PrefixParenParselet(), 0);

            // Infix operators
            Register(TokenType.LogicalAnd, new BinaryOperationParselet(), 30); // TODO AND and OR should have different precedence. I think.
            Register(TokenType.LogicalOr, new BinaryOperationParselet(), 30);

            Register(TokenType.Equality, new BinaryOperationParselet(), 40);
            Register(TokenType.Inequality, new BinaryOperationParselet(), 40);
            Register(TokenType.LessThan, new BinaryOperationParselet(), 40);
            Register(TokenType.LessThanEqual, new BinaryOperationParselet(), 40);
            Register(TokenType.GreaterThan, new BinaryOperationParselet(), 40);
            Register(TokenType.GreaterThanEqual, new BinaryOperationParselet(), 40);
            
            Register(TokenType.Equals, new AssignParselet(), 50);
            Register(TokenType.Plus, new BinaryOperationParselet(), 50);
            Register(TokenType.Minus, new BinaryOperationParselet(), 50);
            Register(TokenType.Asterisk, new BinaryOperationParselet(), 60);
            Register(TokenType.Divslash, new BinaryOperationParselet(), 60);

            Register(TokenType.LParen, new CallParselet(), 80);
        }

        void Register(TokenType token, PrefixParselet parselet, int precedence)
        {
            parselet.Precedence = precedence;
            prefixParselets[token] = parselet;
        }

        void Register(TokenType token, InfixParselet parselet, int precedence)
        {
            parselet.Precedence = precedence;
            infixParselets[token] = parselet;
        }

        // =====================================================================
        //  Tokenstream stuff
        // =====================================================================

        public Token LookAhead(int distance)
        {
            while (distance >= readTokens.Count)
                readTokens.Add(lexer.Next());

            return readTokens[distance];
        }

        public bool Match(TokenType expected)
        {
            var token = LookAhead(0);
            if (token.Type != expected)
                return false;
            Consume();
            return true;
        }

        public Token Consume()
        {
            var token = LookAhead(0);
            readTokens.RemoveAt(0);
            return token;
        }

        public Token Consume(TokenType expected)
        {
            var token = LookAhead(0);
            if (token.Type != expected)
                throw new Exception("expected " + expected + " and found " + token.Type);
            readTokens.RemoveAt(0);
            return token;
        }

        // =====================================================================
        //  Expression Parsing
        // =====================================================================

        public Expression ParseExpression(int precedence = 0)
        {
            var token = Consume();
            var firstOp = prefixParselets[token.Type];

            var leftExpr = firstOp.Parse(this, token);
            while (precedence < NextPrecedence())
            {
                var nextToken = Consume();
                var nextOp = infixParselets[nextToken.Type];
                leftExpr = nextOp.Parse(this, leftExpr, nextToken);
            }

            return leftExpr;
        }

        int NextPrecedence()
        {
            var type = LookAhead(0).Type;
            if (infixParselets.ContainsKey(type) == false)
                return 0;
            return infixParselets[LookAhead(0).Type].Precedence;
        }

        // =====================================================================
        //  Statement Parsing
        // =====================================================================

        // TODO: differentiate toplevel statements from within-code statements. (?)
        public Statement ParseStatement(bool toplevel = false)
        {
            Token first = LookAhead(0);
            switch (first.Type)
            {
                case TokenType.Id:
                {
                    Token second = LookAhead(1);
                    switch (second.Type)
                    {
                        case TokenType.Colon:
                        {
                            var decl = ParseVariableDeclaration();
                            Consume(TokenType.Semi);
                            return decl;
                        }
                    }

                    break;
                }
                case TokenType.Struct:
                    return ParseStructDeclaration();
                case TokenType.Func:
                    return ParseFuncDeclaration();
                case TokenType.Return:
                    return ParseReturnStatement();
                case TokenType.Defer:
                    return ParseDeferStatement();
                case TokenType.While:
                    return ParseWhileStatement();
                case TokenType.If:
                    return ParseIfStatement();
                case TokenType.Pragma:
                    return ParsePragmaStatement();
                case TokenType.Version:
                    return ParseVersionStatement();
                case TokenType.EOF:
                    return null;
            }

            // If we are still here, lets try parsing an expression.
            var exp = ParseExpression();
            if (exp is AssignExpr)
                return ParseAssignStatement(exp as AssignExpr);
            if (exp is CallExpr)
                return ParseCallStatement(exp as CallExpr);

            // throw parse error
            return null;
        }

        TypeDeclaration ParseTypeDeclaration()
        {
            var decl = new TypeDeclaration();
            decl.Name = Consume(TokenType.Id).Text;
            
            // Check if its an array
            if (Match(TokenType.LBracket))
            {
                if (Match(TokenType.RBracket))
                    decl.UnsizedArray = true;
                else
                {
                    var size = Consume(TokenType.IntLiteral);
                    decl.SizedArray = true;
                    decl.ArraySize = (int) size.IntegerLiteral;
                    Consume(TokenType.RBracket);
                }
            }
            if (Match(TokenType.Asterisk))
                decl.Pointer = true;
            return decl;
        }

        VariableDeclaration ParseVariableDeclaration()
        {
            var name = Consume(TokenType.Id);
            Consume(TokenType.Colon);

            var decl = new VariableDeclaration();
            decl.Name = name.Text;

            // Now we either have to parse a type, or the type may be inferred.
            
            if (Match(TokenType.Equals)) 
            {
                // Invoked type-inference syntax

                decl.Type = TypeDeclaration.Auto;
                decl.InitializationExpression = ParseExpression();
            } else {
                // type-specified syntax. May or may not have an initializer.

                decl.Type = ParseTypeDeclaration();
                if (Match(TokenType.Equals))
                    decl.InitializationExpression = ParseExpression();
            }
            return decl;
        }

        MethodParameterDeclaration ParseMethodParameterDeclaration()
        {
            var name = Consume(TokenType.Id);
            var decl = new MethodParameterDeclaration { Name = name.Text };
            Consume(TokenType.Colon);
            decl.Type = ParseTypeDeclaration();
            return decl;
        }

        StructDeclaration ParseStructDeclaration()
        {
            Consume(TokenType.Struct);
            var name = Consume(TokenType.Id);
            Consume(TokenType.LCurly);

            var decl = new StructDeclaration { Name = name.Text };

            while (!Match(TokenType.RCurly))
            {
                decl.Members.Add(ParseVariableDeclaration());
                Consume(TokenType.Semi);
            }
            return decl;
        }

        FuncDeclaration ParseFuncDeclaration()
        {
            Consume(TokenType.Func);
            var name = Consume(TokenType.Id);
            Consume(TokenType.LParen);

            var decl = new FuncDeclaration { Name = name.Text };

            while (!Match(TokenType.RParen))
            {
                decl.Parameters.Add(ParseMethodParameterDeclaration());
                Match(TokenType.Comma);
            }

            if (Match(TokenType.Colon))
                decl.ReturnType = ParseTypeDeclaration();

            // TODO function prototypes, extern/ffi decls? export decls? visibility?

            Consume(TokenType.LCurly);

            while (!Match(TokenType.RCurly))
            {
                decl.Body.Add(ParseStatement());
            }

            return decl;
        }

        ReturnStatement ParseReturnStatement()
        {
            Consume(TokenType.Return);
            var stmt = new ReturnStatement { Expr = ParseExpression() };
            Consume(TokenType.Semi);
            return stmt;
        }

        DeferStatement ParseDeferStatement()
        {
            Consume(TokenType.Defer);
            var stmt = new DeferStatement();
            if (Match(TokenType.LCurly))
                while (!Match(TokenType.RCurly))
                    stmt.Body.Add(ParseStatement());
            else
                stmt.Body.Add(ParseStatement());
            return stmt;
        }

        AssignStatement ParseAssignStatement(AssignExpr expr)
        {
            Consume(TokenType.Semi);
            return new AssignStatement { AssignExpr = expr };
        }

        CallStatement ParseCallStatement(CallExpr expr)
        {
            Consume(TokenType.Semi);
            return new CallStatement { CallExpr = expr };
        }

        WhileStatement ParseWhileStatement()
        {
            Consume(TokenType.While);
            Consume(TokenType.LParen);
            var stmt = new WhileStatement { ConditionalExpr = ParseExpression() };
            Consume(TokenType.RParen);
            ParseStatementOrBlock(stmt.Body);
            return stmt;
        }

        IfStatement ParseIfStatement()
        {
            Consume(TokenType.If);
            Consume(TokenType.LParen);
            var stmt = new IfStatement { IfExpr = ParseExpression() };
            Consume(TokenType.RParen);
            ParseStatementOrBlock(stmt.ThenBody);
            if (Match(TokenType.Else))
                ParseStatementOrBlock(stmt.ElseBody);
            return stmt;
        }

        void ParseStatementOrBlock(List<Statement> list)
        {
            if (Match(TokenType.Semi))
                return; // just terminates in the case of: "while (condition);" or similar

            if (Match(TokenType.LCurly))
                while (!Match(TokenType.RCurly))
                    list.Add(ParseStatement());
            else
                list.Add(ParseStatement());
        }

        PragmaStatement ParsePragmaStatement()
        {
            Consume(TokenType.Pragma);
            var stmt = new PragmaStatement();
            while (!Match(TokenType.Semi))
                stmt.Body.Add(Consume());
            return stmt;
        }

        VersionStatement ParseVersionStatement()
        {
            Consume(TokenType.Version);
            Consume(TokenType.LParen);
            var stmt = new VersionStatement { ConditionalExpr = ParseExpression() };
            Consume(TokenType.RParen);
            ParseStatementOrBlock(stmt.Body);
            return stmt;
        }
    }
}