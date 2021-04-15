using System.Windows.Media;

namespace LEDVoltex.Helper.KeyboardControl
{
    class KC_LEDState
    {
        private int LED_COUNT;
        private int Gap_FX = 0, Gap_BT = 8, Gap_BT_edge = 4;
        private int Length_VOL = 40, Speed_VOL = 12;
        private Color C_NULL = Color.FromRgb(0, 0, 0),
                        C_FX = Color.FromRgb(255, 127, 0),
                        C_BT = Color.FromRgb(0, 0, 255),
                        C_VOL_R = Color.FromRgb(242, 53, 176),
                        C_VOL_L = Color.FromRgb(41, 242, 222),
                        C_Idle = Color.FromRgb(63, 88, 127);


        private byte[] LED_Array;
        private byte[] L1_Idle, L2_FX, L3_BT, L4_Start, L5_VOL;
        private LEDStaticZone Zone_BT_A, Zone_BT_B, Zone_BT_C, Zone_BT_D, Zone_FX_L, Zone_FX_R, Zone_Idle;
        private LEDDynamicZone Zone_VOL_L, Zone_VOL_R;
        public int VOL_R_Direction = 0, VOL_L_Direction = 0;

        public KC_LEDState(int LED_COUNT)
        {
            this.LED_COUNT = LED_COUNT;
            Zone_VOL_L = new LEDDynamicZone(-1, Length_VOL, 0, LED_COUNT-1, true);
            Zone_VOL_R = new LEDDynamicZone(LED_COUNT, Length_VOL, 0, LED_COUNT - 1, true);
            Zone_Idle = new LEDStaticZone(0, LED_COUNT - 1);

            int t_Idx = (int)(LED_COUNT - Gap_FX) / 2;
            int mod = (LED_COUNT - Gap_FX) % 2;
            Zone_FX_R = new LEDStaticZone(0, t_Idx - 1);
            t_Idx += mod == 0 ? Gap_FX : Gap_FX + 1;
            Zone_FX_L = new LEDStaticZone(t_Idx, LED_COUNT - 1);

          
            int t_cnt = (int)(LED_COUNT - 3 * Gap_BT - 2 * Gap_BT_edge) / 4;
            t_Idx = t_cnt + Gap_BT_edge;
            Zone_BT_A = new LEDStaticZone(Gap_BT_edge - 1, t_Idx - 1);
            t_Idx += Gap_BT;
            t_Idx += mod < 2 ? 0 : 1;
            Zone_BT_B = new LEDStaticZone(t_Idx, t_Idx + t_cnt);
            t_Idx += t_cnt + Gap_BT + 1;
            t_Idx += mod % 2 == 0 ? 0 : 1;
            Zone_BT_C = new LEDStaticZone(t_Idx, t_Idx + t_cnt);
            t_Idx += t_cnt + Gap_BT + 1;
            t_Idx += mod < 2 ? 0 : 1;
            Zone_BT_D = new LEDStaticZone(t_Idx, LED_COUNT - Gap_BT_edge);


            LED_Array = new byte[LED_COUNT*3];
            L1_Idle = new byte[LED_COUNT*3];
            L2_FX = new byte[LED_COUNT*3];
            L3_BT = new byte[LED_COUNT*3];
            L4_Start = new byte[LED_COUNT*3];
            L5_VOL = new byte[LED_COUNT*3];
            FillZone(ref L1_Idle, Zone_Idle, C_Idle);
            FillZone(ref LED_Array, Zone_Idle, C_Idle);
        }


        public void UpdateArray(int[] changes)
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
                if(VOL_L_Direction == 0) { Zone_VOL_L.ShiftToEnd(); }
                VOL_L_Direction = 1; 
            }
            if (changes[(int)ButtonIDs.VOL_L_Right] == 1) {
                if (VOL_L_Direction == 0) { Zone_VOL_L.ShiftToStart(); }
                VOL_L_Direction = -1; 
            }
            if (changes[(int)ButtonIDs.VOL_R_Left] == 1) {
                if (VOL_R_Direction == 0) { Zone_VOL_R.ShiftToEnd(); }
                VOL_R_Direction = 1; 
            }
            if (changes[(int)ButtonIDs.VOL_R_Right] == 1) {
                if (VOL_R_Direction == 0) { Zone_VOL_R.ShiftToStart(); }
                VOL_R_Direction = -1; 
            }


