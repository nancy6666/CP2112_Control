using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CP2112_Control
{
    class InstCP2112DataTransfer
    {
        #region Properties
        // SMbus Configuration Limits
        public const ushort HID_SMBUS_MIN_BIT_RATE = 1;//The bit rate for SMBus communication must be non-zero

        public const ushort HID_SMBUS_MAX_RETRIES = 1000;//transferRetries is the number of times to retry (0 - 1000)
        public const ushort HID_SMBUS_MIN_TIMEOUT = 0;
        public const ushort HID_SMBUS_MAX_TIMEOUT = 1000;
        public const byte HID_SMBUS_MIN_ADDRESS = 0x02;
        public const byte HID_SMBUS_MAX_ADDRESS = 0xFE;
        // Read/Write Limits
        public const ushort HID_SMBUS_MIN_READ_REQUEST_SIZE = 1;
        public const ushort HID_SMBUS_MAX_READ_REQUEST_SIZE = 512;

        public const ushort HID_SMBUS_MIN_TARGET_ADDRESS_SIZE = 1;
        public const ushort HID_SMBUS_MAX_TARGET_ADDRESS_SIZE = 16;

        public const ushort HID_SMBUS_MIN_WRITE_REQUEST_SIZE = 1;
        public const ushort HID_SMBUS_MAX_WRITE_REQUEST_SIZE = 61;

        private uint _bitRate;
        public uint BitRate
        {
            get
            {
                return _bitRate;
            }
            set
            {
                if (value <HID_SMBUS_MIN_BIT_RATE)
                {
                    throw new Exception("$bit rate for SMBus communication must be non-zero!");
                }
                else
                {
                    _bitRate = value;
                }
            }
        }

        private ushort _readTimeout;
        public ushort ReadTimeout
        {
            get
            {
                return _readTimeout;
            }
            set
            {
                if (value < HID_SMBUS_MIN_TIMEOUT| value > HID_SMBUS_MAX_TIMEOUT)
                {
                    throw new Exception("$TimeOut should be 0-1000!");
                }
                else
                {
                    _readTimeout = value;
                }
            }
        }

        private ushort _writeTimeout;
        public ushort WriteTimeout
        {
            get
            {
                return _writeTimeout;
            }
            set
            {
                if (value < HID_SMBUS_MIN_TIMEOUT | value > HID_SMBUS_MAX_TIMEOUT)
                {
                    throw new Exception("$TimeOut should be 0-1000!");
                }
                else
                {
                    _writeTimeout = value;
                }
            }
        }
        private ushort _transferRetries;
        public ushort TransferRetries
        {
            get
            {
                return _transferRetries;
            }
            set
            {
                if (value > HID_SMBUS_MAX_RETRIES)
                {
                    throw new Exception("transferRetries is the number of times to retry (0 - 1000)!");
                }
                else
                {
                    _transferRetries = value;
                }
            }
        }

       // private ushort 
        private byte _slaveAddress;
        public byte SlaveAddress
        {
            get
            {
                return _slaveAddress;
            }
            set
            {
                if (value < HID_SMBUS_MIN_ADDRESS | value > HID_SMBUS_MAX_ADDRESS)
                {
                    throw new Exception("$Address error,the slave address should between 0x02 - 0xFE!");
                }
                else
                {
                    if (_slaveAddress % 2 == 0)
                    {
                        _slaveAddress = value;
                    }
                    else
                    {
                        throw new Exception($"Address error,the slave address should be even number!");
                    }
                }
            }
        }
        public  bool AutoReadRespond;
        public bool SclLowTimeout;
        public uint ResponseTimeout;
        private ushort _numBytesToRead;
        public ushort NumBytesToRead
        {
            get
            {
                return _numBytesToRead;
            }
            set
            {
                if(value> HID_SMBUS_MAX_READ_REQUEST_SIZE | value < HID_SMBUS_MIN_READ_REQUEST_SIZE)
                {
                    throw new Exception("NumBytesToRead should be 1-512!");
                }
                else
                {
                    _numBytesToRead = value;
                }
            }
        }
        private ushort _numBytesToWrite;
        public ushort NumBytesToWrite
        {
            get
            {
                return _numBytesToWrite;
            }
            set
            {
                if (value > HID_SMBUS_MAX_WRITE_REQUEST_SIZE | value < HID_SMBUS_MIN_WRITE_REQUEST_SIZE)
                {
                    throw new Exception("NumBytesToWrite should be 1-61!");
                }
                else
                {
                    _numBytesToWrite = value;
                }
            }
        }
        private byte _targetAddressSize;
        public byte TargetAddressSize
        {
            get
            {
                return _targetAddressSize;
            }
            set
            {
                if(value>HID_SMBUS_MAX_TARGET_ADDRESS_SIZE | value <HID_SMBUS_MIN_TARGET_ADDRESS_SIZE)
                {
                    throw new Exception("TargetAddressSize should be 1-16!");
                }
                else
                _targetAddressSize = value;
            }
        }
        private IntPtr DeviceHandle;
        private ushort VID = 0;//0x10c4;
        private ushort PID = 0;//0xEA90;
        private uint DeviceNum = 0;//CP2112的设备序列号
        private uint NumDevices = 0;//PC检测到的设备数量
        private CP2112_API.HID_SMBUS_STATUS Status;
        #endregion

        #region Constructor
        public InstCP2112DataTransfer()
        {
        }
        #endregion

        #region Methods

        /// <summary>
        /// set properties, sumbsconfig and response timeout
        /// </summary>
        /// <param name="slaveAddress">Device address</param>
        /// <param name="bitRate">rate,unit is Hz</param>
        public void SetDefault(byte slaveAddress,uint bitRate)
        {
            BitRate = bitRate;//Hz
            SlaveAddress = slaveAddress;
            AutoReadRespond = true; //autoReadRespond controls the read response behavior of the device. If enabled, the device will automatically send read response interrupt reports to the device after initiating a read transfer.
            ReadTimeout = 0;
            WriteTimeout = 0;
            TransferRetries = 0;
            SclLowTimeout = true;
            ResponseTimeout = 1000;
            NumBytesToRead = 1;

            if (IsDeviceOpened())
            {
                Status = CP2112_API.HidSmbus_SetSmbusConfig(DeviceHandle, BitRate, SlaveAddress, AutoReadRespond, WriteTimeout, ReadTimeout, SclLowTimeout, TransferRetries);

                if (Status != CP2112_API.HID_SMBUS_STATUS.HID_SMBUS_SUCCESS)
                {
                    throw new Exception($"SetSmbusConfig Error! {Status.ToString()}");
                }
                Status = CP2112_API.HidSmbus_SetTimeouts(DeviceHandle, ResponseTimeout);
                if (Status != CP2112_API.HID_SMBUS_STATUS.HID_SMBUS_SUCCESS)
                {
                    throw new Exception($"HidSmbus_SetTimeouts Error! {Status.ToString()}");
                }
            }
        }
       
        public void ConnectCP2112EK()
        {
            try
            {
                GetNumDevices(ref NumDevices, 0, 0);

                ushort deviceReleaseNum = 0;
                for (uint i = 0; i < NumDevices; i++)
                {
                    Status = CP2112_API.HidSmbus_Open(ref DeviceHandle, i, 0, 0);
                    if (Status == CP2112_API.HID_SMBUS_STATUS.HID_SMBUS_SUCCESS)
                    {
                        DeviceNum = i;

                        CP2112_API.HidSmbus_GetOpenedAttributes(DeviceHandle, ref VID, ref PID, ref deviceReleaseNum);
                        break;
                    }
                    else if (Status != 0 & i == NumDevices - 1)
                    {
                        throw new Exception($"Can't Connect to an HID USB-to-SMBus device object:{Status.ToString()}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IsDeviceOpened()
        {
            bool isOpened = false;
            bool ret = false;
            if (DeviceHandle != null)
            {
                Status = CP2112_API.HidSmbus_IsOpened(DeviceHandle, ref isOpened);

                if (Status == CP2112_API.HID_SMBUS_STATUS.HID_SMBUS_SUCCESS & isOpened)
                {
                    ret = true;
                }
            }
            return ret;
           
        }

        private void GetNumDevices(ref uint numDevices, ushort vid, ushort pid)
        {
            Status = CP2112_API.HidSmbus_GetNumDevices(ref numDevices, vid, pid);
            if (Status != 0)
            {
                throw new Exception($"Failed to GetNumDevices! {Status.ToString()}");
            }
            else if (numDevices < 1)
            {
                throw new Exception($"Can't find any device connected to the PC!");
            }
        }

        /// <summary>
        /// Intiate a read transfer to the specified slave device address
        /// </summary>
        /// <param name="slaveAddress"></param>
        /// <param name="bytesToRead"></param>
        public byte[] ReadData(byte slaveAddress,ushort bytesToRead=1)
        {
            this.SlaveAddress = slaveAddress;
            this.NumBytesToRead = bytesToRead;
            //make sure the device is opened
            if(IsDeviceOpened())
            {
                Status = CP2112_API.HidSmbus_ReadRequest(DeviceHandle, SlaveAddress, this.NumBytesToRead);
                if(Status!=CP2112_API.HID_SMBUS_STATUS.HID_SMBUS_SUCCESS)
                {
                    throw new Exception($"ReadRequest error！{Status}");
                }
            }
           return GetDataAfterReadRequest(this.NumBytesToRead);
        }

        /// <summary>
        /// Intiate a read transfer to the specified slave device address and specifies the address to read from on the device.
        /// </summary>
        /// <param name="slaveAddress"></param>
        /// <param name="targetRegAddress">the address to read from the slave device.</param>
        /// <param name="bytesToRead"></param>
        public byte[] ReadData(byte slaveAddress,byte[] targetRegAddress,ushort bytesToRead = 1)
        {
            this.SlaveAddress = slaveAddress;
            this.NumBytesToRead = bytesToRead;
            this.TargetAddressSize = Convert.ToByte(targetRegAddress.Length);
            //make sure the device is opened
            if (IsDeviceOpened())
            {
                Status = CP2112_API.HidSmbus_AddressReadRequest(DeviceHandle, SlaveAddress, (byte)this.NumBytesToRead,TargetAddressSize, targetRegAddress);

                if (Status != CP2112_API.HID_SMBUS_STATUS.HID_SMBUS_SUCCESS)
                {
                    throw new Exception($"AddressReadRequest error！{Status}");
                }

            }
            return GetDataAfterReadRequest(this.NumBytesToRead);
        }

        /// <summary>
        /// GetReadResponse followed ReadRequest()
        /// </summary>
        /// <returns>valid data read from device</returns>
        private byte[] GetDataAfterReadRequest(ushort numBytesToRead)
        {
            byte[] dataRead=new byte[numBytesToRead];
            CP2112_API.HID_SMBUS_S0 hID_SMBUS_S0 = new CP2112_API.HID_SMBUS_S0();
            byte[] buffer = new byte[61];
            Status = CP2112_API.HidSmbus_ForceReadResponse(DeviceHandle, numBytesToRead);
            if(Status!=CP2112_API.HID_SMBUS_STATUS.HID_SMBUS_SUCCESS)
            {
                throw new Exception($"HidSmbus_ForceReadResponse error!{Status}");
            }
            byte numBytesReadInBuffer=0;

            //call HidSmbus_GetReadResponse() repeatedly until all read data is returned
            while (numBytesReadInBuffer != numBytesToRead)
            {
                Status = CP2112_API.HidSmbus_GetReadResponse(DeviceHandle, ref hID_SMBUS_S0, buffer, 61, ref numBytesReadInBuffer);

                if (Status == CP2112_API.HID_SMBUS_STATUS.HID_SMBUS_SUCCESS)
                {
                    if (hID_SMBUS_S0 == CP2112_API.HID_SMBUS_S0.HID_SMBUS_S0_COMPLETE)
                    {
                        if (numBytesReadInBuffer == numBytesToRead)
                        {
                            for (int i = 0; i < numBytesReadInBuffer; i++)
                            {
                                dataRead[i] = buffer[i];
                            }
                            break;
                        }
                    }
                }
                else
                {
                    throw new Exception($"HidSmbus_GetReadResponse error! HID_SMBUS_STATUS:{Status},HID_SMBUS_S0:{hID_SMBUS_S0}");
                }
            }
           
            return dataRead;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slaveAddress"></param>
        /// <param name="regAddr">寄存器地址</param>
        /// <param name="data">要写入寄存器的数据列表</param>
        public void WriteData(byte slaveAddress,byte regAddr, byte[] data)
        {
            this.SlaveAddress = slaveAddress;

            byte[] buffer = new byte[data.Length + 1];
            this.NumBytesToWrite =(ushort) buffer.Length;
            CP2112_API.HID_SMBUS_S0 hID_SMBUS_S0 = new CP2112_API.HID_SMBUS_S0();
            ushort detailedStatus = 0;
            ushort numRetries = 0;
            ushort bytesRead = 0;
            if (IsDeviceOpened())
            {
                //write 的buffer第一个自己为寄存器地址，后面为data
                buffer[0] = regAddr;
                for(int i=0;i<data.Length;i++)
                {
                    buffer[i + 1] = data[i];
                }
                Status = CP2112_API.HidSmbus_WriteRequest(DeviceHandle, SlaveAddress, buffer, (byte)NumBytesToWrite);

                if (Status == CP2112_API.HID_SMBUS_STATUS.HID_SMBUS_SUCCESS)
                {
                    Thread.Sleep(100);
                    Status = CP2112_API.HidSmbus_TransferStatusRequest(DeviceHandle);
                    if (Status == CP2112_API.HID_SMBUS_STATUS.HID_SMBUS_SUCCESS)
                    {
                        Thread.Sleep(100);

                        Status = CP2112_API.HidSmbus_GetTransferStatusResponse(DeviceHandle, ref hID_SMBUS_S0, ref detailedStatus, ref numRetries, ref bytesRead);
                        if (Status == CP2112_API.HID_SMBUS_STATUS.HID_SMBUS_SUCCESS)
                        {
                            if (hID_SMBUS_S0 != CP2112_API.HID_SMBUS_S0.HID_SMBUS_S0_COMPLETE)
                            {
                                throw new Exception($"TransferStatus:{DecodeTransferStatus(hID_SMBUS_S0, detailedStatus)}");
                            }
                        }
                        else
                        {
                            throw new Exception($"HidSmbus_GetTransferStatusResponse error!{Status.ToString()}");
                        }
                    }
                    else
                    {
                        throw new Exception($"HidSmbus_TransferStatusRequest error!{Status.ToString()}");
                    }
                }
                else
                {
                    throw new Exception($"HidSmbus_WriteRequest error!{Status.ToString()}");
                }
            }
        }

        private StringBuilder  DecodeTransferStatus(CP2112_API.HID_SMBUS_S0 status,ushort detailedStatus)
        {
            StringBuilder str = new StringBuilder();

            switch (status)
            {
                case CP2112_API.HID_SMBUS_S0.HID_SMBUS_S0_IDLE: str.Append("Idle"); break;
                case CP2112_API.HID_SMBUS_S0.HID_SMBUS_S0_BUSY: str.Append("Busy - "); break;
                case CP2112_API.HID_SMBUS_S0.HID_SMBUS_S0_COMPLETE: str.Append("Complete"); break;
                case CP2112_API.HID_SMBUS_S0.HID_SMBUS_S0_ERROR: str.Append("Error - "); break;
                default: str.Append("Unknown Status"); break;
            }

            if (status == CP2112_API.HID_SMBUS_S0.HID_SMBUS_S0_BUSY)
            {
                switch (detailedStatus)
                {
                    case 0x00: str.Append("Address Acked"); break;
                    case 0x01: str.Append("Address Nacked"); break;
                    case 0x02: str.Append("Read in Progress"); break;
                    case 0x03: str.Append("Write in Progress"); break;
                    default: str.Append("Unknown Status"); break;
                }
            }
            else if (status == CP2112_API.HID_SMBUS_S0.HID_SMBUS_S0_ERROR)
            {
                switch (detailedStatus)
                {
                    case 0x00: str.Append("Timeout (Address Nacked)"); break;
                    case 0x01: str.Append("Timeout (Bus Not Free)"); break;
                    case 0x02: str.Append("Arbitration Lost"); break;
                    case 0x03: str.Append("Read Incomplete"); break;
                    case 0x04: str.Append("Write Incomplete"); break;
                    case 0x05: str.Append("Success After Retries"); break;
                    default: str.Append("Unknown Status"); break;
                }
            }
            return str;
        }

        public byte[] HexStringToByteArray(string s)
        {
            s = s.Replace("0x", "");
            s = s.Replace("0X", "");
            byte[] buffer;
            if (s.Length == 1)
            {
                buffer = new byte[1];
                buffer[0] = (byte)Convert.ToByte(s);
            }
            else
            {
                buffer = new byte[s.Length / 2];
                for (int i = 0; i < s.Length; i += 2)
                {
                    buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
                }
            }
            return buffer;
        }
        #endregion
    }
}
