using System;
using System.Collections;
using System.Collections.Generic;

namespace DSP
{
    public abstract class Wave
    {
        public int Offset { protected set; get; }

        public abstract byte[] Generate(int length);


        virtual public List<Wave> Waves()
        {
            var result = new List<Wave> {this};
            return result;
        }
    }

    public class ResultWave : Wave
    {
        public ResultWave()
        {
            Offset = 0;
            _waves = new List<Wave>();
        }

       
        private readonly List<Wave> _waves;
        override public List<Wave> Waves()
        {
            return _waves;
        }


        public override byte[] Generate(int length)
        {
            var byteArray = new Byte[length];
            foreach (var wave in _waves)
            {
                byte[] bytewave = wave.Generate(length);
                for (int i = 0; i < length; i++)
                    byteArray[i] = Math.Max(bytewave[i], byteArray[i]);
            }
            return byteArray;
        }

        public void AddWave(Wave newWave)
        {
            _waves.Add(newWave);
        }
    }
}
