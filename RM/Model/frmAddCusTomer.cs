using System;
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
    public partial class frmAddCusTomer : Form
    {
        public frmAddCusTomer()
        {
            InitializeComponent();
        }

        public string OrderType = "";
        public int mainID = 0;
        public int driverID = 0;
        public string cusName = "";

        private void frmAddCusTomer_Load(object sender, EventArgs e)
        {
            if (OrderType == "Take Away")
            {
                lblDriver.Visible = false;
                cbDriver.Visible = false;
            }

            string qry = "Select staffID 'id', sName 'name' from staff Where sRole = 'Driver' ";
            MainClass.CBFILL(qry, cbDriver);

            if (mainID >0)
            {
                cbDriver.SelectedValue = driverID;
            }
        }

        private void cbDriver_SelectedIndexChanged(object sender, EventArgs e)
        {
            driverID = Convert.ToInt32(cbDriver.SelectedValue);
        }
    }
}
