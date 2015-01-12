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
            var binder = Binder.FromString(@"func a() { }"); binder.Bind();
            Assert.IsTrue(binder.Funcs.Count == 1);
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