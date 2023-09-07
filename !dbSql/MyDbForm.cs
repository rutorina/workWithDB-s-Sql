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
    public partial class MyDbForm : Form
    {
        public MyDbForm()
        {
            InitializeComponent();
        }
        DBManager db = DBManager.GetInstance();
        private void button1_Click(object sender, EventArgs e)
        {
            db.Execute("create table Release(Code tinyint not null primary key, Worker tinyint not null, Product tinyint not null, Quantity int check(Quantity > 0), Release_date varchar(10), Order_ tinyint not null);");// Release_date Date
            //db.Execute("create table Release(Code tinyint not null primary key, Worker tinyint not null, Product tinyint not null, Quantity int check(Quantity > 0), Release_date date, Order_ tinyint not null);");// Release_date Date

        }

        private void button2_Click(object sender, EventArgs e)
        {
            db.Execute("create table Worker(Code tinyint not null primary key, Name Varchar(20), Last_Name Varchar(20), Description Text);");
            //db.Execute("create table Transaction_(Code tinyint not null primary key, From_ tinyint, To_ tinyint, Quantity int, Date_ datetime);");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            db.Execute("create table Order_(Code tinyint not null primary key, Company tinyint not null, Order_Date date, Description Text);");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            db.Execute("create table Company(Code tinyint not null primary key, Name varchar(20), Description text);");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            db.Execute("create table Product(Code tinyint not null primary key, Name varchar(20), Type varchar(15), Description text);");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            db.Execute("create table Product_Accessories(Code tinyint not null, Accessories tinyint not null, Value int, primary key(Code, Accessories));");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            db.Execute("create table Accessories(Code tinyint not null primary key, Name varchar(20), Unit varchar(10), Description Text, Quantity_rest int);");
        }
    }
}
