// UsbEject version 1.0 March 2006
// written by Simon Mourier <email: simon [underscore] mourier [at] hotmail [dot] com>

namespace UsbEject
{
    using System;

    /// <summary>
    /// The device class for disk devices.
    /// </summary>
    public class DiskDeviceClass : DeviceClass
    {
        /// <summary>
        /// Initializes a new instance of the DiskDeviceClass class.
        /// </summary>
        public DiskDeviceClass()
            :base(new Guid(Native.GUID_DEVINTERFACE_DISK))
        {
        }
    }
}
