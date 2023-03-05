using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Atlas.Settings
{
    internal class ConnectionSettings
    {
        public enum vaildResponces
        {
            NA,
            doFileBackup
        }

        public String serverAddress = "http://localhost/atlas";

        public dynamic serverEndpoints = new
        {
            getCommand = "/api/get_command",
            updateStatus = "/api/update_status"
        };
    }
}