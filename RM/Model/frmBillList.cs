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
    public partial class frmBillList : SampleAdd
    {
        public frmBillList()
        {
            InitializeComponent();
            LoadImage();
        }

        public int MainID = 0;
        private void LoadImage()
        {
            guna2PictureBox1.Image = Properties.Resources.productPic;
            guna2PictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

        }

        private void frmBillList_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            string qry = "select MainID, TableName, WaiterName, orderType, status, total from tblMain where status <> 'Pending' ";
            ListBox lb = new ListBox();
            lb.Items.Add(dgvid);
            lb.Items.Add(dgvTable);
            lb.Items.Add(dgvWaiter);
            lb.Items.Add(dgvType);
            lb.Items.Add(dgvTotal);

            MainClass.LoadData(qry, guna2DataGridView1, lb);
        }

        private void guna2DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // for searil no
            int count = 0;

            foreach (DataGridViewRow row in guna2DataGridView1.Rows)
            {
                count++;
                row.Cells[0].Value = count;
            }
        }

        private void guna2DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (guna2DataGridView1.CurrentCell.OwningColumn.Name == "dgvedit")
            {
                //this is changes as we have to set form text properties before open

                MainID = Convert.ToInt32(guna2DataGridView1.CurrentRow.Cells["dgvid"].Value);
                this.Close();
               
            }

            if (guna2DataGridView1.CurrentCell.OwningColumn.Name == "dgvedel")
            {
                //print bill
            }
        }
    }
}
