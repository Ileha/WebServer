using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.IO;
using Configurate.Host.Connection.Exceptions;
using RequestHandlers;

namespace Configurate.Host.Connection.HTTPConnection
{

    public class Reqest : IDisposable
    {
        private static byte[] new_line = new byte[] { 13, 10 };

        public static Reqest Create(TcpClient client, out Func<Stream> Input)
        {
            Reqest res = new Reqest(client);
            Input = () => res.Data;
            return res;
        }

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

        public string URL;
        public MemoryStream Data { get; private set; }
        public Dictionary<string, string> headers { get; private set; }//заголовки
        public Dictionary<string, string> cookies { get; private set; }

        public Reqest(TcpClient client)
        {
            headers = new Dictionary<string, string>();
            cookies = new Dictionary<string, string>();
            Data = new MemoryStream();
            Reqest result = this;

            MemoryStream _receive_data = new MemoryStream();
            byte[] bytes = new byte[256];
            int _index = -1;
            int additive = 0;
            NetworkStream _receive = client.GetStream();

            do {
                int _count = 0;
                try { _count = _receive.Read(bytes, 0, bytes.Length); } catch (Exception err) { throw new ConnectionExecutorClose(); }
                if (_count == 0) { throw new ConnectionExecutorClose(); }
                _receive_data.Write(bytes, 0, _count);
                ExistSeqeunce(0, _count, new_line, bytes, out _index);
                if (_index == -1) {
                    additive += _count;
                }
                else {
                    _index += additive;
                }
            } while (_index == -1);
            bytes = new byte[_receive_data.Length];
            _receive_data.Seek(0, SeekOrigin.Begin);
            _receive_data.Read(bytes, 0, bytes.Length);
            _receive_data.Dispose();
            string first_row = Encoding.UTF8.GetString(bytes, 0, _index);
            string[] header = first_row.Split(' ');
            ABSHttpHandler _handler = Repository.ReqestsHandlers[header[0] + header[2]];
            RequestDataStream stream = new RequestDataStream(bytes, _receive);
            _handler.ParseHeaders(ref result, stream);
            Data.Seek(0, SeekOrigin.Begin);
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
            throw Repository.ExceptionFabrics["Moved Permanently"].Create(new_url);
        }

        public void Dispose()
        {
            if (Data != null) {
                Data.Dispose();
            }
        }
    }
}
