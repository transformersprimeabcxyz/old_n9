using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using n9.core;

namespace n9.test
{
	[TestClass]
	public class SourceFileTests
	{
        [TestMethod]
        public void SourceFile_Version()
        {
            var ctx = N9Context.FromString("version (a) i:int;").Construct();
            Assert.IsTrue(ctx.SourceFiles[0].Statements.Count == 0);

            ctx = N9Context.FromString("version (a) i:int;").Tags("a").Construct();
            Assert.IsTrue(ctx.SourceFiles[0].Statements.Count == 1);

            ctx = N9Context.FromString("version (!a) i:int;").Tags("a").Construct();
            Assert.IsTrue(ctx.SourceFiles[0].Statements.Count == 0);

            ctx = N9Context.FromString("version (a && b) i:int;").Tags("a").Construct();
            Assert.IsTrue(ctx.SourceFiles[0].Statements.Count == 0);

            ctx = N9Context.FromString("version (a || b) i:int;").Tags("a").Construct();
            Assert.IsTrue(ctx.SourceFiles[0].Statements.Count == 1);

            ctx = N9Context.FromString("version (a && b) { i:int; }").Tags("a","b").Construct();
            Assert.IsTrue(ctx.SourceFiles[0].Statements.Count == 1);
        }

        [TestMethod]
        public void SourceFile_VersionElse()
        {
            var ctx = N9Context.FromString("version (a) i:int; else j:int;").Construct();
            Assert.IsTrue(ctx.SourceFiles[0].Statements.Count == 1);
            var decl = ctx.SourceFiles[0].Statements[0] as VariableDeclaration;
            Assert.IsTrue(decl.Name == "j");

            ctx = N9Context.FromString("version (a) i:int; else j:int;").Tags("a").Construct();
            Assert.IsTrue(ctx.SourceFiles[0].Statements.Count == 1);
            decl = ctx.SourceFiles[0].Statements[0] as VariableDeclaration;
            Assert.IsTrue(decl.Name == "i");

        }

        [TestMethod]
        public void SourceFile_VersionComplex()
        {
            string pgm = @"
                func foo() 
                {
                    version (a)
                        x:=5;
                    version (b)
                        i:=0;

                    while (bar())
                    {
                        version (a)
                            x = x + 1;
                        version (b)
                            return;
                    }

                }
            ";
            var ctx = N9Context.FromString(pgm).Tags("a").Construct();
            Assert.IsTrue(ctx.SourceFiles[0].Statements.Count == 1);
            var func = ctx.SourceFiles[0].Statements[0] as FuncDeclaration;
            var stmt1 = func.Body[0] as VariableDeclaration;
            Assert.IsTrue(stmt1.Name == "x");
            
            var stmt2 = func.Body[1] as WhileStatement;
            Assert.IsTrue(stmt2 is WhileStatement);
            Assert.IsTrue(stmt2.Body.Count == 1);
            Assert.IsTrue(stmt2.Body[0] is AssignStatement);
        }
	}
}