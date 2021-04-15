
namespace LEDVoltex.Helper
{
    class LEDStaticZone
    {
        private int Idx_Start, Idx_End, Idx_Count;


        public LEDStaticZone(int Idx_Start, int Idx_End)
        {
            if (Idx_Start > Idx_End)
            {
                this.Idx_Start = Idx_End;
                this.Idx_End = Idx_Start;
            }
            else
            {
                this.Idx_Start = Idx_Start;
                this.Idx_End = Idx_End;
            }
            this.Idx_Count = (this.Idx_Start - this.Idx_End) + 1;
        }

        public int Start()
        {
            return this.Idx_Start;
        }
        public int End()
        {
            return this.Idx_End;
        }
        public int Count()
        {
            return this.Idx_Count;
        }

        public int[] Center()
        {
            int c = Count() % 2 + 1;
            int[] r = new int[c];
            r[0] = Start() + (int)((End() - Start()) / 2);
            if (c == 2) { r[1] = r[0] +1; }
            return r;
        }
    }
}
