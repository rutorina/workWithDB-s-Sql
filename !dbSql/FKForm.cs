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
    public partial class FKForm : Form
    {
        public FKForm()
        {
            InitializeComponent();
        }

        DBManager db = DBManager.GetInstance();

        private void FKForm_Load(object sender, EventArgs e)
        {
            db.AllTables(comboBox1);
            db.AllTables(comboBox2);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                db.SelectAll(comboBox1.SelectedItem.ToString(), comboBox3);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex != -1)
            {
                db.SelectAll(comboBox2.SelectedItem.ToString(), comboBox4);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != comboBox2.SelectedItem)
            {
                db.TableConnection(comboBox1.SelectedItem.ToString(), comboBox2.SelectedItem.ToString(), comboBox3.SelectedItem.ToString(), comboBox4.SelectedItem.ToString());
            }
        }
    }
}
