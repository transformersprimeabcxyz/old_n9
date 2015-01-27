using System;
using System.Collections.Generic;

namespace n9.core
{
    public class N9Context
    {
        // ==============================================================================

        public ArchBits ArchBits = ArchBits.Arch32Bit;
        public CompilationTarget CompilationTarget = CompilationTarget.Executable;
        public CCompilerBackend Backend = CCompilerBackend.TCC;
        public OptimizationLevel OptimizationLevel = OptimizationLevel.None;
        public bool GenerateDebugSymbols = true;

        public List<String> VersionTags = new List<string>();
        public List<SourceFile> SourceFiles = new List<SourceFile>();

        // ==============================================================================

        public N9Context Construct()
        {
            if (GenerateDebugSymbols)
                Tags("debug");

            switch (ArchBits)
            {
                case ArchBits.Arch32Bit: Tags("x86", "arch32bit"); break;
                case ArchBits.Arch64Bit: Tags("x64", "arch64bit"); break;
            }

            foreach (var src in SourceFiles)
                src.Analyze();
            
            return this;
        }

        public N9Context DefaultReleaseSettings()
        {
            OptimizationLevel = OptimizationLevel.Max;
            GenerateDebugSymbols = true;
            return this;
        }

        public N9Context Tags(params string[] tags)
        {
            foreach (var tag in tags)
                if (!VersionTags.Contains(tag))
                    VersionTags.Add(tag);
            return this;
        }

        public static N9Context FromString(string pgm, string filename = "default.n9")
        {
            var ctx = new N9Context();
            ctx.SourceFiles.Add(SourceFile.FromString(ctx, pgm, filename));
            return ctx;
        }
    }

    public enum ArchBits { Arch32Bit, Arch64Bit }
    public enum CompilationTarget { Executable, StaticLibrary, DynamicLibrary }
    public enum CCompilerBackend { TCC }
    public enum OptimizationLevel { None, Some, Max }
}