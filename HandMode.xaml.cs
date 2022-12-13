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
using System.Threading;

namespace JetterPanal
{
    /// <summary>
    /// Логика взаимодействия для HandMode.xaml
    /// </summary>
    public partial class HandMode : Window
    {
        public HandMode(UdpClass udp, Window main)
        {
            InitializeComponent();
            udp_ = udp;
            main_ = main;
        }                              
                                                              
        List<int> addressVariables = new List<int>() { 1001010 /*bits | [0]*/, 1000111/*ActZero | [1]*/, 1001106 /*ActAng | [2]*/,
            1000109/*ActFeed | [3]*/, 1001105/*Thickness | [4]*/, 1001101/*Oszillation | [5]*/, 1001012/*bits | [6]*/, 1001013 /*bits*/, 1000100/*side A*/,
        1000113/*BackstopPos*/, 1000112/*Adjastment set*/, 1000005/*Angle set*/, 1000004/*Target pos*/, 1001120/*Upper Limit*/, 1001121/*Low Limit*/,
            171/*State motor*/, 100000402/*ProtectMotor*/, 100000211/*BeltTens*/, 100000212/*SensorTop*/, 100000213/*SensorBotton*/,
            154/*correct value*/, 181/*no release*/};

        List<BitArray> bitArr = new List<BitArray>();
        List<Ellipse> ellipses = new List<Ellipse>();

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
            timerUpdateData.Enabled = false;
        }
        private void TimerUpdate(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                new Action(() => {
                    this.timerUpdateData.Stop();                 
                    tags.reqGetTags(addressVariables, udp_);
                    List<float>  floatList = udp_.getFloatList();
                    List<int> intTagList = udp_.getIntList();
                    List<byte> flags = udp_.getByteList();
                    
                    bitArr.Clear();

                    if (floatList != null && floatList.Count != 0) //float var
                    {
                        try
                        {
                            tbActZero.Text = Convert.ToString(floatList[0]); //1000111
                            tbActFeed.Text = Convert.ToString(floatList[1]); //1000109                        
                            tbBackPos.Text = Convert.ToString(floatList[3]); //1000113

                            if (!firstCycleFloat)
                            {
                                tbSideA.Text = Convert.ToString(floatList[2]); //1000100
                                tbSetZero.Text = Convert.ToString(floatList[4]); //1000112
                                tbSetAng.Text = Convert.ToString(floatList[5]); //1000005
                                tbTargetPos.Text = Convert.ToString(floatList[6]); //1000004
                                firstCycleFloat = true;
                            }
                        }
                        catch(Exception ex)
                        {
                           // MessageBox.Show(ex.Message);
                        }
                    }

                    if (intTagList != null && intTagList.Count != 0)
                    {
                        try
                        {                        
                            tbActAng.Text = Convert.ToString(intTagList[1]);  //1001106
                            tbActAngl2.Text = Convert.ToString(intTagList[1]);  //1001106
                            tbThickness.Text = Convert.ToString(intTagList[2]); //1001105
                            tbOsz.Text = Convert.ToString(intTagList[3]); //1001101 
                            tbActPosOsz.Text = Convert.ToString(intTagList[3]); //1001101

                            if (!firstCycleInt)
                            {
                                tbUpLim.Text = Convert.ToString(intTagList[6]); //1001120
                                tbLowLim.Text = Convert.ToString(intTagList[7]); //1001121
                                tbUpLim2.Text = Convert.ToString(intTagList[6]); //1001120
                                tbLowLim2.Text = Convert.ToString(intTagList[7]); //1001121
                                firstCycleInt = true;
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
                            if (flags[0] == 0x21)/*State motor*/
                            {
                                ellipses[0].Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF50BB3F");
                            }
                            else
                            {
                                ellipses[0].Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFC95634");
                            }

                            if (flags[1] == 0x21)/*ProtectMotor*/
                            {
                                ellipses[1].Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF50BB3F");
                            }
                            else
                            {
                                ellipses[1].Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFC95634");
                            }

                            if (flags[2] == 0x21)/*BeltTens*/
                            {
                                ellipses[2].Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF50BB3F");
                            }
                            else
                            {
                                ellipses[2].Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFC95634");
                            }

                            if (flags[3] == 0x21)/*SensorTop*/
                            {
                                ellipses[3].Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF50BB3F");
                            }
                            else
                            {
                                ellipses[3].Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFC95634");
                            }

                            if (flags[4] == 0x21)/*SensorBotton*/
                            {
                                ellipses[4].Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF50BB3F");
                            }
                            else
                            {
                                ellipses[4].Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFC95634");
                            }

                            if (flags[5] == 0x21)/*correct value*/
                            {
                                 pnIncVal.Visibility = Visibility.Hidden;
                            }
                            else
                            {
                                pnIncVal.Visibility = Visibility.Visible;
                            }

                            if (flags[6] == 0x21)/*no release*/
                            {
                                pnNoRelease.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                pnNoRelease.Visibility = Visibility.Hidden;
                            }
                            //for (int i = 0; i < flags.Count(); ++i)
                            //{
                            //    if (i == 5)//Panel incorrect value
                            //    {
                            //        if (flags[i] == 0x21)
                            //        {
                            //            pnIncVal.Visibility = Visibility.Hidden;
                            //        }
                            //        else
                            //        {
                            //            pnIncVal.Visibility = Visibility.Visible;
                            //        }
                            //    }
                            //    else if (i == 6) //Panel no release
                            //    {
                            //        if (flags[i] == 0x21)
                            //        {
                            //            pnNoRelease.Visibility = Visibility.Visible;
                            //        }
                            //        else
                            //        {
                            //            pnNoRelease.Visibility = Visibility.Hidden;
                            //        }
                            //    }
                            //    else
                            //    {
                            //        if (flags[i] == 0x21)
                            //        {
                            //            ellipses[i].Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF50BB3F");
                            //        }
                            //        else
                            //        {
                            //            ellipses[i].Fill = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFC95634");
                            //        }
                            //    }
                            //}
                        }
                        catch
                        {
                        }
                    }

                    if (intTagList != null && intTagList.Count != 0)
                    {
                        try
                        {
                            bitArr.Add(new BitArray(new int[] { BitConverter.ToInt32(BitConverter.GetBytes(intTagList[0]), 0) })); //1001010
                            bitArr.Add(new BitArray(new int[] { BitConverter.ToInt32(BitConverter.GetBytes(intTagList[4]), 0) })); //1001012
                            bitArr.Add(new BitArray(new int[] { BitConverter.ToInt32(BitConverter.GetBytes(intTagList[5]), 0) })); //1001013

                            if (bitArr[1][0] == true) // Panel oszillation off
                            {
                                pnOszOff.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                pnOszOff.Visibility = Visibility.Hidden;
                            }
                        }
                        catch
                        {

                        }
                    }
                
                    this.timerUpdateData.Start();
                }));
        }

