namespace n9.core
{
    public abstract class ArchDriver
    {
        public abstract int AlignOf(int size);
        public abstract int PointerSize();
    }

    public class x86Driver : ArchDriver
    {
        public override int PointerSize()
        {
            return 4;
        }
        
        public override int AlignOf(int size)
        {
            return size; // TODO
        }
    }

    public class x64Driver : ArchDriver
    {
        public override int PointerSize()
        {
            return 8;
        }

        public override int AlignOf(int size)
        {
            return size; // TODO
        }
    }
}