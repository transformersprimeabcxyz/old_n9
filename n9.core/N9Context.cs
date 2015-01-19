using System;
using System.Collections.Generic;

namespace n9.core
{
    public class N9Context
    {
        // TODO N9Context
        // list of source files
        // intel, arm?
        // debug, release
        // opt-levels, compiler flags
        // generate EXE, static library, shared library

        // ==============================================================================

        public ArchBits ArchBits = ArchBits.Arch32Bit;
        public CompilationTarget CompilationTarget = CompilationTarget.Executable;
        public CCompilerBackend Backend = CCompilerBackend.TCC;
        public OptimizationLevel OptimizationLevel = OptimizationLevel.None;
        public bool GenerateDebugSymbols = true;

        public List<String> VersionTags = new List<string>();

        // ==============================================================================

        public N9Context InitTags()
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
    }

    public enum ArchBits { Arch32Bit, Arch64Bit }
    public enum CompilationTarget { Executable, StaticLibrary, DynamicLibrary }
    public enum CCompilerBackend { TCC }
    public enum OptimizationLevel { None, Some, Max }
}
