using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSP
{
    public abstract class Wave
    {
        public int Offset { protected set; get; }

        public abstract byte[] Generate(int length);
    }

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

    public class ResultWave
    {
        public byte[] ByteArray;

        private int _length;

        public void SetLength(int length)
        {
            _length = length;
        }

        public void SetWaves(List<Wave> waves)
        {
            ByteArray = new Byte[_length];
            foreach (var wave in waves)
            {
                byte[] bytewave = wave.Generate(_length);
                for (int i = 0; i < _length; i++)
                    ByteArray[i] = Math.Max(bytewave[i], ByteArray[i]);
            }
        }
    }
}
