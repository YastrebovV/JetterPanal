using System;
using System.Linq;

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
        private byte[] combine(byte[] first, byte second)
        {
            return first.Concat(second).ToArray();
        }

        void setTag(int address, byte type, float value)
        {
            byte[] skeleton = {0x4a, 0x57, 0x49, 0x50, 0x00, 0x01,
        0x00, 0x02, 0x00, 0x00, 0x1e, 0xf8, 0x00, 0x00, 0xc3, 0x51, 0x00, 0x01, 0x00, 0x00, 0x7d, 0x00,
        0x0f };// 0x42, 0x41, 0x0c, 0x42, 0xf1, 0x33, 0x33};

            byte[] b_address = BitConverter.GetBytes(address);
            byte[] b_value = BitConverter.GetBytes(value);

            byte[] temp1 = combine(skeleton, b_address);
            byte[] temp2 = combine(temp1, type);
            byte[] temp3 = combine(temp2, b_value);

        }

        float getTag(int address, byte type)
        {

        }

    }
}