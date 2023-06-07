using System;

namespace VRChatAPI_New.Exception
{
    [Serializable]
    public class NoTwoFactor : System.Exception
    {
        public NoTwoFactor()
        {
        }

        public NoTwoFactor(string message)
            : base(message)
        {
        }

        public NoTwoFactor(string message, System.Exception inner)
            : base(message, inner)
        {
        }
    }
}