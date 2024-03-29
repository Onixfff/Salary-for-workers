﻿using MySql.Data.MySqlClient;
using System;
using System.Collections;
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
                string query = "SELECT Date, day, night FROM authorization.timework cross join People " +
                    "where (name = @name and surname = @surname and Patronymic = @Patronymic and EmploymentDate = @EmploymentDate)";

                try
                {
                    using (MySqlCommand command = new MySqlCommand(query, mCon))
                    {
                        command.Parameters.AddWithValue($"@name", worrker.Name);
                        command.Parameters.AddWithValue($"@surname", worrker.Surname);
                        command.Parameters.AddWithValue($"@Patronymic", worrker.Patronymic);
                        command.Parameters.AddWithValue($"@EmploymentDate", worrker.EmploymentDate);

                        await mCon.OpenAsync();

                        using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                worrker.AddDayWork(reader.GetDateTime(0), reader.GetInt32(1), reader.GetInt32(2));
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

                    if(night != -1)
                    {
                        textBoxNight.Text = night.ToString();
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
            GetDayAndNightThisDate(comboBoxPeoples.Text, dateTimePicker1.Value);
            _workers = await TryGetDataAsync();
        }

        private async void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            GetDayAndNightThisDate(comboBoxPeoples.Text, dateTimePicker1.Value);
            _workers = await TryGetDataAsync();
        }
    }
}
