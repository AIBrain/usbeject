// UsbEject version 1.0 March 2006
// written by Simon Mourier <email: simon [underscore] mourier [at] hotmail [dot] com>

namespace UsbEject {

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;

    /// <summary>
    ///     A generic base class for physical devices.
    /// </summary>
    [TypeConverter( typeof( ExpandableObjectConverter ) )]
    public class Device : IComparable {
        private DeviceCapabilities _capabilities = DeviceCapabilities.Unknown;
        private String _class;
        private String _classGuid;
        private String _description;

        private String _friendlyName;
        private Device _parent;

        internal Device( DeviceClass deviceClass, Native.SP_DEVINFO_DATA deviceInfoData, String path, Int32 index, Int32 disknum = -1 ) {
            if ( deviceClass == null ) {
                throw new ArgumentNullException( nameof( deviceClass ) );
            }

            if ( deviceInfoData == null ) {
                throw new ArgumentNullException( nameof( deviceInfoData ) );
            }

            this.DeviceClass = deviceClass;
            this.Path = path; // may be null
            this.DeviceInfoData = deviceInfoData;
            this.Index = index;
            this.DiskNumber = disknum;
        }

        /// <summary>
        ///     Gets the device's class instance.
        /// </summary>
        [Browsable( false )]
        public DeviceClass DeviceClass {
            get;
        }

        public Int32 DiskNumber {
            get;
        }

        /// <summary>
        ///     Gets the device's index.
        /// </summary>
        public Int32 Index {
            get;
        }

        /// <summary>
        ///     Gets the device's path.
        /// </summary>
        public String Path {
            get;
        }

        private Native.SP_DEVINFO_DATA DeviceInfoData {
            get;
        }

        /// <summary>
        ///     Compares the current instance with another object of the same type.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>A 32-bit signed integer that indicates the relative order of the comparands.</returns>
        public virtual Int32 CompareTo( Object obj ) {
            var device = obj as Device;
            if ( device == null ) {
                throw new ArgumentException();
            }

            return this.Index.CompareTo( device.Index );
        }

        /// <summary>
        ///     Ejects the device.
        /// </summary>
        /// <param name="allowUI">Pass true to allow the Windows shell to display any related UI element, false otherwise.</param>
        /// <returns>null if no error occured, otherwise a contextual text.</returns>
        public String Eject( Boolean allowUI ) {
            foreach ( var device in this.GetRemovableDevices() ) {
                if ( allowUI ) {
                    Native.CM_Request_Device_Eject_NoUi( device.GetInstanceHandle(), IntPtr.Zero, null, 0, 0 );

                    // don't handle errors, there should be a UI for this
                }
                else {
                    var sb = new StringBuilder( 1024 );

                    Native.PNP_VETO_TYPE veto;
                    var hr = Native.CM_Request_Device_Eject( device.GetInstanceHandle(), out veto, sb, sb.Capacity, 0 );
                    if ( hr != 0 ) {
                        throw new Win32Exception( hr );
                    }

                    if ( veto != Native.PNP_VETO_TYPE.Ok ) {
                        return veto.ToString();
                    }
                }
            }
            return null;
        }

        /// <summary>
        ///     Gets the device's capabilities.
        /// </summary>
        public DeviceCapabilities GetCapabilities() {
            if ( this._capabilities == DeviceCapabilities.Unknown ) {
                this._capabilities = ( DeviceCapabilities )this.DeviceClass.GetProperty( this.DeviceInfoData, Native.SPDRP_CAPABILITIES, 0 );
            }
            return this._capabilities;
        }

        /// <summary>
        ///     Gets the device's class name.
        /// </summary>
        public String GetClass() => this._class ?? ( this._class = this.DeviceClass.GetProperty( this.DeviceInfoData, Native.SPDRP_CLASS, null ) );

        /// <summary>
        ///     Gets the device's class Guid as a string.
        /// </summary>
        public String GetClassGuid() {
            return this._classGuid ?? ( this._classGuid = this.DeviceClass.GetProperty( this.DeviceInfoData, Native.SPDRP_CLASSGUID, null ) );
        }

        /// <summary>
        ///     Gets the device's description.
        /// </summary>
        public String GetDescription() {
            return this._description ?? ( this._description = this.DeviceClass.GetProperty( this.DeviceInfoData, Native.SPDRP_DEVICEDESC, null ) );
        }

        /// <summary>
        ///     Gets the device's friendly name.
        /// </summary>
        public String GetFriendlyName() => this._friendlyName ?? ( this._friendlyName = this.DeviceClass.GetProperty( this.DeviceInfoData, Native.SPDRP_FRIENDLYNAME, null ) );

        /// <summary>
        ///     Gets the device's instance handle.
        /// </summary>
        public UInt32 GetInstanceHandle() => this.DeviceInfoData.devInst;

        /// <summary>
        ///     Gets the device's parent device or null if this device has not parent.
        /// </summary>
        public Device GetParent() {
            if ( this._parent != null ) {
                return this._parent;
            }
            var parentDevInst = 0;
            var hr = Native.CM_Get_Parent( ref parentDevInst, this.DeviceInfoData.devInst, 0 );
            if ( hr == 0 ) {
                this._parent = new Device( this.DeviceClass, this.DeviceClass.GetInfo( parentDevInst ), null, -1 );
            }
            return this._parent;
        }

        /// <summary>
        ///     Gets this device's list of removable devices.
        ///     Removable devices are parent devices that can be removed.
        /// </summary>
        public virtual IEnumerable<Device> GetRemovableDevices() {
            if ( ( this.GetCapabilities() & DeviceCapabilities.Removable ) != 0 ) {
                yield return this;
            }
            else {
                if ( this.GetParent() == null ) {
                    yield break;
                }
                foreach ( var device in this.GetParent().GetRemovableDevices() ) {
                    yield return device;
                }
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this device is a USB device.
        /// </summary>
        public virtual Boolean IsUsb() {
            if ( this.GetClass() == "USB" ) {
                return true;
            }

            return this.GetParent() != null && this.GetParent().IsUsb();
        }
    }
}