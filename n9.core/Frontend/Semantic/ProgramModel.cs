using System;

namespace n9.core
{
    public class ProgramModel
    {
        // ===========================================================================

        N9Context ctx;
        Module UnboundRoot;
        
        // ===========================================================================

        public static ProgramModel Bind(N9Context _ctx, UnboundProgramModel unbound)
        {
            var model = new ProgramModel { UnboundRoot = unbound.PgmRoot, ctx = _ctx };

            model.Analyze();

            return model;
        }

        public void Analyze()
        {
            UnboundRoot.Visit<VariableDeclaration>(VariableVisitor);
        }

        void VariableVisitor(string module, string name, VariableDeclaration decl)
        {
            Console.WriteLine("VARIABLE {0}.{1} = {2}", module, name, decl);
        }
    }
}