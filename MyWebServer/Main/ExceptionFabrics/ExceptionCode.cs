﻿using System;
using System.Text;
using RequestHandlers;
using DataHandlers;
using Configurate.Host.Connection.HTTPConnection;
using Configurate.Host.Connection;

namespace ExceptionFabric
{
	public abstract class ExceptionCode : Exception
	{
		protected string Code;
        public virtual void ExceptionHandleCode(Response response, IConnetion data) {
            response.Clear();
            response.AddToHeader("Content-Type", "text/html; charset=UTF-8", AddMode.rewrite);
			byte[] _data = Encoding.UTF8.GetBytes("<html><body><h2>An error has occurred code of error " + Code + "</h2></body></html>");
            data.OutputData.Write(_data, 0, _data.Length);
        }
		public string GetExeptionCode() {
			return Code;
		}
	}		
}
