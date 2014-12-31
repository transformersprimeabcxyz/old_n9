namespace n9.core
{
    public enum DiagnosticLevel
    {
        Error,
        Warning,
        Info        
    }

    public class Diagnostic
    {
        public DiagnosticLevel Level;
        public string Message;
        public FilePosition? Position;

        public static Diagnostic Error(string message, FilePosition? position = null)
        {
            return new Diagnostic { Level = DiagnosticLevel.Error, Message = message, Position = position };
        }

        public static Diagnostic Warning(string message, FilePosition? position = null)
        {
            return new Diagnostic { Level = DiagnosticLevel.Warning, Message = message, Position = position };
        }
    }

    // TODO do I use this? dammit I overengineer things in C#.
    public class CompilationException : System.Exception
    {
        public Diagnostic Error { get; private set; }
        public CompilationException(Diagnostic e):base(e.Message)
        {
            Error = e;
        }
    }
}