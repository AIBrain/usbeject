// UsbEject version 1.0 March 2006
// written by Simon Mourier <email: simon [underscore] mourier [at] hotmail [dot] com>

namespace UsbEject {

    using System;
    using System.IO;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;
    using Microsoft.Win32.SafeHandles;

    public static class Native {
        public const Int32 DIGCF_DEVICEINTERFACE = ( 0x00000010 );

        // from setupapi.h
        public const Int32 DIGCF_PRESENT = ( 0x00000002 );

        public const Int32 ERROR_INSUFFICIENT_BUFFER = 122;

        public const Int32 ERROR_INVALID_DATA = 13;

        // from winerror.h
        public const Int32 ERROR_NO_MORE_ITEMS = 259;

        public const Int32 GENERIC_READ = unchecked(( Int32 )0x80000000);

        public const String GUID_DEVINTERFACE_DISK = "53f56307-b6bf-11d0-94f2-00a0c91efb8b";

        // from winioctl.h
        public const String GUID_DEVINTERFACE_VOLUME = "53f5630d-b6bf-11d0-94f2-00a0c91efb8b";

        // from winbase.h
        public const Int32 INVALID_HANDLE_VALUE = -1;

        public const Int32 IOCTL_STORAGE_GET_DEVICE_NUMBER = 0x002d1080;

        public const Int32 IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS = 0x00560000;

        public const Int32 SPDRP_CAPABILITIES = 0x0000000F;

        public const Int32 SPDRP_CLASS = 0x00000007;

        public const Int32 SPDRP_CLASSGUID = 0x00000008;

        public const Int32 SPDRP_DEVICEDESC = 0x00000000;

        public const Int32 SPDRP_FRIENDLYNAME = 0x0000000C;

        // from winuser.h
        public const Int32 WM_DEVICECHANGE = 0x0219;

        // from cfg.h
        public enum PNP_VETO_TYPE {
            Ok,

            TypeUnknown,
            LegacyDevice,
            PendingClose,
            WindowsApp,
            WindowsService,
            OutstandingOpen,
            Device,
            Driver,
            IllegalDeviceRequest,
            InsufficientPower,
            NonDisableable,
            LegacyDriver
        }

        [DllImport( "kernel32.dll", SetLastError = true )]
        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.Success )]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs( UnmanagedType.Bool )]
        public static extern Boolean CloseHandle( IntPtr hObject );

        [DllImport( "setupapi.dll" )]
        public static extern Int32 CM_Get_Device_ID( Int32 dnDevInst, StringBuilder buffer, Int32 bufferLen, Int32 ulFlags );

        // from cfgmgr32.h
        [DllImport( "setupapi.dll" )]
        public static extern Int32 CM_Get_Parent( ref Int32 pdnDevInst, UInt32 dnDevInst, Int32 ulFlags );

        [DllImport( "setupapi.dll" )]
        public static extern Int32 CM_Request_Device_Eject( UInt32 dnDevInst, out PNP_VETO_TYPE pVetoType, StringBuilder pszVetoName, Int32 ulNameLength, Int32 ulFlags );

        [DllImport( "setupapi.dll", EntryPoint = "CM_Request_Device_Eject" )]
        public static extern Int32 CM_Request_Device_Eject_NoUi( UInt32 dnDevInst, IntPtr pVetoType, StringBuilder pszVetoName, Int32 ulNameLength, Int32 ulFlags );

        [DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Auto )]
        public static extern SafeFileHandle CreateFile( String lpFileName, [MarshalAs( UnmanagedType.U4 )] FileAccess dwDesiredAccess, [MarshalAs( UnmanagedType.U4 )] FileShare dwShareMode, IntPtr lpSecurityAttributes, [MarshalAs( UnmanagedType.U4 )] FileMode dwCreationDisposition, [MarshalAs( UnmanagedType.U4 )] FileAttributes dwFlagsAndAttributes, IntPtr hTemplateFile );

        [DllImport( "kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto )]
        public static extern Boolean DeviceIoControl( IntPtr hDevice, UInt32 dwIoControlCode, IntPtr lpInBuffer, UInt32 nInBufferSize, IntPtr lpOutBuffer, UInt32 nOutBufferSize, out UInt32 lpBytesReturned, IntPtr lpOverlapped );

        [DllImport( "kernel32", CharSet = CharSet.Auto, SetLastError = true )]
        public static extern Boolean GetVolumeNameForVolumeMountPoint( String volumeName, StringBuilder uniqueVolumeName, UInt32 uniqueNameBufferCapacity );

        [DllImport( "setupapi.dll" )]
        public static extern UInt32 SetupDiDestroyDeviceInfoList( IntPtr deviceInfoSet );

        [DllImport( "setupapi.dll", SetLastError = true, CharSet = CharSet.Auto )]
        public static extern Boolean SetupDiEnumDeviceInterfaces( IntPtr deviceInfoSet, SP_DEVINFO_DATA deviceInfoData, ref Guid interfaceClassGuid, Int32 memberIndex, SP_DEVICE_INTERFACE_DATA deviceInterfaceData );

        [DllImport( "setupapi.dll" )]
        public static extern IntPtr SetupDiGetClassDevs( ref Guid classGuid, Int32 enumerator, IntPtr hwndParent, Int32 flags );

        [DllImport( "setupapi.dll", SetLastError = true, CharSet = CharSet.Auto )]
        public static extern Boolean SetupDiGetDeviceInterfaceDetail( IntPtr deviceInfoSet, SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData, Int32 deviceInterfaceDetailDataSize, ref Int32 requiredSize, SP_DEVINFO_DATA deviceInfoData );

        [DllImport( "setupapi.dll", CharSet = CharSet.Auto, SetLastError = true )]
        public static extern Boolean SetupDiGetDeviceRegistryProperty( IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, UInt32 property, out UInt32 propertyRegDataType, Byte[] propertyBuffer, UInt32 propertyBufferSize, out UInt32 requiredSize );

        [DllImport( "setupapi.dll" )]
        public static extern Boolean SetupDiOpenDeviceInfo( IntPtr deviceInfoSet, String deviceInstanceId, IntPtr hwndParent, Int32 openFlags, SP_DEVINFO_DATA deviceInfoData );

        [StructLayout( LayoutKind.Sequential )]
        public struct DISK_EXTENT {
            public Int32 DiskNumber;
            public Int64 StartingOffset;
            public Int64 ExtentLength;
        }

        [StructLayout( LayoutKind.Sequential )]
        public struct STORAGE_DEVICE_NUMBER {
            public Int32 DeviceType;
            public Int32 DeviceNumber;
            public Int32 PartitionNumber;
        }

        [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Auto )]
        public class SP_DEVICE_INTERFACE_DATA {
            public UInt32 cbSize;
            public UInt32 Flags;
            public Guid InterfaceClassGuid;
            public IntPtr Reserved;
        }

        [StructLayout( LayoutKind.Sequential, Pack = 2 )]
        public class SP_DEVICE_INTERFACE_DETAIL_DATA {
            public Int32 cbSize;
            public Int16 devicePath;
        }

        [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Auto )]
        public class SP_DEVINFO_DATA {
            public UInt32 cbSize;
            public Guid classGuid;
            public UInt32 devInst;
            public IntPtr reserved;
        }
    }
}