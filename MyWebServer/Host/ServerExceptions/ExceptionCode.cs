﻿using System;
using System.Text;

namespace Host.ServerExceptions
{
	public abstract class ExceptionCode : Exception
	{
		protected string Code;
		protected bool _IsFatal;
		public bool IsFatal {
			get { return _IsFatal; }
		}

		public virtual void ExceptionHandle(ref Reqest request, ref Response response) {
			response.AddToHeader("Content-Type", "text/html; charset=UTF-8");
			response.AddToBody("<html><body><h2>An error has occurred code of error " + Code + "</h2></body></html>");
		}

		public string GetExeptionCode() {
			return Code;
		}
	}		
}
