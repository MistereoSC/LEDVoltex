using System;
using System.IO.Ports;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LEDVoltex.Helper.KeyboardControl
{
    class KC_Updater
    {
        private int POLLIING_RATE, POLLING_DELAY, LED_COUNT;
        private bool RUNNING = false;
        private Thread updaterThread;

        private bool[] LiveState;
        private bool[] LastState;
        private bool[] BufferState;        private KC_LEDState LEDState;

        private static WriteableBitmap writeableBitmap;
        private SerialPort ComPort;

        private System.Windows.Forms.Timer refreshTimer;
        private void timerTick(object sender, EventArgs e)
        {
            bool REVERSE = false;
            var arr = LEDState.getArray();
            if (REVERSE)
            {
                var arr2 = arr;
                arr = new string[arr2.Length];
                for (int c = 0; c < arr2.Length; c++)
                {
                    int p = arr2.Length - c - 1;
                    arr[p] = arr2[c];
                }
            }
            string s = "";
            foreach(var el in arr) { s += el; }
            //ComPort.Write(s);
            VisualizerDraw(arr);
        }

        public KC_Updater(int PollingRate, int LEDCount, System.Windows.Controls.Image VIS, SerialPort ComPort)
        {
            writeableBitmap = new WriteableBitmap(LEDCount * 4, 4, 96, 96, PixelFormats.Bgr32, null);
            this.ComPort = ComPort;
            VIS.Source = writeableBitmap;
            this.LED_COUNT = LEDCount;
            if (PollingRate < 1) { PollingRate = 1; }
            else if (PollingRate > 120) { PollingRate = 120; }
            this.LED_COUNT = LEDCount;
            this.POLLIING_RATE = PollingRate;
            this.POLLING_DELAY = (int)(1000 / PollingRate);
            LiveState = new bool[] { false, false, false, false, false, false, false, false, false, false, false };
            LastState = new bool[] { false, false, false, false, false, false, false, false, false, false, false };
            BufferState = new bool[] { false, false, false, false, false, false, false, false, false, false, false };
            
            LEDState = new KC_LEDState(LEDCount);

            refreshTimer = new System.Windows.Forms.Timer();
            refreshTimer.Interval = POLLING_DELAY;
            refreshTimer.Tick += new EventHandler(timerTick);
            refreshTimer.Start();
        }




        public void Start() {
            RUNNING = true;
            updaterThread = new Thread(new ThreadStart(Loop));
            updaterThread.Start();
            VisualizerDraw(LEDState.getArray());
        }


        public void Stop() {
            RUNNING = false;
        }

        private void Loop()
        {
            while (RUNNING)
            {
                int[] changes = CompareStates(LastState, BufferState);
                string[] newArr = LEDState.UpdateArray(changes);

                for (int c = 0; c < BufferState.Length; c++)
                {
                    if (!LiveState[c]) { BufferState[c] = false; }
                }
                Thread.Sleep(POLLING_DELAY);
            }
        }
        private int[] CompareStates(bool[] Old, bool[] New)
        {
            int[] StateChanges = new int[11];
            for (int c = 0; c < Old.Length; c++)
            {
                if (Old[c] && !New[c]) { StateChanges[c] = -1; }
                else if (!Old[c] && New[c]) { StateChanges[c] = 1; }
                else { StateChanges[c] = 0; }
                Old[c] = New[c];
            }
            return StateChanges;
        }
        public void updateButtonState_Down(Keys K)
        {
            switch (K)
            {
                case (Keys)Button.BT_A:
                    LiveState[(int)ButtonIDs.BT_A] = true;
                    BufferState[(int)ButtonIDs.BT_A] = true;
                    break;
                case (Keys)Button.BT_B:
                    LiveState[(int)ButtonIDs.BT_B] = true;
                    BufferState[(int)ButtonIDs.BT_B] = true;
                    break;
                case (Keys)Button.BT_C:
                    LiveState[(int)ButtonIDs.BT_C] = true;
                    BufferState[(int)ButtonIDs.BT_C] = true;
                    break;
                case (Keys)Button.BT_D:
                    LiveState[(int)ButtonIDs.BT_D] = true;
                    BufferState[(int)ButtonIDs.BT_D] = true;
                    break;
                case (Keys)Button.FX_L:
                    LiveState[(int)ButtonIDs.FX_L] = true;
                    BufferState[(int)ButtonIDs.FX_L] = true;
                    break;
                case (Keys)Button.FX_R:
                    LiveState[(int)ButtonIDs.FX_R] = true;
                    BufferState[(int)ButtonIDs.FX_R] = true;
                    break;
                case (Keys)Button.BT_Start:
                    LiveState[(int)ButtonIDs.BT_Start] = true;
                    BufferState[(int)ButtonIDs.BT_Start] = true;
                    break;

                case (Keys)Button.VOL_L_Left:
                    LiveState[(int)ButtonIDs.VOL_L_Left] = true;
                    BufferState[(int)ButtonIDs.VOL_L_Left] = true;
                    BufferState[(int)ButtonIDs.VOL_L_Right] = false;
                    break;
                case (Keys)Button.VOL_L_Right:
                    LiveState[(int)ButtonIDs.VOL_L_Right] = true;
                    BufferState[(int)ButtonIDs.VOL_L_Right] = true;
                    BufferState[(int)ButtonIDs.VOL_L_Left] = false;
                    break;
                case (Keys)Button.VOL_R_Left:
                    LiveState[(int)ButtonIDs.VOL_R_Left] = true;
                    BufferState[(int)ButtonIDs.VOL_R_Left] = true;
                    BufferState[(int)ButtonIDs.VOL_R_Right] = false;
                    break;
                case (Keys)Button.VOL_R_Right:
                    LiveState[(int)ButtonIDs.VOL_R_Right] = true;
                    BufferState[(int)ButtonIDs.VOL_R_Right] = true;
                    BufferState[(int)ButtonIDs.VOL_R_Left] = false;
                    break;
            }
        }
        public void updateButtonState_Up(Keys K)
        {
            switch (K)
            {
                case (Keys)Button.BT_A:
                    LiveState[(int)ButtonIDs.BT_A] = false;
                    break;
                case (Keys)Button.BT_B:
                    LiveState[(int)ButtonIDs.BT_B] = false;
                    break;
                case (Keys)Button.BT_C:
                    LiveState[(int)ButtonIDs.BT_C] = false;
                    break;
                case (Keys)Button.BT_D:
                    LiveState[(int)ButtonIDs.BT_D] = false;
                    break;
                case (Keys)Button.FX_L:
                    LiveState[(int)ButtonIDs.FX_L] = false;
                    break;
                case (Keys)Button.FX_R:
                    LiveState[(int)ButtonIDs.FX_R] = false;
                    break;
                case (Keys)Button.BT_Start:
                    LiveState[(int)ButtonIDs.BT_Start] = false;
                    break;

                case (Keys)Button.VOL_L_Left:
                    LiveState[(int)ButtonIDs.VOL_L_Left] = false;
                    break;
                case (Keys)Button.VOL_L_Right:
                    LiveState[(int)ButtonIDs.VOL_L_Right] = false;
                    break;
                case (Keys)Button.VOL_R_Left:
                    LiveState[(int)ButtonIDs.VOL_R_Left] = false;
                    break;
                case (Keys)Button.VOL_R_Right:
                    LiveState[(int)ButtonIDs.VOL_R_Right] = false;
                    break;
            }
        }

        private static void VisualizerDraw(string[] LED_Array)
        {

            for (int c = 0; c < LED_Array.Length; c++)
            {
                int column = c * 4;
                int RGB = int.Parse(LED_Array[c], System.Globalization.NumberStyles.HexNumber);
                try
                {
                    writeableBitmap.Lock();

                    unsafe
                    {
                        IntPtr pBackBuffer = writeableBitmap.BackBuffer;
                        int color_data = RGB;

                        pBackBuffer += writeableBitmap.BackBufferStride + 4;
                        pBackBuffer += column * 4;
                        *((int*)pBackBuffer) = color_data;
                        pBackBuffer += 4;
                        *((int*)pBackBuffer) = color_data;
                        pBackBuffer += writeableBitmap.BackBufferStride - 4;
                        *((int*)pBackBuffer) = color_data;
                        pBackBuffer += 4;
                        *((int*)pBackBuffer) = color_data;
                    }
                    writeableBitmap.AddDirtyRect(new Int32Rect(column, 1, 3, 3));
                }
                finally
                {
                    writeableBitmap.Unlock();
                }
            }
        }



        private enum Button
        {
            BT_A = Keys.A,
            BT_B = Keys.B,
            BT_C = Keys.C,
            BT_D = Keys.D,
            FX_L = Keys.X,
            FX_R = Keys.Y,
            BT_Start = Keys.Enter,
            VOL_L_Left = Keys.Q,
            VOL_L_Right = Keys.W,
            VOL_R_Left = Keys.O,
            VOL_R_Right = Keys.P
        }
        public enum ButtonIDs
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
