using System;
using System.Collections.Generic;

namespace n9.core
{

    public class PrattParser
    {
        // =====================================================================
        //  Parser state
        // =====================================================================

        Lexer lexer;
        List<Token> readTokens = new List<Token>();
        Dictionary<TokenType, Parselet> ops = new Dictionary<TokenType, Parselet>();
        

        // =====================================================================
        //  Constructor 
        // =====================================================================

        public static PrattParser FromFile(string filename)
        {
            var reader = FileReader.FromFile(filename);
            var lx = new Lexer(reader);
            return new PrattParser { lexer = lx };
        }

        public static PrattParser FromString(string pgm, string filename = "default.n9")
        {
            var reader = FileReader.FromString(pgm, filename);
            var lx = new Lexer(reader);
            return new PrattParser { lexer = lx };
        }

        PrattParser()
        {
            Register(TokenType.Id, new NameParselet());
            Register(TokenType.IntLiteral, new IntLiteralParselet());

            Register(TokenType.Plus, new AddParselet());
            Register(TokenType.Minus, new SubParselet());
            Register(TokenType.LParen, new ParenPareslet());
        }

        void Register(TokenType token, Parselet parselet)
        {
            ops[token] = parselet;
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
            var token = LookAhead(0);
            var firstOp = ops[token.Type];
            
            var leftExpr = firstOp.NullDenotation(this);
            while (precedence < NextPrecedence())
            {
                var nextToken = LookAhead(0);
                var nextOp = ops[nextToken.Type];
                leftExpr = nextOp.LeftDenoation(this, leftExpr);
            }

            return leftExpr;
        }

        int NextPrecedence()
        {
            var type = LookAhead(0).Type;
            Console.WriteLine("NextPrecedence: " + type);
            if (ops.ContainsKey(type)== false)
                return 0;

            return ops[LookAhead(0).Type].Precedence;
        }
    }
}
