﻿using MySql.Data.MySqlClient;
using MySql.Data.Types;
using Mysqlx.Crud;
using Org.BouncyCastle.Crypto;
using Salary_for_workers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Salary_for_workers
{
    public partial class Form2 : Form
    {
        public static int IdDay = -1;
        public static string TextDay;
        public static int IdNight = -1;
        public static string TextNight;
        
        private MySqlConnection _mCon;
        private List<Worker> _workers;
        private DateTime _datetime;
        private CancellationTokenSource _cancellationTokenSource;
        private List<SelectedWorkers> selectedWorkers = new List<SelectedWorkers>();

        public Form2(List<Worker> workers, MySqlConnection mCon)
        {
            _mCon = mCon;
            InitializeComponent();
            _cancellationTokenSource = new CancellationTokenSource();
            this._workers = workers;
        }

        public Form2(List<Worker> workers, DateTime date, MySqlConnection mCon)
        {
            _mCon = mCon;
            _workers = workers;
            _datetime = date;
            _cancellationTokenSource = new CancellationTokenSource();
            InitializeComponent();
        }

        private async void Form2_Load(object sender, EventArgs e)
        {
            _mCon = await CreateConnectionAsync();
            UpdateComboBox();
            ChengeToolTipDescription();
            toolTip1.SetToolTip(buttonSubmit, "");
            dataGridView1.DataSource = UpdateDataGridView();
            dataGridView1.Refresh();
        }

        private async Task<MySqlConnection> CreateConnectionAsync()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["local"].ConnectionString;
            MySqlConnection connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            return connection;
        }

        private void DataGridViewFullSelected()
        {
            dataGridView1.SelectAll();

            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.Blue;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;

            
        }

        private DataSet UpdateDataGridView()
        {
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();

            dataTable.Columns.Add("id", typeof(int));
            dataTable.Columns.Add("Имя", typeof(string));
            dataTable.Columns.Add("Фамилия", typeof(string));
            dataTable.Columns.Add("Отчество", typeof(string));
            dataTable.Columns.Add("Значение дня", typeof(string));
            dataTable.Columns.Add("День", typeof(int));
            dataTable.Columns.Add("Значение ночи", typeof(string));
            dataTable.Columns.Add("Ночь", typeof(int));


            foreach (var worker in _workers)
            {
                DataRow dataRow = dataTable.NewRow();

                dataRow["id"] = worker.Id;
                dataRow["Имя"] = worker.Name;
                dataRow["Фамилия"] = worker.Surname;
                dataRow["Отчество"] = worker.Patronymic;
                dataRow["Значение дня"] = worker.GetDayAbbreviation(_datetime);
                dataRow["День"] = worker.GetDay(_datetime);
                dataRow["Значение ночи"] = worker.GetNightAbbreviation(_datetime);
                dataRow["Ночь"] = worker.GetNight(_datetime);

                dataTable.Rows.Add(dataRow);
            }

            dataSet.Tables.Add(dataTable);

            return dataSet;
        }

        private void SetsFurstDayOfMonth()
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            var startDay = new DateTime(year, month, 1);
            //dateTimePicker1.Value = startDay;
        }

        private async Task<List<Worker>> TryGetDataAsync(CancellationToken cancellationToken)
        {
            string query = "SELECT timework.Id, Date, states_day.id as idDay, day, states_day.abbreviation , states_night.id as idNight, night, states_night.abbreviation FROM authorization.timework left join people on timework.idPeople = people.Id left join states as states_day on timework.IdStateDay = states_day.Id left join states as states_night on timework.IdStateNight = states_night.id where (name = @name and surname = @surname and Patronymic = @Patronymic and EmploymentDate = @EmploymentDate and idPeople = @idPeople and date = @date)";
            
            
            try
            {
                if(_mCon.State == ConnectionState.Closed)
                    await _mCon.OpenAsync();

                foreach (var worrker in _workers)
                {
                    MySqlDateTime mySqlDateTime = new MySqlDateTime(_datetime.Date);
                    var dateMysql = mySqlDateTime.GetDateTime().Date.ToString("yyyy-MM-dd");


                    using (MySqlCommand command = new MySqlCommand(query, _mCon))
                    {
                        command.Parameters.AddWithValue($"@name", worrker.Name);
                        command.Parameters.AddWithValue($"@surname", worrker.Surname);
                        command.Parameters.AddWithValue($"@Patronymic", worrker.Patronymic);
                        command.Parameters.AddWithValue($"@EmploymentDate", worrker.EmploymentDate);
                        command.Parameters.AddWithValue($"@idPeople", worrker.Id);
                        command.Parameters.AddWithValue($"@date", dateMysql);

                        await Task.Run(async () =>
                        {
                            using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    worrker.AddDayWork(reader.GetInt32(0),
                                        reader.GetDateTime(1),
                                        reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2),
                                        reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                                        reader.IsDBNull(4) ? null : reader.GetString(4),
                                        reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5),
                                        reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                                        reader.IsDBNull(7) ? null : reader.GetString(7));
                                }

                                reader.Close();
                            }
                        }, cancellationToken);
                    }
                }
            }
            catch(MySqlException ex)
            {
                MessageBox.Show("Ошибка чтения данных");
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { 
                if(_mCon.State == ConnectionState.Open)
                    _mCon.Close(); 
            }

            return _workers;
        }

        private void ChengeToolTipDescription()
        {
            var col = buttonSubmit.BackColor;
            //if(buttonSubmit.BackColor ) ;
        }

        private void UpdateComboBox()
        {
            comboBoxPeoples.Items.Clear();

            foreach (var worker in _workers)
            {
                comboBoxPeoples.Items.Add(worker.Name + " " + worker.Surname + " " + worker.Patronymic);
            }

            comboBoxPeoples.Items.Add("Выбранные");
            comboBoxPeoples.Items.Add("Все");
            comboBoxPeoples.SelectedIndex = comboBoxPeoples.Items.Count - 1;
        }

        private void GetDayAndNightThisDate(string fullName, DateTime date)
        {
            string name, surname, patronymic;

            ParseFullName(fullName, out name, out surname, out patronymic);

            foreach (var worker in _workers)
            {
                if (worker.Name == name && worker.Surname == surname && worker.Patronymic == patronymic)
                {
                    int? day, night;
                    string nightName, dayName;

                    day = worker.GetDay(date);
                    dayName = worker.GetDayAbbreviation(date);

                    night = worker.GetNight(date);
                    nightName = worker.GetNightAbbreviation(date);

                    if (day != -1 && day != null)
                    {
                        textBoxDay.Text = day.ToString();
                    }
                    else
                    {
                        textBoxDay.Text = "";
                    }

                    if (dayName != "-1" && dayName != null)
                    {
                        textBoxDayName.Text = dayName;
                    }
                    else
                    {
                        textBoxDayName.Text = "";
                    }

                    if (night != -1 && night != null)
                    {
                        textBoxNight.Text = night.ToString();
                    }
                    else
                    {
                        textBoxNight.Text = "";
                    }

                    if (nightName != "-1" && nightName != null)
                    {
                        textBoxNightName.Text = nightName;
                    }
                    else
                    {
                        textBoxNightName.Text = "";
                    }
                }
            }
        }

        private void ParseFullName(string fullName, out string name, out string surname, out string patronymic)
        {
            name = "";
            surname = "";
            patronymic = "";

            string[] parts = fullName.Split(' ');

            if (parts.Length >= 1)
            {
                name = parts[0];
            }
            if (parts.Length >= 2)
            {
                surname = parts[1];
            }
            if (parts.Length >= 3)
            {
                patronymic = parts[2];
            }
        }

        private async void comboBoxPeoples_TextChanged(object sender, EventArgs e)
        {
            _workers = await TryGetDataAsync(_cancellationTokenSource.Token);
            GetDayAndNightThisDate(comboBoxPeoples.Text, _datetime);
            DataSet dataSet = UpdateDataGridView();
            dataGridView1.DataSource = dataSet.Tables["Table1"];
            dataGridView1.Refresh();
        }

        private async void dateTimePicker1_ValueChangedAsync(object sender, EventArgs e)
        {
            _workers = await TryGetDataAsync(_cancellationTokenSource.Token);
            GetDayAndNightThisDate(comboBoxPeoples.Text, _datetime);
            dataGridView1.DataSource = UpdateDataGridView();
            dataGridView1.Refresh();
        }

        private List<Worker> GetThisWorkers()
        {
            string name, surname, patronymic;

            List<Worker> returnWorkers = new List<Worker>();

            if (comboBoxPeoples.Text.Trim() == null && comboBoxPeoples.Text.Trim() == "")
                return null;

            ParseFullName(comboBoxPeoples.Text, out name, out surname, out patronymic);

            switch (name)
            {
                case "Все":
                    foreach (var selectedWorkers in selectedWorkers)
                    {
                        foreach (var worker in _workers)
                        {
                            if(selectedWorkers.Name ==  worker.Name 
                                && selectedWorkers.Surname == worker.Surname 
                                && selectedWorkers.Patronymic == worker.Patronymic 
                                && selectedWorkers.Id == worker.Id)
                            {
                                returnWorkers.Add(worker);
                            }
                        }
                    }
                    break;
                case "Выбранные":
                    foreach (var selectedWorkers in selectedWorkers)
                    {
                        foreach (var worker in _workers)
                        {
                            if (selectedWorkers.Name == worker.Name
                                && selectedWorkers.Surname == worker.Surname
                                && selectedWorkers.Patronymic == worker.Patronymic
                                && selectedWorkers.Id == worker.Id)
                            {
                                returnWorkers.Add(worker);
                            }
                        }
                    }
                    break;
                default:
                    foreach (var worker in _workers)
                    {
                        if (worker.Name == name && worker.Surname == surname && worker.Patronymic == patronymic)
                        {
                            returnWorkers.Add(worker);
                        }
                    }
                    break;
            }

            return returnWorkers;
        }

        //private async Task UpdateDataAsync(int day, int night, List<int> id, List<int> idDay)
        //{
        //    string query = $"UPDATE `authorization`.`timework` SET `Day` = '{day}', `Night` = '{night}', `idPeople` = '{id}' WHERE (`Id` = '{idDay}');";

        //    if (IdDay != -1 && IdNight != -1)
        //    {
        //        query = $"INSERT INTO `authorization`.`timework` (`Date`, `Day`, `Night`, `idPeople`, `IdStateDay`, `IdStateNight`) VALUES ";
        //        foreach (var id in ids)
        //        {
        //            query += $"('{dateMysql}', '{day}', '{night}', '{id}', '{IdDay}', '{IdNight}')";
        //        }
        //        query += ";";
        //    }
        //    else if (IdNight == -1)
        //    {
        //        query = $"INSERT INTO `authorization`.`timework` (`Date`, `Day`, `idPeople`, `IdStateDay`) VALUES ";
        //        foreach (var id in ids)
        //        {
        //            query += $"('{dateMysql}', '{day}', '{id}', '{IdDay}')";
        //        }
        //        query += ";";
        //    }
        //    else if (IdDay == -1)
        //    {
        //        query = $"INSERT INTO `authorization`.`timework` (`Date`, `Night`, `idPeople`, `IdStateNight`) VALUES ";
        //        foreach (var id in ids)
        //        {
        //            query += $"('{dateMysql}', '{night}', '{id}', '{IdNight}');";
        //        }
        //        query += ";";
        //    }
        //    else
        //    {
        //        MessageBox.Show("Данные по атрибутам дня или ночи отсутствуют");
        //        return;
        //    }

        //    try
        //    {
        //        await _mCon.OpenAsync();

        //        using (MySqlCommand command = new MySqlCommand(query, _mCon))
        //        {
        //            await command.ExecuteNonQueryAsync();
        //            MessageBox.Show("Данные изменены");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //    finally { _mCon.Close(); }
        //}

        private async Task InsertDataAsync(int day, int night, DateTime date, List<int> ids)
        {
            MySqlDateTime mySqlDateTime = new MySqlDateTime(date.Date);

            var dateMysql = mySqlDateTime.GetDateTime().Date.ToString("yyyy-MM-dd");

            string query;

            if (IdDay == -1 && IdNight == -1)
            {
                MessageBox.Show("Не установлены атрибуты", "Ошибка!");
                return;
            }
            else if(IdDay != -1 && IdNight != -1)
            {
                query = $"INSERT INTO `authorization`.`timework` (`Date`, `Day`, `Night`, `idPeople`, `IdStateDay`, `IdStateNight`) VALUES ";
                foreach (var id in ids)
                {
                    query += $"('{dateMysql}', '{day}', '{night}', '{id}', '{IdDay}', '{IdNight}')";
                }
                query += ";";
            }
            else if (IdNight == -1)
            {
                query = $"INSERT INTO `authorization`.`timework` (`Date`, `Day`, `idPeople`, `IdStateDay`) VALUES ";

                int lastDate = ids.Count -1;

                for(int i = 0; i < ids.Count; i++)
                {

                    if(lastDate == i)
                    {
                        query += $"('{dateMysql}', '{day}', '{ids[i]}', '{IdDay}');";
                    }
                    else
                    {
                        query += $"('{dateMysql}', '{day}', '{ids[i]}', '{IdDay}'), ";
                    }

                }
            }
            else if (IdDay == -1)
            {
                query = $"INSERT INTO `authorization`.`timework` (`Date`, `Night`, `idPeople`, `IdStateNight`) VALUES ";
                
                foreach (var id in ids)
                {
                    query += $"('{dateMysql}', '{night}', '{id}', '{IdNight}')";
                }
                query += ";";
            }
            else
            {
                MessageBox.Show("Данные по атрибутам дня или ночи отсутствуют");
                return;
            }

            try
            {
                await _mCon.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, _mCon))
                {

                    await command.ExecuteNonQueryAsync();
                    MessageBox.Show("Данные добавлены в базу данных");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { _mCon.Close(); }
        }

        private async void buttonSubmit_Click(object sender, EventArgs e)
        {
            List<Worker> workers = GetThisWorkers();

            bool insert = false;
            bool update = false;
            bool isCompliteParseDay = int.TryParse(textBoxDay.Text, out int dayInt);
            bool isCompliteParseNight = int.TryParse(textBoxNight.Text, out int nightInt);

            if (workers.Count == 0)
            {
                MessageBox.Show("Не найден работник для добавления");
                return;
            }

            if (!isCompliteParseDay && !isCompliteParseNight)
            {
                MessageBox.Show("Пустые время день/ночь");
                return;
            }

            foreach (var worker in workers)
            {
                int? day = worker.GetDay(_datetime);
                int? night = worker.GetNight(_datetime);

                if (day == -1 && night == -1)
                {
                    insert = true;
                }
                else if (day != -1)
                {
                    update = true;
                }
                else if (night != -1)
                {
                    update = true;
                }

                if (insert == true && update == true)
                {
                    MessageBox.Show($"Невозможно добавить данные к пользователю у которого уже есть данные\n(выберите его отдельно для обновления)\nПользователь {worker.Id} {worker.Name} {worker.Surname} {worker.Patronymic}", "Ошибка !");
                    return;
                }

                if (dayInt == day && nightInt == night)
                {
                    MessageBox.Show($"Данные не изменились для 1 из пользователей {worker.Name} {worker.Surname} {worker.Patronymic}");
                    return;
                }
            }

            foreach (var worker in workers)
            {
                int? day = worker.GetDay(_datetime);
                int? night = worker.GetNight(_datetime);

                if (day == -1)
                    day = 0;
                if (night == -1)
                    night = 0;

                if (dayInt == day && nightInt == night)
                {
                    MessageBox.Show($"Данные не изменились для 1 из пользователей {worker.Name} {worker.Surname} {worker.Patronymic}");
                    return;
                }
            }

            if (insert)
            {
                List<int> ids = new List<int>();
                foreach (var worker in workers)
                {
                    ids.Add(worker.Id);
                }

                await InsertDataAsync(dayInt, nightInt, _datetime, ids);
            }
            else
            {
                List<int> ids = new List<int>();
                List<int> idDays = new List<int>();

                foreach (var worker in workers)
                {
                    ids.Add(worker.Id);
                    int idDay = worker.GetId(_datetime);
                    if (idDay != -1)
                        idDays.Add(idDay);
                    else
                    {
                        MessageBox.Show($"Не удалось получить id дня для пользователя {worker.Name} {worker.Surname} {worker.Patronymic}");
                        return;
                    }
                }

                //await UpdateDataAsync(dayInt, nightInt, ids, idDays);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectedWorkers selectedWorker;

            if(dataGridView1.SelectedRows.Count == dataGridView1.RowCount)
            {
                selectedWorkers.Clear();
                comboBoxPeoples.SelectedIndex = comboBoxPeoples.Items.Count - 1;
            }
            else if (dataGridView1.SelectedRows.Count > 1)
            {
                comboBoxPeoples.SelectedIndex = comboBoxPeoples.Items.Count - 2;
            }
            else
            {
                selectedWorkers.Clear();
            }

            if (dataGridView1.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    if (!row.IsNewRow)
                    {
                        bool isCompliteParse = int.TryParse(row.Cells["id"].Value.ToString(), out int id);

                        if(!isCompliteParse)
                        {
                            MessageBox.Show("Ошибка в захвате id с datagrid");
                            return;
                        }

                        string name = row.Cells["Имя"].Value.ToString();
                        string surname = row.Cells["Фамилия"].Value.ToString();
                        string patronymic = row.Cells["Отчество"].Value.ToString();
                        string dayName = row.Cells["Значение дня"].Value.ToString();
                        string day = row.Cells["День"].Value.ToString();
                        string nightString = row.Cells["Значение ночи"].Value.ToString();
                        string night = row.Cells["Ночь"].Value.ToString();

                        selectedWorker = new SelectedWorkers(id, name, surname, patronymic);
                        selectedWorkers.Add(selectedWorker);
                    }
                }
            }
        }

        private void textBoxDayName_Click(object sender, EventArgs e)
        {
            States states = new States(true);
            states.ShowDialog();
            if (TextDay != null)
                textBoxDayName.Text = TextDay;
        }

        private void textBoxNightName_Click(object sender, EventArgs e)
        {
            States states = new States(false);
            states.ShowDialog();
            if (TextNight != null)
                textBoxNightName.Text = TextNight;
        }

        private void textBoxNightName_TextChanged(object sender, EventArgs e)
        {
            if (textBoxNightName.Text.Trim().Length > 0)
                return;
            else
                IdNight = -1;
        }

        private void textBoxDayName_TextChanged(object sender, EventArgs e)
        {
            if (textBoxDayName.Text.Trim().Length > 0)
                return;
            else
                IdDay = -1;
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DataGridViewFullSelected();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            _cancellationTokenSource.Cancel();
        }
    }
}

public class SelectedWorkers
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Surname { get; private set; }
    public string Patronymic { get; private set; }

    public SelectedWorkers( int id, string name, string surname, string patronymic)
    {
        Id = id;
        Name = name;
        Surname = surname;
        Patronymic = patronymic;
    }
}
