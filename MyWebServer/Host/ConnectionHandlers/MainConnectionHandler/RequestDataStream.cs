using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Host.ConnectionHandlers
{
    public class RequestDataStream : Stream
    {
        private byte[] _data;
        private long _index;
        private NetworkStream _client;

        public RequestDataStream(byte[] data, NetworkStream client)
        {
            _index = 0;
            _data = data;
            _client = client;
        }

        public override bool CanRead { get { throw new NotImplementedException(); } }
        public override bool CanSeek { get { return false; } }
        public override bool CanWrite { get { return false; } }
        public override long Length { get { throw new NotImplementedException(); } }
        public override long Position { get { return _index; } set { throw new NotImplementedException(); } }
        public override void Flush() { throw new NotImplementedException(); }
        protected override void Dispose(bool disposing) {
            _client.Dispose();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int res = 0;
            try
            {
                for (int i = 0; i < count; i++)
                {
                    buffer[_index] = _data[i + offset];
                    _index++;
                    res++;
                }
            }
            catch (Exception err)
            {
                int k = _client.Read(buffer, res + offset, count - res);
                res += k;
                _index += k;
            }
            return res;
        }

        public override long Seek(long offset, SeekOrigin origin) { throw new NotImplementedException(); }
        public override void SetLength(long value) { throw new NotImplementedException(); }
        public override void Write(byte[] buffer, int offset, int count) { throw new NotImplementedException(); }

        public static bool ExistSeqeunce(int start, int count, byte[] sequence, IEnumerable<byte> array, out int index)
        {
            int seq_i = 0;
            for (int i = start; i < Math.Min(count, array.Count() - start); i++)
            {
                if (array.ElementAt(i) != sequence[seq_i])
                {
                    i = i - seq_i;
                    seq_i = 0;
                }
                else
                {
                    if (seq_i == sequence.Length - 1)
                    {
                        index = i - seq_i;
                        return true;
                    }
                    seq_i++;
                }
            }
            index = -1;
            return false;
        }
    }
}
