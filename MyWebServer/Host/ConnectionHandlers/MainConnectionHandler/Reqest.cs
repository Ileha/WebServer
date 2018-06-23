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

    public class Reqest : IDisposable
    {
        private static byte[] new_line = new byte[] { 13, 10 };

        public string URL;
        public MemoryStream Data { get; private set; }
        public Dictionary<string, string> headers { get; private set; }//заголовки
        public Dictionary<string, string> cookies { get; private set; }

        public static Reqest Create(TcpClient client, out Func<Stream> Input) {
            Reqest res = new Reqest(client);
            Input = () => res.Data;
            return res;
        }

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
                int _count = _receive.Read(bytes, 0, bytes.Length);
                if (_count == 0) { throw new ConnectionExecutorClose(); }
                _receive_data.Write(bytes, 0, _count);
                RequestDataStream.ExistSeqeunce(0, _count, new_line, bytes, out _index);
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

        public void Dispose()
        {
            if (Data != null) {
                Data.Dispose();
            }
        }
    }
}
