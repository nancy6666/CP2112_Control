using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CP2112_Control
{
   public class MyButton1:Button
    {
       
        public UnitConverter.EnumUnit Add_Property_Unit
        {
            get;set;
        }
        /// <summary>
        /// for ADC, choose the type to set into 0x80 before reading data
        /// </summary>
        public byte Add_Property_TypeValueSet
        {
            get;
            set;
        }
        public TextBox Add_Property_txtReadValueDisPlay
        {
            get;
            set;
        }
        public void TriggerOnMouseClick()
        {
            OnMouseClick(new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0));
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            try
            {
                if (Add_Property_txtReadValueDisPlay == null )
                {
                    throw new Exception($"Error!请在控件按钮{this.Text}的属性中为ReadValueDisPlay赋值！");
                }
                var data = Module50G.ReadADC(Add_Property_TypeValueSet);
                this.Add_Property_txtReadValueDisPlay.Text= ConvertUnit(data).ToString();
               
            }
            catch (Exception ex)
            {
                throw new Exception($"{ ex.Message }");
            }
        }

        private decimal ConvertUnit(int data)
        {
            decimal ret = 0;
            switch(Add_Property_Unit)
            {
                case UnitConverter.EnumUnit.mA:
                   ret= UnitConverter.ConvertTomA(data);
                    break;

            }
            return ret;
        }
    }
}
