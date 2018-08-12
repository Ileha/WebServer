using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Configurate.Host.Connection.HTTPConnection
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
                    buffer[offset+i] = _data[_index];
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
    }
}
