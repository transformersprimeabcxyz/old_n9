using System;

namespace n9.core
{
    public enum OperatorType
    {
        Prefix,
        Infix,
        Postfix
    }

    public class Operator
    {
        public string Name;
        public int Precedence;
        public OperatorType Type;
        public TokenType Token;
        public Func<PrattParser, Expression> PrefixParse;
        public Func<PrattParser, Expression, Expression> InfixParse;
    }
}