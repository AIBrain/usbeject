// Copyright 2016 Rick@AIBrain.org.
// 
// This notice must be kept visible in the source.
// 
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified, or the
// original license has been overwritten by the automatic formatting of this code. Any unmodified
// sections of source code borrowed from other projects retain their original license and thanks
// goes to the Authors.
// 
// Donations and royalties can be paid via
//  PayPal: paypal@aibrain.org
//  bitcoin: 1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
//  litecoin: LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
// 
// Usage of the source code or compiled binaries is AS-IS. I am not responsible for Anything You Do.
// 
// Contact me by email if you have any questions or helpful criticism.
// 
// "USB Eject Unit Tests/UnitsTests.cs" was last cleaned by Rick on 2016/07/23 at 9:09 PM

namespace USB_Eject_Unit_Tests {
    using NUnit.Framework;
    using UsbEject;

    [ TestFixture ]
    public static class UnitsTests {
        [ Test ]
        public static void TestOne() {
            var volumes = new VolumeDeviceClass();

            foreach ( var device in volumes.GetDevices() ) {
                var volume = device as Volume;

                var logicalDrive = volume?.GetLogicalDrive();

                //if ( logicalDrive != null && ( logicalDrive.Equals( 1 ) ) ) {
                //    Debug.WriteLine( "Attempting to eject drive: " + cur_write_drive );
                //    vol.Eject( false );
                //    eventLog.WriteEntry( "Done ejecting drive." );
                //    break;
                //}
            }
        }
    }
}
