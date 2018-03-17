﻿using System;
namespace Host.Session
{
	public class UserNotFound : Exception
	{
		private string message;
		public UserNotFound(string message)
		{
			this.message = message;
		}

		public override string ToString()
		{
			return string.Format("id {0} not found", message);
		}
	}
	public class UserDataNotFound : Exception
	{
		private string message;
		public UserDataNotFound(string message)
		{
			this.message = message;
		}

		public override string ToString()
		{
			return string.Format("user data with name {0} not found", message);
		}	
	}
	public class DataTypeException : Exception
	{
		private string _current;
		private string _need;

		public DataTypeException(Type current, Type need)
		{
			_current = current.FullName;
			_need = need.FullName;
		}

		public override string ToString()
		{
			return string.Format("Type of data {0} does not match with requested type: {1}", _current, _need);
		}	
	}
}
