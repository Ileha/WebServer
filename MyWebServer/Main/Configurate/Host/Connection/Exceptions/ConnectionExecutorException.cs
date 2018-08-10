using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configurate.Host.Connection.Exceptions
{
    public class ConnectionExecutorException : Exception { }

    public class ConnectionExecutorClose : ConnectionExecutorException {}
    public class ConnectionExecutorBadAccess : ConnectionExecutorException { }
}
