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
}
