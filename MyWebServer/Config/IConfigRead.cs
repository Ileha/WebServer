using System;
using System.Linq;
using System.Xml.Linq;

namespace Config
{
    public interface IConfigRead {
		XElement GetElement(string name);
    }
}
