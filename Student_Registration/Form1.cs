using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using System.Collections.Specialized;
using System.Configuration;
using log4net;

namespace Student_Registration
{
    public partial class Form1 : Form
    {
        string connectString = ConfigurationManager.AppSettings["dbConnection"];
        OracleConnection oracleConnection;
        OracleCommand oracleCommand;
        int Id;
        bool flag = true;
       private readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString());

        public Form1()
        {
            InitializeComponent();
            LoadGrid();
        }
        private void Save_Click(object sender, EventArgs e)
        {
           // _log.Info("Entered Save Click");
            string Name = StdName.Text;
            string Course = StdCourse.Text;
            float Fees = float.Parse(StdFees.Text);
            try
            {
                oracleConnection = new OracleConnection(connectString);
                if (flag)
                {
                    oracleCommand = new OracleCommand(String.Format("INSERT INTO Student(NAME,FEES,COURSE) VALUES ('{0}',{1},'{2}')", Name.ToString(), Fees, Course.ToString()), oracleConnection)
                    {CommandType=CommandType.Text };

                    if (oracleConnection.State == ConnectionState.Closed)
                    {
                        oracleConnection.Open();
                    }
                    oracleCommand.ExecuteNonQuery();
                    
                    MessageBox.Show("Record Inserted");
                    StdCourse.Clear();
                    StdFees.Clear();
                    StdName.Clear();
                    StdName.Focus();
                    LoadGrid();
                }
                else
                {
                    string sql = "UPDATE STUDENT SET NAME = '" + StdName.Text + "' ,COURSE= '"
                        + StdCourse.Text + "' ,FEES= " + float.Parse(StdFees.Text) + " WHERE ID= "+Id;

                    oracleCommand = new OracleCommand(sql, oracleConnection)
                    { CommandType = CommandType.Text };
                    if (oracleConnection.State == ConnectionState.Closed)
                    {
                        oracleConnection.Open();
                    }
                    oracleCommand.ExecuteNonQuery();

                    MessageBox.Show("Record Updated");
                    StdCourse.Text = String.Empty;
                    StdFees.Text = String.Empty;
                    StdName.Text = String.Empty;
                    StdName.Focus();
                    LoadGrid();

                }
               
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (oracleConnection.State == ConnectionState.Open)
                {
                    oracleConnection.Close();
                }
            }

        }

        private void LoadGrid()
        {
            try
            {
                _log.Info("Entered Load Grid");
                string sql = "SELECT * FROM STUDENT";             

                oracleConnection = new OracleConnection(connectString);

                oracleCommand = new OracleCommand(sql, oracleConnection)
                { CommandType = CommandType.Text };

                if (oracleConnection.State == ConnectionState.Closed)
                {
                    oracleConnection.Open();
                }

                OracleDataReader DataReader = oracleCommand.ExecuteReader();
                dataGridView1.Rows.Clear();
                
                while(DataReader.Read())
                {
                    dataGridView1.Rows.Add(DataReader[0], DataReader[1], DataReader[3], DataReader[2]);
                }
                dataGridView1.Sort(dataGridView1.Columns["StdID"], ListSortDirection.Ascending);
                DataReader.Dispose();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Errror while adding to grid  " + ex.Message);
            }
            finally
            {
                if (oracleConnection.State == ConnectionState.Open)
                {
                    oracleConnection.Close();
                }
                
            }


        }

        private void GetId(int Id)
        {
            string sql = "SELECT * FROM STUDENT WHERE ID= " + Id;

            oracleConnection = new OracleConnection(connectString);

            oracleCommand = new OracleCommand(sql, oracleConnection)
            { CommandType = CommandType.Text };

            if (oracleConnection.State == ConnectionState.Closed)
            {
                oracleConnection.Open();
            }

            OracleDataReader oracleDataReader = oracleCommand.ExecuteReader();

            while(oracleDataReader.Read())
            {
                StdCourse.Clear();
                StdFees.Clear();
                StdName.Clear();
                StdName.Text = oracleDataReader[1].ToString();
                StdCourse.Text = oracleDataReader[3].ToString();
                StdFees.Text = oracleDataReader[2].ToString();

            }
            oracleDataReader.Dispose();
            oracleConnection.Close();
        }

        private void DeleteId(int Id)
        {
            string sql = "DELETE FROM STUDENT WHERE ID= " + Id;

            oracleConnection = new OracleConnection(connectString);

            oracleCommand = new OracleCommand(sql, oracleConnection)
            { CommandType = CommandType.Text };
            

            if (oracleConnection.State == ConnectionState.Closed)
            {
                oracleConnection.Open();
            }
            oracleCommand.ExecuteNonQuery();
            LoadGrid();
            oracleConnection.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex==dataGridView1.Columns["Edit"].Index && e.RowIndex>=0)
            {
                flag = false;
                this.Id = int.Parse(dataGridView1.CurrentRow.Cells[0].Value.ToString());
                GetId(Id);

            }

            if (e.ColumnIndex == dataGridView1.Columns["Delete"].Index && e.RowIndex >= 0)
            {
                this.Id = int.Parse(dataGridView1.CurrentRow.Cells[0].Value.ToString());
                DeleteId(Id);

            }



        }

        private void Clear_Click(object sender, EventArgs e)
        {
            StdCourse.Text = String.Empty;
            StdFees.Text = String.Empty;
            StdName.Text = String.Empty;
        }
    }
}
