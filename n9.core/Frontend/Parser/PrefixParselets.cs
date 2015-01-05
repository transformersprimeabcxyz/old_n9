using System;

namespace n9.core
{
    public abstract class PrefixParselet
    {
        public int Precedence;
        public abstract Expression Parse(Parser p, Token t);
    }

    // =====================================================

    public class NameParselet : PrefixParselet
    {
        public override Expression Parse(Parser p, Token t)
        {
            return new NameExpr { Name = t.Text };
        }
    }

    // =====================================================

    public class IntLiteralParselet : PrefixParselet
    {
        public override Expression Parse(Parser p, Token t)
        {
            return new IntLiteralExpr { IntLiteral = t };
        }
    }

    // =====================================================

    public class FloatLiteralParselet : PrefixParselet
    {
        public override Expression Parse(Parser p, Token t)
        {
            return new FloatLiteralExpr { FloatLiteral = t };
        }
    }
        
    // =====================================================

    public class PrefixMinusParselet : PrefixParselet
    {
        public override Expression Parse(Parser p, Token t)
        {
            var right = p.Consume();
            switch (right.Type)
            {
                case TokenType.IntLiteral:
                    var negatedInt = right.Clone();
                    negatedInt.IntegerLiteral = -negatedInt.IntegerLiteral;
                    return new IntLiteralExpr { IntLiteral = negatedInt };

                case TokenType.FloatLiteral:
                    var negatedFloat = right.Clone();
                    negatedFloat.FloatLiteral = -negatedFloat.FloatLiteral;
                    return new FloatLiteralExpr { FloatLiteral = negatedFloat };

                default: 
                    throw new Exception("The compiler doesnt know how to unary - this expression type yet");
            }
        }
    }

    // =====================================================

    public class PrefixParenParselet : PrefixParselet
    {
        public override Expression Parse(Parser p, Token t)
        {
            // Prefix format for grouping of expressions, such as "a + (b * c)"
            var exp = p.ParseExpression();
            p.Consume(TokenType.RParen);
            return exp;
        }
    }
}