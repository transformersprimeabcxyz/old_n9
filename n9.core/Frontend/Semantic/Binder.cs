using System;
using System.Collections.Generic;

namespace n9.core
{
    public class Binder
    {
        // =====================================================================

        N9Context ctx;
        ProgramModel model;

        Scope scope;

        // =====================================================================
        
        // In the interest of continuing to make progress and not brain-blocking ourselves, we are ignoring namespaces (not scopes) totally.
        // Everything in the root namespace. We will deal with modules and imports at a future date.

        // TODO modules/imports
        // TODO.... error handling!

        public Binder(N9Context ctx, ProgramModel model)
        {
            this.ctx = ctx;
            this.model = model;
        }

        public void Bind()
        {
            BindStructs();
            BindVariables();

        }

        void BindStructs()
        {
        }

        void BindVariables()
        {
            foreach (var v in model.GlobalVariables)
            {
                v.Type.N9Type = model.Root.Get(v.Type.Name) as N9Type;
                if (v.Type.N9Type == null)
                    throw new Exception("unable to bind type " + v.Type.Name);

                if (v.Type.N9Type is IntegerType && v.InitializationExpression != null)
                    AttemptToValidateIntegerInitializerExpression(v);
            }
        }

        void AttemptToValidateIntegerInitializerExpression(VariableDeclaration decl)
        {
            var val = AttemptResolveIntegerLiteralExpression(decl.InitializationExpression);
            if (val == null)
                return; // Can't easily figure out that this is a simple number so abandon attempt to range-check it

            bool ok = (decl.Type.N9Type as IntegerType).CanRepresent(val.Value);
            if (!ok) 
                throw new Exception(string.Format("The literal {0} will not fit inside integer type {1}",val.Value, decl.Type.Name));
            // TODO maybe this should be a warning instead of an error, this is a low-level language after all
        }

        public static decimal? AttemptResolveIntegerLiteralExpression(Expression expr)
        {
            if (expr is IntLiteralExpr)
                return (expr as IntLiteralExpr).Literal.IntegerLiteral;

            if (expr is BinaryOperatorExpr)
            {
                var e = expr as BinaryOperatorExpr;
                decimal? left = AttemptResolveIntegerLiteralExpression(e.Left);
                decimal? right = AttemptResolveIntegerLiteralExpression(e.Right);
                if (left == null || right == null)
                    return null;
                if (e.Op == TokenType.Plus)
                    return left + right;
                if (e.Op == TokenType.Minus)
                    return left - right;
                if (e.Op == TokenType.Asterisk)
                    return left * right;
                if (e.Op == TokenType.Divslash)
                    return left / right;
            }

            return null;
        }

        void Bind(SourceFile src)
        {
        }
    }
}
