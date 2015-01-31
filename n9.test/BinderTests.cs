using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using n9.core;

namespace n9.test
{
	[TestClass]
	public class BinderTests
	{
        [TestMethod]
        public void Binder_Basic()
        {
            var binder = oldBinder.FromString(@"func a() { }"); binder.Bind();
            Assert.IsTrue(binder.Funcs.Count == 1);
        }

        [TestMethod]
        public void IntegerTypes_Representable()
        {
            Assert.IsTrue(BuiltinTypes.i8.CanRepresent(IntToken("0")));
            Assert.IsTrue(BuiltinTypes.i8.CanRepresent(IntToken("-128")));
            Assert.IsTrue(BuiltinTypes.i8.CanRepresent(IntToken("127")));
            Assert.IsFalse(BuiltinTypes.i8.CanRepresent(IntToken("-129")));
            Assert.IsFalse(BuiltinTypes.i8.CanRepresent(IntToken("128")));

            Assert.IsTrue(BuiltinTypes.u8.CanRepresent(IntToken("0")));
            Assert.IsTrue(BuiltinTypes.u8.CanRepresent(IntToken("255")));
            Assert.IsFalse(BuiltinTypes.u8.CanRepresent(IntToken("-1")));
            Assert.IsFalse(BuiltinTypes.u8.CanRepresent(IntToken("256")));

            Assert.IsTrue(BuiltinTypes.i16.CanRepresent(IntToken("-32768")));
            Assert.IsTrue(BuiltinTypes.i16.CanRepresent(IntToken("32767")));
            Assert.IsFalse(BuiltinTypes.i16.CanRepresent(IntToken("-32769")));
            Assert.IsFalse(BuiltinTypes.i16.CanRepresent(IntToken("32768")));

            Assert.IsTrue(BuiltinTypes.u16.CanRepresent(IntToken("0")));
            Assert.IsTrue(BuiltinTypes.u16.CanRepresent(IntToken("65535")));
            Assert.IsFalse(BuiltinTypes.u16.CanRepresent(IntToken("-1")));
            Assert.IsFalse(BuiltinTypes.u16.CanRepresent(IntToken("65536")));

            Assert.IsTrue(BuiltinTypes.i32.CanRepresent(IntToken("-2147483648")));
            Assert.IsTrue(BuiltinTypes.i32.CanRepresent(IntToken("2147483647")));
            Assert.IsFalse(BuiltinTypes.i32.CanRepresent(IntToken("-2147483649")));
            Assert.IsFalse(BuiltinTypes.i32.CanRepresent(IntToken("2147483648")));

            Assert.IsTrue(BuiltinTypes.u32.CanRepresent(IntToken("0")));
            Assert.IsTrue(BuiltinTypes.u32.CanRepresent(IntToken("4294967295")));
            Assert.IsFalse(BuiltinTypes.u32.CanRepresent(IntToken("-1")));
            Assert.IsFalse(BuiltinTypes.u32.CanRepresent(IntToken("4294967296")));

            Assert.IsTrue(BuiltinTypes.i64.CanRepresent(IntToken("-9223372036854775808")));
            Assert.IsTrue(BuiltinTypes.i64.CanRepresent(IntToken("9223372036854775807")));
            Assert.IsFalse(BuiltinTypes.i64.CanRepresent(IntToken("-9223372036854775809")));
            Assert.IsFalse(BuiltinTypes.i64.CanRepresent(IntToken("9223372036854775808")));

            // TODO !!!! fails! CanRepresent uses the .Text, and if you use hex, oct, binary literals, .Text contains
            // the original denotation and not the decimal denotation.
            // Soooooooooooooooooooo. Hmm. Decide how to resolve.
            //var t = N9Util.Tokenize("0xFF")[0];
            //Assert.IsTrue(BuiltinTypes.u8.CanRepresent(t));
        }

        Token IntToken(string i)
        {
            return new Token { Type = TokenType.IntLiteral, Text = i };
        }

        void AssertException(Action e)
        {
            bool failed = false;
            try
            {
                e();
            }
            catch
            {
                failed = true;
            }
            Assert.IsTrue(failed);
        }
	}
}