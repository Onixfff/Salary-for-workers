using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Salary_for_workers
{
    public partial class States : Form
    {
        bool isDay;
        MySqlConnection mCon = new MySqlConnection(ConfigurationManager.ConnectionStrings["local"].ConnectionString);

        public States(bool isDay)
        {
            InitializeComponent();
            this.isDay = isDay;
        }

        private async void States_Load(object sender, EventArgs e)
        {
            DataSet dataSet = await GetDateAsync();
            dataGridView1.DataSource = dataSet.Tables[0];
        }

        private async Task<DataSet> GetDateAsync()
        {
            string query = "SELECT id, abbreviation as абривиатура , Decoding_abbreviation as описание FROM authorization.states;";

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
            // от 0 до 2 max;
            //columnIndex 0/2
            //rowIndex = (строка которую выбрал пользователь и по которой надо идти и забирать данные;

            DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];

            int id = Convert.ToInt32(selectedRow.Cells["id"].Value);
            string atribyt = selectedRow.Cells["абривиатура"].Value.ToString();

            if (isDay)
            {
                EditorDay.IdDay = id;
                EditorDay.TextDay = atribyt;
            }
            else
            {
                EditorDay.IdNight = id;
                EditorDay.TextNight = atribyt;
            }

            this.Close();
        }
    }
}
