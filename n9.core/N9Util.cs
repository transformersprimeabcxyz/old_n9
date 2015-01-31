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

        public static Expression ParseExpression(string pgm)
        {
            var p = Parser.FromString(pgm);
            return p.ParseExpression();
        }

        public static Statement ParseStatement(string pgm)
        {
            var p = Parser.FromString(pgm);
            return p.ParseStatement();
        }

        public static Parser GetParser(string pgm)
        {
            return Parser.FromString(pgm);
        }

        public static UnboundModel GetUnboundModel(string pgm)
        {
            var ctx = N9Context.FromString(pgm).Construct();
            return UnboundModel.Generate(ctx);
        }

        public static ProgramModel GetModel(string pgm)
        {
            var ctx = N9Context.FromString(pgm).Construct();
            var unbound = UnboundModel.Generate(ctx);
            return ProgramModel.Bind(ctx, unbound);
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