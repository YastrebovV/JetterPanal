using System;
using System.Linq;
using System.Collections.Generic;

namespace JetterPanal
{
    public class WorkWithTags
    {
        public WorkWithTags()
        {

        }

        private byte[] combine(byte[] first, byte[] second)
        {
            return first.Concat(second).ToArray();
        }

        public void setTag(int address, byte type, float value, UdpClass udp)
        {
            byte[] skeleton = {0x4a, 0x57, 0x49, 0x50, 0x00, 0x01,
            0x00, 0x02, 0x00, 0x00, 0x1e, 0xf8, 0x00, 0x00, 0xc3, 0x51, 0x00, 0x01, 0x00, 0x00, 0x7d };

            byte[] b_address = BitConverter.GetBytes(address);
            Array.Reverse(b_address);
            byte[] b_type = new byte[1];
            b_type[0] = type;
            byte[] b_value = null;
            if (type == 0x0a)
            {
                int temp = Convert.ToInt32(value);
                b_value = BitConverter.GetBytes(temp);
            }
            else if(type == 0x0c) {
                b_value = BitConverter.GetBytes(value);
            }
           
            Array.Reverse(b_value);

            byte[] temp1 = combine(skeleton, b_address);
            byte[] temp2 = combine(temp1, b_type);
            byte[] temp3 = combine(temp2, b_value);

            udp.sendUDP(temp3);
        }
        public void setTag(int address, byte state, UdpClass udp)
        {

            byte[] skeleton = {0x4a, 0x57, 0x49, 0x50, 0x00, 0x01,
            0x00, 0x02, 0x00, 0x00, 0x1e, 0xf8, 0x00, 0x00, 0xc3, 0x51, 0x00, 0x01, 0x00, 0x00 };

            byte[] b_address = BitConverter.GetBytes(address);
            Array.Reverse(b_address);
            byte[] b_state= new byte[2];
            b_state[0] = state;
            b_state[1] = 0x00;

            byte[] sendArr = combine(skeleton, b_state);
            sendArr = combine(sendArr, b_address);

            udp.sendUDP(sendArr);
        }
        public void reqGetTags(List<int> addresses, UdpClass udp)
        {
            byte[] skeleton = {0x4a, 0x57, 0x49, 0x50, 0x00, 0x01,
                0x00, 0x02, 0x00, 0x00, 0x1e, 0xf8, 0x00, 0x00, 0xc3, 0x52, 0x00, 0x01};

            byte[] b_addresses = combine(skeleton, new byte[0]);

            foreach (var adr in addresses)
            {
                byte[] b_address = BitConverter.GetBytes(adr);
                Array.Reverse(b_address);

                if(adr < 1000)//flags
                {
                    byte[] betweenVar = { 0x00, 0x03, 0x4b };
                    byte[] twoByte = new byte [2];
                    twoByte[0] = b_address[2];
                    twoByte[1] = b_address[3];

                    b_addresses = combine(b_addresses, betweenVar);
                    b_addresses = combine(b_addresses, twoByte);
                } else if(adr > 10000000) //входа
                {
                    byte[] betweenVar = { 0x00, 0x0a, 0x85, 0x00 };
                    byte[] zero = { 0x00, 0x00, 0x00, 0x00 };
                    b_addresses = combine(b_addresses, betweenVar);
                    b_addresses = combine(b_addresses, b_address);
                    b_addresses = combine(b_addresses, zero);
                }
                else
                {
                    byte[] betweenVar = { 0x00, 0x05, 0x7c };
                    b_addresses = combine(b_addresses, betweenVar);
                    b_addresses = combine(b_addresses, b_address);
                }                           
            }

            udp.sendUDP(b_addresses);
        }
    }
}