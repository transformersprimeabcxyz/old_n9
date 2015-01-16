using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using n9.core;

namespace n9.test
{
	[TestClass]
	public class CodegenBasicTests
	{
        [TestMethod]
        public void CGen_Basic1()
        {
            Test("func n9main():int { return 42; }", 42);
            Test("func n9main():int { return 5*2-1; }", 9);
            Test("func n9main():int { return 5*(2-1); }", 5);

            Test("func n9main():int { return 0 == 0; }", 1);
            Test("func n9main():int { return 1 != 1; }", 0);

            Test("func n9main():int { i:int=5; return i*3; }", 15);
        }

        [TestMethod]
        public void CGen_Basic2()
        {
            Test(@"

                func returns_true() : int { return 1; }

                func n9main() : int { 
                    i : int = 3;
                    if (returns_true())
                        i = i * 3;
                    else
                        i = i * 2;
                    return i;
                }

            ", 9);
        }

        void Test(string pgm, int expectedResult)
        {
            var binder = Binder.FromString(pgm); 
            binder.Bind();
            var cgen = new CGen(binder);
            cgen.Generate();
            cgen.Compile();
            int res = cgen.Run();
            Assert.IsTrue(res == expectedResult);
        }
	}
}