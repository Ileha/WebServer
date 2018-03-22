﻿using System;
using Host.MIME;
using Host;
using Host.ConnectionHandlers;

namespace MIMEHandlers
{
	public class XMLMIME : IMIME
	{
		private string[] _file_extensions = { ".xml" };
		public string[] file_extensions { get { return _file_extensions; } }

		public void Handle(ref IConnetion connection) {
			connection.OutputData.Write(connection.ReadData.data, 0, connection.ReadData.data.Length);
		}

		public void Headers(ref Response response, ref Reqest reqest, ref Reader read) {
			response.AddToHeader("Content-Type", "text/xml", AddMode.rewrite);
		}
	}
}
