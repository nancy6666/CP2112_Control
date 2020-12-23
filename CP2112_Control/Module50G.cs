using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CP2112_Control
{
    class Module50G
    {
        #region Const 

        /* reg address define */
        public const byte PageType_RegAddr = 0x80;
        public const byte STATUS = 0x85;
        public const byte PARAMETERS1_RegAddr = 0x81;
        public const byte PARAMETERS2_RegAddr = 0x82;
        public const byte PARAMETERS3_RegAddr = 0x83;
        public const byte PARAMETERS4_RegAddr = 0x84;
        public const byte PARAMETERS5_RegAddr = 0xA8;
        public const byte PARAMETERS6_RegAddr = 0xA9;
        public const byte OUTPUT1_RegAddr = 0x86;
        public const byte OUTPUT2_RegAddr = 0x87;
        public const byte OUTPUT3_RegAddr = 0x88;
        public const byte OUTPUT4_RegAddr = 0x89;

        /* type value define,set the values into TYPE_RegAddr */
        const byte TYPE_NULL = 0x00;
        public const byte TYPE_IN015050_W = 0x01;
        public const byte TYPE_IN015050_R = 0x02;

        public const byte TYPE_ADC_MCU_TEMP = 0x03; //ADC 
        public const byte TYPE_ADC_MCU_VCC = 0x04;
        public const byte TYPE_ADC_TX_POWER0 = 0x06;
        public const byte TYPE_ADC_RX_POWER0 = 0x07;
        public const byte TYPE_ADC_TEMP_TOSA = 0x09;
        public const byte TYPE_ADC_IDRV_MON = 0x0A;
        public const byte TYPE_ADC_TX_VP = 0x16;

        public const byte TYPE_DAC_LD_IBIAS_SET = 0x0B; //DAC 
        public const byte TYPE_DAC_RX_APD_SET = 0x0C;
        public const byte TYPE_DAC_TX_VC_SET = 0x0D;
        public const byte TYPE_DAC_TX_VEA_SET = 0x0E;
        public const byte TYPE_DAC_TX_VG_SET = 0x0F;
        public const byte TYPE_DAC_TX_TEC_SET = 0x10;
        public const byte TYPE_DAC_RX_VGC_SET = 0x11;

        public const byte TYPE_GPO_RSTN_GBL = 0x12; //GPO 
        public const byte TYPE_GPO_INIT_GBL = 0x13;
        public const byte TYPE_GPO_VGB_0V75 = 0x14;
        public const byte TYPE_GPO_TX_TEC_EN = 0x15;

        public const byte TYPE_GPO_VGB_1V1_EN = 0x17;
        public const byte TYPE_GPO_3V3_EN = 0x18;
        public const byte TYPE_GPO_INTN_EN = 0x19;

        public const byte TYPE_GPI_LPMODE = 0x1A;//GPI
        public const byte TYPE_GPI_INT_GB = 0x1B;
        public const byte TYPE_GPI_MODSELN = 0x1C;

        public const byte TYPE_DAC_RX_LOS_SET = 0x20;  //DAC  ER

        public const byte TYPE_GPO_TX_DIS = 0x21;
        public const byte TYPE_GPO_VAPD_EN = 0x22;

        public const byte TYPE_GPI_RX_LOS = 0x23;

        public const byte TYPE_PAM4_LTX_PRE_TAP = 0x28;//PAM4 debug
        public const byte TYPE_PAM4_LTX_MAIN_TAP = 0x29;
        public const byte TYPE_PAM4_LTX_POST_TAP = 0x2A;
        public const byte TYPE_PAM4_LTX_SWING = 0x2B;
        public const byte TYPE_PAM4_LTX_INNER_EYE1 = 0x2C;
        public const byte TYPE_PAM4_LTX_INNER_EYE2 = 0x2D;

        public const byte TYPE_PAM4_7TAP_PRE3 = 0x2E;//PAM4 7 TAP debug
        public const byte TYPE_PAM4_7TAP_PRE2 = 0x2F;//PAM4 7 TAP debug
        public const byte TYPE_PAM4_7TAP_PRE1 = 0x30;//PAM4 7 TAP debug
        public const byte TYPE_PAM4_7TAP_MAIN_TAP = 0x31;//PAM4 7 TAP debug
        public const byte TYPE_PAM4_7TAP_POST1 = 0x32;//PAM4 7 TAP debug
        public const byte TYPE_PAM4_7TAP_POST2 = 0x33;//PAM4 7 TAP debug
        public const byte TYPE_PAM4_7TAP_POST3 = 0x34;//PAM4 7 TAP debug


        public const byte TYPE_ADC_ITEC_TOSA = 0x40;
        public const byte TYPE_ADC_VAPD_MON = 0x41;


        public const byte DSP_FW_DOWNLOAD_START = 0x54;//DSP download firmware
        public const byte DSP_FW_DOWNLOAD = 0x55;//DSP download firmware
        public const byte DSP_FW_DOWNLOAD_STOP = 0x56;//DSP download firmware

        /* debug status define */
        public const byte STATUS_IDLE = 0;
        public const byte STATUS_BUSY = 1;
        public const byte STATUS_ERROR_CMD = 2;

        #endregion
        public const byte SlaveAddr = 0xA0;
        private uint BitRate = 400000;//400KHz

        private bool GetEngMode()
        {
            SelectPage(PageType_RegAddr);
            bool ret = false;
            try
            {
                byte[] data = GlobalVar.CP2112DataTransfer.ReadData(SlaveAddr, new byte[] { 0x8C }, 1);
                if (data[0] == 0x03)//  0x03:"Eng",0x00: "Normal",0x02:"Customer"
                {
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Read module work mode error!{ex.Message}");
            }
            return ret;
        }

        private void EnterEngMod()
        {
            try
            {
                GlobalVar.CP2112DataTransfer.SetDefault(SlaveAddr, this.BitRate);
                if (GetEngMode())
                {
                    return;//已经处于EngMod，无需再向下执行
                }
                byte regAddr = 0x7B;
                byte[] data = new byte[] { 0x49, 0x52, 0x58, 0x49 };//需要确认地址连续的情况下，只写首位地址是否可行
                //input password to access engineering module
                GlobalVar.CP2112DataTransfer.WriteData(SlaveAddr, regAddr, data);
                if (!GetEngMode())
                {
                    throw new Exception("Model is not in EngMod！");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Enter EngMod error!{ex.Message}");
            }
        }

        private void SelectPage(byte pageType)
        {
            try
            {
                byte regAddr = 0x7F;
                //page select UP80H_Config,write value=TYPE_RegAddr into reg 0x7F
                GlobalVar.CP2112DataTransfer.WriteData(SlaveAddr, regAddr, new byte[] { pageType });
            }
            catch (Exception ex)
            {
                throw new Exception($"Select page {pageType.ToString()} error!{ex.Message}");
            }
        }
        /// <summary>
        /// connect Cp2112EK,then enter engineering module and select page as UP80H_Config
        /// </summary>
        public void IniteModule()
        {
            EnterEngMod();
            SelectPage(PageType_RegAddr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public static void SetDAC(int value, byte typeDAC)
        {
            byte[] ibias = BitConverter.GetBytes(value);

            try
            {
                //MSB 写入PARAMETERS3_RegAddr
                GlobalVar.CP2112DataTransfer.WriteData(SlaveAddr, PARAMETERS3_RegAddr, new byte[] { ibias[1] });
                //LSB 写入PARAMETERS4_RegAddr
                GlobalVar.CP2112DataTransfer.WriteData(SlaveAddr, PARAMETERS4_RegAddr, new byte[] { ibias[0] });
                //set debug cmd type as typeDAC
                GlobalVar.CP2112DataTransfer.WriteData(SlaveAddr, PageType_RegAddr, new byte[] { typeDAC });
            }
            catch (Exception ex)
            {
                throw new Exception($"Set LD IBIAS error!{ex.Message}");
            }
        }

        private static bool CheckModuleReady()
        {
            bool bReady = false;
            double totalTimeFind = 0;
            DateTime startTime = System.DateTime.Now;
            
            while (true)
            {
               byte[] Data = GlobalVar.CP2112DataTransfer.ReadData(SlaveAddr, new byte[] {STATUS });
                totalTimeFind = (System.DateTime.Now - startTime).TotalMilliseconds;
                if (Data[0] == 0)
                {
                    bReady = true;
                    break;
                }
                if (totalTimeFind > 2000)
                    break;
            }
            return bReady;
        }

        public static int ReadADC(byte typeADC)
        {
            try
            {
                //set debug cmd type as typeADC
                GlobalVar.CP2112DataTransfer.WriteData(SlaveAddr, PageType_RegAddr, new byte[] { typeADC });
                //if(!CheckModuleReady())
                //{
                //    throw new Exception("Module is not ready when Read ADC");
                //}
                var data = GlobalVar.CP2112DataTransfer.ReadData(SlaveAddr, new byte[] { OUTPUT1_RegAddr });
                byte MSB = data[0];
                data = GlobalVar.CP2112DataTransfer.ReadData(SlaveAddr, new byte[] { OUTPUT2_RegAddr });
                byte LSB = data[0];
                return(int)(MSB << 8) | LSB;
            }
            catch (Exception ex)
            {
                throw new Exception($"Read ADC error!{ex.Message}");
            }
        }

        /// <summary>
        /// 在对IN015050进行操作之前，需要将拿到的Addr分别写入 PARAMETERS5，6,1,2的寄存器中
        /// </summary>
        /// <param name="addr">地址</param>
        public static void SetAddrForIN015050(UInt32 addr)
        {
            try
            {
                byte[] addrs = BitConverter.GetBytes(addr);
                GlobalVar.CP2112DataTransfer.WriteData(SlaveAddr, PARAMETERS5_RegAddr, new byte[] { addrs[3] });
                GlobalVar.CP2112DataTransfer.WriteData(SlaveAddr, PARAMETERS6_RegAddr, new byte[] { addrs[2] });
                GlobalVar.CP2112DataTransfer.WriteData(SlaveAddr, PARAMETERS1_RegAddr, new byte[] { addrs[1] });
                GlobalVar.CP2112DataTransfer.WriteData(SlaveAddr, PARAMETERS2_RegAddr, new byte[] { addrs[0] });
            }
            catch(Exception ex)
            {
                throw new Exception($"Set Address For IN015050 error!{ex.Message}");
            }
        }
    }
}
