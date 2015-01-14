using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace n9.core
{
    public class CGen
    {
        // =====================================================================
        //  CGen state
        // =====================================================================

        Binder binder;

        // =====================================================================
        //  Constructor
        // =====================================================================

        public CGen(Binder b) // TODO refactoring related to data flow between steps.
        {
            binder = b;
        }

        // =====================================================================
        //  Code generator
        // =====================================================================

        public void Generate()
        {
            using (TextWriter w = File.CreateText("n9out.c"))
            {
                w.WriteLine("int main(int argc, char** argv)");
                w.WriteLine("{");
                w.WriteLine("}");
            }
        }
    }
}
