﻿using System;
using Config;
using Host.ConnectionHandlers;

namespace Host.MIME
{
    public class HttpMIME : IMIME {
		private string[] _file_extensions = { ".html" };
        public string[] file_extensions { get { return _file_extensions; } }

		public void Handle(ref Response response, ref Reqest reqest, ref Reader read) {
			response.AddToHeader("Content-Type", "text/html; charset=UTF-8", AddMode.rewrite);
			response.AddToBody(read.data);
		}
	}
}