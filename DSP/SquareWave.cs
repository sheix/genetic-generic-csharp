namespace DSP
{
    public class SquareWave : Wave
    {
        public byte Height { protected set; get; }
        public byte Width { protected set; get; }
        public byte Pause { protected set; get; }

        public SquareWave(byte height, byte width, byte pause, int offset)
        {
            Height = height;
            Width = width;
            Pause = pause;
            Offset = offset;
        }


        public override byte[] Generate(int length)
        {
            byte[] result = new byte[length];
            bool width = true;
            byte count = Width;
            for (int i = Offset; i < length; i++)
            {
                if (width)
                {
                    result[i] += Height;
                }
                count--;
                if (count == 0)
                {
                    width = !width;
                    if (width) count = Width;
                    if (!width) count = Pause;
                }
            }
            return result;
        }
    }
}