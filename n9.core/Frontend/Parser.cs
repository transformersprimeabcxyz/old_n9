using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace n9.core
{
    public class Parser
    {
        // =====================================================================
        //  Parser state
        // =====================================================================

        Lexer lexer;
        List<Token> readTokens = new List<Token>();

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

        // =====================================================================
        //  Parser
        // =====================================================================
        
        public List<Statement> Parse()
        {
            var stmts = new List<Statement>();
            while (true)
            {
                var t = LookAhead(0);
                switch (t.Type)
                {
                    case TokenType.Struct:
                        stmts.Add(ParseStruct());
                        break;
                    case TokenType.EOF:
                        return stmts;
                    default: throw new CompilationException(Diagnostic.Error("Unhandled top-level statement type, TokenType: " + t.Type)); // TODO replace with Diagnostic error.
                }
            }
        }

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
        /*
        public Expression ParseExpression()
        {
            return ParseExpression(0);
        }

        public Expression ParseExpression(int precendece)
        {
            return null;
            var token = Consume();
            if (prefixParselets.ContainsKey(token.Type) == false)
                throw new Exception("Parse error for token " + token.Type);

            var prefixParselet = prefixParselets[token.Type];
            var left = prefixParselet.Parse(this, token);
        }*/

        // =====================================================================
        //  Parselets
        // =====================================================================

        StructDeclaration ParseStruct()
        {
            Consume(TokenType.Struct);
            var name = Consume(TokenType.Id);
            Consume(TokenType.LCurly);

            var structDecl = new StructDeclaration();
            structDecl.Name = name.Text;

            while (true)
            {
                var next = LookAhead(0);
                if (next.Type == TokenType.RCurly)
                {
                    Consume();
                    return structDecl;
                }
                var id = Consume(TokenType.Id);
                Consume(TokenType.Colon);
                
                // TODO, the "member declaration" part here should be genericed to other variable declaration parse code.
                
                // Now we either have to parse a type, or the type may be inferred.
                next = LookAhead(0);
                if (next.Type == TokenType.Equals)
                {
                    // Invoked type-inference syntax
                    Consume();
                    // at this point we would parse an expression. Inferring type will require resolution of functions that may be called....
                    // So we can't finalizing inference now.
                    // Right now, I dont even have an expression parser, so...
                    Consume(TokenType.Semi);
                    structDecl.Members.Add(new StructMember { MemberName = id.Text, Type = "auto" });
                    continue;
                }
                // We are in the explicitly-typed path.
                var type = Consume(TokenType.Id);
                structDecl.Members.Add(new StructMember { MemberName = id.Text, Type = type.Text });
                // TODO, pointers
                // TODO, arrays,
                // TODO, initializers
                // TODO should just generalize "parse a type"
                Consume(TokenType.Semi); // for now we just assume semi is here
                continue;
            }
        }
    }
}
