﻿using MySql.Data.MySqlClient;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Salary_for_workers
{
    public partial class MainForm : Form
    {
        private int _changeIdUser;
        private int _userIdPosition;
        private SelectedWorkers selectedWorker;
        private MySqlConnection _mCon;
        private List<Worker> _workers;
        private List<DateSetMainForm> dateSetMainForms = new List<DateSetMainForm>();
        
        private int _dayTotal = 0;
        private int _nightTotal = 0;

        private int index;
        private bool _isCornev = false;

        public MainForm(List<Worker> workers, MySqlConnection mCon, int userIdPosition)
        {
            _userIdPosition = userIdPosition;
            _mCon = mCon;
            _workers = workers;
            InitializeComponent();
            comboBox1.Visible = false;
            dateTimePicker1.CustomFormat = "MM-yyyy";
        }

        public MainForm(List<Worker> workers, MySqlConnection mCon, int userIdPosition, bool isCornev)
        {

            _userIdPosition = userIdPosition;
            _mCon = mCon;
            _workers = workers;
            InitializeComponent();
            comboBox1.Visible = true;
            dateTimePicker1.CustomFormat = "MM-yyyy";
            _isCornev = isCornev;
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
            DateTime lastDayOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            return lastDayOfMonth;
        }

        private async Task<DataSet> GetDataWorkers(DateTimePicker dateTime, int id)
        {
            DateTime firstDay = GetDateTimeFirstDay(dateTime.Value);
            DateTime lastDay = GetDateTimeLastDay(dateTime.Value);

            MySqlDateTime mySqlDateTimeFirst = new MySqlDateTime(firstDay);
            var dateMysqlFirst = mySqlDateTimeFirst.GetDateTime().Date.ToString("yyyy-MM-dd");


            MySqlDateTime mySqlDateTimeLast = new MySqlDateTime(lastDay);
            var dateMysqlLast = mySqlDateTimeLast.GetDateTime().Date.ToString("yyyy-MM-dd");
            
            string query = null;

            if (index == 0)
            {
                query = $"SELECT Id, Name as Имя, Surname as Фамилия, Patronymic as Отчество  FROM authorization.people";
            }
            else
            {
                if (_isCornev)
                {
                    query = $"SELECT Id, Name as Имя, Surname as Фамилия, Patronymic as Отчество  FROM authorization.people where idPositions = @id group by id";
                }
                else
                {
                    query = $"SELECT Id, Name as Имя, Surname as Фамилия, Patronymic as Отчество  FROM authorization.people where idPositions = (select idPositions from people where id = @id LIMIT 1) group by id";
                }
            }



            DataSet ds = new DataSet();

            try
            {
                ds.Tables.Clear();
                await _mCon.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, _mCon))
                {
                    command.Parameters.AddWithValue($"@id", id);

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
            System.Windows.Forms.Button text = (System.Windows.Forms.Button)sender;
            int day = Convert.ToInt32(text.Text);
            int lastDayOfMonth = DateTime.DaysInMonth(year, month);

            if(day <= lastDayOfMonth)
            {
                EditorDay form2 = new EditorDay(_workers, new DateTime(year, month, day), _mCon);
                this.Visible = false;
                form2.ShowDialog();
                this.Visible = true;
            }
        }

        private async void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            
            if (!_isCornev)
                ds = await GetDataWorkers(dateTimePicker1, _workers[0].Id);
            else
                ds = await GetDataWorkers(dateTimePicker1, index);
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
            string query = $"SELECT ";

            for (int i = 1; i <= maxDay; i++)
            {
                if(maxDay == i)
                {
                    query += $"CASE WHEN Date = '{firstDay.Year}-{firstDay.Month}-{i}' THEN case" + 
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL AND states_day.id = 6 AND states_night.id = 6  THEN CONCAT_WS('', states_day.abbreviation, ' / ', states_night.abbreviation )" +
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL AND states_day.id = 6 THEN CONCAT_WS('', states_day.abbreviation, ' / Н', '-', Night)" +
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL AND states_night.id = 6 THEN CONCAT_WS('', 'Д', '-', Day, ' / ', states_night.abbreviation ) " +
                        " WHEN states_night.abbreviation IS NULL AND states_night.id = 6  THEN CONCAT_WS('', 'Д', '-' '0' ' / ', states_day.abbreviation) " +
                        " WHEN states_day.abbreviation IS not NULL AND states_day.id = 6 THEN CONCAT_WS('', states_day.abbreviation, ' / Н', '-', '0') " +
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL AND states_day.id = 30 AND states_night.id = 30 THEN CONCAT_WS('', states_day.abbreviation, ' / ', states_night.abbreviation ) " +
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL AND states_day.id = 30 THEN CONCAT_WS('', states_day.abbreviation, ' / Н', '-', Night) " +
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL AND states_night.id = 30 THEN CONCAT_WS('', 'Д', '-', Day, ' / ', states_night.abbreviation ) " +
                        " WHEN states_night.abbreviation IS NULL AND states_night.id = 30  THEN CONCAT_WS('', 'Д', '-' '0' ' / ', states_day.abbreviation) " +
                        " WHEN states_day.abbreviation IS not NULL AND states_day.id = 30 THEN CONCAT_WS('', states_day.abbreviation, ' / Н', '-', '0') " +
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL AND states_day.id = 16 AND states_night.id = 16  THEN CONCAT_WS('', states_day.abbreviation, ' / ', states_night.abbreviation ) " +
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL AND states_day.id = 16 THEN CONCAT_WS('', states_day.abbreviation, ' / Н', '-', Night)" +
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL AND states_night.id = 16 THEN CONCAT_WS('', 'Д', '-', Day, ' / ', states_night.abbreviation ) " +
                        " WHEN states_night.abbreviation IS NULL AND states_night.id = 16  THEN CONCAT_WS('', 'Д', '-' '0' ' / ', states_day.abbreviation) " +
                        " WHEN states_day.abbreviation IS not NULL AND states_day.id = 16 THEN CONCAT_WS('', states_day.abbreviation, ' / Н', '-', '0') " +
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL AND states_day.id = 9 AND states_night.id = 9  THEN CONCAT_WS('', states_day.abbreviation, ' / ', states_night.abbreviation ) " +
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL AND states_day.id = 9 THEN CONCAT_WS('', states_day.abbreviation, ' / Н', '-', Night) " +
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL AND states_night.id = 9 THEN CONCAT_WS('', 'Д', '-', Day, ' / ', states_night.abbreviation ) " +
                        " WHEN states_night.abbreviation IS NULL AND states_night.id = 9  THEN CONCAT_WS('', 'Д', '-' '0' ' / ', states_day.abbreviation) " +
                        " WHEN states_day.abbreviation IS not NULL AND states_day.id = 9 THEN CONCAT_WS('', states_day.abbreviation, ' / Н', '-', '0') " +
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL AND states_day.id = 19 AND states_night.id = 19  THEN CONCAT_WS('', states_day.abbreviation, ' / ', states_night.abbreviation ) " +
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL AND states_day.id = 19 THEN CONCAT_WS('', states_day.abbreviation, ' / Н', '-', Night) " +
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL AND states_night.id = 19 THEN CONCAT_WS('', 'Д', '-', Day, ' / ', states_night.abbreviation )" +
                        " WHEN states_night.abbreviation IS NULL AND states_night.id = 19  THEN CONCAT_WS('', 'Д', '-' '0' ' / ', states_day.abbreviation) " +
                        " WHEN states_day.abbreviation IS not NULL AND states_day.id = 19 THEN CONCAT_WS('', states_day.abbreviation, ' / Н', '-', '0') " +
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL THEN CONCAT_WS('', 'Д', '-', Day, ' / Н', '-', Night) " +
                        " when states_day.abbreviation IS not NULL THEN CONCAT_WS('', 'Д', '-', Day, ' / Н', '-', '0') " +
                        $" when states_night.abbreviation IS not NULL THEN CONCAT_WS('','Д', '-', '0', ' / Н', '-', Night) end else null END AS '{i}'";
                }
                else
                {
                    query += $"CASE WHEN Date = '{firstDay.Year}-{firstDay.Month}-{i}' THEN case" +
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL AND states_day.id = 6 AND states_night.id = 6  THEN CONCAT_WS('', states_day.abbreviation, ' / ', states_night.abbreviation )" +
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL AND states_day.id = 6 THEN CONCAT_WS('', states_day.abbreviation, ' / Н', '-', Night)" +
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL AND states_night.id = 6 THEN CONCAT_WS('', 'Д', '-', Day, ' / ', states_night.abbreviation ) " +
                        " WHEN states_night.abbreviation IS NULL AND states_night.id = 6  THEN CONCAT_WS('', 'Д', '-' '0' ' / ', states_day.abbreviation) " +
                        " WHEN states_day.abbreviation IS not NULL AND states_day.id = 6 THEN CONCAT_WS('', states_day.abbreviation, ' / Н', '-', '0') " +
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL AND states_day.id = 30 AND states_night.id = 30 THEN CONCAT_WS('', states_day.abbreviation, ' / ', states_night.abbreviation ) " +
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL AND states_day.id = 30 THEN CONCAT_WS('', states_day.abbreviation, ' / Н', '-', Night) " +
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL AND states_night.id = 30 THEN CONCAT_WS('', 'Д', '-', Day, ' / ', states_night.abbreviation ) " +
                        " WHEN states_night.abbreviation IS NULL AND states_night.id = 30  THEN CONCAT_WS('', 'Д', '-' '0' ' / ', states_day.abbreviation) " +
                        " WHEN states_day.abbreviation IS not NULL AND states_day.id = 30 THEN CONCAT_WS('', states_day.abbreviation, ' / Н', '-', '0') " +
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL AND states_day.id = 16 AND states_night.id = 16  THEN CONCAT_WS('', states_day.abbreviation, ' / ', states_night.abbreviation ) " +
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL AND states_day.id = 16 THEN CONCAT_WS('', states_day.abbreviation, ' / Н', '-', Night)" +
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL AND states_night.id = 16 THEN CONCAT_WS('', 'Д', '-', Day, ' / ', states_night.abbreviation ) " +
                        " WHEN states_night.abbreviation IS NULL AND states_night.id = 16  THEN CONCAT_WS('', 'Д', '-' '0' ' / ', states_day.abbreviation) " +
                        " WHEN states_day.abbreviation IS not NULL AND states_day.id = 16 THEN CONCAT_WS('', states_day.abbreviation, ' / Н', '-', '0') " +
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL AND states_day.id = 9 AND states_night.id = 9  THEN CONCAT_WS('', states_day.abbreviation, ' / ', states_night.abbreviation ) " +
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL AND states_day.id = 9 THEN CONCAT_WS('', states_day.abbreviation, ' / Н', '-', Night) " +
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL AND states_night.id = 9 THEN CONCAT_WS('', 'Д', '-', Day, ' / ', states_night.abbreviation ) " +
                        " WHEN states_night.abbreviation IS NULL AND states_night.id = 9  THEN CONCAT_WS('', 'Д', '-' '0' ' / ', states_day.abbreviation) " +
                        " WHEN states_day.abbreviation IS not NULL AND states_day.id = 9 THEN CONCAT_WS('', states_day.abbreviation, ' / Н', '-', '0') " +
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL AND states_day.id = 19 AND states_night.id = 19  THEN CONCAT_WS('', states_day.abbreviation, ' / ', states_night.abbreviation ) " +
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL AND states_day.id = 19 THEN CONCAT_WS('', states_day.abbreviation, ' / Н', '-', Night) " +
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL AND states_night.id = 19 THEN CONCAT_WS('', 'Д', '-', Day, ' / ', states_night.abbreviation )" +
                        " WHEN states_night.abbreviation IS NULL AND states_night.id = 19  THEN CONCAT_WS('', 'Д', '-' '0' ' / ', states_day.abbreviation) " +
                        " WHEN states_day.abbreviation IS not NULL AND states_day.id = 19 THEN CONCAT_WS('', states_day.abbreviation, ' / Н', '-', '0') " +
                        " WHEN states_day.abbreviation IS not NULL AND states_night.abbreviation IS not NULL THEN CONCAT_WS('', 'Д', '-', Day, ' / Н', '-', Night) " +
                        " when states_day.abbreviation IS not NULL THEN CONCAT_WS('', 'Д', '-', Day, ' / Н', '-', '0') " +
                        $" when states_night.abbreviation IS not NULL THEN CONCAT_WS('','Д', '-', '0', ' / Н', '-', Night) end else null END AS '{i}',";
                }
            }

            query += "FROM timework left join states as states_day on states_day.id = timework.IdStateDay left join states as states_night on states_night.id = timework.IdStateNight\r\nleft join people on people.id = timework.idPeople where timework.idPeople = @Id and date between @firstDay and @lastDay";

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

                                if(value != "Д-0 / Н-0")
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

            ChangeTotal(await GetTotal(selectedWorker.Id, dateMysqlFirst, dateMysqlLast));
            return ds;

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

            for(int i = 0; i < max; i++)
            {
                dataRow[i] = "Д-0 / Н-0";
            }

            for (int i = 0; i < dateSetMainForms.Count; i++)
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

        private void ChangeTotal(DataSet ds)
        {
            _dayTotal = 0;
            _nightTotal = 0;

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

                            if (columnName == "day")
                            {
                                bool convert = Int32.TryParse(value, out _dayTotal);

                            }
                            else if (columnName == "night")
                            {
                                bool convert = Int32.TryParse(value, out _nightTotal);
                            }
                        }
                    }
                }
            }

            labelTotalDayNight.Text = "Количество отработанного времени";

            label3.Text = $"Д: {_dayTotal}" +
                $"\nН: {_nightTotal}" +
                $"\nВсего: {_dayTotal + _nightTotal}" +
                $"\nДней: {dateSetMainForms.Count}";
        }

        private async Task<DataSet> GetTotal(int idPeople, string firstDay, string lastDay)
        {
            string query = "SELECT sum(case when IdStateDay != 19 and IdStateDay !=9 and IdStateDay !=6 and IdStateDay !=16 and IdStateDay !=30 then day else 0 end) as 'day', sum(case when IdStateNight != 19 and IdStateNight !=9 and IdStateNight !=6 and IdStateNight !=16 and IdStateNight !=30 then night else 0 end) as 'night' FROM authorization.timework where timework.idPeople = @idPeople and date between @firstDay and @lastDay";

            DataSet ds = new DataSet();

            try
            {
                ds.Tables.Clear();
                await _mCon.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, _mCon))
                {
                    command.Parameters.AddWithValue($"@firstDay", firstDay);
                    command.Parameters.AddWithValue($"@lastDay", lastDay);
                    command.Parameters.AddWithValue($"@idPeople", idPeople);

                    using (MySqlDataAdapter dsAdapter = new MySqlDataAdapter(command))
                    {
                        dsAdapter.Fill(ds);
                    }
                }
            }
            catch(Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
            }
            finally { _mCon.Close(); }

            return ds;

        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            CreateUser createUser = new CreateUser();
            this.Visible = false;
            createUser.ShowDialog();
            this.Visible = true;
        }

        private async void MainForm_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                DataSet ds = new DataSet();

                if (!_isCornev)
                {
                    ds = await GetDataWorkers(dateTimePicker1, _workers[0].Id);
                    dataGridViewPeople.DataSource = ds.Tables[0];
                    _workers = await GetWorkersAsync(_userIdPosition);
                }
                else
                {
                    await GetPosition();
                    comboBox1.SelectedItem = comboBox1.Items[0];
                }
            }
        }

        private async Task<List<Worker>> GetWorkersAsync(int idPosition)
        {
            List<Worker> workers = new List<Worker>();
            string query = "SELECT people.Id, people.Name, people.Surname, people.Patronymic, people.EmploymentDate FROM people left join passwords ON people.idPassword = passwords.id left join positions ON people.idPositions = positions.id WHERE positions.id = @idPositions;";

            try
            {
                using (MySqlCommand command = new MySqlCommand(query, _mCon))
                {
                    command.Parameters.AddWithValue($"@idPositions", idPosition);

                    await _mCon.OpenAsync();

                    using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Worker worker = new Worker(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetDateTime(4));
                            workers.Add(worker);
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { _mCon.Close(); }

            return workers;
        }

        private void dataGridViewPeople_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int selectedRowIndex = dataGridViewPeople.SelectedRows[0].Index; // Получаем индекс выбранной строки
                int selectedColumnIndex = 0; // Например, предположим, что нам нужно получить значение в первом столбце

                ContextMenuStrip contextMenu = new ContextMenuStrip();
                ToolStripMenuItem item1 = new ToolStripMenuItem("Изменить");
                contextMenu.Items.Add(item1);
                contextMenu.Items[0].Click += Option_Click;

                if (selectedRowIndex >= 0 && selectedColumnIndex >= 0)
                {
                    DataGridViewCell selectedCell = dataGridViewPeople.Rows[selectedRowIndex].Cells[selectedColumnIndex];
                    if (selectedCell.Value != null)
                    {
                        string cellValue = selectedCell.Value.ToString();
                        Int32.TryParse(cellValue, out _changeIdUser);
                        contextMenu.Show(dataGridViewPeople, dataGridViewPeople.PointToClient(Cursor.Position));
                        // Используйте переменную cellValue, содержащую значение ячейки, для дальнейшей обработки
                    }
                }
            }
        }

        private async Task GetPosition()
        {
            string query = $"SELECT Name FROM authorization.positions";

            try
            {
                await _mCon.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, _mCon))
                {

                    using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync())
                    {
                        comboBox1.Items.Add("Все");

                        while (await reader.ReadAsync())
                        {
                            comboBox1.Items.Add(reader.GetString(0));
                        }

                        reader.Close();
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

        }

        private void Option_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            if(_changeIdUser != null && _changeIdUser != -1)
            {
                CreateUser createUser = new CreateUser(_changeIdUser);
                createUser.ShowDialog();
            }
            this.Click -= Option_Click;
            this.Visible = true;
        }

        private async void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            index = comboBox1.Items.IndexOf(comboBox1.SelectedItem);

            DataSet ds = new DataSet();

            if (!_isCornev)
                ds = await GetDataWorkers(dateTimePicker1, _workers[0].Id);
            else
                ds = await GetDataWorkers(dateTimePicker1, index);

            dataGridViewPeople.DataSource = ds.Tables[0];
        }
    }
}