        void HandModeClosed(object sender, EventArgs e)
        {
            main_.Visibility = Visibility.Visible;
        }

        private static int ToNumeral(BitArray binary)
        {
            var toInt = new int[1];
            binary.CopyTo(toInt, 0);
            return toInt[0];
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ellipses.Add(elStateMotor);
            ellipses.Add(elStateProtMot);
            ellipses.Add(elStateBeltTens);
            ellipses.Add(elStateBeltRupTop);
            ellipses.Add(elStateBeltRupBot);
            startTimer();
        }

        private void tbSetZero_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                timerUpdateData.Stop();
                tags.setTag(1000112, 0x0c, Convert.ToSingle(tbSetZero.Text), udp_); //set value zero position
                timerUpdateData.Start();
            }
        }
        private void tbSetAng_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                timerUpdateData.Stop();
                tags.setTag(1000005, 0x0c, Convert.ToSingle(tbSetAng.Text), udp_); //set value angle
                timerUpdateData.Start();
            }
        }

        private void btStartToZero_Click(object sender, RoutedEventArgs e)
        {
            if (bitArr != null)
            {            
                timerUpdateData.Stop();
                bitArr[1][3] = true;
                tags.setTag(1001012, 0x0a, Convert.ToSingle(ToNumeral(bitArr[1])), udp_); //start to zero position
                timerUpdateData.Start();
            }
        }
        private void btStartToAngle_Click(object sender, RoutedEventArgs e)
        {
            timerUpdateData.Stop();
            tags.setTag(155, 0x4c, udp_); //start to angle drive (flag - 155)
            timerUpdateData.Start();
        }

        private void tbTargetPos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                timerUpdateData.Stop();
                tags.setTag(1000004, 0x0c, Convert.ToSingle(tbTargetPos.Text), udp_);   //Set the target position.  0x0c - var type float
                timerUpdateData.Start();
            }
        }
        private void tbUpLim_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                timerUpdateData.Stop();
                tags.setTag(1001120, 0x0a, Convert.ToSingle(tbUpLim.Text), udp_);   //Set the pos upper limit switch.  0x0a - var type decimal
                timerUpdateData.Start();
            }
        }
        private void tbLowLim_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                timerUpdateData.Stop();
                tags.setTag(1001121, 0x0a, Convert.ToSingle(tbLowLim.Text), udp_);   //Set the pos lower limit switch.  0x0a - var type decimal
                timerUpdateData.Start();
            }
        }
        private void tbSideA_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                timerUpdateData.Stop();
                tags.setTag(1000100, 0x0c, Convert.ToSingle(tbLowLim.Text), udp_);    //Set the Side A.  0x0c - var type float
                timerUpdateData.Start();
            }
        }

        private void btChPlateThich_Click(object sender, RoutedEventArgs e)
        {
            timerUpdateData.Stop();
            tags.setTag(136, 0x4c, udp_); //change plate thickness (flag - 136)
            timerUpdateData.Start();
        }
        private void btChOszillation_Click(object sender, RoutedEventArgs e)
        {
            if (bitArr != null)
            {                
                timerUpdateData.Stop();
                bitArr[2][0] = true;
                tags.setTag(1001013, 0x0a, Convert.ToSingle(ToNumeral(bitArr[2])), udp_); //change oszillation
                timerUpdateData.Start();
            }
        }
        private void btZeroToZero_Click(object sender, RoutedEventArgs e)
        {
            if (bitArr != null)
            {             
                timerUpdateData.Stop();
                bitArr[1][2] = true;
                tags.setTag(1001012, 0x0a, Convert.ToSingle(ToNumeral(bitArr[1])), udp_); //zero to zero ?
                timerUpdateData.Start();
            }
        }
        private void btFeedZero_Click(object sender, RoutedEventArgs e)
        {
            if (bitArr != null)
            {             
                timerUpdateData.Stop();
                bitArr[1][1] = true;
                tags.setTag(1001012, 0x0a, Convert.ToSingle(ToNumeral(bitArr[1])), udp_); //feed to zero ?
                timerUpdateData.Start();
            }
        }
        private void btCalc_Click(object sender, RoutedEventArgs e)
        {
            if (bitArr != null)
            {              
                timerUpdateData.Stop();
                bitArr[1][4] = true;
                tags.setTag(1001012, 0x0a, Convert.ToSingle(ToNumeral(bitArr[1])), udp_); //zero to zero ?
                timerUpdateData.Start();
            }
        }
        private void btMoveUp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (bitArr != null)
            {           
                timerUpdateData.Stop();
                bitArr[2][1] = true;
                tags.setTag(1001013, 0x0a, Convert.ToSingle(ToNumeral(bitArr[2])), udp_); 
                timerUpdateData.Start();
            }
        }
        private void btMoveUp_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (bitArr != null)
            {
                timerUpdateData.Stop();
                bitArr[2][1] = false;
                tags.setTag(1001013, 0x0a, Convert.ToSingle(ToNumeral(bitArr[2])), udp_); 
                timerUpdateData.Start();
            }
        }

        private void btMoveDown_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (bitArr != null)
            {
                timerUpdateData.Stop();
                bitArr[2][2] = true;
                tags.setTag(1001013, 0x0a, Convert.ToSingle(ToNumeral(bitArr[2])), udp_);
                timerUpdateData.Start();
            }
        }
        private void btMoveDown_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (bitArr != null)
            {
                timerUpdateData.Stop();
                bitArr[2][2] = false;
                tags.setTag(1001013, 0x0a, Convert.ToSingle(ToNumeral(bitArr[2])), udp_);
                timerUpdateData.Start();
            }
        }
        private void btSaveUpPos_Click(object sender, RoutedEventArgs e)
        {
            if (bitArr != null)
            {
                timerUpdateData.Stop();
                bitArr[2][3] = true;
                tags.setTag(1001013, 0x0a, Convert.ToSingle(ToNumeral(bitArr[2])), udp_);
                timerUpdateData.Start();
            }
        }
        private void btSaveLowPos_Click(object sender, RoutedEventArgs e)
        {
            if (bitArr != null)
            {
                timerUpdateData.Stop();
                bitArr[2][4] = true;
                tags.setTag(1001013, 0x0a, Convert.ToSingle(ToNumeral(bitArr[2])), udp_);
                timerUpdateData.Start();
            }
        }


    }
}
