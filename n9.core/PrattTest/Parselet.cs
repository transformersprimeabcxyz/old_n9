using System;
using System.Collections.Generic;
namespace n9.core
{
    public abstract class Parselet
    {
        // NOTE: I dont know if there would ever be a situation where a 
        // token would need different precedence as a prefix operator vs infix operator. In that case they would need to be separate objects, or separate fields.

        public int Precedence;
        public abstract Expression NullDenotation(PrattParser p);
        public abstract Expression LeftDenoation(PrattParser p, Expression left);
    }

    // =========================================================
    
    public class NameExpression : Expression
    {
        public string Name;
    }

    public class NameParselet : Parselet
    {
        public NameParselet()
        {
            Precedence = 0;
        }

        public override Expression NullDenotation(PrattParser p)
        {
            return new NameExpression { Name = p.Consume().Text };
        }

        public override Expression LeftDenoation(PrattParser p, Expression left)
        {
            return null; 
        }
    }

    // =========================================================

    public class IntLiteralExpression : Expression
    {
        public Token IntToken; // TODO maybe not keep this. or decide if we need to keep it.
        public long IntLiteral; // In any case storing both of these is wasteful. Although maybe its cache friendly?
    }

    public class IntLiteralParselet : Parselet
    {
        public IntLiteralParselet()
        {
            Precedence = 0;
        }

        public override Expression NullDenotation(PrattParser p)
        {
            var literal = p.Consume();
            return new IntLiteralExpression { IntToken = literal, IntLiteral = literal.IntegerLiteral };
        }

        public override Expression LeftDenoation(PrattParser p, Expression left)
        {
            return null;
        }
    }

    // =========================================================

    public class AddExpression : Expression
    {
        public Expression Left;
        public Expression Right;
    }

    public class AddParselet : Parselet
    {
        public AddParselet()
        {
            Precedence = 50;
        }

        public override Expression NullDenotation(PrattParser p)
        {
            // TODO token grabber
            return null;
        }

        public override Expression LeftDenoation(PrattParser p, Expression left)
        {
            p.Consume();
            var e = new AddExpression 
            { 
                Left = left,
                Right = p.ParseExpression(Precedence)
            };
            return e;
        }
    }

    // =========================================================

    public class SubExpression : Expression
    {
        public Expression Left;
        public Expression Right;
    }

    public class SubParselet : Parselet
    {
        public SubParselet()
        {
            Precedence = 50;
        }

        public override Expression NullDenotation(PrattParser p)
        {
            p.Consume();
            var t = p.Consume();
            switch (t.Type)
            {
                case TokenType.IntLiteral:
                    return new IntLiteralExpression { IntLiteral = -t.IntegerLiteral, IntToken = t };
            }
            throw new Exception("internal compile error");
            return null;
        }

        public override Expression LeftDenoation(PrattParser p, Expression left)
        {
            p.Consume();
            var e = new SubExpression
            {
                Left = left,
                Right = p.ParseExpression(Precedence)
            };
            return e;
        }
    }


    // =========================================================

    public class CallExpression : Expression
    {
        public Expression Function;
        public List<Expression> Args = new List<Expression>();
    }

    public class ParenPareslet : Parselet
    {
        public ParenPareslet()
        {
            Precedence = 80;
        }

        public override Expression NullDenotation(PrattParser p)
        {
            // nud format is for grouping of expressions, such as "a + (b * c)"
            p.Consume();
            var exp = p.ParseExpression();
            p.Consume(TokenType.RParen);
            return exp; 
        }

        public override Expression LeftDenoation(PrattParser p, Expression left)
        {
            // led format is for call expressions.  "foo(args)"
            // TODO: verify that Left expression is an LValue / possible function call site. Maybe that's the binder's job.

            p.Consume();
            var exp = new CallExpression();
            exp.Function = left;

            if (p.Match(TokenType.RParen))
                return exp;

            do
            {
                exp.Args.Add(p.ParseExpression());
            } while (p.Match(TokenType.Comma));
            p.Consume(TokenType.RParen);

            return exp;
        }
    }

}
