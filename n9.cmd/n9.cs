using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace n9.cmd
{
	public class n9
	{
		public static void _Main(string[] args)
		{
			if (args.Length == 0)
			{
				PrintBasicHelp();
				return;
			}
		}

		static void PrintBasicHelp()
		{
			Console.WriteLine("n9 version whatever");
			Console.WriteLine("Usage:   ");
			Console.WriteLine();
			Console.WriteLine("    n9 command [arguments]");
			Console.WriteLine();
			Console.WriteLine("Commands:");
			Console.WriteLine();
			Console.WriteLine("    build [target]      compile specified or default project file");
			Console.WriteLine("    run [target]        compile project and execute");
			Console.WriteLine("    test [target]       compile project execute unit tests");
			Console.WriteLine("    clean               remove compilation byproducts");
			Console.WriteLine("    fmt [target]        format source code in project");
			Console.WriteLine("    help [topic]        print detailed help");
			Console.WriteLine();
			Console.WriteLine("[target], if specified, can be either a *.n9proj project file, or a single ");
			Console.WriteLine("*.n9 source file. If [target] is not specified, n9 will attempt to resolve..");

			// If [target] is specified without an extension, n9 will first attempt to resolve [target].n9proj, as a project, in the current directory.
			// Then it will try [target].n9 as a single source file with default project.

			// If target is completely unspecified, n9 will attempt to resolve in the following order:
			// 1) If a single .n9proj file exists, that will be used as the project file.
			// 2) if multiple .n9proj files exist, attempt a file named build.n9proj
			// 3) if a single .n9 source file exists, attempt that as source file with default project.
			// 4) else error and request clarfication.
		}
	}
}
