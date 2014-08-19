namespace DSP
{
    public class TriangleWave: Wave
    {
        
        public byte Height { protected set; get; }
        public byte AttackEdge { protected set; get; }
        public byte BackEdge { protected set; get; }
        public byte Pause { protected set; get; }
        
        public TriangleWave(byte height, byte attackEdge, byte backEdge, byte pause, int offset)
        {
            Height = height;
            AttackEdge = attackEdge;
            BackEdge = backEdge;
            Pause = pause;
            Offset = offset;
        }

        public override byte[] Generate(int length)
        {
            var result = new byte[length];
            byte[] attack = new byte[AttackEdge];
            byte[] back = new byte[BackEdge];
            for (byte i = 0; i < AttackEdge; i++)
            {
                attack[i] += (byte)(i*(Height/AttackEdge));
            }
            for (byte i = 0; i < BackEdge; i++)
            {
                back[i] += (byte)(i * (Height / BackEdge));
            }

            byte selector = 0; 
            byte count = AttackEdge;
            
            // selector = 0 attack edge
            //          = 1 back edge
            //          = 2 pause
            for (int i = Offset; i < length; i++)
            {
                switch (selector)
                {
                    case 0:
                        if (AttackEdge!=0)
                        result[i] += attack[AttackEdge - count];
                        break;
                    case 1:
                        if (BackEdge !=0)
                        result[i] += back[BackEdge - count];
                        break;
                    case 2:
                        break;
                }
                count--;
                if (count==0)
                    switch (selector)
                    {
                        case 0:
                            selector = 1;
                            count = BackEdge;
                            break;
                        case 1:
                            selector = 2;
                            count = Pause;
                            break;
                        case 2:
                            selector = 0;
                            count = AttackEdge;
                            break;
                    }
            }
            return result;
        }
    }
}