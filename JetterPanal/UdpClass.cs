using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Windows;

namespace JetterPanal
{
    public class UdpClass
    {
        public UdpClass(IPAddress remoteIPAddress, int PortServer, int PortClient, typeVariable type) {
            remoteIPAddress_ = remoteIPAddress;
            PortServer_ = PortServer;
            PortClient_ = PortClient;
            typeVarEnum = type;

            UDP_Thread = new Thread(new ThreadStart(Thread_UDP));
                try
                {
                    if (!(UDP_Thread.ThreadState == ThreadState.Running))
                    {
                        UDP_Thread = new Thread(new ThreadStart(Thread_UDP));
                        UdpServer = new UdpClient(PortServer);
                        
                        UDP_Thread.Start();
                    }
                }
                catch (Exception ex)
                {
                    UdpServer.Close();
                    UDP_Thread.Abort();
                    MessageBox.Show("Возникло исключение: " + ex.ToString() + "\n  " + ex.Message);
                }
          
        }
         ~UdpClass()
        {

        }

        Thread UDP_Thread;
        UdpClient UdpServer;       

        private static IPAddress remoteIPAddress_ = IPAddress.Parse("192.168.48.70");
        private static int PortServer_ = 50002;
        private static int PortClient_ = 50000;

        public enum typeVariable
        {
            typeFloat,
            typeInt
        }

        typeVariable typeVarEnum;

        List<float> valueFloat = new List<float>();
        List<int> valueInt = new List<int>();
        List<byte> valueByte = new List<byte>();

        public List<float> getFloatList()
        {
            return valueFloat;
        }
        public List<int> getIntList()
        {
            return valueInt;
        }
        public List<byte> getByteList() //read state flags
        {
            return valueByte;
        }

        public void sendUDP(byte[] bytes)
        {
            // Создаем endPoint по информации об удаленном хосте
            IPEndPoint endPoint = new IPEndPoint(remoteIPAddress_, PortClient_);
            UdpClient UdpClientObj = new UdpClient();
            try
            {             
                UdpClientObj.Send(bytes, bytes.Length, endPoint);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникло исключение: " + ex.ToString() + "\n  " + ex.Message);
            }
            finally
            {
                // Закрыть соединение
                UdpClientObj.Close();
            }
        }
        private void Thread_UDP()
        {
            IPEndPoint RemoteIpEndPoint = null;

            while (true)
            {
                try
                {
                    // Ожидание дейтаграммы
                    byte[] receiveBytes = UdpServer.Receive(
                       ref RemoteIpEndPoint);

                    valueFloat.Clear();
                    valueInt.Clear();
                    valueByte.Clear();

                    if (BitConverter.ToString(receiveBytes, 0, 4) == "4A-57-49-50")
                    {
                        List<int> place = new List<int>();
                        List<int> type = new List<int>();

                        //определяем стартовую позицию в пакете байт для разных типов переменных
                        for (int j = 20; j < receiveBytes.Length; ++j)
                        {
                            //для int и float
                            if((receiveBytes[j] == 0x0c || receiveBytes[j] == 0x0b) && receiveBytes[j - 1] == 0x20)
                            {
                                place.Add(j + 1);
                                type.Add(4);
                            }
                            //для флагов
                            if ((receiveBytes[j] == 0x20 || receiveBytes[j] == 0x21) && (receiveBytes[j - 1] == 0x01 ||
                               receiveBytes[j - 1] == 0x00) && receiveBytes[j - 2] == 0x00)
                            {
                                place.Add(j);
                                type.Add(1);
                            }
                        }

                        for (int i = 0; i < place.Count; ++i)
                        {
                            Array.Reverse(receiveBytes, place[i], type[i]);
                            if (type[i] == 4)
                            {
                                if(receiveBytes[place[i] - 1] == 0x0c)
                                {
                                    valueFloat.Add(BitConverter.ToSingle(receiveBytes, place[i]));
                                }else{
                                    valueInt.Add(BitConverter.ToInt32(receiveBytes, place[i]));
                                }                             
                            }else
                            {
                                valueByte.Add(receiveBytes[place[i]]);
                            }

                        }                    
                              
                    }
                    UDP_Thread.Join(2);
                }
                catch (ThreadAbortException ex)
                {
                    break;
                }
            }
        }
        public void closeUDP()
        {
            if (UDP_Thread.ThreadState == ThreadState.Running)
            {
                UDP_Thread.Abort();
                UdpServer.Close();             
            }
        }
    }
}
