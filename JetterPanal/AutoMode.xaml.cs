using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Timers;

namespace JetterPanal
{
    /// <summary>
    /// Логика взаимодействия для AutoMode.xaml
    /// </summary>
    public partial class AutoMode : Window
    {
        public AutoMode(UdpClass udp, Window main)
        {
            InitializeComponent();
            udp_ = udp;
            main_ = main;
        }

        List<int> addressVariables = new List<int>() { 1001015 /*bits*/, 1001010 /*bits*/};
        List<BitArray> bitArr = new List<BitArray>();

        WorkWithTags tags = new WorkWithTags();
        UdpClass udp_;
        Window main_;
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

                    this.tags.reqGetTags(addressVariables, udp_);
                    List<int> intTagList = udp_.getIntList();

                    bitArr.Clear();
                    try
                    {
                        if (intTagList != null && intTagList.Count != 0)
                        {
                            bitArr.Add(new BitArray(new int[] { BitConverter.ToInt32(BitConverter.GetBytes(intTagList[0]), 0) })); //1001015
                            bitArr.Add(new BitArray(new int[] { BitConverter.ToInt32(BitConverter.GetBytes(intTagList[1]), 0) })); //1001010
                        }
                    }
                    catch(Exception ex)
                    {
                       // MessageBox.Show(ex.Message);
                    }

                }));
        }

        private static int ToNumeral(BitArray binary)
        {
            var toInt = new int[1];
            binary.CopyTo(toInt, 0);
            return toInt[0];
        }

        private void bnNewPlate_Click(object sender, RoutedEventArgs e)
        {
            if (bitArr != null)
            {
                timerUpdateData.Stop();
                bitArr[0][15] = true;
                tags.setTag(1001015, 0x0a, Convert.ToSingle(ToNumeral(bitArr[0])), udp_);
            }

            AutoNewPlate anp = new AutoNewPlate(udp_, this);
            anp.Show();
            this.Visibility = Visibility.Hidden;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            stopTimer();
            timerUpdateData.Dispose();
            main_.Visibility = Visibility.Visible;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            startTimer();
        }
    }
}
