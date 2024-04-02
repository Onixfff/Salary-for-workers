using MySql.Data.MySqlClient;
using MySql.Data.Types;
using Mysqlx.Crud;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Salary_for_workers
{
    public partial class Form2 : Form
    {
        MySqlConnection mCon = new MySqlConnection(ConfigurationManager.ConnectionStrings["local"].ConnectionString);
        private List<Worker> _workers;
        private DateTime _datetime;

        public static int IdDay = -1;
        public static string TextDay;
        public static int IdNight = -1;
        public static string TextNight;

        public Form2(List<Worker> workers)
        {
            InitializeComponent();
            this._workers = workers;
        }

        public Form2(List<Worker> workers, DateTime date)
        {
            _workers = workers;
            _datetime = date;
            InitializeComponent();
        }

        private async void Form2_Load(object sender, EventArgs e)
        {
            UpdateComboBox();
            await TryGetDataAsync();
            ChengeToolTipDescription();
            toolTip1.SetToolTip(buttonSubmit, "");
            dataGridView1.DataSource = UpdateDataGridView();
            dataGridView1.Refresh();
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

        private async Task<List<Worker>> TryGetDataAsync()
        {
            foreach (var worrker in _workers)
            {
                //string query = "SELECT timework.Id, Date, day, night FROM authorization.timework cross join People " +
                //    "where (name = @name and surname = @surname and Patronymic = @Patronymic and EmploymentDate = @EmploymentDate and idPeople =@idPeople)";
                string query = "SELECT timework.Id, Date, states_day.id as idDay, day, states_day.abbreviation , states_night.id as idNight, night, states_night.abbreviation FROM authorization.timework left join people on timework.idPeople = people.Id left join states as states_day on timework.IdStateDay = states_day.Id left join states as states_night on timework.IdStateNight = states_night.id where (name = @name and surname = @surname and Patronymic = @Patronymic and EmploymentDate = @EmploymentDate and idPeople = @idPeople)";

                try
                {
                    using (MySqlCommand command = new MySqlCommand(query, mCon))
                    {
                        command.Parameters.AddWithValue($"@name", worrker.Name);
                        command.Parameters.AddWithValue($"@surname", worrker.Surname);
                        command.Parameters.AddWithValue($"@Patronymic", worrker.Patronymic);
                        command.Parameters.AddWithValue($"@EmploymentDate", worrker.EmploymentDate);
                        command.Parameters.AddWithValue($"@idPeople", worrker.Id);

                        await mCon.OpenAsync();

                        using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                worrker.AddDayWork(reader.GetInt32(0), reader.GetDateTime(1),
                                    reader.GetInt32(2), reader.GetInt32(3), reader.GetString(4),
                                    reader.GetInt32(5), reader.GetInt32(6), reader.GetString(7));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally { mCon.Close(); }
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
                if(worker.Name == name && worker.Surname == surname && worker.Patronymic == patronymic)
                {
                    int day, night;
                    string nightName, dayName;

                    day = worker.GetDay(date);
                    dayName = worker.GetDayAbbreviation(date);

                    night = worker.GetNight(date);
                    nightName = worker.GetNightAbbreviation(date);

                    if(day != -1)
                    {
                        textBoxDay.Text = day.ToString();
                    }
                    else
                    {
                        textBoxDay.Text = "";
                    }

                    if(dayName != "-1")
                    {
                        textBoxDayName.Text = dayName;
                    }
                    else
                    {
                        textBoxDayName.Text = "";
                    }

                    if(night != -1)
                    {
                        textBoxNight.Text = night.ToString();
                    }
                    else
                    {
                        textBoxNight.Text = "";
                    }

                    if(nightName != "-1")
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
            _workers = await TryGetDataAsync();
            GetDayAndNightThisDate(comboBoxPeoples.Text, _datetime);
            DataSet dataSet = UpdateDataGridView();
            dataGridView1.DataSource = dataSet.Tables["Table1"];
            dataGridView1.Refresh();
        }

        private async void dateTimePicker1_ValueChangedAsync(object sender, EventArgs e)
        {
            _workers = await TryGetDataAsync();
            GetDayAndNightThisDate(comboBoxPeoples.Text, _datetime);
            dataGridView1.DataSource = UpdateDataGridView();
            dataGridView1.Refresh();
        }

        private Worker GetThisWorker()
        {
            string name, surname, patronymic;

            Worker returnWorker = null;

            if (comboBoxPeoples.Text.Trim() == null && comboBoxPeoples.Text.Trim() == "")
                return null;

            ParseFullName(comboBoxPeoples.Text, out name, out surname, out patronymic);


            foreach (var worker in _workers)
            {
                if(worker.Name == name && worker.Surname == surname && worker.Patronymic == patronymic)
                {
                    returnWorker = worker;
                }
            }

            return returnWorker;
        }

        private async Task UpdateDataAsync(int day, int night, int id , int idDay)
        {
            string query = $"UPDATE `authorization`.`timework` SET `Day` = '{day}', `Night` = '{night}', `idPeople` = '{id}' WHERE (`Id` = '{idDay}');";

            try
            {
                await mCon.OpenAsync();

                using (MySqlCommand command = new MySqlCommand(query, mCon))
                {
                    await command.ExecuteNonQueryAsync();
                    MessageBox.Show("Данные изменены");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { mCon.Close(); }
        }

        private async Task InsertDataAsync(int day, int night, DateTime date, int id)
        {
            MySqlDateTime mySqlDateTime = new MySqlDateTime(date.Date);

            var dateMysql =  mySqlDateTime.GetDateTime().Date.ToString("yyyy-MM-dd");

            string query;

            if(IdDay != -1 && IdNight != -1)
            {
                query = $"INSERT INTO `authorization`.`timework` (`Date`, `Day`, `Night`, `idPeople`, `IdStateDay`, `IdStateNight`) VALUES ('{dateMysql}', '{day}', '{night}', '{id}', '{IdDay}', '{IdNight}');";
            }
            else if(IdNight == -1)
            {
                query = $"INSERT INTO `authorization`.`timework` (`Date`, `Day`, `idPeople`, `IdStateDay`) VALUES ('{dateMysql}', '{day}', '{id}', '{IdDay}');";
            }
            else if(IdDay == -1)
            {
                query = $"INSERT INTO `authorization`.`timework` (`Date`, `Night`, `idPeople`, `IdStateNight`) VALUES ('{dateMysql}', '{night}', '{id}', '{IdNight}');";
            }
            else
            {
                MessageBox.Show("Данные по атрибутам дня или ночи отсутствуют");
                return;
            }

            try
            {
                await mCon.OpenAsync();
                
                using (MySqlCommand command = new MySqlCommand(query, mCon))
                {

                    await command.ExecuteNonQueryAsync();
                    MessageBox.Show("Данные добавлены в базу данных");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { mCon.Close(); }
        }

        private async void buttonSubmit_Click(object sender, EventArgs e)
        {
            Worker worker = GetThisWorker();

            bool isCompliteParseDay = int.TryParse(textBoxDay.Text, out int dayInt);
            bool isCompliteParseNight = int.TryParse(textBoxNight.Text, out int nightInt);

            if(!isCompliteParseDay && !isCompliteParseNight)
            {
                MessageBox.Show("Пустые время день/ночь");
                return;
            }

            if(worker == null)
            {
                MessageBox.Show("Не найден работник для добавления");
                return;
            }

            int day = worker.GetDay(_datetime);
            int night = worker.GetNight(_datetime);

            if(dayInt == day && nightInt == night)
            {
                MessageBox.Show("Данные не изменились");
                return;
            }

            if(day == -1 && night == -1) 
            {
                await InsertDataAsync(dayInt, nightInt, _datetime, worker.Id);
            }
            else
            {
                int idDay = worker.GetId(_datetime);

                if(idDay != -1)
                {
                    await UpdateDataAsync(dayInt, nightInt, worker.Id, idDay);
                }
                else
                {
                    MessageBox.Show("Не удалось получить id дня");
                    return;
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // от 0 до 7 max;
            //эти значения передать уже куда-то для сбора idDay и idNight и после отпавлять
            //columnIndex 0/7
            //rowIndex = (строка которую выбрал пользователь и по которой надо идти и забирать данные;
            var f = dataGridView1.CurrentCell;
        }

        private void textBoxDayName_Click(object sender, EventArgs e)
        {
            States states = new States(true);
            states.ShowDialog();
            if(TextDay != null)
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
    }
}
