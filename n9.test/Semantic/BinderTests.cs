using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using n9.core;

namespace n9.test
{
	[TestClass]
	public class BinderTests
	{
        [TestMethod]
        public void IntegerTypes_Representable()
        {
            Assert.IsTrue(BuiltinTypes.i8.CanRepresent(AsDecimal("0")));
            Assert.IsTrue(BuiltinTypes.i8.CanRepresent(AsDecimal("-128")));
            Assert.IsTrue(BuiltinTypes.i8.CanRepresent(AsDecimal("127")));
            Assert.IsTrue(BuiltinTypes.i8.CanRepresent(AsDecimal("-129")) == false);
            Assert.IsTrue(BuiltinTypes.i8.CanRepresent(AsDecimal("128")) == false);

            Assert.IsTrue(BuiltinTypes.u8.CanRepresent(AsDecimal("0")));
            Assert.IsTrue(BuiltinTypes.u8.CanRepresent(AsDecimal("255")));
            Assert.IsTrue(BuiltinTypes.u8.CanRepresent(AsDecimal("-1")) == false);
            Assert.IsTrue(BuiltinTypes.u8.CanRepresent(AsDecimal("256")) == false);

            Assert.IsTrue(BuiltinTypes.i16.CanRepresent(AsDecimal("-32768")));
            Assert.IsTrue(BuiltinTypes.i16.CanRepresent(AsDecimal("32767")));
            Assert.IsTrue(BuiltinTypes.i16.CanRepresent(AsDecimal("-32769")) == false);
            Assert.IsTrue(BuiltinTypes.i16.CanRepresent(AsDecimal("32768")) == false);

            Assert.IsTrue(BuiltinTypes.u16.CanRepresent(AsDecimal("0")));
            Assert.IsTrue(BuiltinTypes.u16.CanRepresent(AsDecimal("65535")));
            Assert.IsTrue(BuiltinTypes.u16.CanRepresent(AsDecimal("-1")) == false);
            Assert.IsTrue(BuiltinTypes.u16.CanRepresent(AsDecimal("65536")) == false);

            Assert.IsTrue(BuiltinTypes.i32.CanRepresent(AsDecimal("-2147483648")));
            Assert.IsTrue(BuiltinTypes.i32.CanRepresent(AsDecimal("2147483647")));
            Assert.IsTrue(BuiltinTypes.i32.CanRepresent(AsDecimal("-2147483649")) == false);
            Assert.IsTrue(BuiltinTypes.i32.CanRepresent(AsDecimal("2147483648")) == false);

            Assert.IsTrue(BuiltinTypes.u32.CanRepresent(AsDecimal("0")));
            Assert.IsTrue(BuiltinTypes.u32.CanRepresent(AsDecimal("4294967295")));
            Assert.IsTrue(BuiltinTypes.u32.CanRepresent(AsDecimal("-1")) == false);
            Assert.IsTrue(BuiltinTypes.u32.CanRepresent(AsDecimal("4294967296")) == false);

            Assert.IsTrue(BuiltinTypes.i64.CanRepresent(AsDecimal("-9223372036854775808")));
            Assert.IsTrue(BuiltinTypes.i64.CanRepresent(AsDecimal("9223372036854775807")));
            Assert.IsTrue(BuiltinTypes.i64.CanRepresent(AsDecimal("-9223372036854775809")) == false);
            Assert.IsTrue(BuiltinTypes.i64.CanRepresent(AsDecimal("9223372036854775808")) == false);

            var t = N9Util.Tokenize("0xFF")[0];
            Assert.IsTrue(BuiltinTypes.u8.CanRepresent(t.IntegerLiteral));

            t = N9Util.Tokenize("0x100")[0];
            Assert.IsTrue(BuiltinTypes.u8.CanRepresent(t.IntegerLiteral) == false);

            t = N9Util.Tokenize("0b11111111")[0];
            Assert.IsTrue(BuiltinTypes.u8.CanRepresent(t.IntegerLiteral));

            t = N9Util.Tokenize("0b100000000")[0];
            Assert.IsTrue(BuiltinTypes.u8.CanRepresent(t.IntegerLiteral) == false);
        }

        [TestMethod]
        public void Binder_IntLiteralExprs()
        {
            var v = Binder.AttemptResolveIntegerLiteralExpression(N9Util.ParseExpression("0"));
            Assert.IsTrue(v == 0);

            v = Binder.AttemptResolveIntegerLiteralExpression(N9Util.ParseExpression("-1"));
            Assert.IsTrue(v == -1);

            v = Binder.AttemptResolveIntegerLiteralExpression(N9Util.ParseExpression("999999999999"));
            Assert.IsTrue(v == 999999999999);

            v = Binder.AttemptResolveIntegerLiteralExpression(N9Util.ParseExpression("100+1"));
            Assert.IsTrue(v == 101);

            v = Binder.AttemptResolveIntegerLiteralExpression(N9Util.ParseExpression("100+(2*5)"));
            Assert.IsTrue(v == 110);

            v = Binder.AttemptResolveIntegerLiteralExpression(N9Util.ParseExpression("100*(2-4)"));
            Assert.IsTrue(v == -200);

            v = Binder.AttemptResolveIntegerLiteralExpression(N9Util.ParseExpression("100+a"));
            Assert.IsTrue(v == null);

            v = Binder.AttemptResolveIntegerLiteralExpression(N9Util.ParseExpression("100+a(1)"));
            Assert.IsTrue(v == null);
        }

        static decimal AsDecimal(string i)
        {
            return decimal.Parse(i);
        }
 	}
}