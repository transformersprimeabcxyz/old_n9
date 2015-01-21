using System;
using System.Collections.Generic;

namespace n9.core
{
    public static class N9Util
    {
        public static List<Token> Tokenize(string pgm)
        {
            var tokens = new List<Token>();
            var fr = FileReader.FromString(pgm);
            var lx = new Lexer(fr);
            while (true)
            {
                var t = lx.Next();
                if (t.Type == TokenType.EOF)
                    return tokens;
                tokens.Add(t);
            }
        }

        public static Statement ParseStatement(string pgm)
        {
            var p = Parser.FromString(pgm);
            return p.ParseStatement();
        }

        public static Expression ParseExpression(string pgm)
        {
            var p = Parser.FromString(pgm);
            return p.ParseExpression();
        }

        public static Parser GetParser(string pgm)
        {
            return Parser.FromString(pgm);
        }

        public static void AssertException(Action e)
        {
            bool failed = false;
            try
            {
                e();
            }
            catch
            {
                failed = true;
            }
            if (!failed) throw new Exception("Erroneously succeeded!");
        }
    }
}
