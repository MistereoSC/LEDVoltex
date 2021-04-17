using System;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.IO.Ports;

namespace LEDVoltex
{
    public partial class MainWindow : Window
    {
        private LVX_KeyboardControls LED_Controller_SDVX;
        private SerialPort ComPort;
        private string TMP_ComPort_Name = "COM4";
        public MainWindow()
        {
            InitializeComponent();
        }
        
        #region Basic Events
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
            Environment.Exit(0);
        }
        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try {
                ComPort = new SerialPort(TMP_ComPort_Name)
                {
                    BaudRate = 500000,
                    ReadTimeout = 500,
                    WriteTimeout = 500,
                    DtrEnable = true,
                    RtsEnable = true
                };
                ComPort.Open();
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }
            LED_Controller_SDVX = new LVX_KeyboardControls(this.LEDVisualizer, this.ComPort);
            LED_Controller_SDVX.Init();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LED_Controller_SDVX.Dispose();
            ComPort.Close();
        }
        #endregion
    }
}