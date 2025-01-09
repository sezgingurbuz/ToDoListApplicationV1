using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace ToDoListApplicationV1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SqlConnection connection = new SqlConnection("Data Source=DESKTOP-A0SS61H\\SQLEXPRESS01;Initial Catalog=todo_list;Integrated Security=True;");

        
        private void Form1_Load(object sender, EventArgs e)
        {
            ShowData();
        }
        void ShowData()
        {
            try
            {
                string q = "SELECT *FROM tbl_Todolist ORDER BY DAY ASC";
                SqlDataAdapter da = new SqlDataAdapter(q, connection);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if(dt.Rows.Count > 0 )
                {
                    dataGridView1.DataSource = dt;
                    dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dataGridView1.ClearSelection();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void AddTodoList()
        {
            
            DateTime selectedDate = monthCalendar1.SelectionStart.Date;
            connection.Open();
            string query = "INSERT INTO tbl_Todolist (Description, Title, DAY, Status) VALUES (@desc, @title, @day, @stat)";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@desc", rchDescription.Text);
            command.Parameters.AddWithValue("@title", txtTitle.Text);
            command.Parameters.AddWithValue("@stat", "Continues");
            command.Parameters.AddWithValue("@day", selectedDate);
            command.ExecuteNonQuery();
            connection.Close();
            ShowData();
            txtTitle.Text = "";
            rchDescription.Text = "";

            
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
                AddTodoList();
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            
            
        }

        private void btnToday_Click(object sender, EventArgs e)
        {
            DateTime todayy = DateTime.Today;
            connection.Open();
            SqlCommand command = new SqlCommand("SELECT * FROM tbl_Todolist WHERE CONVERT(date, DAY) = @day", connection);
            command.Parameters.AddWithValue("@day", todayy);

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            dataGridView1.DataSource = dt;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ClearSelection();

            connection.Close();
        }

        private void monthCalendar2_DateChanged(object sender, DateRangeEventArgs e)
        {
           DateTime selectedDayforChoose = monthCalendar2.SelectionStart.Date;
            connection.Open();
            SqlCommand command = new SqlCommand("SELECT * FROM tbl_Todolist WHERE CONVERT(date, DAY) = @day", connection);
            command.Parameters.AddWithValue("@day", selectedDayforChoose);

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            dataGridView1.DataSource = dt;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ClearSelection();

            connection.Close();
        }

        private void btnTomorrow_Click(object sender, EventArgs e)
        {
            connection.Open();
            SqlCommand command = new SqlCommand("SELECT * FROM tbl_Todolist WHERE DATEPART(WEEK, DAY) = DATEPART(WEEK, GETDATE())", connection);

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            dataGridView1.DataSource = dt;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.ClearSelection();

            connection.Close();
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            ShowData();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            int selectedCell = dataGridView1.SelectedCells[0].RowIndex;
            txtTitle.Text = dataGridView1.Rows[selectedCell].Cells[2].Value.ToString();
            rchDescription.Text = dataGridView1.Rows[selectedCell].Cells[1].Value.ToString();
            btnSave.Visible = true;
            btnAdd.Enabled = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DateTime selectedDate = monthCalendar1.SelectionStart.Date;
            string query = "UPDATE tbl_Todolist SET Description=@desc, Title=@title, DAY=@day WHERE ID=@p1";
            connection.Open();
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@desc",rchDescription.Text);
            command.Parameters.AddWithValue("@title", txtTitle.Text);
            command.Parameters.AddWithValue("@day", selectedDate);
            command.Parameters.AddWithValue("@p1",lblid.Text);
            command.ExecuteNonQuery();
            ShowData();
            btnAdd.Enabled = true;
            btnSave.Visible = false;
            connection.Close();

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                int selectedCell1 = dataGridView1.SelectedCells[0].RowIndex;
                lblid.Text = dataGridView1.Rows[selectedCell1].Cells[0].Value.ToString();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
           if(lblid.Text == "-" || lblid.Text == "")
            {
                MessageBox.Show("Please select an item to be deleted.");
            }
           else
            {
                DialogResult result = MessageBox.Show("The selected item will be deleted. Are you sure?","WARNING",MessageBoxButtons.YesNo,MessageBoxIcon.Warning);
                if(result == DialogResult.Yes)
                {
                    try
                    {
                        string query = "DELETE FROM tbl_Todolist WHERE ID=@p1";
                        connection.Open();
                        SqlCommand delete = new SqlCommand(query, connection);
                        delete.Parameters.AddWithValue("@p1", lblid.Text);
                        delete.ExecuteNonQuery();
                        
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Something went wrong." + ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                        ShowData();

                    }
                }
                
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (lblid.Text == "-" || lblid.Text == "")
            {
                MessageBox.Show("Please select an item");
            }
            else
            {
                try
                {
                    connection.Open();
                    SqlCommand change = new SqlCommand("UPDATE tbl_Todolist SET Status=@status WHERE ID=@id",connection);
                    change.Parameters.AddWithValue("@status", "COMPLETED");
                    change.Parameters.AddWithValue("@id", lblid.Text);
                    change.ExecuteNonQuery();
                    ShowData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Something went wrong." + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
