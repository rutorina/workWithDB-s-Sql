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
    public partial class ConstraintsForm : Form
    {
        public ConstraintsForm()
        {
            InitializeComponent();
        }
        DBManager db = DBManager.GetInstance();
        private void ConstraintsForm_Load(object sender, EventArgs e)
        {
            db.AllTables(comboBox1);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox4.SelectedIndex = -1;
            comboBox3.SelectedIndex = -1;
            db.SelectAll(comboBox1.SelectedItem.ToString(), comboBox3);
            db.ViewConstraintsTable(comboBox4, comboBox1.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            db.AddConstraint(comboBox2.Text, comboBox1.Text, comboBox3.Text, textBox1.Text);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            label4.Visible = false;
            textBox1.Visible = false;
            if (comboBox2.SelectedItem.ToString() == "check")
            {
                label4.Visible = true;
                textBox1.Visible = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            db.ConstraintDrop(comboBox1.Text, comboBox4.Text);
            comboBox4.SelectedIndex = -1;
        }
    }
}
