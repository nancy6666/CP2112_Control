using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CP2112_Control
{
    class CP2112_API
    {
        #region Properties

        public enum HID_SMBUS_STATUS
        {
            HID_SMBUS_SUCCESS = 0x0,
            HID_SMBUS_DEVICE_NOT_FOUND = 0x1,
            HID_SMBUS_INVALID_HANDLE = 0x2,
            HID_SMBUS_INVALID_DEVICE_OBJECT = 0x3,
            HID_SMBUS_INVALID_PARAMETER = 0x4,
            HID_SMBUS_INVALID_REQUEST_LENGTH = 0x5,
            HID_SMBUS_READ_ERROR = 0x10,
            HID_SMBUS_WRITE_ERROR = 0x11,
            HID_SMBUS_READ_TIMED_OUT = 0x12,
            HID_SMBUS_WRITE_TIMED_OUT = 0x13,
            HID_SMBUS_DEVICE_IO_FAILED = 0x14,
            HID_SMBUS_DEVICE_ACCESS_ERROR = 0x15,
            HID_SMBUS_DEVICE_NOT_SUPPORTED = 0x16,
            HID_SMBUS_UNKNOWN_ERROR = 0xFF
        }

        public enum HID_SMBUS_S0
        {
            HID_SMBUS_S0_IDLE=0x00,
            HID_SMBUS_S0_BUSY=0x01,
            HID_SMBUS_S0_COMPLETE=0x02,
            HID_SMBUS_S0_ERROR=0x03
        }

        #endregion

        #region C++ Convert Methods

        [DllImport("SLABHIDtoSMBus.dll", EntryPoint = "HidSmbus_GetNumDevices")]
        public static extern HID_SMBUS_STATUS HidSmbus_GetNumDevices(ref uint numDevices, ushort vid, ushort pid);

        [DllImport("SLABHIDtoSMBus.dll", EntryPoint = "HidSmbus_Open")]
        public static extern HID_SMBUS_STATUS HidSmbus_Open(ref IntPtr device, UInt32 deviceNum, ushort vid, ushort pid);


        [DllImport("SLABHIDtoSMBus.dll", EntryPoint = "HidSmbus_IsOpened")]
        public static extern HID_SMBUS_STATUS HidSmbus_IsOpened(IntPtr device, ref bool IsOpened);

        [DllImport("SLABHIDtoSMBus.dll", EntryPoint = "HidSmbus_Close")]
        public static extern HID_SMBUS_STATUS HidSmbus_Close(IntPtr device);

        [DllImport("SLABHIDtoSMBus.dll", EntryPoint = "HidSmbus_GetString")]
        public static extern HID_SMBUS_STATUS HidSmbus_GetString(UInt32 deviceNum, ushort vid, ushort pid, StringBuilder deviceString, UInt32 options);

        [DllImport("SLABHIDtoSMBus.dll", EntryPoint = "HidSmbus_GetOpenedString")]
        public static extern HID_SMBUS_STATUS HidSmbus_GetOpenedString(IntPtr device, StringBuilder deviceString, UInt32 options);

        [DllImport("SLABHIDtoSMBus.dll")]
        public static extern HID_SMBUS_STATUS HidSmbus_GetAttributes(uint deviceNum, ushort vid, ushort pid, ref ushort deviceVid, ref ushort devicePid, ref ushort deviceReleaseNumber);

        [DllImport("SLABHIDtoSMBus.dll")]
        public static extern HID_SMBUS_STATUS HidSmbus_GetOpenedAttributes(IntPtr device, ref ushort deviceVid, ref ushort devicePid, ref ushort deviceReleaseNumber);

        [DllImport("SLABHIDtoSMBus.dll")]
        public static extern HID_SMBUS_STATUS HidSmbus_ReadRequest(IntPtr device, Byte slaveAddress, ushort numBytesToRead);

        /// <summary>
        ///  Intiates a read transfer to the specified slave device address and specifies the address to read from on the device
        /// </summary>
        /// <param name="device"></param>
        /// <param name="slaveAddress">the address of the slave device to read from 0x02 - 0xFE. </param>
        /// <param name="numBytesToRead">The number of bytes to read from the device (1–512)</param>
        /// <param name="targetAddressSize"> The size of the target address in bytes (1-16)</param>
        /// <param name="targetAddress">The address to read from the slave device</param>
        /// <returns></returns>
        [DllImport("SLABHIDtoSMBus.dll")]
        public static extern HID_SMBUS_STATUS HidSmbus_AddressReadRequest(IntPtr device, Byte slaveAddress, byte numBytesToRead, byte targetAddressSize, byte[] targetAddress);

        /// <summary>
        /// causes the device to send a read response to the host after a read transfer has been issued.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="numBytesToRead"></param>
        /// <returns></returns>
        [DllImport("SLABHIDtoSMBus.dll")]
        public static extern HID_SMBUS_STATUS HidSmbus_ForceReadResponse(IntPtr device, ushort numBytesToRead);

        /// <summary>
        /// returns the read response to a read reques
        /// </summary>
        /// <param name="device"></param>
        /// <param name="status">returns the status of the read request.</param>
        /// <param name="buffer">buffer returns up to 61 read data bytes</param>
        /// <param name="bufferSize">the size of buffer and must be at least 61 bytes</param>
        /// <param name="numBytesRead">returns the number of valid data bytes returned in buffer</param>
        /// <returns></returns>
        [DllImport("SLABHIDtoSMBus.dll")]
        public static extern HID_SMBUS_STATUS HidSmbus_GetReadResponse(IntPtr device, ref HID_SMBUS_S0 status, byte[] buffer,byte bufferSize,ref byte numBytesRead);

        /// <summary>
        /// writes the specified number of bytes from the supplied buffer to the specified slave device and returns immediately after sending the request to the CP2112
        /// </summary>
        /// <param name="device"></param>
        /// <param name="slaveAddress"></param>
        /// <param name="buffer"></param>
        /// <param name="numBytesToWrite">the number of bytes to write to the device (1–61). This value must be less than or equal to the size of buffer.</param>
        /// <returns></returns>
        [DllImport("SLABHIDtoSMBus.dll")]
        public static extern HID_SMBUS_STATUS HidSmbus_WriteRequest(IntPtr device, Byte slaveAddress, byte[] buffer, byte numBytesToWrite);

        /// <summary>
        /// requests the status of the current read or write transfer
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        [DllImport("SLABHIDtoSMBus.dll")]
        public static extern HID_SMBUS_STATUS HidSmbus_TransferStatusRequest(IntPtr device);

        [DllImport("SLABHIDtoSMBus.dll")]
        public static extern HID_SMBUS_STATUS HidSmbus_GetTransferStatusResponse(IntPtr device,ref HID_SMBUS_S0 hID_SMBUS_S0,ref ushort detailStatus,ref ushort numRetries,ref ushort bytesRead);

        /// <summary>
        /// Set the response timeouts
        /// </summary>
        /// <param name="device"></param>
        /// <param name="responseTimeout">the HidSmbus_GetReadResponse() and HidSmbus_GetTransferStatusResponse()timeout.</param>
        /// <returns></returns>
        [DllImport("SLABHIDtoSMBus.dll")]
        public static extern HID_SMBUS_STATUS HidSmbus_SetTimeouts(IntPtr device, uint responseTimeout);

        [DllImport("SLABHIDtoSMBus.dll")]
        public static extern HID_SMBUS_STATUS HidSmbus_GetTimeouts(IntPtr device, ref ushort responseTimeout);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="bitRate">bit rate for SMBus communication. The default is 100 kHz. This value must be non-zero.</param>
        /// <param name="address">device’s slave address (0x02– 0xFE). </param>
        /// <param name="autoReadRespond"></param>
        /// <param name="writeTimeOut"></param>
        /// <param name="readTimeOut"></param>
        /// <param name="sclLowTimeOut"></param>
        /// <param name="transferRetries"></param>
        /// <returns></returns>
        [DllImport("SLABHIDtoSMBus.dll")]
        public static extern HID_SMBUS_STATUS HidSmbus_SetSmbusConfig(IntPtr device, uint bitRate, byte address, bool autoReadRespond, ushort writeTimeOut, ushort readTimeOut, bool sclLowTimeOut, ushort transferRetries);

        [DllImport("SLABHIDtoSMBus.dll")]
        public static extern HID_SMBUS_STATUS HidSmbus_GetSmbusConfig(IntPtr device, ref uint bitRate, ref byte address, ref bool autoReadRespond, ref ushort writeTimeOut, ref ushort readTimeOut, ref bool sclLowTimeOut, ref ushort transferRetries);

        [DllImport("SLABHIDtoSMBus.dll")]
        public static extern HID_SMBUS_STATUS HidSmbus_Reset(IntPtr device);

        /// <summary>
        /// Configure the GPIO pins' directions and modes.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="direction">1:output 0:input</param>
        /// <param name="mode">0:open-Dain 1:Push-Pull</param>
        /// <param name="special">GPIO.7:0x01 GPIO.0:0x02 GPIO.1:0x04</param>
        /// <param name="cklDIv"></param>
        /// <returns></returns>
        [DllImport("SLABHIDtoSMBus.dll")]
        public static extern HID_SMBUS_STATUS HidSmbus_SetGpioConfig(IntPtr device, byte direction, byte mode, byte special, byte cklDIv);
        #endregion

       
    }
}
