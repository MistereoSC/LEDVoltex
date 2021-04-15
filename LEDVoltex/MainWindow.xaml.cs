using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;

namespace LEDVoltex
{
    public partial class MainWindow : Window
    {
        private LVX_KeyboardControls LED_Controller_SDVX;
        private SerialPort ComPort;

        public MainWindow()
        {

            InitializeComponent();

        }
        
        #region Basic Events
        //Dragmove
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
                var ports = SerialPort.GetPortNames();
                ComPort = new SerialPort("COM4");
                ComPort.BaudRate = 500000;
                ComPort.ReadTimeout = 1000;
                ComPort.WriteTimeout = 1000;
                ComPort.Open();

                Debug.Print("DEBUG_Ports:::" + ports.Length.ToString());
                foreach (var element in ports)
                {
                    Debug.Print(element);
                }
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