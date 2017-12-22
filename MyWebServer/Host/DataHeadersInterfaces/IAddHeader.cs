using System;
namespace Host.DataHeaderInterfaces
{
	public interface IAddHeader
	{
		void GetAddingToHeader(Action<string, string> add_to_header);
	}
}
