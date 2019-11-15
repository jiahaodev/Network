using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Server
{
    class MsgBase
    {
        public string protoName = "null";

        //编码器
        static JavaScriptSerializer Js = new JavaScriptSerializer();

        //编码
        public static byte[] Encode(MsgBase msgBase)
        {
            string s = Js.Serialize(msgBase);
            return System.Text.Encoding.UTF8.GetBytes(s);
        }

        //解码
        public static MsgBase Decode(string protoName, byte[] bytes, int offset, int count)
        {
            string s = System.Text.Encoding.UTF8.GetString(bytes, offset, count);
            MsgBase msgBase = (MsgBase)Js.Deserialize(s, Type.GetType(protoName));
            return msgBase;
        }


        //编码协议名（2字符串长度 + 字符串）
        public static byte[] EncodeName(MsgBase msgBase)
        {
            //名字bytes和长度
            byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(msgBase.protoName);
            Int16 len = (Int16)nameBytes.Length;
            //申请bytes数组
            byte[] bytes = new byte[2 + len];
            //组装两字节的长度信息（小端）
            bytes[0] = (byte)(len % 256);
            bytes[1] = (byte)(len / 256);
            //组装协议名bytes
            Array.Copy(nameBytes, 0, bytes, 2, len);
            return bytes;

        }

        //解码协议名（2字符串长度 + 字符串）
        public static string DecodeName(byte[] bytes, int offset, out int count)
        {
            count = 0;
            //必须大于2字节
            if (offset + 2 > bytes.Length)
            {
                return "";
            }
            //读取长度
            Int16 len = (Int16)((bytes[offset + 1] << 8 | bytes[offset]));
            if (len <= 0)
            {
                return "";
            }
            //长度必须足够
            if (offset + 2 + len > bytes.Length)
            {
                return "";
            }
            //解析
            count = 2 + len;  // out变量，用于设置外部idx
            string name = System.Text.Encoding.UTF8.GetString(bytes, offset + 2, len);
            return name;
        }

    }
}