            if (VOL_L_Direction == 1) {
                Zone_VOL_L.Shift(LEDDynamicZone.ShiftDirection.LEFT, Speed_VOL); 
            }
            if (VOL_L_Direction == -1) { 
                Zone_VOL_L.Shift(LEDDynamicZone.ShiftDirection.RIGHT, Speed_VOL); 
            }
            if (VOL_R_Direction == 1) { 
                Zone_VOL_R.Shift(LEDDynamicZone.ShiftDirection.LEFT, Speed_VOL); 
            }
            if (VOL_R_Direction == -1) { 
                Zone_VOL_R.Shift(LEDDynamicZone.ShiftDirection.RIGHT, Speed_VOL);  
            }


            FillZone(ref L5_VOL, Zone_VOL_L, C_VOL_L);
            FillZone(ref L5_VOL, Zone_VOL_R, C_VOL_R);
            if (Zone_VOL_L.IsInvisible()) { VOL_L_Direction = 0; }
            if (Zone_VOL_R.IsInvisible()) { VOL_R_Direction = 0; }

            MergeArrays(0, LED_COUNT-1);
        }
        private void FillZone(ref byte[] Zone, LEDStaticZone Range, Color Col)
        {
            for(int c = Range.Start(); c <= Range.End(); c++)
            {
                Zone[c * 3 + 0] = Col.G;
                Zone[c * 3 + 1] = Col.R;
                Zone[c * 3 + 2] = Col.B;
            }
        }
        private void FillZone(ref byte[] Zone, LEDDynamicZone Range, Color Col)
        {
            if (Range.IsInvisible()) { return; }
            int S = Range.Start();
            int E = Range.End();
            if (S < Range.Min() || S < 0) { S = Range.Min(); }
            if (E > Range.Max() || E < 0) { E = Range.Max(); }

            for (int c = S; c <= E; c++)
            {
                Zone[c * 3 + 0] = Col.G;
                Zone[c * 3 + 1] = Col.R;
                Zone[c * 3 + 2] = Col.B;
            }
        }

        private void MergeArrays(int start, int end)
        {
            for (int c = 3*start; c <= 3*end; c+=3)
            {
                if (L5_VOL[c] != 0 || L5_VOL[c+1] != 0 || L5_VOL[c+2] != 0)
                {
                    LED_Array[c] = L5_VOL[c];
                    LED_Array[c+1] = L5_VOL[c+1];
                    LED_Array[c+2] = L5_VOL[c+2];
                    continue;
                }
                if (L4_Start[c] != 0 || L4_Start[c+1] != 0 || L4_Start[c+2] != 0)
                {
                    LED_Array[c] = L4_Start[c];
                    LED_Array[c+1] = L4_Start[c+1];
                    LED_Array[c+2] = L4_Start[c+2];
                    continue;
                }
                if (L3_BT[c] != 0 || L3_BT[c+1] != 0 || L3_BT[c+2] != 0)
                {
                    LED_Array[c] = L3_BT[c];
                    LED_Array[c+1] = L3_BT[c+1];
                    LED_Array[c+2] = L3_BT[c+2];
                    continue;
                }
                if (L2_FX[c] != 0 || L2_FX[c+2] != 0 || L2_FX[c+2] != 0)
                {
                    LED_Array[c] = L2_FX[c];
                    LED_Array[c+1] = L2_FX[c+1];
                    LED_Array[c+2] = L2_FX[c+2];
                    continue;
                }
                LED_Array[c] = L1_Idle[c];
                LED_Array[c+1] = L1_Idle[c+1];
                LED_Array[c+2] = L1_Idle[c+2];
            }
        }
        public byte[] GetArray() { return LED_Array; }
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
