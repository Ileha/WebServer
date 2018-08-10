using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HostInteractive
{
    public class StringOIInterface
    {
        public TcpClient Client { get; private set; }
        private NetworkStream IOStream;
        public StringOIInterface(TcpClient Client) {
            this.Client = Client;
            IOStream = Client.GetStream();
        }

        public string Read() {
            StringBuilder sb = new StringBuilder();
            bool is_end = true;
            byte[] read_buffer = new byte[1024];
            do
            {
                byte[] headers = new byte[3];
                IOStream.Read(headers, 0, 3);
                if (headers[0] == 0xFE) { is_end = false; }
                int lenght = Convert.ToInt32(BitConverter.ToUInt16(headers, 1));
                while (lenght > 0) { 
                    int count = IOStream.Read(read_buffer, 0, lenght);
                    sb.Append(Encoding.UTF8.GetString(read_buffer, 0, count));
                    lenght -= count;
                }
            } while (is_end);
            return sb.ToString();
        }
        public void Write(string message) {
            byte[] msg = Encoding.UTF8.GetBytes(message);
            byte[] headers = new byte[3];
            int lenght = msg.Length;
            int start = 0;
            Action<int> substract_lenght = (num) =>
            {
                lenght = (lenght - num);
                start = (start + num);
            };

            while (lenght != 0) {
                int ulenght = Math.Min(lenght, Int32.MaxValue);
                headers[0] = 0xFE;
                if (lenght-ulenght != 0) { headers[0] = (byte)(headers[0] | 1); }
                byte[] number = BitConverter.GetBytes(Convert.ToUInt16(ulenght));
                headers[1] = number[0];
                headers[2] = number[1];
                IOStream.Write(headers, 0, 3);
                IOStream.Write(msg, start, ulenght);
                substract_lenght(ulenght);
            }
        }
        public void Write(string message, params object[] args)
        {
            Write(string.Format(message, args));
        }
    }
}
