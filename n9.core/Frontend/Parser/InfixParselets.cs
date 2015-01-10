namespace n9.core
{
    abstract class InfixParselet
    {
        public int Precedence;
        public abstract Expression Parse(Parser p, Expression left, Token t);
    }

    // ======================================================

    class BinaryOperationParselet : InfixParselet
    {
        public override Expression Parse(Parser p, Expression left, Token t)
        {
            return new BinaryOperatorExpr
            {
                Op = t.Type,
                Left = left,
                Right = p.ParseExpression(Precedence)
            };
        }
    }

    // ======================================================

    class CallParselet : InfixParselet
    {
        public override Expression Parse(Parser p, Expression left, Token t)
        {
            // infix format is for call expressions.  "foo(args)"
            // TODO: verify that Left expression is an LValue / possible function call site. Maybe that's the binder's job.

            var exp = new CallExpr();
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

    // ======================================================

    class AssignParselet : InfixParselet
    {
        public override Expression Parse(Parser p, Expression left, Token t)
        {
            // Assignments are right-associative. a=b=c will be parsed as a=(b=c).
            // The -1 on "Precedence-1" achieves the right-associativity.
            // TODO: verify that Left expression is an LValue. Maybe that's the binder's job.
            return new AssignExpr
            {
               Left = left,
               Right = p.ParseExpression(Precedence - 1)
            };
        }
    }
}