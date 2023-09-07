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
    public partial class EditForm : Form
    {
        public EditForm()
        {
            InitializeComponent();
        }

        DBManager db = DBManager.GetInstance();

        private void EditForm_Load(object sender, EventArgs e)
        {
            db.AllTables(listBox1);
        }

        int top = 0;
        List<TextBox> textBoxes = new List<TextBox>();
        List<string> Fields;

        public void CreateTextBox(string Name)
        {
            Label label = new Label();
            label.Text = Name;
            label.AutoSize = true;
            label.Top = top;
            label.Left = 20;
            panel1.Controls.Add(label);

            TextBox textBox = new TextBox();
            textBox.Size = new Size(100, 22);
            textBox.Top = top + 20;
            textBox.Left = 20;
            top += 50;

            panel1.Controls.Add(textBox);
            textBoxes.Add(textBox);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                textBoxes.Clear();
              /*  foreach (Control control in panel1.Controls)
                {
                    control.Dispose();                    
                }*/
                top = 0;
                panel1.Controls.Clear();
                //db.SelectAll(listBox1.SelectedItem.ToString(), dataGridView1, null);
                Fields = db.GetFields(listBox1.SelectedItem.ToString());
                foreach (string field in Fields)
                {
                    CreateTextBox(field);
                }
                db.TableRecords(listBox1.SelectedItem.ToString(), dataGridView1);
                CurRow = -1;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FillFields();
            db.Insert(Fields, listBox1.SelectedItem.ToString());
            //db.SelectAll(listBox1.SelectedItem.ToString(), dataGridView1, null);
            db.TableRecords(listBox1.SelectedItem.ToString(), dataGridView1);
            Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1 && CurRow != -1)
            {
                db.DeleteRec(listBox1.SelectedItem.ToString(), dataGridView1[0, CurRow].Value.ToString());
                db.TableRecords(listBox1.SelectedItem.ToString(), dataGridView1);
                CurRow = -1;
                Clear();
            }
        }
        int CurRow = -1;

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CurRow = e.RowIndex;
            int i = 1;
            foreach (DataGridViewCell s in dataGridView1.Rows[e.RowIndex].Cells)
            {
                panel1.Controls[i].Text = s.Value.ToString();
                i += 2;
            }
        }

        public void Clear()
        {
            foreach (TextBox textBox in textBoxes)
            {
                textBox.Text = "";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                List<string> FieldNames = new List<string>();
                Fields = db.GetFields(listBox1.SelectedItem.ToString());
                foreach (string field in Fields)
                {
                    FieldNames.Add(field);
                }
                FillFields();
                db.Update(Fields, FieldNames, listBox1.SelectedItem.ToString());
                db.TableRecords(listBox1.SelectedItem.ToString(), dataGridView1);
                Clear();
            }
        }

        public void FillFields()
        {
            Fields.Clear();
            foreach (TextBox textBox in textBoxes)
            {
                Fields.Add(textBox.Text);
            }
        }
    }
}
