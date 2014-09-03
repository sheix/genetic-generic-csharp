using System;
using System.Collections.Generic;

namespace DSP
{
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