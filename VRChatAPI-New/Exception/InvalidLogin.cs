using System;

namespace VRChatAPI_New.Exception
{
    [Serializable]
    public class InvalidLogin : System.Exception
    {
        public InvalidLogin()
        {
        }

        public InvalidLogin(string message)
            : base(message)
        {
        }

        public InvalidLogin(string message, System.Exception inner)
            : base(message, inner)
        {
        }
    }
}