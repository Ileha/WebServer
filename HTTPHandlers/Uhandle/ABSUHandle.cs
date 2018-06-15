using System;
using System.Xml.Linq;

namespace HTTPHandlers.Uhandle
{
    public abstract class ABSUHandle {

        public abstract void Handle(XElement data);
    }
}
