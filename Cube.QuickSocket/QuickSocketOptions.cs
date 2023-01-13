using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cube.QuickSocket
{
    public class QuickSocketOptions
    {
        public string Protocols { get; set; } = "";


        public bool EnableServer { get; set; } = false;
        public string ServerIpPorts { get; set; } = string.Empty;
        public int ServerConnectionLimit { get; set; } = 2048;


        public bool EnableClient { get; set; } = false;




    }

    public class QuickSocketServer
    {
        public bool Enable { get; set; } = true;
        public string EndPoints { get; set; } = "";
        public int ConnectionLimit { get; set; } = 2048;

    }

}
