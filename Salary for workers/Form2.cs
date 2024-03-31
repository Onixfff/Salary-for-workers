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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Salary_for_workers
{
    public partial class Form2 : Form
    {
        MySqlConnection mCon = new MySqlConnection(ConfigurationManager.ConnectionStrings["local"].ConnectionString);
        private List<Worker> _workers;

        public Form2(List<Worker> workers)
        {
            InitializeComponent();
            this._workers = workers;
        }

        private async void Form2_Load(object sender, EventArgs e)
        {
            UpdateComboBox();
            SetsFurstDayOfMonth();
            await TryGetDataAsync();
            ChengeToolTipDescription();
            toolTip1.SetToolTip(buttonSubmit, "");
        }

        private void SetsFurstDayOfMonth()
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            var startDay = new DateTime(year, month, 1);
            dateTimePicker1.Value = startDay;
        }

        private async Task<List<Worker>> TryGetDataAsync()
        {
            foreach (var worrker in _workers)
            {
                string query = "SELECT timework.Id, Date, day, night FROM authorization.timework cross join People " +
                    "where (name = @name and surname = @surname and Patronymic = @Patronymic and EmploymentDate = @EmploymentDate and idPeople =@idPeople)";

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
                                worrker.AddDayWork(reader.GetInt32(0), reader.GetDateTime(1), reader.GetInt32(2), reader.GetInt32(3));
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

                    day = worker.GetDay(date);
                    night = worker.GetNight(date);

                    if(day != -1)
                    {
                        textBoxDay.Text = day.ToString();
                    }
                    else
                    {
                        textBoxDay.Text = "";
                    }

                    if(night != -1)
                    {
                        textBoxNight.Text = night.ToString();
                    }
                    else
                    {
                        textBoxNight.Text = "";
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
            GetDayAndNightThisDate(comboBoxPeoples.Text, dateTimePicker1.Value);
        }

        private async void dateTimePicker1_ValueChangedAsync(object sender, EventArgs e)
        {
            _workers = await TryGetDataAsync();
            GetDayAndNightThisDate(comboBoxPeoples.Text, dateTimePicker1.Value);
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

            string query = $"INSERT INTO `authorization`.`timework` (`Date`, `Day`, `Night`, `idPeople`) VALUES ('{dateMysql}', '{day}', '{night}', '{id}');";

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

            int dayInt, nightInt;

            try
            {
                dayInt = Convert.ToInt32(textBoxDay.Text.Trim());
                nightInt = Convert.ToInt32(textBoxNight.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Неправельно записаны данные в поля 'день/ночь'");
                return;
            }

            if(worker == null)
            {
                MessageBox.Show("Не найден работник для добавления");
                return;
            }

            int day = worker.GetDay(dateTimePicker1.Value);
            int night = worker.GetNight(dateTimePicker1.Value);

            if(dayInt == day && nightInt == night)
            {
                MessageBox.Show("Данные не изменились");
                return;
            }

            if(day == -1 && night == -1) 
            {
                await InsertDataAsync(dayInt, nightInt, dateTimePicker1.Value, worker.Id);
            }
            else
            {
                int idDay = worker.GetId(dateTimePicker1.Value);

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
    }
}
