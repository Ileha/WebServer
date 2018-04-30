﻿using System;
using Host;
using Host.MIME;
using Host.ConnectionHandlers;
using System.IO;

namespace HTTPHandlers
{
	public class PlainTextMIME : IMIME
	{
		private string[] _file_extensions = { ".txt", ".TXT" };
		public string[] file_extensions { get { return _file_extensions; } }

		public void Handle(ref IConnetion connection) {
			//connection.OutputData.Write(connection.ReadData.data, 0, connection.ReadData.data.Length);
			connection.ReadData.data.CopyTo(connection.OutputData);
            connection.OutputData.Seek(0, SeekOrigin.Begin);
		}
		
		public void Headers(ref Response response, ref Reqest reqest, ref Reader read) {
			response.AddToHeader("Content-Type", "text/plain; charset=UTF-8", AddMode.rewrite);
		}
	}
}
