using System;
using System.Collections.Generic;
using Host.HeaderData;

namespace Host.DataInput
{
	public abstract class ABSReqestData
	{
		public abstract string Name { get; set; }
		public abstract Dictionary<string, HeaderValueMain> settings { get; }

		public virtual byte[] ToByteArray() { throw new NotImplementedException(); }
		public virtual string ToStr() { throw new NotImplementedException(); }
	}
}
