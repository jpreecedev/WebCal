namespace Webcal.EmailWorker
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    /// <summary>
    ///     Represents a colleciton of recipients for a mail message.
    /// </summary>
    public class RecipientCollection : CollectionBase
    {
        /// <summary>
        ///     Returns the recipient stored in this collection at the specified index.
        /// </summary>
        public Recipient this[int index]
        {
            get { return (Recipient) List[index]; }
        }

        /// <summary>
        ///     Adds the specified recipient to this collection.
        /// </summary>
        public void Add(Recipient value)
        {
            List.Add(value);
        }

        /// <summary>
        ///     Adds a new recipient with the specified address to this collection.
        /// </summary>
        public void Add(string address)
        {
            Add(new Recipient(address));
        }

        /// <summary>
        ///     Adds a new recipient with the specified address and display name to this collection.
        /// </summary>
        public void Add(string address, string displayName)
        {
            Add(new Recipient(address, displayName));
        }

        /// <summary>
        ///     Adds a new recipient with the specified address and recipient type to this collection.
        /// </summary>
        public void Add(string address, RecipientType recipientType)
        {
            Add(new Recipient(address, recipientType));
        }

        /// <summary>
        ///     Adds a new recipient with the specified address, display name and recipient type to this collection.
        /// </summary>
        public void Add(string address, string displayName, RecipientType recipientType)
        {
            Add(new Recipient(address, displayName, recipientType));
        }

        internal InteropRecipientCollection GetInteropRepresentation()
        {
            return new InteropRecipientCollection(this);
        }

        /// <summary>
        ///     Struct which contains an interop representation of a colleciton of recipients.
        /// </summary>
        internal struct InteropRecipientCollection : IDisposable
        {
            private int _count;
            private IntPtr _handle;


            /// <summary>
            ///     Default constructor for creating InteropRecipientCollection.
            /// </summary>
            /// <param name="outer"></param>
            public InteropRecipientCollection(RecipientCollection outer)
            {
                _count = outer.Count;

                if (_count == 0)
                {
                    _handle = IntPtr.Zero;
                    return;
                }

                // allocate enough memory to hold all recipients
                int size = Marshal.SizeOf(typeof (MAPIHelperInterop.MapiRecipDesc));
                _handle = Marshal.AllocHGlobal(_count*size);

                // place all interop recipients into the memory just allocated
                var ptr = (int) _handle;
                foreach (Recipient native in outer)
                {
                    MAPIHelperInterop.MapiRecipDesc interop = native.GetInteropRepresentation();

                    // stick it in the memory block
                    Marshal.StructureToPtr(interop, (IntPtr) ptr, false);
                    ptr += size;
                }
            }


            public IntPtr Handle
            {
                get { return _handle; }
            }


            /// <summary>
            ///     Disposes of resources.
            /// </summary>
            public void Dispose()
            {
                if (_handle != IntPtr.Zero)
                {
                    Type type = typeof (MAPIHelperInterop.MapiRecipDesc);
                    int size = Marshal.SizeOf(type);

                    // destroy all the structures in the memory area
                    var ptr = (int) _handle;
                    for (int i = 0; i < _count; i++)
                    {
                        Marshal.DestroyStructure((IntPtr) ptr, type);
                        ptr += size;
                    }

                    // free the memory
                    Marshal.FreeHGlobal(_handle);

                    _handle = IntPtr.Zero;
                    _count = 0;
                }
            }
        }
    }
}