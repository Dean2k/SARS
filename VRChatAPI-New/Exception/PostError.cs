using System;

namespace VRChatAPI_New.Exception
{
    [Serializable]
    public class PostError : System.Exception
    {
        public PostError()
        {
        }

        public PostError(string message)
            : base(message)
        {
        }

        public PostError(string message, System.Exception inner)
            : base(message, inner)
        {
        }
    }
}