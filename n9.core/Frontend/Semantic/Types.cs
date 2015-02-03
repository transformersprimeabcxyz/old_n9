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
        public int Alignment;
    }

    public abstract class BuiltinType : N9Type 
    {
    }

    public class IntegerType : BuiltinType
    {
        public bool Signed;
        //public bool CanRepresent(Token literal)
        //{
        //    switch (Signed)
        //    {
        //        case true:
        //        {
        //            switch(Size)
        //            {
        //                case 1: { sbyte ignore; return sbyte.TryParse(literal.Text, out ignore); }
        //                case 2: { short ignore; return short.TryParse(literal.Text, out ignore); }
        //                case 4: { int   ignore; return int.  TryParse(literal.Text, out ignore); }
        //                case 8: { long  ignore; return long. TryParse(literal.Text, out ignore); }
        //            }
        //            break;
        //        }
        //        case false:
        //        {
        //            switch (Size)
        //            {
        //                case 1: { byte   ignore; return byte.  TryParse(literal.Text, out ignore); }
        //                case 2: { ushort ignore; return ushort.TryParse(literal.Text, out ignore); }
        //                case 4: { uint   ignore; return uint.  TryParse(literal.Text, out ignore); }
        //                case 8: { ulong  ignore; return ulong. TryParse(literal.Text, out ignore); }
        //            }
        //        }
        //        break;
        //    }    
        //    return true;
        //}

        public bool CanRepresent(decimal literal)
        {
            switch (Signed)
            {
                case true:
                {
                    switch(Size)
                    {
                        case 1: return !(literal < sbyte.MinValue || literal > sbyte.MaxValue);
                        case 2: return !(literal < short.MinValue || literal > short.MaxValue);
                        case 4: return !(literal < int  .MinValue || literal > int  .MaxValue);
                        case 8: return !(literal < long .MinValue || literal > long .MaxValue);
                    }
                    break;
                }
                case false:
                {
                    switch (Size)
                    {
                        case 1: return !(literal < byte.MinValue || literal > byte.MaxValue);
                        case 2: return !(literal < ushort.MinValue || literal > ushort.MaxValue);
                        case 4: return !(literal < uint.MinValue || literal > uint.MaxValue);
                        case 8: return !(literal < ulong.MinValue || literal > ulong.MaxValue);
                    }
                }
                break;
            }    
            return true;
        }



    }

    public static class BuiltinTypes
    {
        public static IntegerType i8  = new IntegerType { Module = "", Name = "i8",  Size = 1, Alignment = 1, Signed = true };
        public static IntegerType i16 = new IntegerType { Module = "", Name = "i16", Size = 2, Alignment = 2, Signed = true };
        public static IntegerType i32 = new IntegerType { Module = "", Name = "i32", Size = 4, Alignment = 4, Signed = true };
        public static IntegerType i64 = new IntegerType { Module = "", Name = "i64", Size = 8, Alignment = 8, Signed = true };

        public static IntegerType u8  = new IntegerType { Module = "", Name = "u8",  Size = 1, Alignment = 1, Signed = false };
        public static IntegerType u16 = new IntegerType { Module = "", Name = "u16", Size = 2, Alignment = 2, Signed = false };
        public static IntegerType u32 = new IntegerType { Module = "", Name = "u32", Size = 4, Alignment = 4, Signed = false };
        public static IntegerType u64 = new IntegerType { Module = "", Name = "u64", Size = 8, Alignment = 8, Signed = false };

        public static void RegisterBuiltins(Module module)
        {
            module.RegisterSymbol("i8",  i8);
            module.RegisterSymbol("i16", i16);
            module.RegisterSymbol("i32", i32);
            module.RegisterSymbol("i64", i64);

            module.RegisterSymbol("u8", u8);
            module.RegisterSymbol("u16", u16);
            module.RegisterSymbol("u32", u32);
            module.RegisterSymbol("u64", u64);
        }
    }

    public class StructType : N9Type
    {

    }
}