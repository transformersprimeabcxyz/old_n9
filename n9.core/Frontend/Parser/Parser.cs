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

        Parser()
        {
            // Prefix operators
            Register(TokenType.Id, new NameParselet(), 0);
            Register(TokenType.IntLiteral, new IntLiteralParselet(), 0);
            Register(TokenType.FloatLiteral, new FloatLiteralParselet(), 0);
            Register(TokenType.Minus, new PrefixMinusParselet(), 0);
            Register(TokenType.LParen, new PrefixParenParselet(), 0);

            // Infix operators
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
        //  Parsing
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
    }
}
