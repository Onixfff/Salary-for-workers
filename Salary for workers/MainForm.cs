using MySql.Data.MySqlClient;
using MySql.Data.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Salary_for_workers
{
    public partial class MainForm : Form
    {
        private MySqlConnection _mCon;
        private readonly List<Worker> _workers;

        public MainForm(List<Worker> workers, MySqlConnection mCon)
        {
            _mCon = mCon;
            _workers = workers;
            InitializeComponent();
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            dateTimePicker1.CustomFormat = "MM-yyyy";
            DataSet ds = new DataSet();
            ds = await GetDataWorkers(dateTimePicker1);
            dataGridView1.DataSource = ds.Tables[0];
        }

        private DateTime GetDateTimeFirstDay(DateTime date)
        {
            int year = date.Year;
            int month = date.Month;
            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            return firstDayOfMonth;
        }

        private DateTime GetDateTimeLastDay(DateTime date)
        {
            int year = date.Year;
            int month = date.Month;
            DateTime lastDayOfMonth = new DateTime(year, month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
            return lastDayOfMonth;
        }

        private async Task<DataSet> GetDataWorkers(DateTimePicker dateTime)
        {
            DateTime firstDay = GetDateTimeFirstDay(dateTime.Value);
            DateTime lastDay = GetDateTimeLastDay(dateTime.Value);

            MySqlDateTime mySqlDateTimeFirst = new MySqlDateTime(firstDay);
            var dateMysqlFirst = mySqlDateTimeFirst.GetDateTime().Date.ToString("yyyy-MM-dd");


            MySqlDateTime mySqlDateTimeLast = new MySqlDateTime(lastDay);
            var dateMysqlLast = mySqlDateTimeLast.GetDateTime().Date.ToString("yyyy-MM-dd");

            string query = $"SELECT people.Name, people.Surname, people.Patronymic, timework.Date, timework.Day, timework.Night FROM authorization.timework left join people on idPeople = people.id where (date between @firstDay and @lastDay);";

            DataSet ds = new DataSet();

            try
            {
                ds.Tables.Clear();
                await _mCon.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, _mCon))
                {
                    command.Parameters.AddWithValue($"@firstDay", dateMysqlFirst);
                    command.Parameters.AddWithValue($"@lastDay", dateMysqlLast);

                    using (MySqlDataAdapter dsAdapter = new MySqlDataAdapter(command))
                    {
                        dsAdapter.Fill(ds);
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { _mCon.Close(); }

            return ds;
        }

        private void Button_Click(object sender, EventArgs e)
        {
            int year = dateTimePicker1.Value.Year;
            int month = dateTimePicker1.Value.Month;
            Button text = (Button)sender;
            int day = Convert.ToInt32(text.Text);
            int lastDayOfMonth = DateTime.DaysInMonth(year, month);

            if(day <= lastDayOfMonth)
            {
                Form2 form2 = new Form2(_workers, new DateTime(year, month, day), _mCon);
                this.Visible = false;
                form2.ShowDialog();
                this.Visible = true;
            }
        }

        private async void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            ds = await GetDataWorkers(dateTimePicker1);
            dataGridView1.DataSource = ds.Tables[0];
        }
    }
}
