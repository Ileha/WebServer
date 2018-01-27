﻿using System;
using Host;
using Host.MIME;

namespace HTTPHandlers
{
	public class CSVMIME : IMIME
	{
		private string[] _file_extensions = { ".csv" };
		public string[] file_extensions { get { return _file_extensions; } }

		public void Handle(ref Response response, ref Reqest reqest, ref Reader read) {
			response.AddToHeader("Content-Type", "text/csv");
			response.AddToBody(read.data);
		}
	}
}
