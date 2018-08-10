using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurate.Resouces
{
    public abstract class IitemRead : IItem {
        public IitemRead() : base() {}
        public virtual Stream GetData() { throw new NotImplementedException(); }
    }
}
