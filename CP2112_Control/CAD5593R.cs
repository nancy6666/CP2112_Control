using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP2112_Control
{
    class CAD5593R
    {
        private uint BitRate = 100000;//Hz,standard mode (100 kHz) and fast mode (400 kHz)
        public byte A0 = 0x1;
        public byte slavaAddr = 0x20;
        public byte regDACConfigAddr=0x05;
        public byte regADCConfigAddr = 0x04;
        public byte regGeneralCtrlAddr = 0x03;

        /// <summary>
        /// control registers configure the I/O pins and set various operating parameters in the AD5593R
        /// </summary>
        public enum EnumContrlRegAddr
        {
            ADCSequen=0x02,
            GPContrl=0x03,
            ADCConfig=0x04,
            DACConfig=0x05,
            PulDownConfig=0x06,
            LDACMode=0x07,
            GPIOWriteConfig=0x08,
            GPIOWriteData=0x09,
            GPIOReadConfig=0x0A,
            PowerDownContrl=0x0B,
            OpenDrainConfig=0x0C,
            ThreeStatePin=0x0D,
            ResetSoftware=0x0F
        }

        /// <summary>
        /// Data is written to the selected DAC input register.
        /// </summary>
        public enum EnumWriteDACAddr
        {
            IO0=0x10,
            IO1,
            IO2,
            IO3,
            IO4,
            IO5,
            IO6,
            IO7
        }
        public byte MSB;
        public byte LSB;
        public enum EnumIO
        {
            IO0 = 0,
            IO1,
            IO2,
            IO3,
            IO4,
            IO5,
            IO6,
            IO7
        }
        public CAD5593R()
        {
            slavaAddr =Convert.ToByte( slavaAddr| (A0 << 1));
        }
        public void SetIOMode(EnumContrlRegAddr regAddr,EnumIO IO)
        {
            MSB = 0x00;
            LSB = Convert.ToByte(0x01 << (int)IO);
        }
        public void SetGPControlReg()
        {

        }
    }
}
