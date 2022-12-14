using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using System.Net;


namespace JetterPanal
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        ~MainWindow()
        {
           
        }

        private static IPAddress remoteIPAddress = IPAddress.Parse("192.168.48.70");
        private static int PortServer = 50002;
        private static int PortClient = 50000;

        List<int> addressVariables = new List<int>() { 1001010, 90};
        List<BitArray> bitArr = new List<BitArray>();

        WorkWithTags tags = new WorkWithTags();
        UdpClass udp = new UdpClass(remoteIPAddress, PortServer, PortClient, UdpClass.typeVariable.typeFloat);
        public Timer timerUpdateData = new Timer(1000);       

        private void startTimer()
        {
            timerUpdateData.Enabled = true;
            timerUpdateData.Elapsed += TimerUpdate;
        }
        private void stopTimer()
        {
            timerUpdateData.Enabled = false;
        }
        private void TimerUpdate(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                new Action(() => {
                    this.timerUpdateData.Stop();

                    this.tags.reqGetTags(addressVariables, udp);
                    List<int> intTagList = udp.getIntList();
                    List<byte> flags = udp.getByteList();

                    bitArr.Clear();

                    if (flags != null && flags.Count != 0)
                    {
                        if (flags[0] == 0x21)
                        {
                            btStartReferencing.Visibility = Visibility.Hidden;
                            label1.Visibility = Visibility.Hidden;
                        }
                        else
                        {
                            btStartReferencing.Visibility = Visibility.Visible;
                            label1.Visibility = Visibility.Visible;
                        }
                    }

                    if (intTagList != null && intTagList.Count != 0)
                    {
                         bitArr.Add(new BitArray(new int[] { BitConverter.ToInt32(BitConverter.GetBytes(intTagList[0]), 0) })); //1001010
                    }
                    
                    this.timerUpdateData.Start();
                }));
        }


        private void btHandMode_Click(object sender, RoutedEventArgs e)
        { 
            if (bitArr != null)
            {               
                timerUpdateData.Stop();
                bitArr[0][10] = true;
                tags.setTag(1001010, 0x0a, Convert.ToSingle(ToNumeral(bitArr[0])), udp);             
            }

            HandMode hm = new HandMode(udp, this);
            hm.Show();
            this.Visibility = Visibility.Hidden;
        }

        private static int ToNumeral(BitArray binary)
        {
            var toInt = new int[1];
            binary.CopyTo(toInt, 0);
            return toInt[0];
        }

        private void btStartReferencing_Click(object sender, RoutedEventArgs e)
        {         
            if (bitArr != null)
            {              
                timerUpdateData.Stop();
                bitArr[0][2] = true;
                tags.setTag(1001010, 0x0a, Convert.ToSingle(ToNumeral(bitArr[0])), udp);

                Referens rf = new Referens(udp, this);
                rf.Show();
                this.Visibility = Visibility.Hidden;
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            if(!timerUpdateData.Enabled)
                timerUpdateData.Start();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            startTimer();
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            udp.closeUDP();
        }

        private void btServiceMode_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btAutoMode_Click(object sender, RoutedEventArgs e)
        {
            if (bitArr != null)
            {              
                stopTimer();
                bitArr[0][5] = true;
                tags.setTag(1001010, 0x0a, Convert.ToSingle(ToNumeral(bitArr[0])), udp);
            }

            AutoMode am = new AutoMode(udp, this);
            am.Show();
            this.Visibility = Visibility.Hidden;
        }
    }
}
