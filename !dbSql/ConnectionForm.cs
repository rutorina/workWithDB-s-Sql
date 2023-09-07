using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace _dbSql
{
    public partial class ConnectionForm : Form
    {
        public ConnectionForm()
        {
            InitializeComponent();
            //string ConStr = (@"Server=" + textBox1.Text + ";Database=" + textBox2.Text + ";Uid=" + textBox3.Text + ";Pwd=" + textBox4.Text);
            //db = new DBManager(ConStr);
        }

        public DBManager db;
        string ConStr;

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                CurTable = "";
                ConStr = @"Server=" + textBox1.Text + ";Database=" + textBox2.Text + ";Uid=" + textBox3.Text + ";Pwd=" + textBox4.Text;
                db = new DBManager(ConStr);
                // db.AllTables(listBox1);
                db.TableRecordsMySql("INFORMATION_SCHEMA.TABLES", dataGridView1);
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }
        }

        string CurTable;

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CurRow = e.RowIndex;
            if (CurTable == "")
            {
                CurTable = dataGridView1[2, e.RowIndex].Value.ToString();
                db.TableRecordsMySql(CurTable, dataGridView1);
                CreateSomeFields();
                CurRow = -1;
            }
            else
            {
                CreateSomeFields();
            }
            if (CurTable != "" && CurRow != -1)
            {                
                if (CurRow != -1)
                {
                    int i = 1;
                    foreach (DataGridViewCell s in dataGridView1.Rows[CurRow].Cells)
                    {
                        panel1.Controls[i].Text = s.Value.ToString();
                        i += 2;
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (db != null)
                {
                    CurTable = "";
                    db.TableRecordsMySql("INFORMATION_SCHEMA.TABLES", dataGridView1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "JSON (*.json)|";
            ConStr = @"Server=" + textBox1.Text + ";Database=" + textBox2.Text + ";Uid=" + textBox3.Text + ";Pwd=" + textBox4.Text;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter writer = File.CreateText($"{saveFileDialog1.FileName}.json"))
                {
                    string output = JsonConvert.SerializeObject(ConStr);
                    writer.Write(output);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                CurTable = "";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    using (StreamReader reader = File.OpenText(openFileDialog1.FileName))
                    {
                        var fileText = reader.ReadToEnd();
                        ConStr = JsonConvert.DeserializeObject<string>(fileText);
                    }
                }
                db = new DBManager(ConStr);
                db.TableRecordsMySql("INFORMATION_SCHEMA.TABLES", dataGridView1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            db.ExecuteMySQL("create table Kosianchuk(Code tinyint not null primary key, Worker tinyint not null, Quantity int check(Quantity > 0), Release_date varchar(8), Order_ tinyint not null);");
            //db.CreateTableMySQL()
            //db.DropTableMySQL("Kosianchuk");
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

        int CurRow = -1;

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (CurRow != -1)
            {
                List<string> FieldNames = new List<string>();
                Fields = db.GetFieldsMySQL(CurTable);
                foreach (string field in Fields)
                {
                    FieldNames.Add(field);
                }
                FillFields();
                db.UpdateMySQL(Fields, FieldNames, CurTable);
                db.TableRecordsMySql(CurTable, dataGridView1);
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

        public void Clear()
        {
            foreach (TextBox textBox in textBoxes)
            {
                textBox.Text = "";
            }
        }

        public void CreateSomeFields()
        {
            textBoxes.Clear();
            top = 0;
            panel1.Controls.Clear();
            Fields = db.GetFieldsMySQL(CurTable);
            foreach (string field in Fields)
            {
                CreateTextBox(field);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            FillFields();
            db.InsertMySQL(Fields, CurTable);
            db.TableRecordsMySql(CurTable, dataGridView1);
            Clear();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (CurTable != "" && CurRow != -1)
            {
                db.DeleteRecMySQL(CurTable, dataGridView1[0, CurRow].Value.ToString());
                db.TableRecordsMySql(CurTable, dataGridView1);
                CurRow = -1;
                Clear();
            }
        }


    }
}
