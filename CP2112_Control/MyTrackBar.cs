using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CP2112_Control
{
    class MyTrackBar: ZoomTrackBarControl
    {
        /// <summary>
        /// The label is used to show the current value of TrackBar
        /// </summary>
        public Label Add_Property_lblDisplayValue { get; set; }
        public byte Add_Property_TypeValueSet { get; set; }
        public MyTrackBar()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
            UpdateStyles();
        }

        /// <summary>
        /// value 需要实时显示，故放入OnEditValueChanged
        /// </summary>
        protected override void OnEditValueChanged()
        {
            base.OnEditValueChanged();
            if(this.Add_Property_lblDisplayValue!=null)
            {
                this.Add_Property_lblDisplayValue.Text = this.Value.ToString();
            }
        }
        /// <summary>
        /// 鼠标松开的时刻执行值写入设备，避免值变化时不断写入
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            try
            {
                if (this.Add_Property_lblDisplayValue == null)
                {
                    throw new Exception($"Pls assign Label control for property Add_Property_lblDisplayValue");
                }
                Module50G.SetDAC(this.Value, Add_Property_TypeValueSet);
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}");
            }
        }
    }
}
