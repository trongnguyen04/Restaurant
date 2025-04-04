using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms.VisualStyles;
using System.Collections;

namespace RM.Model
{
    public partial class frmPOS : Form
    {
        public frmPOS()
        {
            InitializeComponent();
        }

        public int MainID = 0;
        public string OrderType;

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmPOS_Load(object sender, EventArgs e)
        {
            guna2DataGridView1.BorderStyle = BorderStyle.FixedSingle;
            AddCategory();

            ProductPanel.Controls.Clear();
            LoadProduct();
            
        }

        private void AddCategory()
        {
            string qry = "Select * from category";
            SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            CategoryPanel.Controls.Clear();

            if (dt.Rows.Count > 0)
            {
                foreach(DataRow row in dt.Rows) {
                    Guna.UI2.WinForms.Guna2Button b = new Guna.UI2.WinForms.Guna2Button();
                    b.FillColor = Color.FromArgb(50, 55, 89);
                    b.Size = new Size(134, 45);
                    b.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
                    b.Text = row["catName"].ToString();

                    //event for click
                    b.Click += new EventHandler(b_Click);
                    CategoryPanel.Controls.Add(b);
                }             
            }
        }

        private void b_Click(object sender, EventArgs e)
        {
            Guna.UI2.WinForms.Guna2Button b = (Guna.UI2.WinForms.Guna2Button)sender;
            if (b.Text == "All Categories")
            {
                txtSearch.Text = "1";
                txtSearch.Text = "";
                return;
            }
            foreach (var item in ProductPanel.Controls)
            {
                var pro = (usProduct)item;
                pro.Visible = pro.PCategory.ToLower().Contains(b.Text.Trim().ToLower());
            }
        }

        private void AddItems(string id, String proID, string name, string cat, string price, Image pimage)
        {
            var w = new usProduct
            {
                PName = name,
                PPrice = price,
                PCategory = cat,
                PImage = pimage,
                id = Convert.ToInt32(proID)
            };

            ProductPanel.Controls.Add(w);

            w.onSelect += (ss, ee) =>
            {
                var wdg = (usProduct)ss;

                foreach (DataGridViewRow item in guna2DataGridView1.Rows)
                {
                    //this will check it product already there then a one to quantity and update price
                    if (Convert.ToInt32(item.Cells["dgvproID"].Value) == wdg.id)
                    {
                        item.Cells["dgvQty"].Value = int.Parse(item.Cells["dgvQty"].Value.ToString()) + 1;
                        item.Cells["dgvAmount"].Value = int.Parse(item.Cells["dgvQty"].Value.ToString()) *
                                                         double.Parse(item.Cells["dgvPrice"].Value.ToString());
                        return;
                    }
                }
                //this line add new product First for sr# and 2nd 0 from id
                guna2DataGridView1.Rows.Add(new object[] { 0, wdg.id, wdg.PName, 1, wdg.PPrice, wdg.PPrice });
                GetTotal();
            };
        }

        //geting product from database

