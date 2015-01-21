using System;
using System.Collections.Generic;

namespace n9.core
{
    public class N9Context
    {
        // TODO N9Context
        // intel, arm?
        // debug, release
        // opt-levels, compiler flags

        // ==============================================================================

        public ArchBits ArchBits = ArchBits.Arch32Bit;
        public CompilationTarget CompilationTarget = CompilationTarget.Executable;
        public CCompilerBackend Backend = CCompilerBackend.TCC;
        public OptimizationLevel OptimizationLevel = OptimizationLevel.None;
        public bool GenerateDebugSymbols = true;

        public List<String> VersionTags = new List<string>();

        // ------------------------------------------------------------------------------

        public List<Parser> SourceFiles = new List<Parser>();

        // ==============================================================================

        public N9Context Construct()
        {
            if (GenerateDebugSymbols)
                VersionTags.Add("debug");

            switch (ArchBits)
            {
                case ArchBits.Arch32Bit: VersionTags.Add("x86"); VersionTags.Add("arch32bit"); break;
                case ArchBits.Arch64Bit: VersionTags.Add("x64"); VersionTags.Add("arch64bit"); break;
            }
                VersionTags.Add("debug");

            return this;
        }

        public N9Context DefaultReleaseSettings()
        {
            OptimizationLevel = OptimizationLevel.Max;
            GenerateDebugSymbols = true;
            return this;
        }

        public static N9Context FromString(string pgm, string filename = "default.n9")
        {
            var ctx = new N9Context();
            ctx.SourceFiles.Add(Parser.FromString(pgm, filename));
            return ctx;
        }
    }

    public enum ArchBits { Arch32Bit, Arch64Bit }
    public enum CompilationTarget { Executable, StaticLibrary, DynamicLibrary }
    public enum CCompilerBackend { TCC }
    public enum OptimizationLevel { None, Some, Max }
}
