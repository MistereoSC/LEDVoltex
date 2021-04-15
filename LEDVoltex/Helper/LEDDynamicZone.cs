using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEDVoltex.Helper
{
    class LEDDynamicZone
    {

        private int Idx_Start, Idx_End, Idx_Count;
        private int Idx_Min, Idx_Max;

        private bool AllowCutoff;
        

        public LEDDynamicZone(int Idx_Start, int Length, int Idx_Min, int Idx_Max, bool AllowCutoff)
        {
            this.Idx_Count = Length;
            this.AllowCutoff = AllowCutoff;
            this.Idx_Min = Idx_Min;
            this.Idx_Max = Idx_Max;

            if (AllowCutoff)
            {
                if (Idx_Start<0)
                {
                    this.Idx_Start = Idx_Min - Length;
                    this.Idx_End = Idx_Min - 1;
                }
                else if (Idx_Start > Idx_Max)
                {
                    this.Idx_Start = Idx_Max + 1;
                    this.Idx_End = Idx_Max + Length;
                }
                else
                {
                    this.Idx_Start = Idx_Start;
                    this.Idx_End = Idx_Start + Length - 1;
                }
            }
            else
            {
                if (Idx_Start < Idx_Min) { this.Idx_Start = Idx_Min; this.Idx_End = Idx_Min + Length - 1; }
                else if (Idx_Start > Idx_Max) { this.Idx_Start = Idx_Max - Length + 1; this.Idx_End = Idx_Max; }
                else { this.Idx_Start = Idx_Start; this.Idx_End = Idx_Start + Length - 1; }
            }
        }

        public int Start()
        {
            if (Idx_Start < Idx_Min || Idx_Start > Idx_Max) { return -1; }
            else return this.Idx_Start;
        }
        public int End()
        {
            if (Idx_End < Idx_Min || Idx_End > Idx_Max) { return -1; }
            else return this.Idx_End;
        }
        public int Min() { return this.Idx_Min; }
        public int Max() { return this.Idx_Max; }
        public int Count()
        {
            return this.Idx_Count;
        }
        public bool ReachedStart()
        {
            if (!AllowCutoff && Idx_Start == Idx_Min) { return true; }
            else if (AllowCutoff && Idx_Start == Idx_Min - Idx_Count) { return true; }
            else return false;
        }
        public bool ReachedEnd()
        {
            if (!AllowCutoff && Idx_End == Idx_Max) { return true; }
            else if (AllowCutoff && Idx_End == Idx_Max + Idx_Count) { return true; }
            else return false;
        }

        public bool IsInvisible()
        {
            if (AllowCutoff)
            {
                if(ReachedEnd() || ReachedStart()) { return true; }
            }
            return false;
        }


        public void shift(ShiftDirection Direction, int ShiftAmount)
        {
            switch (Direction){
                case ShiftDirection.LEFT:
                    if (Idx_Start - ShiftAmount < Idx_Min) 
                    {
                        if (AllowCutoff)
                        {
                            if (Idx_Start - ShiftAmount < Idx_Min - Idx_Count)
                            {
                                Idx_Start = Idx_Min - Idx_Count;
                                Idx_End = Idx_Min - 1;
                            }
                            else
                            {
                                Idx_Start -= ShiftAmount;
                                Idx_End -= ShiftAmount;
                            }
                        }
                        else
                        {
                            Idx_Start = Idx_Min;
                            Idx_End = Idx_Min + Idx_Count;
                        }
                    }
                    else { 
                        Idx_Start -= ShiftAmount;
                        Idx_End = Idx_Start + Idx_Count;
                    }
                    break;

                case ShiftDirection.RIGHT:
                    if (Idx_End + ShiftAmount > Idx_Max)
                    {
                        if (AllowCutoff)
                        {
                            if (Idx_End + ShiftAmount > Idx_Max + Idx_Count)
                            {
                                Idx_End = Idx_Max + Idx_Count;
                                Idx_Start = Idx_Max + 1;
                            }
                            else
                            {
                                Idx_Start += ShiftAmount;
                                Idx_End = Idx_Start + Idx_Count;
                            }
                        }
                        else
                        {
                            Idx_End = Idx_Max;
                            Idx_Start = Idx_Max - Idx_Count;
                        }
                    }
                    else
                    {
                        Idx_Start += ShiftAmount;
                        Idx_End = Idx_Start + Idx_Count;
                    }
                    break;

            }
        }
        public void shiftToStart() {
            if (AllowCutoff)
            {
                Idx_Start = Idx_Start = Idx_Min - Idx_Count;
                Idx_End = Idx_Start - 1;
            }
            else
            {
                Idx_Start = Idx_Min;
                Idx_End = Idx_Min + Idx_Count - 1;
            }

        }
        public void shiftToEnd() {
            if (AllowCutoff)
            {
                Idx_End = Idx_Max + Idx_Count;
                Idx_Start = Idx_Max + 1;
            }
            else
            {
                Idx_End = Idx_Max;
                Idx_Start = Idx_Max - Idx_Count + 1;
            }
        }

        public enum ShiftDirection
        {
            LEFT,
            RIGHT
        }

    }
}
