﻿using Host.MIME;
using Host;
using Host.ConnectionHandlers;

namespace MIMEHandlers
{
	public class CSSIMIME : IMIME
	{
		private string[] _file_extensions = { ".css" };
		public string[] file_extensions { get { return _file_extensions; } }

        //public void Handle(ref Response response, ref Reqest reqest, ref Reader read) {
        //    response.AddToHeader("Content-Type", "text/css; charset=UTF-8", AddMode.rewrite);
        //    response.AddToBody(read.data);
        //}


        public void Headers(ref Response response, ref Reqest reqest, ref Reader read) {
            throw new System.NotImplementedException();
        }

        public void Handle(ref IConnetion connection) {
            connection.
        }
    }
}
