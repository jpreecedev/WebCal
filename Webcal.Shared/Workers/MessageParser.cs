namespace Webcal.Shared.Workers
{
    using System;
    using System.Collections.Generic;

    public static class MessageParser
    {
        public static KeyValuePair<int, string>? Parse(string rawMessage)
        {
            if (string.IsNullOrEmpty(rawMessage))
            {
                return null;
            }

            string[] msg = rawMessage.Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);

            if (msg.Length == 2)
            {
                return new KeyValuePair<int, string>(int.Parse(msg[0]), msg[1]);
            }

            return null;
        }
    }
}