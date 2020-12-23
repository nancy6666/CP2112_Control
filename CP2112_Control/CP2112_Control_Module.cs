using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CP2112_Control
{
    public partial class CP2112_Control_Module : Form
    {
        #region Properties

        Module50G m50G = new Module50G();
        #endregion

        public CP2112_Control_Module()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitiateDevice();
        }
        private void DisableContols()
        {
            foreach (Control whole in Controls)
            {
                whole.Enabled = false;
            }
            this.btnConnect.Enabled = true;
        }
        private void EnableControls()
        {
            foreach (Control whole in Controls)
            {
                whole.Enabled = true;
            }
        }
        private void btnConnect_Click(object sender, EventArgs e)
        {
            InitiateDevice();
        }
        #region Methods

        private void InitiateDevice()
        {
            try
            {
                //connect Cp2112EK at first
                GlobalVar.CP2112DataTransfer.ConnectCP2112EK();
                //access module 
                m50G.IniteModule();
                ShowMsg("Open device successfully! Module is in Eng Mod!", true);
            }
            catch (Exception ex)
            {
                ShowMsg($"Open device failed!{ex.Message}", false);
            }
        }

        private  void ShowMsg(string msg, bool blPass)
        {
            this.Invoke(new Action<string, bool>((x, y) =>
            {
                MsgEvent(x, y);
            }), new object[] { msg, blPass });
        }
        private void MsgEvent(string msg, bool blPass)
        {
            this.lstLog.Items.Clear();
            var liv = new ListViewItem(msg);
            lstLog.Items.Add(liv);
            if (!blPass)
                liv.ForeColor = Color.Red;
            else liv.ForeColor = Color.Green;
        }
        #endregion


        private void myNumbericIN105050Write_ValueChanged(object sender, EventArgs e)
        {
            if (this.txtIN015050RegAddr_Write.Text == "")
            {
                MessageBox.Show("Pls input RegAddr for Writing In015050!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                Module50G.SetAddrForIN015050(Convert.ToUInt32(this.txtIN015050RegAddr_Write.Text));
                this.myNumbericIN105050Write.Add_Property_TypeValueSet = Module50G.TYPE_IN015050_W;
            }
            catch (Exception ex)
            {
                ShowMsg(ex.Message, false);
            }
        }

        private void btnReadIN015050_Click_1(object sender, EventArgs e)
        {
            if (this.txtIN015050RegAddr_Read.Text == "")
            {
                MessageBox.Show("Pls input RegAddr for Reading In015050!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                Module50G.SetAddrForIN015050(Convert.ToUInt32(this.txtIN015050RegAddr_Read.Text));

                this.btnReadIN015050.Add_Property_TypeValueSet = Module50G.TYPE_IN015050_R;
            }
            catch (Exception ex)
            {
                ShowMsg(ex.Message, false);
            }
        }

        private void btnReadAll_Click(object sender, EventArgs e)
        {
            try
            {
                btnTempTx.TriggerOnMouseClick();
                btnImpd.TriggerOnMouseClick();
                //  btnILDMon.TriggerOnMouseClick();
                //btnIEAM.TriggerOnMouseClick();
                btnITECTX.TriggerOnMouseClick();
                myButton11.TriggerOnMouseClick();
                ShowMsg($"Read All ADC successfully!", true);

            }
            catch (Exception ex)
            {
                ShowMsg($"Read All ADC error!{ex.Message}",false);
            }
        }

    }
}
