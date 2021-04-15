using System;
using System.Windows.Media;
using System.Diagnostics;

namespace LEDVoltex.Helper.KeyboardControl
{
    class KC_LEDState
    {
        private int LED_COUNT;
        private int Gap_FX = 0, Gap_BT = 0;
        private int Length_VOL = 40, Speed_VOL = 6;
        private Color C_NULL = Color.FromRgb(0, 0, 0),
                        C_FX = Color.FromRgb(255, 127, 0),
                        C_BT = Color.FromRgb(0, 0, 255),
                        C_VOL_R = Color.FromRgb(242, 53, 176),
                        C_VOL_L = Color.FromRgb(41, 242, 222),
                        C_Idle = Color.FromRgb(63, 88, 127);


        private string[] LED_Array;
        private string[] L1_Idle, L2_FX, L3_BT, L4_Start, L5_VOL;
        private LEDStaticZone Zone_BT_A, Zone_BT_B, Zone_BT_C, Zone_BT_D, Zone_FX_L, Zone_FX_R, Zone_Idle, Zone_BT_Start;
        private LEDDynamicZone Zone_VOL_L, Zone_VOL_R;

        public int[] Sliders;
        public int[] Cycles;

        public KC_LEDState(int LED_COUNT)
        {
            this.LED_COUNT = LED_COUNT;
            Cycles = new int[] { 0, 0, 0, 0, 0, 0, 0 };
            Sliders = new int[] { -1, 0, LED_COUNT, 0 };


            Zone_VOL_L = new LEDDynamicZone(-1, Length_VOL, 0, LED_COUNT-1, true);
            Zone_VOL_R = new LEDDynamicZone(LED_COUNT, Length_VOL, 0, LED_COUNT - 1, true);

            Zone_Idle = new LEDStaticZone(0, LED_COUNT - 1);
            Zone_BT_Start = new LEDStaticZone(0, LED_COUNT - 1);

            int t_Idx = (int)(LED_COUNT - Gap_FX) / 2;
            int mod = (LED_COUNT - Gap_FX) % 2;
            Zone_FX_R = new LEDStaticZone(0, t_Idx - 1);
            t_Idx += mod == 0 ? Gap_FX : Gap_FX + 1;
            Zone_FX_L = new LEDStaticZone(t_Idx, LED_COUNT - 1);


            t_Idx = (int)(LED_COUNT - 3 * Gap_BT) / 4;
            int t_cnt = t_Idx;
            mod = (LED_COUNT - 3 * Gap_BT) % 4;
            Zone_BT_A = new LEDStaticZone(0, t_Idx - 1);
            t_Idx += Gap_BT;
            t_Idx += mod < 2 ? 0 : 1;
            Zone_BT_B = new LEDStaticZone(t_Idx, t_Idx + t_cnt);
            t_Idx += t_cnt + Gap_BT + 1;
            t_Idx += mod % 2 == 0 ? 0 : 1;
            Zone_BT_C = new LEDStaticZone(t_Idx, t_Idx + t_cnt);
            t_Idx += t_cnt + Gap_BT + 1;
            t_Idx += mod < 2 ? 0 : 1;
            Zone_BT_D = new LEDStaticZone(t_Idx, LED_COUNT - 1);


            LED_Array = new string[LED_COUNT];
            L1_Idle = new string[LED_COUNT];
            L2_FX = new string[LED_COUNT];
            L3_BT = new string[LED_COUNT];
            L4_Start = new string[LED_COUNT];
            L5_VOL = new string[LED_COUNT];

            for (int c = 0; c < LED_Array.Length; c++)
            {
                L2_FX[c] = "000000";
                L3_BT[c] = "000000";
                L4_Start[c] = "000000";
                L5_VOL[c] = "000000";
            }
            for (int c = 0; c < LED_Array.Length; c++)
            {
                L1_Idle[c] = C_Idle.ToString().Substring(3);
                LED_Array[c] = C_Idle.ToString().Substring(3);
            }
        }


