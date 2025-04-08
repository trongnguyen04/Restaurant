using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RM.Model
{
    public partial class frmCheckout : SampleAdd
    {
        public frmCheckout()
        {
            InitializeComponent();
            LoadImage();
        }

        private void frmCheckout_Load(object sender, EventArgs e)
        {
            txtBillAmount.Text = amt.ToString();
        }

        private void LoadImage()
        {
            guna2PictureBox1.Image = Properties.Resources.productPic;
            guna2PictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

        }

        public double amt;
        public int MainID = 0;

        private void txtReceived_TextChanged(object sender, EventArgs e)
        {
            double amt = 0;
            double receipt = 0;
            double change = 0;

            double.TryParse(txtBillAmount.Text, out amt);
            double.TryParse(txtReceived.Text, out receipt);

            change =Math.Abs(amt - receipt); //always positive
            txtChange.Text = change.ToString();
        }

        public override void btnSave_Click(object sender, EventArgs e)
        {
            string qry = @"Update tblMain set total = @total, received = @rec, change = @change, status ='Paid'
                                          Where MainID = @id";
            Hashtable ht = new Hashtable();
            ht.Add("@id", MainID);
            ht.Add("@total", txtBillAmount.Text);
            ht.Add("@rec", txtReceived.Text);
            ht.Add("@change", txtChange.Text);

            if (MainClass.SQl(qry,ht)>0)
            {
                guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
                guna2MessageDialog1.Show("Saved successfully");
                this.Close();
            }
        }
    }
}
