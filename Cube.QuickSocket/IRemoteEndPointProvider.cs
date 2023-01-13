using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Cube.QuickSocket
{
    public interface IRemoteEndPointProvider
    {

        string[] IpPorts { get; }

        EndPoint[] EndPoints { get; }

    }
}
