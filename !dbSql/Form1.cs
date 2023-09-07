using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _dbSql
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //DBManager.conStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=E:\!projVis\!dbSql\!dbSql\Database2.mdf;Integrated Security=True";//|DataDirectory|
            //DBManager.DbName = "Database2.mdf";
            db = new DBManager();
            db.Connect();
        }

        DBManager db;
        List<string> Fields = new List<string>();

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox3.Text != "")
            {
                if (Fields.Count != 0)
                {
                    bool contains = false;
                    foreach (string s in Fields)
                    {
                        if (s.Contains("not null primary key"))
                            contains = true;
                        break;

                    };
                    if (!contains)
                    {
                        Fields[0] += " not null primary key";
                    }
                    listBox1.Items.Clear();
                    //textBox4.Text = "";
                    listBox2.Items.Clear();
                    db.CreateTable(textBox3.Text, Fields);
                    Fields.Clear();
                    /* foreach (string s in db.Tables)
                     {
                         listBox1.Items.Add(s);
                     }*/
                   //db.AllTables(listBox1);
                    textBox3.Clear();
                }
                else
                {
                    MessageBox.Show("Add fields");
                }
            }
            else
            {
                MessageBox.Show("Enter table name");
            }
            db.AllTables(listBox1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //textBox4.Text = "";
            if (!listBox1.Items.Contains(textBox3.Text))
            {
                listBox2.Items.Clear();
                Fields.Add(textBox1.Text + " " + textBox2.Text);
                textBox1.Text = "";
                textBox2.Text = "";
                foreach (string s in Fields)
                {
                    //textBox4.Text += s + Environment.NewLine;
                    listBox2.Items.Add(s);
                }
            }
            else
            {
                db.AddField(textBox3.Text, textBox1.Text + " " + textBox2.Text);
                db.SelectAll(listBox1.Items[listBox1.SelectedIndex].ToString(), dataGridView1, listBox2);
                textBox3.Text = listBox1.Items[listBox1.SelectedIndex].ToString();
            }
            this.Refresh();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                db.SelectAll(listBox1.Items[listBox1.SelectedIndex].ToString(), dataGridView1, listBox2);
                db.TableRecords(listBox1.Items[listBox1.SelectedIndex].ToString(), dataGridView1);
                textBox3.Text = listBox1.Items[listBox1.SelectedIndex].ToString();         
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            db.AllTables(listBox1);
            db.AllTables(comboBox1);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            db.DropField(listBox1.Items[listBox1.SelectedIndex].ToString(), listBox2.Items[listBox2.SelectedIndex].ToString());
            db.SelectAll(listBox1.Items[listBox1.SelectedIndex].ToString(), dataGridView1, listBox2);
            textBox3.Text = listBox1.Items[listBox1.SelectedIndex].ToString();
            ReFill();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MyDbForm myDbForm = new MyDbForm();
            myDbForm.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FKForm fk = new FKForm();
            fk.Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            db.DropAllFields(listBox2, listBox1.SelectedItem.ToString());
            ReFill();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            db.DropAllTables(listBox1);
            ReFill();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            ConstraintsForm ConstForm = new ConstraintsForm();
            ConstForm.Show();
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            ReFill();       
            //db = DBManager.GetInstance();
            if (listBox1.SelectedIndex != -1)
                db.TableRecords(listBox1.SelectedItem.ToString(), dataGridView1);
                
        }

        private void button4_Click(object sender, EventArgs e)
        {
            db.DropTable(textBox3.Text);
            ReFill();            
        }

        public void ReFill()
        {
            int SellectedIndex = listBox1.SelectedIndex;
            db.AllTables(listBox1);
            if (SellectedIndex < listBox1.Items.Count)
            {
                listBox1.SelectedIndex = SellectedIndex;
            }
            SellectedIndex = listBox2.SelectedIndex;
            db.SelectAll(textBox3.Text, dataGridView1, listBox2);
            if (SellectedIndex < listBox2.Items.Count)
            {
                listBox2.SelectedIndex = SellectedIndex;
            }
            db.AllTables(comboBox1);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            EditForm editForm = new EditForm();
            editForm.Show();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            ConnectionForm ConForm = new ConnectionForm();
            ConForm.Show();
            
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1 && comboBox2.SelectedIndex != -1 && comboBox3.SelectedIndex != -1)
            {
                string Command = $"Select * From {comboBox1.SelectedItem} where {comboBox2.SelectedItem} = '{comboBox3.SelectedItem}';";
                db.Select(Command, dataGridView1, comboBox1.SelectedItem.ToString());
            }
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            comboBox3.SelectedIndex = -1;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex != -1)
                db.AllColumns(comboBox1.SelectedItem.ToString(), comboBox2);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1 && comboBox2.SelectedIndex != -1)
                db.AllValues(comboBox1.SelectedItem.ToString(), comboBox2.SelectedItem.ToString(), comboBox3);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            db.SelectTheBestShift(dataGridView1, "Release", "Worker", "Product");

            //db.SelectProductsInDay("Release", "Product", "11.03.2023", dataGridView1);
            //db.SelectTheBestShift(dataGridView1, "Release", "Worker", "Product");
            //db.SelectSummary(dataGridView1, "Release", "Product", "11.12.2018");
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (textBox4.Text != "")
            {
                db.SelectProductsInDay("Release", "Product", textBox4.Text, dataGridView1);
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (textBox4.Text != "" && textBox6.Text != "")
            {
                db.SelectProductInMonth(dataGridView1, "Release", "Worker", "Product", textBox4.Text, textBox6.Text);
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (textBox4.Text != "" && textBox5.Text != "")
            {
                db.SelectSummary(dataGridView1, "Release", "Product", textBox4.Text, textBox5.Text);
            }
            
        }

        private void button17_Click(object sender, EventArgs e)
        {
            if (textBox7.Text != "" && textBox8.Text != "" && textBox9.Text != "")
            {
                db.TransactionFromTo(Convert.ToInt32(textBox7.Text), Convert.ToInt32(textBox8.Text), Convert.ToInt32(textBox9.Text));
                db.TableRecords("Release", dataGridView1);
            }
        }
    }
}
