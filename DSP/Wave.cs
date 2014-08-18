using System;
using System.Collections.Generic;

namespace DSP
{
    public abstract class Wave
    {
        public int Offset { protected set; get; }

        public abstract byte[] Generate(int length);
    }

    public class ResultWave : Wave
    {
        public ResultWave()
        {
            Offset = 0;
        }
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

        public override byte[] Generate(int length)
        {
            return ByteArray;
        }
    }
}
