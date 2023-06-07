using System;

namespace VRChatAPI_New.Exception
{
    [Serializable]
    public class FileDownloadError : System.Exception
    {
        public FileDownloadError()
        {
        }

        public FileDownloadError(string message)
            : base(message)
        {
        }

        public FileDownloadError(string message, System.Exception inner)
            : base(message, inner)
        {
        }
    }
}