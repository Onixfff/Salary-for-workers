using MySql.Data.MySqlClient;
using MySql.Data.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Salary_for_workers
{
    public partial class MainForm : Form
    {
        private SelectedWorkers selectedWorker;
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
            dataGridViewPeople.DataSource = ds.Tables[0];
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

            string query = $"SELECT people.Id, people.Name as Имя, people.Surname as Фамилия, people.Patronymic as Отчество FROM authorization.timework left join people on idPeople = people.id where (date between @firstDay and @lastDay) group by people.id;";

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
            dataGridViewPeople.DataSource = ds.Tables[0];
        }

        private async void dataGridViewPeople_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewPeople.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridViewPeople.SelectedRows)
                {
                    if (!row.IsNewRow)
                    {
                        bool isCompliteParse = int.TryParse(row.Cells["id"].Value.ToString(), out int id);

                        if (!isCompliteParse)
                        {
                            MessageBox.Show("Ошибка в захвате id с datagrid");
                            return;
                        }

                        string name = row.Cells["Имя"].Value.ToString();
                        string surname = row.Cells["Фамилия"].Value.ToString();
                        string patronymic = row.Cells["Отчество"].Value.ToString();

                        selectedWorker = new SelectedWorkers(id, name, surname, patronymic);

                        DataSet ds = await GetDayNight(selectedWorker, dateTimePicker1);

                        dataGridViewDate.DataSource = ds.Tables[0];

                        int day = ChengeTotal(1);
                        int night = ChengeTotal(2);

                        labelTotalDayNight.Text = $"Кол-во д - {day} н - {night}";
                    }
                }
            }
        }

        private async Task<DataSet> GetDayNight(SelectedWorkers selectedWorker, DateTimePicker dateTime)
        {
            DateTime firstDay = GetDateTimeFirstDay(dateTime.Value);
            DateTime lastDay = GetDateTimeLastDay(dateTime.Value);

            MySqlDateTime mySqlDateTimeFirst = new MySqlDateTime(firstDay);
            var dateMysqlFirst = mySqlDateTimeFirst.GetDateTime().Date.ToString("yyyy-MM-dd");


            MySqlDateTime mySqlDateTimeLast = new MySqlDateTime(lastDay);
            var dateMysqlLast = mySqlDateTimeLast.GetDateTime().Date.ToString("yyyy-MM-dd");

            string query = $"SELECT timework.Date as Дата, timework.Day as День, timework.Night as Ночь FROM authorization.timework left join people on idPeople = people.id where people.Id = @Id and people.Name = @Name and people.Surname = @Surname and people.Patronymic = @Patronymic and (date between @firstDay and @lastDay)";

            DataSet ds = new DataSet();

            try
            {
                ds.Tables.Clear();
                await _mCon.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, _mCon))
                {
                    command.Parameters.AddWithValue($"@firstDay", dateMysqlFirst);
                    command.Parameters.AddWithValue($"@lastDay", dateMysqlLast);
                    command.Parameters.AddWithValue($"@Name", selectedWorker.Name);
                    command.Parameters.AddWithValue($"@Surname", selectedWorker.Surname);
                    command.Parameters.AddWithValue($"@Patronymic", selectedWorker.Patronymic);
                    command.Parameters.AddWithValue($"@Id", selectedWorker.Id);

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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startId"> 1 == Day or 2 == Night</param>
        private int ChengeTotal(int startId)
        {
            int total = 0;
            int number;

            bool convert;

            for (int i = 0; i < dataGridViewDate.Rows.Count - 1; i++)
            {
                var t = dataGridViewDate.Rows[i].Cells[startId];
                convert = int.TryParse(t.FormattedValue.ToString(), out number);
                
                if (convert)
                {
                    total += number;
                }
            }

            return total;
        }

    }
}
