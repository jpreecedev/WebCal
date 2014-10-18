namespace Webcal.EmailWorker
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;

    public class MapiMailMessage
    {
        private readonly ManualResetEvent _manualResetEvent;

        public MapiMailMessage()
        {
            Files = new ArrayList();
            Recipients = new RecipientCollection();
            _manualResetEvent = new ManualResetEvent(false);
        }

        public MapiMailMessage(string subject)
            : this()
        {
            Subject = subject;
        }

        public MapiMailMessage(string subject, string body)
            : this()
        {
            Subject = subject;
            Body = body;
        }

        public string Subject { get; set; }
        public string Body { get; set; }
        public RecipientCollection Recipients { get; private set; }
        public ArrayList Files { get; private set; }

        public void ShowDialog()
        {
            ShowMail();

            _manualResetEvent.WaitOne();
            _manualResetEvent.Reset();
        }

        private void ShowMail()
        {
            var message = new MAPIHelperInterop.MapiMessage();

            using (RecipientCollection.InteropRecipientCollection interopRecipients = Recipients.GetInteropRepresentation())
            {
                message.Subject = Subject;
                message.NoteText = Body;

                message.Recipients = interopRecipients.Handle;
                message.RecipientCount = Recipients.Count;

                // Check if we need to add attachments
                if (Files.Count > 0)
                {
                    // Add attachments
                    message.Files = AllocAttachments(out message.FileCount);
                }

                // Signal the creating thread (make the remaining code async)
                _manualResetEvent.Set();

                const int MAPI_DIALOG = 0x8;
                //const int MAPI_LOGON_UI = 0x1;
                const int SUCCESS_SUCCESS = 0;
                int error = MAPIHelperInterop.MAPISendMail(IntPtr.Zero, IntPtr.Zero, message, MAPI_DIALOG, 0);

                if (Files.Count > 0)
                {
                    // Deallocate the files
                    DeallocFiles(message);
                }

                // Check for error
                if (error != SUCCESS_SUCCESS)
                {
                    LogErrorMapi(error);
                }
            }
        }

        private static void DeallocFiles(MAPIHelperInterop.MapiMessage message)
        {
            if (message.Files == IntPtr.Zero)
            {
                return;
            }

            Type fileDescType = typeof (MapiFileDescriptor);
            int fsize = Marshal.SizeOf(fileDescType);

            // Get the ptr to the files
            var runptr = (int) message.Files;
            // Release each file
            for (int i = 0; i < message.FileCount; i++)
            {
                Marshal.DestroyStructure((IntPtr) runptr, fileDescType);
                runptr += fsize;
            }
            // Release the file
            Marshal.FreeHGlobal(message.Files);
        }

        private IntPtr AllocAttachments(out int fileCount)
        {
            fileCount = 0;
            if (Files == null)
            {
                return IntPtr.Zero;
            }
            if ((Files.Count <= 0) || (Files.Count > 100))
            {
                return IntPtr.Zero;
            }

            Type atype = typeof (MapiFileDescriptor);
            int asize = Marshal.SizeOf(atype);
            IntPtr ptra = Marshal.AllocHGlobal(Files.Count*asize);

            var mfd = new MapiFileDescriptor();
            mfd.position = -1;
            var runptr = (int) ptra;
            for (int i = 0; i < Files.Count; i++)
            {
                var path = Files[i] as string;
                mfd.name = Path.GetFileName(path);
                mfd.path = path;
                Marshal.StructureToPtr(mfd, (IntPtr) runptr, false);
                runptr += asize;
            }

            fileCount = Files.Count;
            return ptra;
        }

        private static void LogErrorMapi(int errorCode)
        {
            const int MAPI_USER_ABORT = 1;
            const int MAPI_E_FAILURE = 2;
            const int MAPI_E_LOGIN_FAILURE = 3;
            const int MAPI_E_DISK_FULL = 4;
            const int MAPI_E_INSUFFICIENT_MEMORY = 5;
            const int MAPI_E_BLK_TOO_SMALL = 6;
            const int MAPI_E_TOO_MANY_SESSIONS = 8;
            const int MAPI_E_TOO_MANY_FILES = 9;
            const int MAPI_E_TOO_MANY_RECIPIENTS = 10;
            const int MAPI_E_ATTACHMENT_NOT_FOUND = 11;
            const int MAPI_E_ATTACHMENT_OPEN_FAILURE = 12;
            const int MAPI_E_ATTACHMENT_WRITE_FAILURE = 13;
            const int MAPI_E_UNKNOWN_RECIPIENT = 14;
            const int MAPI_E_BAD_RECIPTYPE = 15;
            const int MAPI_E_NO_MESSAGES = 16;
            const int MAPI_E_INVALID_MESSAGE = 17;
            const int MAPI_E_TEXT_TOO_LARGE = 18;
            const int MAPI_E_INVALID_SESSION = 19;
            const int MAPI_E_TYPE_NOT_SUPPORTED = 20;
            const int MAPI_E_AMBIGUOUS_RECIPIENT = 21;
            const int MAPI_E_MESSAGE_IN_USE = 22;
            const int MAPI_E_NETWORK_FAILURE = 23;
            const int MAPI_E_INVALID_EDITFIELDS = 24;
            const int MAPI_E_INVALID_RECIPS = 25;
            const int MAPI_E_NOT_SUPPORTED = 26;
            const int MAPI_E_NO_LIBRARY = 999;
            const int MAPI_E_INVALID_PARAMETER = 998;

            string error = string.Empty;
            switch (errorCode)
            {
                case MAPI_USER_ABORT:
                    error = "User Aborted.";
                    break;
                case MAPI_E_FAILURE:
                    error = "MAPI Failure.";
                    break;
                case MAPI_E_LOGIN_FAILURE:
                    error = "Login Failure.";
                    break;
                case MAPI_E_DISK_FULL:
                    error = "MAPI Disk full.";
                    break;
                case MAPI_E_INSUFFICIENT_MEMORY:
                    error = "MAPI Insufficient memory.";
                    break;
                case MAPI_E_BLK_TOO_SMALL:
                    error = "MAPI Block too small.";
                    break;
                case MAPI_E_TOO_MANY_SESSIONS:
                    error = "MAPI Too many sessions.";
                    break;
                case MAPI_E_TOO_MANY_FILES:
                    error = "MAPI too many files.";
                    break;
                case MAPI_E_TOO_MANY_RECIPIENTS:
                    error = "MAPI too many recipients.";
                    break;
                case MAPI_E_ATTACHMENT_NOT_FOUND:
                    error = "MAPI Attachment not found.";
                    break;
                case MAPI_E_ATTACHMENT_OPEN_FAILURE:
                    error = "MAPI Attachment open failure.";
                    break;
                case MAPI_E_ATTACHMENT_WRITE_FAILURE:
                    error = "MAPI Attachment Write Failure.";
                    break;
                case MAPI_E_UNKNOWN_RECIPIENT:
                    error = "MAPI Unknown recipient.";
                    break;
                case MAPI_E_BAD_RECIPTYPE:
                    error = "MAPI Bad recipient type.";
                    break;
                case MAPI_E_NO_MESSAGES:
                    error = "MAPI No messages.";
                    break;
                case MAPI_E_INVALID_MESSAGE:
                    error = "MAPI Invalid message.";
                    break;
                case MAPI_E_TEXT_TOO_LARGE:
                    error = "MAPI Text too large.";
                    break;
                case MAPI_E_INVALID_SESSION:
                    error = "MAPI Invalid session.";
                    break;
                case MAPI_E_TYPE_NOT_SUPPORTED:
                    error = "MAPI Type not supported.";
                    break;
                case MAPI_E_AMBIGUOUS_RECIPIENT:
                    error = "MAPI Ambiguous recipient.";
                    break;
                case MAPI_E_MESSAGE_IN_USE:
                    error = "MAPI Message in use.";
                    break;
                case MAPI_E_NETWORK_FAILURE:
                    error = "MAPI Network failure.";
                    break;
                case MAPI_E_INVALID_EDITFIELDS:
                    error = "MAPI Invalid edit fields.";
                    break;
                case MAPI_E_INVALID_RECIPS:
                    error = "MAPI Invalid Recipients.";
                    break;
                case MAPI_E_NOT_SUPPORTED:
                    error = "MAPI Not supported.";
                    break;
                case MAPI_E_NO_LIBRARY:
                    error = "MAPI No Library.";
                    break;
                case MAPI_E_INVALID_PARAMETER:
                    error = "MAPI Invalid parameter.";
                    break;
            }

            Debug.WriteLine("Error sending MAPI Email. Error: " + error + " (code = " + errorCode + ").");
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        private class MapiFileDescriptor
        {
            public int reserved = 0;
            public int flags = 0;
            public int position;
            public string path;
            public string name;
            public IntPtr type = IntPtr.Zero;
        }
    }
}