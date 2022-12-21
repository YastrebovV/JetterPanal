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
    /// Логика взаимодействия для AutoScreen.xaml
    /// </summary>
    public partial class AutoNewPlate : Window
    {
        public AutoNewPlate(UdpClass udp, Window main)
        {
            InitializeComponent();
            udp_ = udp;
            main_ = main;
        }

        List<int> addressVariables = new List<int>() { 1000100, 1000119, 1000130, 1000101, 1001015, 1001010, /*screen 2*/ 1000113, 1001140, 1001128, 1001001,
        /*screen 3*/1000118, 148, 1001017};
        List<BitArray> bitArr = new List<BitArray>();

        WorkWithTags tags = new WorkWithTags();
        UdpClass udp_;
        Window main_;
        bool firstCycleFloat = false;
        bool firstCycleInt = false;

        System.Timers.Timer timerUpdateData = new System.Timers.Timer(1000);

        private void startTimer()
        {
            timerUpdateData.Enabled = true;
            timerUpdateData.Elapsed += TimerUpdate;
        }
        private void stopTimer()
        {
            timerUpdateData.Stop();
            timerUpdateData.Enabled = false;
        }
        private void TimerUpdate(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                new Action(() => {
                   
                    tags.reqGetTags(addressVariables, udp_);
                    List<float> floatList = udp_.getFloatList();
                    List<int> intTagList = udp_.getIntList();
                    List<byte> flags = udp_.getByteList();

                    bitArr.Clear();

                    if (floatList != null && floatList.Count != 0) //float var
                    {
                        try
                        {
                            tbBackPos.Text = Convert.ToString(floatList[5]); //1000140

                            if (!firstCycleFloat)
                            {
                                tbBelWidth.Text = Convert.ToString(floatList[0]); //1000100
                                tbAngel.Text = Convert.ToString(floatList[1]); //1000119
                                tbPlateThick.Text = Convert.ToString(floatList[3]); //1000101
                                tbRemConst.Text = Convert.ToString(floatList[2]); //1000130
                                tbAdjOffset.Text = Convert.ToString(floatList[4]);//1000118
                                firstCycleFloat = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            // MessageBox.Show(ex.Message);
                        }
                    }

                    if (intTagList != null && intTagList.Count != 0)
                    {
                        try
                        {
                            if (!firstCycleInt)
                            {
                                tbEndPlate.Text = Convert.ToString(intTagList[2]);//1001140
                                tbPass.Text = Convert.ToString(intTagList[3]);//1001128
                                firstCycleInt = true;
                            }

                            lbMess.DataContext = "";

                            if(intTagList[3] == 32)
                            {
                                lbMess.Items.Add("Start with Button \"Basic-Position\"");
                            }

                            bitArr.Add(new BitArray(new int[] { BitConverter.ToInt32(BitConverter.GetBytes(intTagList[0]), 0) })); //1001015
                            bitArr.Add(new BitArray(new int[] { BitConverter.ToInt32(BitConverter.GetBytes(intTagList[1]), 0) })); //1001010
                            bitArr.Add(new BitArray(new int[] { BitConverter.ToInt32(BitConverter.GetBytes(intTagList[4]), 0) })); //1001017
                          
                            lbError.Items.Clear();

                            if (bitArr[0][3])
                            {
                                lbError.Items.Add("Bevel too large");
                            }
                            if (bitArr[0][2])
                            {
                                lbError.Items.Add("Bevel-Width/Plate-Thickness");
                            }
                            if (bitArr[0][13])
                            {
                                lbError.Items.Add("Calculation wrong");
                            }
                        }
                        catch (Exception ex)
                        {
                            // MessageBox.Show(ex.Message);
                        }
                    }
                    if (flags != null && flags.Count != 0)
                    {
                        try
                        {
                            if (flags[0] == 0x21)
                            {
                                lbMess.Items.Add("Basic Position OK");
                                lbMess.Items.Add("Start with Button \"Clamping down\"");
                            }
                        }
                        catch (Exception ex)
                        {
                            // MessageBox.Show(ex.Message);
                        }
                    }
                        
                }));
        }

        private static int ToNumeral(BitArray binary)
        {
            var toInt = new int[1];
            binary.CopyTo(toInt, 0);
            return toInt[0];
        }

        private void btCalc_Click(object sender, RoutedEventArgs e)
        {
            if (bitArr != null)
            {
               // timerUpdateData.Stop();
                bitArr[0][1] = true;
                tags.setTag(1001015, 0x0a, Convert.ToSingle(ToNumeral(bitArr[0])), udp_); //
              //  timerUpdateData.Start();
            }
        }
        private void tbBelWidth_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
             //   timerUpdateData.Stop();
                tags.setTag(1000100, 0x0c, Convert.ToSingle(tbBelWidth.Text), udp_); // set Bevel-Width (a)
             //   timerUpdateData.Start();
            }
        }
        private void tbAngel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
              //  timerUpdateData.Stop();
                tags.setTag(1000119, 0x0c, Convert.ToSingle(tbAngel.Text), udp_); // set Angle
              //  timerUpdateData.Start();
            }
        }
        private void tbPlateThick_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
              //  timerUpdateData.Stop();
                tags.setTag(1000101, 0x0c, Convert.ToSingle(tbPlateThick.Text), udp_); // set Plate-Thickness
              //  timerUpdateData.Start();
            }
        }
        private void tbRemConst_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
             //   timerUpdateData.Stop();
                tags.setTag(1000130, 0x0c, Convert.ToSingle(tbRemConst.Text), udp_); // set Removal-Constant
             //   timerUpdateData.Start();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (bitArr != null)
            {
             //   timerUpdateData.Stop();
                bitArr[1][9] = true;
                tags.setTag(1001010, 0x0a, Convert.ToSingle(ToNumeral(bitArr[1])), udp_);
             //   timerUpdateData.Start();
            }

            stopTimer();
            timerUpdateData.Dispose();
            main_.Visibility = Visibility.Visible;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            startTimer();
        }

        private void btConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (bitArr != null)
            {
             //   timerUpdateData.Stop();
                bitArr[0][14] = true;
                tags.setTag(1001015, 0x0a, Convert.ToSingle(ToNumeral(bitArr[0])), udp_);
             //   timerUpdateData.Start();
            }
        }

        private void tbEndPlate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
             //   timerUpdateData.Stop();
                tags.setTag(1001140, 0x0a, Convert.ToSingle(tbEndPlate.Text), udp_); // 
             //   timerUpdateData.Start();
            }
        }
        private void tbPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
             //   timerUpdateData.Stop();
                tags.setTag(1001128, 0x0a, Convert.ToSingle(tbPass.Text), udp_); //
             //   timerUpdateData.Start();
            }
        }

        private void tbAdjOffset_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
             //   timerUpdateData.Stop();
                tags.setTag(1000118, 0x0c, Convert.ToSingle(tbPass.Text), udp_); //Adjstment offset
             //   timerUpdateData.Start();
            }
        }

        private void btSavePlate_Click(object sender, RoutedEventArgs e)
        {
            if (bitArr != null)
            {
              //  timerUpdateData.Stop();
                bitArr[2][2] = true;
                tags.setTag(1001017, 0x0a, Convert.ToSingle(ToNumeral(bitArr[2])), udp_); //save plate
              //  timerUpdateData.Start();
            }
        }
    }
}
