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
            }
        }

        void Bind(SourceFile src)
        {
        }
    }
}
