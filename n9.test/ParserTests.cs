using Microsoft.VisualStudio.TestTools.UnitTesting;
using n9.core;

namespace n9.test
{
	[TestClass]
	public class ParserTests
	{
        [TestMethod]
        public void Parser_ExpressionsBasic()
        {
            ExprParseTest("2", "2");
            ExprParseTest("a", "a");

            ExprParseTest("5+2", "(5 Plus 2)");
            ExprParseTest("(5+2)", "(5 Plus 2)");
            ExprParseTest("2+5*3", "(2 Plus (5 Asterisk 3))");
            ExprParseTest("2*5+3", "((2 Asterisk 5) Plus 3)");

            ExprParseTest("2+5+3+7", "(((2 Plus 5) Plus 3) Plus 7)");
            ExprParseTest("2+(5+3)+7", "((2 Plus (5 Plus 3)) Plus 7)");

            ExprParseTest("foo()", "foo()");
            ExprParseTest("foo(bar)", "foo(bar)");
            ExprParseTest("foo(bar,1,1+2,(a/b))", "foo(bar,1,(1 Plus 2),(a Divslash b))");
        }

        void ExprParseTest(string source, string output)
        {
            var expr = Parser.FromString(source).ParseExpression().ToString();
            Assert.IsTrue(output == expr);
        }

        //[TestMethod]
        //public void Parser_VarDecl()
        //{
        //    var stmts = OldParser.FromString("i : int;").Parse();
        //    Assert.IsTrue(stmts.Count == 1);
        //    Assert.IsTrue(stmts[0] is VariableDeclaration);
        //    var vd = (VariableDeclaration) stmts[0];
        //    Assert.IsTrue(vd.Name == "i");
        //    Assert.IsTrue(vd.Type == "int");

        //    // This test case is not long-term intended behavior
        //    // But it tests that we do now what we think we do now.
        //    stmts = OldParser.FromString("bar := ;").Parse(); 
        //    Assert.IsTrue(stmts.Count == 1);
        //    vd = (VariableDeclaration)stmts[0];
        //    Assert.IsTrue(vd.Name == "bar");
        //    Assert.IsTrue(vd.Type == "auto"); 
        //}

        //[TestMethod]
        //public void Parser_Struct()
        //{
        //    var stmts = OldParser.FromString("struct foo { }").Parse();
        //    Assert.IsTrue(stmts.Count == 1);
        //    Assert.IsTrue(stmts[0] is StructDeclaration);
        //    var sd = (StructDeclaration) stmts[0];
        //    Assert.IsTrue(sd.Name == "foo");
        //    Assert.IsTrue(sd.Members.Count == 0);

        //    stmts = OldParser.FromString("struct foo { i : int; str : string; }").Parse();
        //    Assert.IsTrue(stmts.Count == 1);
        //    sd = (StructDeclaration)stmts[0];
        //    Assert.IsTrue(sd.Name == "foo");
        //    Assert.IsTrue(sd.Members.Count == 2);
        //    Assert.IsTrue(sd.Members[0].MemberName == "i");
        //    Assert.IsTrue(sd.Members[0].Type == "int");
        //    Assert.IsTrue(sd.Members[1].MemberName == "str");
        //    Assert.IsTrue(sd.Members[1].Type == "string");
        //}
	}
}

