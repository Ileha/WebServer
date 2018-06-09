using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Host.HttpHandler;
using Host.ServerExceptions;
using System.Net.Sockets;
using System.IO;
using Host.ConnectionHandlers.ExecutorExceptions;

namespace Host.ConnectionHandlers
{
    public class ReqestDataStream : Stream
    {
        private byte[] _data;
        private long _lenght;
        private long _index;
        private NetworkStream _client;

        public ReqestDataStream(byte[] data, long lenght_client, NetworkStream client)
        {
            _index = 0;
            _data = data;
            _lenght = lenght_client;
            _client = client;
        }

        public override bool CanRead
        {
            get
            {
                if (_index >= _lenght) { return false; }
                else { return true; }
            }
        }
        public override bool CanSeek { get { return false; } }
        public override bool CanWrite { get { return false; } }
        public override long Length { get { return _lenght; } }
        public override long Position { get { return _index; } set { throw new NotImplementedException(); } }
        public override void Flush() { throw new NotImplementedException(); }
        protected override void Dispose(bool disposing)
        { //убирает данные из потока 
            if (disposing)
            {
                byte[] d = new byte[Length-_index];
                Read(d, 0, d.Length);
            }
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
    }

    public class Reqest
    {
        private static byte[] new_line = new byte[] { 13, 10, 13, 10 };

        public string URL;
        public MemoryStream Data;
        public Dictionary<string, string> varibles;//данные
        public Dictionary<string, string> preferens;//заголовки
        public Dictionary<string, string> cookies;

        public Reqest(TcpClient client)
        {
            //if (client.Available == 0) { throw new ConnectionExecutorClose(); }

            varibles = new Dictionary<string, string>();
            preferens = new Dictionary<string, string>();
            cookies = new Dictionary<string, string>();

            List<byte> input_data = new List<byte>();
            byte[] bytes = new byte[1024];
            NetworkStream stream = client.GetStream();
            int index = -1;
            do {
                int count = client.Client.Receive(bytes);
                if (index == -1)
                {
                    ExistSeqeunce(0, count, new_line, bytes, out index);
                    if (index != -1) { index = input_data.Count + index; }
                }
                input_data.AddRange(bytes.Take(count));
            } while (stream.DataAvailable);
            if (input_data.Count == 0) { throw new ConnectionExecutorClose(); }
            index = Math.Max(0, index);
            bytes = input_data.ToArray();

            string headers_data = Encoding.UTF8.GetString(bytes, 0, index);
            Reqest result = this;
            string[] elements = Regex.Split(headers_data, "\r\n");
            try
            {
                string[] header = elements[0].Split(' ');
                ABSHttpHandler _handler = Repository.ReqestsHandlers[header[0] + header[2]];
                _handler.ParseHeaders(ref result, elements.ToList().GetRange(1, elements.Length - 1).ToArray(), header[1]);
                if (_handler.CanHasData(this) && index+4 > bytes.Length)
                {
                    Data = new MemoryStream(bytes, index+4, bytes.Length-(index+4));
                    Data.Seek(0, SeekOrigin.Begin);
                }
            }
            catch (ExceptionCode code) {
                throw code;
            }
            catch (Exception err) {
                throw Repository.ExceptionFabrics["Bad Request"].Create(null, null);
            }
        }

        public void CheckTabelOfRedirect()
        {
            string new_url;
            try
            {
                new_url = Repository.Configurate.RedirectConfigure.GetTargetRedirect(URL);
            }
            catch (Exception err)
            {
                return;
            }
            throw Repository.ExceptionFabrics["Moved Permanently"].Create(null, new_url);
        }

        private static bool ExistSeqeunce(int start, int count, byte[] sequence, IEnumerable<byte> array, out int index)
        {
            int seq_i = 0;
            for (int i = start; i < Math.Min(count, array.Count()-start); i++)
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
