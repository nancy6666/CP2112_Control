using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CP2112_Control
{
    class MyNumbericUpDown1: NumericUpDown
    {
        #region Properties
       
        /// <summary>
        /// for DAC, after set paramters,choose the type to set into 0x80 
        /// </summary>
        public byte Add_Property_TypeValueSet
        {
            get;
            set;
        }
        private decimal oldValue
        {
            get;
            set;
        }
        #endregion

        #region Methods

        /// <summary>
        /// 文本框输入后需要按enter键方执行该响应函数
        /// </summary>
        /// <param name="e"></param>
        protected override void OnValueChanged(EventArgs e)
        {
            base.OnValueChanged(e);
            try
            {
                Module50G.SetDAC((int)this.Value, Add_Property_TypeValueSet);
                oldValue = this.Value;
                this.BackColor = Color.White;//textbox 控件的值写入设备后，控件背景色变为白色
                this.Select(0, this.Text.Length);
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}");
            }
        }
        //protected override void OnKeyPress(KeyPressEventArgs e)
        //{
        //    base.OnKeyPress(e);
        //    if(e.KeyChar==(char)Keys.Enter)
        //    {
        //        try
        //        {
        //            byte[] writeBuffer = BitConverter.GetBytes((ushort)(this.Value));

        //            //MSB 写入PARAMETERS3_RegAddr
        //            GlobalVar.CP2112DataTransfer.WriteData(Module50G.SlaveAddr, Module50G.PARAMETERS3_RegAddr, new byte[] { writeBuffer[1] });
        //            //LSB 写入PARAMETERS4_RegAddr
        //            GlobalVar.CP2112DataTransfer.WriteData(Module50G.SlaveAddr, Module50G.PARAMETERS4_RegAddr, new byte[] { writeBuffer[0] });
        //            //set debug cmd type,such as TypeValueSet= 0x0B means DAC_LD_IBIAS_SET
        //            GlobalVar.CP2112DataTransfer.WriteData(Module50G.SlaveAddr, Module50G.PageType_RegAddr, new byte[] { TypeValueSet });

        //            oldValue = this.Value;
        //            this.BackColor = Color.White;
        //            this.Select(0, this.Text.Length);
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception($"{ex.Message}");
        //        }
        //    }
        //}

            /// <summary>
            /// text 里的值与写入设备的值不同时为灰色
            /// </summary>
            /// <param name="e"></param>
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            if (this.Text != oldValue.ToString())
            {
                this.BackColor = Color.LightGray;
            }
            else
            {
                this.BackColor = Color.White;
            }
        }
        #endregion
    }
}