        public string[] UpdateArray(int[] changes)
        {
            if(changes[(int)ButtonIDs.FX_L] == 1) { FillZone(ref L2_FX, Zone_FX_L, C_FX); }
            else if (changes[(int)ButtonIDs.FX_L] == -1) { FillZone(ref L2_FX, Zone_FX_L, C_NULL); }

            if (changes[(int)ButtonIDs.FX_R] == 1) { FillZone(ref L2_FX, Zone_FX_R, C_FX); }
            else if (changes[(int)ButtonIDs.FX_R] == -1) { FillZone(ref L2_FX, Zone_FX_R, C_NULL); }

            if (changes[(int)ButtonIDs.BT_A] == 1) { FillZone(ref L3_BT, Zone_BT_A, C_BT); }
            else if (changes[(int)ButtonIDs.BT_A] == -1) { FillZone(ref L3_BT, Zone_BT_A, C_NULL); }

            if (changes[(int)ButtonIDs.BT_B] == 1) { FillZone(ref L3_BT, Zone_BT_B, C_BT); }
            else if (changes[(int)ButtonIDs.BT_B] == -1) { FillZone(ref L3_BT, Zone_BT_B, C_NULL); }

            if (changes[(int)ButtonIDs.BT_C] == 1) { FillZone(ref L3_BT, Zone_BT_C, C_BT); }
            else if (changes[(int)ButtonIDs.BT_C] == -1) { FillZone(ref L3_BT, Zone_BT_C, C_NULL); }

            if (changes[(int)ButtonIDs.BT_D] == 1) { FillZone(ref L3_BT, Zone_BT_D, C_BT); }
            else if (changes[(int)ButtonIDs.BT_D] == -1) { FillZone(ref L3_BT, Zone_BT_D, C_NULL); }


            
            FillZone(ref L5_VOL, Zone_VOL_L, C_NULL);
            FillZone(ref L5_VOL, Zone_VOL_R, C_NULL);
            if (changes[(int)ButtonIDs.VOL_L_Left] == 1) {
                if(Sliders[(int)SliderIDs.Dir_VOL_L] == 0) { Zone_VOL_L.shiftToEnd(); }
                Sliders[(int)SliderIDs.Dir_VOL_L] = 1; 
            }
            if (changes[(int)ButtonIDs.VOL_L_Right] == 1) {
                if (Sliders[(int)SliderIDs.Dir_VOL_L] == 0) { Zone_VOL_L.shiftToStart(); }
                Sliders[(int)SliderIDs.Dir_VOL_L] = -1; 
            }
            if (changes[(int)ButtonIDs.VOL_R_Left] == 1) {
                if (Sliders[(int)SliderIDs.Dir_VOL_R] == 0) { Zone_VOL_R.shiftToEnd(); }
                Sliders[(int)SliderIDs.Dir_VOL_R] = 1; 
            }
            if (changes[(int)ButtonIDs.VOL_R_Right] == 1) {
                if (Sliders[(int)SliderIDs.Dir_VOL_R] == 0) { Zone_VOL_R.shiftToStart(); }
                Sliders[(int)SliderIDs.Dir_VOL_R] = -1; 
            }


            if (Sliders[(int)SliderIDs.Dir_VOL_L] == 1) {
                Zone_VOL_L.shift(LEDDynamicZone.ShiftDirection.LEFT, Speed_VOL); 
            }
            if (Sliders[(int)SliderIDs.Dir_VOL_L] == -1) { 
                Zone_VOL_L.shift(LEDDynamicZone.ShiftDirection.RIGHT, Speed_VOL); 
            }
            if (Sliders[(int)SliderIDs.Dir_VOL_R] == 1) { 
                Zone_VOL_R.shift(LEDDynamicZone.ShiftDirection.LEFT, Speed_VOL); 
            }
            if (Sliders[(int)SliderIDs.Dir_VOL_R] == -1) { 
                Zone_VOL_R.shift(LEDDynamicZone.ShiftDirection.RIGHT, Speed_VOL);  
            }


            FillZone(ref L5_VOL, Zone_VOL_L, C_VOL_L);
            FillZone(ref L5_VOL, Zone_VOL_R, C_VOL_R);
            if (Zone_VOL_L.IsInvisible()) { Sliders[(int)SliderIDs.Dir_VOL_L] = 0; }
            if (Zone_VOL_R.IsInvisible()) { Sliders[(int)SliderIDs.Dir_VOL_R] = 0; }

            ReduceCylces();
            MergeArrays(0, LED_COUNT-1);
            return LED_Array;
        }
        private void FillZone(ref string[] Zone, LEDStaticZone Range, Color Col)
        {
            string HexC = Col.ToString().Substring(3);
            for(int c = Range.Start(); c <= Range.End(); c++)
            {
                Zone[c] = HexC;
            }
        }
        private void FillZone(ref string[] Zone, LEDDynamicZone Range, Color Col)
        {
            if (Range.IsInvisible()) { return; }
            int S = Range.Start();
            int E = Range.End();
            if (S < Range.Min() || S < 0) { S = Range.Min(); }
            if (E > Range.Max() || E < 0) { E = Range.Max(); }

            string HexC = Col.ToString().Substring(3);
            for (int c = S; c <= E; c++)
            {
                Zone[c] = HexC;
            }
        }

        private void MergeArrays(int start, int end)
        {
            for (int c = start; c <= end; c++)
            {
                if (L5_VOL[c] != "000000")
                {
                    LED_Array[c] = L5_VOL[c];
                    continue;
                }
                if (L4_Start[c] != "000000")
                {
                    LED_Array[c] = L4_Start[c];
                    continue;
                }
                if (L3_BT[c] != "000000")
                {
                    LED_Array[c] = L3_BT[c];
                    continue;
                }
                if (L2_FX[c] != "000000")
                {
                    LED_Array[c] = L2_FX[c];
                    continue;
                }
                LED_Array[c] = L1_Idle[c];
            }
        }
        public string[] getArray() { return LED_Array; }
        public void ReduceCylces()
        {
            for (int c = 0; c < Cycles.Length; c++)
            {
                if (Cycles[c] > 0) { Cycles[c]--; }
            }
        }
        public enum CycleIDs
        {
            BT_A = 0,
            BT_B = 1,
            BT_C = 2,
            BT_D = 3,
            BT_Start = 6,
            FX_L = 4,
            FX_R = 5
        }
        public enum SliderIDs
        {
            Pos_VOL_L = 0,
            Dir_VOL_L = 1,
            Pos_VOL_R = 2,
            Dir_VOL_R = 3
        }
        private enum ButtonIDs
        {
            BT_A = 0,
            BT_B = 1,
            BT_C = 2,
            BT_D = 3,
            BT_Start = 6,
            FX_L = 4,
            FX_R = 5,
            VOL_L_Left = 7,
            VOL_L_Right = 8,
            VOL_R_Left = 9,
            VOL_R_Right = 10
        }

    }
}
