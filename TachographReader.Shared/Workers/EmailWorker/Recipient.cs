namespace TachographReader.Shared.Workers.EmailWorker
{
    public class Recipient
    {
        /// <summary>
        ///     The email address of this recipient.
        /// </summary>
        public string Address = null;

        /// <summary>
        ///     The display name of this recipient.
        /// </summary>
        public string DisplayName = null;

        /// <summary>
        ///     How the recipient will receive this message (To, CC, BCC).
        /// </summary>
        public RecipientType RecipientType = RecipientType.To;

        /// <summary>
        ///     Creates a new recipient with the specified address.
        /// </summary>
        public Recipient(string address)
        {
            Address = address;
        }

        /// <summary>
        ///     Creates a new recipient with the specified address and display name.
        /// </summary>
        public Recipient(string address, string displayName)
        {
            Address = address;
            DisplayName = displayName;
        }

        /// <summary>
        ///     Creates a new recipient with the specified address and recipient type.
        /// </summary>
        public Recipient(string address, RecipientType recipientType)
        {
            Address = address;
            RecipientType = recipientType;
        }

        /// <summary>
        ///     Creates a new recipient with the specified address, display name and recipient type.
        /// </summary>
        public Recipient(string address, string displayName, RecipientType recipientType)
        {
            Address = address;
            DisplayName = displayName;
            RecipientType = recipientType;
        }

        internal MAPIHelperInterop.MapiRecipDesc GetInteropRepresentation()
        {
            var interop = new MAPIHelperInterop.MapiRecipDesc();

            if (DisplayName == null)
            {
                interop.Name = Address;
            }
            else
            {
                interop.Name = DisplayName;
                interop.Address = Address;
            }

            interop.RecipientClass = (int) RecipientType;

            return interop;
        }
    }
}