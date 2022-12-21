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
using System.Threading;

namespace JetterPanal
{
    /// <summary>
    /// Логика взаимодействия для Referens.xaml
    /// </summary>
    public partial class Referens : Window
    {
        public Referens(UdpClass udp, Window main)
        {
            InitializeComponent();
            udp_ = udp;
            main_ = main;
        }

        List<int> addressVariables = new List<int>() { 1001011 /*bits*/};
        List<Ellipse> ellipses = new List<Ellipse>();

        WorkWithTags tags = new WorkWithTags();
        UdpClass udp_;
        Window main_;
        public System.Timers.Timer timerUpdateData = new System.Timers.Timer(1000);

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
            stopTimer();
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                new Action(() => {
                    tags.reqGetTags(addressVariables, udp_);
                    List<int> intTagList = udp_.getIntList();

                    if (intTagList != null && intTagList.Count != 0)
                    {
                        try
                        {
                            BitArray bitArr = new BitArray(new int[] { BitConverter.ToInt32(BitConverter.GetBytes(intTagList[0]), 0) }); //1001011

                            if (bitArr[1])
                            {
                                pnBackStopDrop.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                pnBackStopDrop.Visibility = Visibility.Hidden;
                            }

                            for (int i = 2; i < bitArr.Length; ++i)
                            {
                                if (bitArr[i] == true)
                                {
                                    ellipses[i-2].Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF50BB3F");
                                }
                                else
                                {
                                    ellipses[i-2].Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFC95634");
                                }
                            }
                        }catch
                        {

                        }
                    }
                  
                }));
            startTimer();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            stopTimer();
            timerUpdateData.Dispose();
            main_.Visibility = Visibility.Visible;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ellipses.Add(elAdjRefRun);
            ellipses.Add(elAdjRefOK);
            ellipses.Add(elOszRefRun);
            ellipses.Add(elOszRefOK);
            ellipses.Add(elAngRefRun);
            ellipses.Add(elAngRefOK);
            ellipses.Add(elBackStopRefRun);
            ellipses.Add(elBackStopRefOK);
            ellipses.Add(elFeedRefRun);
            ellipses.Add(elFeedRefOK);
            ellipses.Add(elClampBarRefRun);
            ellipses.Add(elClampBarRefOK);

            startTimer();
        }
    }
}
