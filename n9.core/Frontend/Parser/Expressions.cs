using System;
using System.Collections.Generic;
using System.Text;

namespace n9.core
{
    public abstract class Expression
    {
    }

    // =====================================================

    public class NameExpr : Expression
    {
        public string Name;

        public override string ToString()
        {
            return Name;
        }
    }

    // =====================================================

    public class IntLiteralExpr : Expression
    {
        public Token Literal;

        public override string ToString()
        {
            return Literal.IntegerLiteral.ToString();
        }
    }

    // =====================================================

    public class FloatLiteralExpr : Expression
    {
        public Token Literal;

        public override string ToString()
        {
            var suffix = "";
            switch (Literal.NumberLiteralClass)
            {
                case NumberLiteralClass.Float32: suffix = "f"; break;
                case NumberLiteralClass.Float64: suffix = "d"; break;
                case NumberLiteralClass.Decimal: suffix = "m"; break;
            }
            return Literal.FloatLiteral.ToString() + suffix;
        }
    }

    // =====================================================

    public class StringLiteralExpr : Expression
    {
        public Token Literal;

        public override string ToString()
        {
            return "\""+Literal.StringLiteral+"\"";
        }
    }

    // =====================================================

    public class BinaryOperatorExpr : Expression
    {
        public TokenType Op; // TODO we may want to map this to a more specialized enum, but we work with this for now.
        public Expression Left;
        public Expression Right;

        public override string ToString()
        {
            return "(" + Left + " " + Op + " " + Right + ")";
        }
    }

    // =====================================================

    public class CallExpr : Expression
    {
        public Expression Function;
        public List<Expression> Args = new List<Expression>();

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Function);
            sb.Append("(");
            bool first = true;
            foreach (var arg in Args)
            {
                if (!first)
                    sb.Append(",");
                sb.Append(arg);
                first = false;
            }
            sb.Append(")");
            return sb.ToString();
        }
    }

    // =====================================================

    public class AssignExpr : Expression
    {
        public Expression Left;
        public Expression Right;

        public override string ToString()
        {
            return Left + " = " + Right;
        }
    }
}