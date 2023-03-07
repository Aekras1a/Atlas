using System;

namespace Atlas
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
            updateStatus = "/api/update_status",
            updateProgres = "/api/update_progress"
        };
    }
}