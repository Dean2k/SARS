using System;

namespace VRChatAPI_New.Exception
{
    [Serializable]
    public class NotSetup : System.Exception
    {
        public NotSetup()
        {
        }

        public NotSetup(string message)
            : base(message)
        {
        }

        public NotSetup(string message, System.Exception inner)
            : base(message, inner)
        {
        }
    }
}