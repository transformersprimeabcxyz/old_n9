using Microsoft.VisualStudio.TestTools.UnitTesting;
using n9.core;

namespace n9.test
{
	[TestClass]
	public class ParserTests
	{
		[TestMethod]
		public void Parser_Test()
		{
            var stmts = Parser.FromString("struct foo { }").Parse();
            Assert.IsTrue(stmts.Count == 1);
            Assert.IsTrue(stmts[0] is StructDeclaration);
            var sd = (StructDeclaration) stmts[0];
            Assert.IsTrue(sd.Name == "foo");
            Assert.IsTrue(sd.Members.Count == 0);

		}
	}
}

