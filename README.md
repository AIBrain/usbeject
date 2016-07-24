# usbeject
C# code to safely eject removable storage (Windows)

This is a class library of code stolen from a codeprojects page, and modified to fix a bug where it wasn't ejecting the drive it
claimed to be ejecting. All the copyright headers in it are from the original author. I'm providing this code here since aparently
the codeprojects page is dead (and the code posted there was broken anyways). 

I'm distributing this software with NO WARRANTY and NO GUARANTEE of suitability for any purpose, etc, etc, yada, yada, don't sue me.

Usage example pulled from my code:

VolumeDeviceClass volumes = new VolumeDeviceClass();
foreach (Volume vol in volumes.Devices)
{
  if (vol.LogicalDrive.Equals(eject_drive))
  {
    eventLog.WriteEntry("Attempting to eject drive: " + cur_write_drive);
    vol.Eject(false);
    eventLog.WriteEntry("Done ejecting drive.");
    break;
  }
}