        private void LoadProduct()
        {
            string qry = "Select * from products inner join category on catID = categoryID";
            SqlCommand cmd = new SqlCommand(qry, MainClass.con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            foreach (DataRow item in dt.Rows)
            {
                Byte[] imagearray = (byte[])item["pImage"];
                byte[] imagebytearray = imagearray;

                AddItems("0",item["pID"].ToString(), item["pName"].ToString(), item["catName"].ToString(),
                              item["pPrice"].ToString(), Image.FromStream(new MemoryStream(imagebytearray)));

            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            foreach(var item in ProductPanel.Controls)
            {
                var pro = (usProduct)item;
                pro.Visible = pro.PName.ToLower().Contains(txtSearch.Text.Trim().ToLower());
            }
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

        private void GetTotal()
        {
            double tot = 0;
            lblTotal.Text = "";
            //foreach (DataGridViewRow item in guna2DataGridView1.Rows)
            //{
            //    tot += double.Parse(item.Cells["dgvAmount"].Value.ToString());
            //}
            foreach (DataGridViewRow item in guna2DataGridView1.Rows)
            {
                if (item.Cells["dgvAmount"].Value != null) // Kiểm tra null trước khi sử dụng
                {
                    double amount;
                    if (double.TryParse(item.Cells["dgvAmount"].Value.ToString(), out amount))
                    {
                        tot += amount;
                    }
                }
            }

            lblTotal.Text = tot.ToString("N2");
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            lblTable.Text = "";
            lblWaiter.Text = "";
            lblWaiter.Visible = false;
            lblTable.Visible = false;
            guna2DataGridView1.Rows.Clear();
            MainID = 0;
            lblTotal.Text = "00";
        }

        private void btnDelivery_Click(object sender, EventArgs e)
        {

            lblTable.Text = "";
            lblWaiter.Text = "";
            lblWaiter.Visible = false;
            lblTable.Visible = false;
            OrderType = "Delivery";
        }

        private void btnTake_Click(object sender, EventArgs e)
        {
            lblTable.Text = "";
            lblWaiter.Text = "";
            lblWaiter.Visible = false;
            lblTable.Visible = false;
            OrderType = "Take Away";
        }

        private void btnDin_Click(object sender, EventArgs e)
        {
            OrderType = "Din In";
            //need to create form for table selection and waiter selection
            frmTableSelect frm = new frmTableSelect();
            MainClass.BlurBackground(frm);
            if (frm.TableName != "")
            {
                lblTable.Text=frm.TableName;
                lblTable.Visible=true;
            }
            else
            {
                lblTable.Text = "";
                lblTable.Visible = false;
            }
            frmWaiterSelect frm2 = new frmWaiterSelect();
            MainClass.BlurBackground(frm2);
            if(frm2.WaiterName != "")
            {
                lblWaiter.Text = frm2.WaiterName;
                lblWaiter.Visible = true;
            }
            else
            {
                lblWaiter.Text = "";
                lblWaiter.Visible = false;
            }
        }

        private void btnKot_Click(object sender, EventArgs e)
        {
            //save the day in database
            //create table
            string qry1 = ""; //Main table 
            string qry2 = ""; //Detail table

            int detailID = 0;

            if(MainID ==0) //Insert
            {
                qry1 = @"Insert into tblMain Values(@aDate, @aTime, @TableName, @WaiterName,@status
                                    @orderType, @total, @received, @change); Select SCOPE_IDENTITY()";

               //this line will get recent add id value 
            }
            else //Update
            {
                qry1 = @"Update tblMain Set status = @status, total = @total,received = @received, 
                                    change = @change where MainID = @ID";
            }

            Hashtable ht = new Hashtable();

            SqlCommand cmd = new SqlCommand(qry1, MainClass.con);
            cmd.Parameters.AddWithValue("ID", MainID);
            cmd.Parameters.AddWithValue("@aDate",Convert.ToDateTime(DateTime.Now.Date));
            cmd.Parameters.AddWithValue("@aTime", DateTime.Now.ToShortTimeString());
            cmd.Parameters.AddWithValue("@TableName", lblTable.Text);
            cmd.Parameters.AddWithValue("@WaiterName", lblWaiter.Text);
            cmd.Parameters.AddWithValue("@status", "Pending");
            cmd.Parameters.AddWithValue("@orderType", OrderType);
            cmd.Parameters.AddWithValue("@total", Convert.ToDouble(lblTotal.Text));
            cmd.Parameters.AddWithValue(" @received",Convert.ToDouble(0)); //as we only saving data for kitchen value will update when payment received
            cmd.Parameters.AddWithValue("@change", Convert.ToDouble(0));
           
            if(MainClass.con.State == ConnectionState.Closed )
            {
                MainClass.con.Open();
            }
            if(MainID == 0) { MainID = Convert.ToInt32(cmd.ExecuteScalar()); } else { cmd.ExecuteNonQuery(); }
            if(MainClass.con.State == ConnectionState.Open ) { MainClass.con.Close(); }

            foreach(DataGridViewRow row in guna2DataGridView1.Rows)
            {
                detailID = Convert.ToInt32(row.Cells["dgvid"].Value);

                if(detailID == 0) //Insert
                {
                    qry2 = @"Insert into tblDetails Values (@MainID, @proID, @qty, @price, @amount";
                }

                else //Update
                {
                    qry2 = @"Update tblDetails Set proID = @proID, qty = @qty, price = @price, amount = @amount
                                                            where DetailID = @ID";
                }

                SqlCommand cmd2 = new SqlCommand(qry2, MainClass.con);
                cmd2.Parameters.AddWithValue("ID", detailID);
                cmd2.Parameters.AddWithValue("@MainID", MainID);
                cmd2.Parameters.AddWithValue("@proID",Convert.ToInt32(row.Cells["dgvproID"].Value));
                cmd2.Parameters.AddWithValue("@qtyID", Convert.ToInt32(row.Cells["dgvQty"].Value));
                cmd2.Parameters.AddWithValue("@priceID", Convert.ToDouble(row.Cells["dgvPrice"].Value));
                cmd2.Parameters.AddWithValue("@amountID", Convert.ToDouble(row.Cells["dgvAmount"].Value));

                if (MainClass.con.State == ConnectionState.Closed)
                {
                    MainClass.con.Open();
                }
                cmd2.ExecuteNonQuery(); }
                if (MainClass.con.State == ConnectionState.Open) { MainClass.con.Close(); }

            guna2MessageDialog1.Show("Saved successfully");
            MainID = 0;
            detailID= 0;
            guna2DataGridView1.Rows.Clear();
            lblTable.Text = "";
            lblWaiter.Text = "";
            lblWaiter.Visible = false;
            lblTable.Visible = false;
            lblTotal.Text = "00";


        }
        }
    }

