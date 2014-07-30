using System;
using System.Runtime.InteropServices;

namespace Webcal.Library.MAPI
{
    internal class MAPIHelperInterop
    {
        #region Constructors

        private MAPIHelperInterop()
        {
            // Intenationally blank
        }

        #endregion Constructors

        #region Constants

        public const int MAPI_LOGON_UI = 0x1;

        #endregion Constants

        #region APIs

        [DllImport("MAPI32.DLL", CharSet = CharSet.Ansi)]
        public static extern int MAPILogon(IntPtr hwnd, string prf, string pw, int flg, int rsv, ref IntPtr sess);

        #endregion APIs

        #region Structs

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class MapiMessage
        {
            public int Reserved = 0;
            public string Subject = null;
            public string NoteText = null;
            public string MessageType = null;
            public string DateReceived = null;
            public string ConversationID = null;
            public int Flags = 0;
            public IntPtr Originator = IntPtr.Zero;
            public int RecipientCount = 0;
            public IntPtr Recipients = IntPtr.Zero;
            public int FileCount = 0;
            public IntPtr Files = IntPtr.Zero;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class MapiRecipDesc
        {
            public int Reserved = 0;
            public int RecipientClass = 0;
            public string Name = null;
            public string Address = null;
            public int eIDSize = 0;
            public IntPtr EntryID = IntPtr.Zero;
        }

        [DllImport("MAPI32.DLL")]
        public static extern int MAPISendMail(IntPtr session, IntPtr hwnd, MapiMessage message, int flg, int rsv);

        #endregion Structs
    }

}
