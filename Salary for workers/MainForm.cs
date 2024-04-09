using MySql.Data.MySqlClient;
using MySql.Data.Types;
using Mysqlx.Crud;
using Org.BouncyCastle.Math.EC.Multiplier;
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
        private List<DateSetMainForm> dateSetMainForms = new List<DateSetMainForm>();

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

            string query = $"SELECT people.Id, people.Name as Имя, people.Surname as Фамилия, people.Patronymic as Отчество FROM authorization.timework left join people on idPeople = people.id group by people.id;";

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

                        //labelTotalDayNight.Text = $"Кол-во д - {day} н - {night}";
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

            int maxDay = MaxDay();
            string query = "SELECT ";

            for (int i = 1; i <= maxDay; i++)
            {
                if(maxDay == i)
                {
                    query += $"CASE WHEN Date = '2024-04-{i}' THEN\r\n\t\tcase\r\n\t\t\tWHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL THEN CONCAT_WS(': ', 'День', states_day.abbreviation, Day, 'Ночь', states_night.abbreviation, Night)\r\n            when states_day.abbreviation IS not NULL THEN CONCAT_WS(': ', 'День', states_day.abbreviation, Day)\r\n            when states_night.abbreviation IS not NULL THEN CONCAT_WS(': ', 'Ночь', states_night.abbreviation, Night)\r\n\t\tend\r\n        else null\r\n\tEND AS '{i}'";
                }
                else
                {
                query += $"CASE WHEN Date = '2024-04-{i}' THEN\r\n\t\tcase\r\n\t\t\tWHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL THEN CONCAT_WS(': ', 'День', states_day.abbreviation, Day, 'Ночь', states_night.abbreviation, Night)\r\n            when states_day.abbreviation IS not NULL THEN CONCAT_WS(': ', 'День', states_day.abbreviation, Day)\r\n            when states_night.abbreviation IS not NULL THEN CONCAT_WS(': ', 'Ночь', states_night.abbreviation, Night)\r\n\t\tend\r\n        else null\r\n\tEND AS '{i}',";
                }
            }

            query += "FROM \r\n    timework\r\nleft join states as states_day on states_day.id = timework.IdStateDay\r\nleft join states as states_night on states_night.id = timework.IdStateNight\r\nleft join people on people.id = timework.idPeople\r\nwhere timework.idPeople = @Id and date between @firstDay and @lastDay";

            DataSet ds = new DataSet();

            try
            {
                ds.Tables.Clear();
                await _mCon.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, _mCon))
                {
                    command.Parameters.AddWithValue($"@firstDay", dateMysqlFirst);
                    command.Parameters.AddWithValue($"@lastDay", dateMysqlLast);
                    command.Parameters.AddWithValue($"@Id", selectedWorker.Id);

                    using (MySqlDataAdapter dsAdapter = new MySqlDataAdapter(command))
                    {
                        dsAdapter.Fill(ds);
                    }
                }

                dateSetMainForms.Clear();

                foreach (DataTable table in ds.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        foreach (DataColumn column in table.Columns)
                        {
                            if (!row.IsNull(column))
                            {
                                // Получаем значение ячейки и имя столбца
                                string value = row[column].ToString();
                                string columnName = column.ColumnName;

                                dateSetMainForms.Add(new DateSetMainForm(columnName, value));
                            }
                        }
                    }
                }

                ds.Clear();
                ds = UpdateDataGridView();

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

        private DataSet UpdateDataGridView()
        {
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();

            int max = MaxDay();

            for(int i = 1; i <= max; i++)
            {
                dataTable.Columns.Add(i.ToString(), typeof(string));
            }

            DataRow dataRow = dataTable.NewRow();
            
            for(int i = 0; i < dateSetMainForms.Count; i++)
            {
                dataRow[dateSetMainForms[i].TableName] = dateSetMainForms[i].Text;
            }

            dataTable.Rows.Add(dataRow);

            dataSet.Tables.Add(dataTable);

            return dataSet;
        }

        private int MaxDay()
        {
            DateTime lastDay = GetDateTimeLastDay(dateTimePicker1.Value);
            return lastDay.Day;
        }


    }
}
