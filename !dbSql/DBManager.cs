using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Reflection;
using System.IO;

namespace _dbSql
{
    public class DBManager
    {
        SqlConnection connection;//static
        SqlCommand cmd;
        public static string DbName;
        static string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*" + ".mdf");
        //static string loc = Path.GetFullPath(DbName);        {files[0]}
        public string conStr = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=E:\\!projVis\\!dbSql\\!dbSql\\Database2.mdf;Integrated Security=True"; //E:\!projVis\!dbSql\!dbSql\{DbName}
        public List<string> Tables = new List<string>();
        public static DBManager instance;
        //public string Type = "SQL";
        MySqlCommand MySQLcmd;
        MySqlConnection MySQLConnection;

        public DBManager()
        {//@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=[DataDirectory]\Street.accdb"
            connection = new SqlConnection(conStr);//@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=E:\!projVis\!dbSql\!dbSql\Database2.mdf;Integrated Security=True"
            cmd = new SqlCommand();
            cmd.Connection = connection;
        }

        public DBManager(string ConString)
        {/*
            cmd = null;
            connection = null;
            Type = "MySQL";*/
            MySQLConnection = new MySqlConnection(ConString);
            MySQLcmd = new MySqlCommand();
            MySQLcmd.Connection = MySQLConnection;
        }

        public static DBManager GetInstance()
        {
            if (instance == null)
                instance = new DBManager();
            return instance;
        }

        public void Connect(string ConStr)
        {
            connection.ConnectionString = ConStr;
        }

        public void Connect()
        {
            connection.ConnectionString = conStr;
        }

        void FillGrid(DataGridView dg, SqlDataReader r, ListBox listBox)
        {
            if (dg.Columns.Count != 0)
                dg.Columns.Clear();
            if(dg.Rows.Count !=0)
                dg.Rows.Clear();
            if(listBox != null)
                listBox.Items.Clear();
            while (r.Read())
            {               
                dg.Columns.Add(r.GetName(3), r.GetString(3) + $" ({r.GetValue(7)})");
                if (listBox != null)
                    listBox.Items.Add(r.GetString(3));//+ " (" + r.GetString(7) + ")"
            }
            //r.Close();
            //connection.Close();
        }

        public void SelectAll(string TableName, DataGridView dg, ListBox listbox)    
        {
            try
            {
                //var res = new List<List<Object>>();
                cmd.CommandText = $"select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{TableName}';";
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();

                FillGrid(dg, r, listbox);
                connection.Close();
                r.Close();
                //return res;
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.ToString());
                //throw ex;
            }
        }

