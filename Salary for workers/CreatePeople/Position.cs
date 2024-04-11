using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Salary_for_workers
{
    public partial class Position : Form
    {
        MySqlConnection mCon = new MySqlConnection(ConfigurationManager.ConnectionStrings["local"].ConnectionString);

        public Position()
        {
            InitializeComponent();
        }

        private async void Position_Load(object sender, EventArgs e)
        {
            DataSet dataSet = await GetDateAsync();
            dataGridView1.DataSource = dataSet.Tables[0];
        }

        private async Task<DataSet> GetDateAsync()
        {
            string query = "SELECT id, Name as Название FROM authorization.positions;";

            DataSet ds = new DataSet();

            try
            {
                ds.Tables.Clear();
                await mCon.OpenAsync();
                await Task.Run(() =>
                {
                    using (MySqlCommand command = new MySqlCommand(query, mCon))
                    {

                        using (MySqlDataAdapter dsAdapter = new MySqlDataAdapter(query, mCon))
                        {
                            dsAdapter.Fill(ds);
                        }
                    }
                });
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { mCon.Close(); }

            return ds;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];

            int id = Convert.ToInt32(selectedRow.Cells["id"].Value);
            string name = selectedRow.Cells["Название"].Value.ToString();

            CreateUser.IdPosition = id;
            CreateUser.TextPosition = name;

            this.Close();
        }
    }
}
