﻿using Host.MIME;
using Host;

namespace MIMEHandlers
{
	public class JSIMIME : IMIME
	{
		private string[] _file_extensions = { ".js" };
		public string[] file_extensions { get { return _file_extensions; } }

		public void Handle(ref Response response, ref Reqest reqest, ref Reader read) {
			response.AddToHeader("Content-Type", "text/javascript");
			response.AddToBody(read.data);   
		}
	}
}