        public void SelectAll(string TableName, ComboBox comboBox)
        {
            try
            {
                cmd.CommandText = $"select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{TableName} ';";
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                comboBox.Items.Clear();
                while (r.Read())
                {                   
                    comboBox.Items.Add(r.GetString(3));
                }
                connection.Close();
                r.Close();
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.ToString());
                throw ex;
            }
        }

        public bool CreateTable(string TableName, List<string> Fields)//string[] Fields
        {
            try
            {
                Tables.Add(TableName);
                cmd.CommandText = "create table " + TableName + "(";
                foreach (string s in Fields)
                {
                    cmd.CommandText += s + ", ";
                }
                cmd.CommandText = cmd.CommandText.Remove(cmd.CommandText.Length - 2) + ");";
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.ToString());
                throw ex;
            }   
        }

        public void AllTables(ListBox box)
        {
            box.Items.Clear();
            cmd.CommandText = "SELECT * FROM sys.Tables";
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                box.Items.Add(reader.GetString(0));
            }
            reader.Close();
            connection.Close();
        }

        public void AllTables(ComboBox box)
        {
            box.Items.Clear();
            cmd.CommandText = "SELECT * FROM sys.Tables";
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                box.Items.Add(reader.GetString(0));
            }
            reader.Close();
            connection.Close();
        }

        public void DropField(string TableName, string FieldName)
        {
            try
            {
                cmd.CommandText = $"Alter table {TableName} drop column {FieldName};";
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.ToString());
            }
        }

        public void DropTable(string TableName)
        {
            try
            {
                cmd.CommandText = $"DROP TABLE {TableName};";
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.ToString());
            }
        }

        public void DropAllFields(ListBox listbox, string TableName)
        {
            try
            {
                foreach (string s in listbox.Items)
                {
                    DropField(TableName, s);
                }
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.ToString());
                throw ex;
            }
        }

        public void DropAllTables(ListBox listbox)
        {
            try
            {
                foreach (string s in listbox.Items)
                {
                    DropTable(s);
                }
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.ToString());
                throw ex;
            }
        }

        public void AddField(string TableName, string Field)
        {
            cmd.CommandText = $"ALTER TABLE {TableName} add {Field};";
            connection.Open();
            cmd.ExecuteReader();
            connection.Close();
        }

        public void TableConnection(string ChildTable, string ParentTable, string ChildField, string ParentField)
        {
            try
            {
                cmd.CommandText = "alter table " + ChildTable + " add foreign key(" + ChildField + ") references " + ParentTable + " (" + ParentField + ");";
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
                MessageBox.Show("Linked");
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.ToString());
            }
        }

        public void ViewConstraintsTable(ComboBox box, string table_name)
        {
            try
            {
                box.Items.Clear();
                cmd.CommandText = $"SELECT CONSTRAINT_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE TABLE_NAME = '{table_name}";
                cmd.CommandText +=  "';";
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    box.Items.Add(reader.GetString(0));
                }
                reader.Close();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.ToString());
            }
        }

        public void AddConstraint(string Constraint, string TableName, string FieldName, string condition)
        {
            try
            {
                if (Constraint != "check" && Constraint != "NotNull")
                {
                    cmd.CommandText = $"Alter table {TableName} add {Constraint} ({FieldName})";
                }
                else if (Constraint == "check" && condition != "")
                {
                    cmd.CommandText = $"Alter table {TableName} add {Constraint} ({FieldName} {condition})";
                }
                else if (Constraint == "NotNull")
                {
                    cmd.CommandText = $"Alter table {TableName} alter column {FieldName} {GetType(TableName, FieldName)} NotNull;";
                }
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.ToString());
            }
        }

        public string GetType(string TableName, string FieldName)
        {
            try
            {
                cmd.CommandText = $"Select top 1 {FieldName} from {TableName}";
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly);
                DataTable schemaTable = reader.GetSchemaTable();
                DataRow row = schemaTable.Rows
                    .Cast<DataRow>()
                    .FirstOrDefault(r => r["ColumnName"].ToString() == FieldName);
                if (row != null)
                {
                    Type FieldType = (Type)row["DataType"];
                    return FieldType.FullName.ToString();
                }
                connection.Close();
                reader.Close();
                return null;
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.ToString());
                throw ex;
            }
        }

        public void ConstraintDrop(string TableName, string ConstraintName)
        {
            try
            {
                if (ConstraintName.Remove(2) == "PK" || ConstraintName.Remove(2) == "UQ" || ConstraintName.Remove(2) == "CK" || ConstraintName.Remove(2) == "FK")
                {
                    cmd.CommandText = $"ALTER TABLE {TableName} DROP CONSTRAINT {ConstraintName};";
                }
                else
                {
                    cmd.CommandText = $"ALTER TABLE {TableName} ALTER COLUMN {ConstraintName} {GetType(ConstraintName, TableName)};";
                }
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.ToString());
            }
        }

        public void Execute(string command)
        {
            try
            {
                cmd.CommandText = command;
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.ToString());
            }
        }

        public List<string> GetFields(string TableName)
        {
            try
            {
                List<string> Fields = new List<string>();
                cmd.CommandText = $"select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{TableName} ';";
                connection.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    Fields.Add(r.GetString(3));
                }
                connection.Close();
                r.Close();
                return Fields;
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.ToString());
                throw ex;
            }
        }

        public void Insert(List<string> Fields, string TableName)
        {
            try
            {
                List<string> FieldNames = GetFields(TableName);
                cmd.CommandText = $"insert into {TableName} (";//values
                foreach (string field in FieldNames)
                {
                    cmd.CommandText += field + ", ";
                }
                cmd.CommandText = cmd.CommandText.Remove(cmd.CommandText.Length - 2) + ") values (";
                foreach (string field in Fields)
                {
                    cmd.CommandText += "'" + field + "', ";
                }
                cmd.CommandText = cmd.CommandText.Remove(cmd.CommandText.Length - 2) + ");";
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                connection.Close();
                //throw;
            }
        }

        public void TableRecords(string tableName, DataGridView grid)
        {
            try
            {
                if (grid.Columns.Count != 0)
                    grid.Columns.Clear();
                if (grid.Rows.Count != 0)
                    grid.Rows.Clear();
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter($"SELECT * FROM {tableName}", connection);
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet, tableName);
                grid.DataSource = dataSet.Tables[tableName];
                connection.Close();
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.Message);
            }
        }

        public void DeleteRec(string TableName, string Code)
        {
            try
            {
                cmd.CommandText = $"delete from {TableName} where Code = '{Code}'";
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.ToString());
            }
        }

        public void Update(List<string> Fields, List<string> FieldNames, string TableName)
        {
            try
            {
                cmd.CommandText = $"UPDATE {TableName} SET ";
                for (int i = 1; i < FieldNames.Count; i++)
                {
                    cmd.CommandText += $"{FieldNames[i]} = '{Fields[i]}', ";
                }
                cmd.CommandText = cmd.CommandText.Remove(cmd.CommandText.Length - 2) + $"WHERE {FieldNames[0]} = '{Fields[0]}';";
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.ToString());
                throw;
            }
        }

        public void TableRecordsMySql(string tableName, DataGridView grid)
        {
            try
            {
                MySQLConnection.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter($"SELECT * FROM {tableName}", MySQLConnection);
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet, tableName);
                grid.DataSource = dataSet.Tables[tableName];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            MySQLConnection.Close();
        }
        public void ViewTableMySql(ListBox box, string TableName)
        {
            box.Items.Clear();
            MySQLcmd.CommandText = $"SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{TableName}';";
            MySQLConnection.Open();
            MySqlDataReader reader = MySQLcmd.ExecuteReader();
            while (reader.Read())
            {
                // box.Items.Add($"name=' {reader.GetString(3)}';type='{reader.GetString(7)}''{reader.GetString(7)}'");
                box.Items.Add(reader.GetString(3));
            }
            reader.Close();
            MySQLConnection.Close();
        }

        public void AllTableMySQL(ListBox listBox)
        {
            listBox.Items.Clear();
            MySQLcmd.CommandText = "SELECT * INFORMATION_SCHEMA.TABLES";
            MySQLConnection.Open();
            MySqlDataReader reader = MySQLcmd.ExecuteReader();
            while (reader.Read())
            {
                listBox.Items.Add(reader.GetString(0));
            }
            reader.Close();
            MySQLConnection.Close();
        }

        public string GetNameMySQL(int index)
        {
            try
            {
                MySQLcmd.CommandText = "Select * From INFORMATION_SCHEMA.COLUMNS where COLUMN_NAME = TABLE_NAME";
                MySQLConnection.Open();
                MySqlDataReader reader = MySQLcmd.ExecuteReader();
                string Name = reader.GetString(index);
                MySQLConnection.Close();
                return Name;
            }
            catch (Exception ex)
            {
                MySQLConnection.Close();
                MessageBox.Show(ex.ToString());
                return "";
            }
        }

        public void ExecuteMySQL(string command)
        {
            try
            {
                MySQLcmd.CommandText = command;
                MySQLConnection.Open();
                MySQLcmd.ExecuteNonQuery();
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.ToString());
            }
        }

        public void CreateTableMySQL(string TableName, List<string> Fields)
        {
            try
            {
                Tables.Add(TableName);
                MySQLcmd.CommandText = "create table " + TableName + "(";
                foreach (string s in Fields)
                {
                    MySQLcmd.CommandText += s + ", ";
                }
                MySQLcmd.CommandText = cmd.CommandText.Remove(cmd.CommandText.Length - 2) + ");";
                MySQLConnection.Open();
                MySQLcmd.ExecuteNonQuery();
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                MySQLConnection.Close();
                MessageBox.Show(ex.ToString());
                throw ex;
            }
        }

        public List<string> GetFieldsMySQL(string TableName)
        {
            try
            {
                List<string> Fields = new List<string>();
                MySQLcmd.CommandText = $"select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{TableName}';";
                MySQLConnection.Open();
                MySqlDataReader r = MySQLcmd.ExecuteReader();
                while (r.Read())
                {
                    Fields.Add(r.GetString(3));
                }
                MySQLConnection.Close();
                r.Close();
                return Fields;
            }
            catch (Exception ex)
            {
                MySQLConnection.Close();
                MessageBox.Show(ex.ToString());
                throw ex;
            }
        }

        public void DropTableMySQL(string TableName)
        {
            try
            {
                MySQLcmd.CommandText = $"DROP TABLE IF EXISTS {TableName};";
                MySQLConnection.Open();
                MySQLcmd.ExecuteNonQuery();
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                MySQLConnection.Close();
                MessageBox.Show(ex.ToString());
            }
        }

        public void InsertMySQL(List<string> Fields, string TableName)
        {
            try
            {
                List<string> FieldNames = GetFieldsMySQL(TableName);
                MySQLcmd.CommandText = $"insert into {TableName} (";//values
                foreach (string field in FieldNames)
                {
                    MySQLcmd.CommandText += field + ", ";
                }
                MySQLcmd.CommandText = MySQLcmd.CommandText.Remove(MySQLcmd.CommandText.Length - 2) + ") values (";
                foreach (string field in Fields)
                {
                    MySQLcmd.CommandText += "'" + field + "', ";
                }
                MySQLcmd.CommandText = MySQLcmd.CommandText.Remove(MySQLcmd.CommandText.Length - 2) + ");";
                MySQLConnection.Open();
                MySQLcmd.ExecuteNonQuery();
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                MySQLConnection.Close();
                //throw;
            }
        }

        public void UpdateMySQL(List<string> Fields, List<string> FieldNames, string TableName)
        {
            try
            {
                MySQLcmd.CommandText = $"UPDATE {TableName} SET ";
                for (int i = 1; i < FieldNames.Count; i++)
                {
                    MySQLcmd.CommandText += $"{FieldNames[i]} = '{Fields[i]}', ";
                }
                MySQLcmd.CommandText = MySQLcmd.CommandText.Remove(MySQLcmd.CommandText.Length - 2) + $"WHERE {FieldNames[0]} = '{Fields[0]}';";
                MySQLConnection.Open();
                MySQLcmd.ExecuteNonQuery();
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                MySQLConnection.Close();
                MessageBox.Show(ex.ToString());
                throw;
            }
        }

        public void DeleteRecMySQL(string TableName, string Code)
        {
            try
            {
                MySQLcmd.CommandText = $"delete from {TableName} where Code = '{Code}'";
                MySQLConnection.Open();
                MySQLcmd.ExecuteNonQuery();
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                MySQLConnection.Close();
                MessageBox.Show(ex.ToString());
            }
        }

        public List<string> GetFieldNamesMySQL(string TableName)
        {
            try
            {
                List<string> Fields = new List<string>();
                MySQLcmd.CommandText = $"select * from INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{TableName} ';";
                MySQLConnection.Open();
                MySqlDataReader r = MySQLcmd.ExecuteReader();
                while (r.Read())
                {
                    Fields.Add(r.GetString(3));
                }
                MySQLConnection.Close();
                r.Close();
                return Fields;
            }
            catch (Exception ex)
            {
                MySQLConnection.Close();
                MessageBox.Show(ex.ToString());
                throw ex;
            }
        }

        public void AllColumns(string TableName, ComboBox box)
        {
            box.Items.Clear();
            cmd.CommandText = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{TableName}'";
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                box.Items.Add(reader.GetString(0));
            }
            reader.Close();
            connection.Close();
        }

        public void AllValues(string TableName, string ColumnName, ComboBox box)
        {
            List<object> Values = new List<object>();
            box.Items.Clear();
            cmd.CommandText = $"SELECT {ColumnName} FROM {TableName}";
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (!Values.Contains(reader.GetValue(0)))
                {
                    Values.Add(reader.GetValue(0));
                }
            }
            foreach (object value in Values)
            {
                box.Items.Add(value.ToString());
            }            
            reader.Close();
            connection.Close();
        }

        public void Select(string Command, DataGridView dataGrid, string TableName)
        {
            try
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(Command, connection);
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet, TableName);
                dataGrid.DataSource = dataSet.Tables[TableName];
                connection.Close();
                
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.Message);
            }
        }

        public int GetTotalQuantity(string TableName, string Date)
        {
            try
            {
                connection.Open();
                cmd.CommandText = $"select Quantity From {TableName} where Release_Date = '{Date}'";
                int Quantity = 0;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Quantity += Convert.ToInt32(reader.GetValue(0));
                }
                connection.Close();
                return Quantity;
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.ToString());
                return -1;
            }
        }

        //DONE
        public void SelectTheBestShift(DataGridView dataGridView, string TableName/*Release*/, string TableName1 /*Worker*/, string TableName2 /*Product*/)
        {
            try
            {
                int Quantity = int.MinValue;
                string CurShift = "";
                List<object> Values = new List<object>();
                cmd.CommandText = $"SELECT Release_date FROM {TableName}";
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (!Values.Contains(reader.GetValue(0)))
                    {
                        Values.Add(reader.GetValue(0));                        
                    }
                }
                reader.Close();
                connection.Close();

                foreach (string shift in Values)
                {
                    if (Quantity < GetTotalQuantity(TableName, shift))
                    {
                        Quantity = GetTotalQuantity(TableName, shift);
                        CurShift = shift;
                    }
                }
                connection.Open();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter($"select {TableName}.Code, {TableName1}.Name as 'WorkerName', {TableName2}.Name as 'ProductName', {TableName}.Quantity, {TableName}.Release_date, {TableName}.Order_ From {TableName} join {TableName1} on {TableName}.Worker = {TableName1}.Code join {TableName2} on {TableName}.Product = {TableName2}.Code Where {TableName}.Release_date = '{CurShift}'", connection);//'{Shift}'
                DataSet dataSet = new DataSet();
                sqlDataAdapter.Fill(dataSet, TableName);
                dataGridView.DataSource = dataSet.Tables[TableName];
                connection.Close();
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.ToString());
            }
        }
        //DONE
        public void SelectProductsInDay(string TableName, string TableName1 /*Product*/, string Date, DataGridView dataGridView)
        {
            try
            {
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter($"select {TableName}.Worker, {TableName}.Product, {TableName}.Quantity, {TableName}.Release_date, {TableName}.Order_, {TableName1}.Name as 'ProductName', {TableName1}.Type as 'ProductType', {TableName1}.Description as 'ProductDescription'  From {TableName} join {TableName1} on {TableName}.Product = {TableName1}.Code Where {TableName}.Release_date = '{Date}'", connection);//'{Shift}'
                DataSet dataSet = new DataSet();
                sqlDataAdapter.Fill(dataSet, TableName);
                dataGridView.DataSource = dataSet.Tables[TableName];
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.ToString());
            }
        }
        //DONE
        public void SelectProductInMonth(DataGridView dataGridView, string TableName/*Release*/, string TableName1 /*Worker*/, string TableName2 ,/*Product*/ string Shift, string Product)
        {
            try
            {
                connection.Open();
                List<string[]> DateList = new List<string[]>();
                cmd.CommandText = $"Select {TableName}.Release_date FROM {TableName}";
                SqlDataReader reader =  cmd.ExecuteReader();                
                while (reader.Read())
                {
                    DateList.Add(reader.GetString(0).Split('.'));
                }
                reader.Close();
                List<string[]> ActualDates = new List<string[]>();
                foreach (var item in DateList)
                {
                    ActualDates.Add(item);
                }
                foreach (var str in DateList)
                {
                    if (str[1] != Shift)
                    {
                        ActualDates.Remove(str);
                    }
                }
                string query = $"select {TableName}.Code, {TableName1}.Name as 'WorkerName', {TableName2}.Name as 'ProductName', {TableName}.Quantity, {TableName}.Release_Date, {TableName}.Order_ From {TableName} inner join {TableName1} on {TableName}.Worker = {TableName1}.Code inner join {TableName2} on {TableName}.Product = {TableName2}.Code  Where (";// join {TableName1} on {TableName}.Worker = {TableName1}.Name join {TableName2} on {TableName}.Product = {TableName2}.Name 
                foreach (var str in ActualDates)
                {
                    query += $"{TableName}.Release_date = '{str[0]}.{str[1]}.{str[2]}' or ";
                }
                query = query.Remove(query.Length - 4) + $") and {TableName}.Product = '{Product}'";
               
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, connection);
                DataSet dataSet = new DataSet();
                sqlDataAdapter.Fill(dataSet, TableName);
                dataGridView.DataSource = dataSet.Tables[TableName];
                connection.Close();
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.ToString());
            }
        }
        //DONE
        public void SelectSummary(DataGridView dataGridView, string TableName/*Release*/, string TableName1 /*Product*/, string Date1, string Date2)
        {
            try
            {
                connection.Open();

                List<string[]> DateList = new List<string[]>();
                cmd.CommandText = $"Select {TableName}.Release_date FROM {TableName}";
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    DateList.Add(reader.GetString(0).Split('.'));
                }
                reader.Close();
                List<string[]> ActualDates = new List<string[]>();
                string[] StartDate = Date1.Split('.');
                string[] EndDate = Date2.Split('.');

                foreach (var item in DateList)
                {
                    if (Convert.ToInt32(item[2]) == Convert.ToInt32(StartDate[2]))
                    {
                        if (Convert.ToInt32(item[1]) == Convert.ToInt32(StartDate[1]))
                        {
                            if (Convert.ToInt32(item[0]) >= Convert.ToInt32(StartDate[0]))
                            {
                                ActualDates.Add(item);
                            }
                        }
                        else if (Convert.ToInt32(item[1]) > Convert.ToInt32(StartDate[1]))
                        {
                            ActualDates.Add(item);
                        }
                    }
                    else if (Convert.ToInt32(item[2]) > Convert.ToInt32(StartDate[2]))
                    {
                        ActualDates.Add(item);
                    }
                }
                DateList.Clear();
                foreach (var item in ActualDates)
                {
                    if (Convert.ToInt32(item[2]) == Convert.ToInt32(EndDate[2]))
                    {
                        if (Convert.ToInt32(item[1]) == Convert.ToInt32(EndDate[1]))
                        {
                            if (Convert.ToInt32(item[0]) <= Convert.ToInt32(EndDate[0]))
                            {
                                DateList.Add(item);
                            }
                        }
                        else if (Convert.ToInt32(item[1]) < Convert.ToInt32(EndDate[1]))
                        {
                            DateList.Add(item);
                        }
                    }
                    else if (Convert.ToInt32(item[2]) < Convert.ToInt32(EndDate[2]))
                    {
                        DateList.Add(item);
                    }
                }

                string query = $"select {TableName}.Code, {TableName1}.Name as 'ProductName', {TableName}.Quantity, {TableName}.Release_Date, {TableName}.Order_ From {TableName} join {TableName1} on {TableName}.Worker = {TableName1}.Code Where (";//{TableName}.Release_Date = '{Date1}' and '{Date2}'"
                foreach (var str in DateList)
                {
                    query += $"{TableName}.Release_date = '{str[0]}.{str[1]}.{str[2]}' or ";
                }
                query = query.Remove(query.Length - 4) + $");";

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, connection);
                DataSet dataSet = new DataSet();
                sqlDataAdapter.Fill(dataSet, TableName);
                dataGridView.DataSource = dataSet.Tables[TableName];
                connection.Close();
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.ToString());
            }
        }

        public void TransactionFromTo(int Release1, int Release2, int quantity)
        {
            SqlTransaction tr = null;
            try
            {
                connection.Open();

                int Product1 = GetProduct("Release", Release1), Product2 = GetProduct("Release", Release2);
                if (Product1 == Product2)
                {
                    tr = connection.BeginTransaction();
                    cmd.Transaction = tr;

                    cmd.CommandText = "update Release set Quantity = Quantity - @quantity where Code = @Release1;";
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter("@Release1", Release1));
                    cmd.Parameters.Add(new SqlParameter("@quantity", quantity));
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "update Release set Quantity = Quantity + @quantity where Code = @Release2;";
                    cmd.Parameters.Add(new SqlParameter("@Release2", Release2));
                    cmd.ExecuteNonQuery();
                    //db.Execute("create table Transaction_(Code tinyint not null primary key, From_ tinyint, To_ tinyint, Quantity int, Date_ datetime);");
                    /*
                    cmd.CommandText = $"insert into Transaction_(Code, From_, To_, Quantity, Date_) values('{GetNextId("Transaction_", "Code")}' , '{Convert.ToInt32(Release1)}','{Convert.ToInt32(Release2)}', '{Convert.ToInt32(quantity)}', '" + DateTime.Now.ToString() + "');";
                    cmd.ExecuteNonQuery();*/

                    

                    cmd.Parameters.AddWithValue("@Code", GetNextId("Transaction_", "Code"));
                    cmd.Parameters.AddWithValue("@From", Convert.ToInt32(Release1));
                    cmd.Parameters.AddWithValue("@To", Convert.ToInt32(Release2));
                    cmd.Parameters.AddWithValue("@Quant", Convert.ToInt32(quantity));
                    cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                    cmd.CommandText = "INSERT INTO Transaction_(Code, From_, To_, Quantity, Date_) VALUES (@Code, @From, @To, @Quant, @Date)";
                    cmd.ExecuteNonQuery();
                    tr.Commit();
                    connection.Close();
                }
                else
                {
                    connection.Close();
                }
            }
            catch(Exception ex)
            {
                tr.Rollback();
                connection.Close();
                MessageBox.Show(ex.ToString());
            }
        }

        public int GetProduct(string TableName, int Code)
        {
            try
            {
                int Product = -1;
                cmd.CommandText = $"Select {TableName}.Product from {TableName} where {TableName}.Code = {Code};";
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Product = Convert.ToInt32(reader.GetValue(0));
                }
                reader.Close();
                return Product;
            }
            catch (Exception ex)
            { 
                MessageBox.Show(ex.ToString());
                return -1;
            }
        }

        public int GetNextId(string TableName, string IdName)
        {
            try
            {
                //connection.Open();
                cmd.CommandText = $"select max({IdName}) from {TableName};";
                
                Object res = cmd.ExecuteScalar();
                if (res == DBNull.Value)
                    res = 0;
                //connection.Close();
                return Convert.ToInt32(res) + 1;
            }
            catch 
            {
                //connection.Close();
                return -1;
            }
        }
    }
}

