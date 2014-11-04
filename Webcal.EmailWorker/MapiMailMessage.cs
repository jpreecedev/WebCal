using Webcal.EmailWorker.Properties;

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
                    error = Resources.TXT_USER_ABORTED;
                    break;
                case MAPI_E_FAILURE:
                    error = Resources.TXT_MAPI_FAILURE;
                    break;
                case MAPI_E_LOGIN_FAILURE:
                    error = Resources.TXT_LOGIN_FAILURE;
                    break;
                case MAPI_E_DISK_FULL:
                    error = Resources.TXT_MAPI_DISK_FULL;
                    break;
                case MAPI_E_INSUFFICIENT_MEMORY:
                    error = Resources.TXT_MAPI_INSUFFICIENT_MEMORY;
                    break;
                case MAPI_E_BLK_TOO_SMALL:
                    error = Resources.TXT_MAPI_BLOCK_TOO_SMALL;
                    break;
                case MAPI_E_TOO_MANY_SESSIONS:
                    error = Resources.TXT_MAPI_TOO_MANY_SESSIONS;
                    break;
                case MAPI_E_TOO_MANY_FILES:
                    error = Resources.TXT_MAPI_TOO_MANY_FILES;
                    break;
                case MAPI_E_TOO_MANY_RECIPIENTS:
                    error = Resources.TXT_MAPI_TOO_MANY_RECIPIENTS;
                    break;
                case MAPI_E_ATTACHMENT_NOT_FOUND:
                    error = Resources.TXT_MAPI_ATTACHMENT_NOT_FOUND;
                    break;
                case MAPI_E_ATTACHMENT_OPEN_FAILURE:
                    error = Resources.TXT_MAPI_ATTACHMENT_OPEN_FAILURE;
                    break;
                case MAPI_E_ATTACHMENT_WRITE_FAILURE:
                    error = Resources.TXT_MAPI_ATTACHMENT_WRITE_FAILURE;
                    break;
                case MAPI_E_UNKNOWN_RECIPIENT:
                    error = Resources.TXT_MAPI_UNKNOWN_RECIPEIENT;
                    break;
                case MAPI_E_BAD_RECIPTYPE:
                    error = Resources.TXT_MAPI_BAD_RECIPIENT_TYPE;
                    break;
                case MAPI_E_NO_MESSAGES:
                    error = Resources.TXT_MAPI_NO_MESSAGES;
                    break;
                case MAPI_E_INVALID_MESSAGE:
                    error = Resources.TXT_MAPI_INVALID_MESSAGE;
                    break;
                case MAPI_E_TEXT_TOO_LARGE:
                    error = Resources.TXT_MAPI_TEXT_TOO_LARGE;
                    break;
                case MAPI_E_INVALID_SESSION:
                    error = Resources.TXT_MAPI_INVALID_SESSION;
                    break;
                case MAPI_E_TYPE_NOT_SUPPORTED:
                    error = Resources.TXT_MAPI_TYPE_NOT_SUPPORTED;
                    break;
                case MAPI_E_AMBIGUOUS_RECIPIENT:
                    error = Resources.TXT_MAPI_AMBIGUOUS_RECIPIENT;
                    break;
                case MAPI_E_MESSAGE_IN_USE:
                    error = Resources.TXT_MAPI_MESSAGE_IN_USE;
                    break;
                case MAPI_E_NETWORK_FAILURE:
                    error = Resources.TXT_MAPI_NETWORK_FAILURE;
                    break;
                case MAPI_E_INVALID_EDITFIELDS:
                    error = Resources.TXT_MAPI_INVALID_EDIT_FIELDS;
                    break;
                case MAPI_E_INVALID_RECIPS:
                    error = Resources.TXT_MAPI_INVALID_RECIPIENTS;
                    break;
                case MAPI_E_NOT_SUPPORTED:
                    error = Resources.TXT_MAPI_NOT_SUPPORTED;
                    break;
                case MAPI_E_NO_LIBRARY:
                    error = Resources.TXT_MAPI_NO_LIBRARY;
                    break;
                case MAPI_E_INVALID_PARAMETER:
                    error = Resources.TXT_MAPI_INVALID_PARAMETER;
                    break;
            }

            Debug.WriteLine(Resources.ERR_ERROR_SEND_MAPI_EMAIL, error, errorCode);
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