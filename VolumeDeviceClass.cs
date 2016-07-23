// UsbEject version 1.0 March 2006
// written by Simon Mourier <email: simon [underscore] mourier [at] hotmail [dot] com>

namespace UsbEject {

    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    ///     The device class for volume devices.
    /// </summary>
    public class VolumeDeviceClass : DeviceClass {

        /// <summary>
        ///     Initializes a new instance of the VolumeDeviceClass class.
        /// </summary>
        public VolumeDeviceClass() : base( new Guid( Native.GUID_DEVINTERFACE_VOLUME ) ) {
            foreach ( var drive in Environment.GetLogicalDrives() ) {
                var sb = new StringBuilder( 1024 );
                if ( !Native.GetVolumeNameForVolumeMountPoint( drive, sb, ( UInt32 )sb.Capacity ) ) {
                    continue;
                }
                this.LogicalDrives[ sb.ToString() ] = drive.Replace( "\\", "" );
                Console.WriteLine( drive + " ==> " + sb );
            }
        }

        protected internal SortedDictionary<String, String> LogicalDrives { get; } = new SortedDictionary<String, String>();

        protected override Device CreateDevice( DeviceClass deviceClass, Native.SP_DEVINFO_DATA deviceInfoData, String path, Int32 index, Int32 disknum = -1 ) {
            return new Volume( deviceClass, deviceInfoData, path, index );
        }
    }
}