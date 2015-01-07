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
            return new IntLiteralExpr { Literal = t };
        }
    }

    // =====================================================

    public class FloatLiteralParselet : PrefixParselet
    {
        public override Expression Parse(Parser p, Token t)
        {
            return new FloatLiteralExpr { Literal = t };
        }
    }

    // =====================================================

    public class StringLiteralParselet : PrefixParselet
    {
        public override Expression Parse(Parser p, Token t)
        {
            return new StringLiteralExpr { Literal = t };
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
                    return new IntLiteralExpr { Literal = negatedInt };

                case TokenType.FloatLiteral:
                    var negatedFloat = right.Clone();
                    negatedFloat.FloatLiteral = -negatedFloat.FloatLiteral;
                    return new FloatLiteralExpr { Literal = negatedFloat };

                default: 
                    throw new Exception("The compiler doesnt know how to unary minus this expression type yet");
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