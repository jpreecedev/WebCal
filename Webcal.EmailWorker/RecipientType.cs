namespace Webcal.EmailWorker
{
    public enum RecipientType
    {
        To = 1,

        /// <summary>
        ///     Recipient will be in the CC list.
        /// </summary>
        CC = 2,

        /// <summary>
        ///     Recipient will be in the BCC list.
        /// </summary>
        BCC = 3
    };
}