using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace n9.core
{
    public abstract class N9Type
    {
        public string Module;
        public string Name;
        public int Size;
    }

    public abstract class BuiltinType : N9Type 
    {
    }

    public class IntegerType : BuiltinType
    {
        public bool Signed;
        public bool CanRepresent(Token literal)
        {
            return true; // TODO
        }
    }

    public class StructType : N9Type
    {

    }

    public static class BuiltinTypes
    {
        public static IntegerType i32 = new IntegerType 
        { 
            Module = "", 
            Name = "int", 
            Size = 4, 
            Signed = true 
            // TODO canrepresent...
        };

        public static void RegisterBuiltins(ProgramModel model)
        {
            model.Root.RegisterSymbol("int", i32);
        }

    }
}