using System.IO;
using System.Text;

namespace n9.core
{
    public class FileReader
    {
        // =============================================================== 

        public int Position;
        public int Line;
        public int Column;

        // =============================================================== 

        public string Filename { get; private set; }
        char[] buffer;

        // =============================================================== 

        public static FileReader FromFile(string filename)
        {
            var r = new FileReader();
            r.buffer = File.ReadAllText(filename, Encoding.UTF8).ToCharArray();
            r.Position = 0;
            r.Line = 1;
            r.Column = 1;
            r.Filename = filename;
            return r;
        }

        public static FileReader FromString(string str, string filename="default.n9")
        {
            var r = new FileReader();
            r.buffer = str.ToCharArray();
            r.Position = 0;
            r.Line = 1;
            r.Column = 1;
            r.Filename = filename;
            return r;
        }

        // =============================================================== 

        const int ONE_TRUE_TAB_LENGTH = 4; // if we think we should count 'bytes' instead of columns, we could make this 1 instead.

        public int Read()
        {
            if (Position >= buffer.Length) return -1;

            char ch = buffer[Position++];

            if (ch == '\t')
            {
                Column += ONE_TRUE_TAB_LENGTH;
            }
            else if (ch == '\n')
            {
                Line++;
                Column = 1;
            }
            else
            {
                Column++;
            }

            return ch;
        }

        public int Peek()
        {
            if (Position >= buffer.Length) return -1;
            return buffer[Position];
        }

        public FilePosition Pos
        { 
            get 
            { 
                return new FilePosition { Filename = this.Filename, col = Column, row = Line }; 
            } 
        }
    }
}