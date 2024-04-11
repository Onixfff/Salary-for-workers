﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Salary_for_workers
{
    public partial class CreateUser : Form
    {
        private int _lastAddId;

        public static int IdPosition = -1;
        public static string TextPosition;
        public static int IdDepartment = -1;
        public static string TextDepartmemt;

        private MySqlConnection _mCon = new MySqlConnection(ConfigurationManager.ConnectionStrings["local"].ConnectionString);
        private CancellationTokenSource _cancellationTokenSource;

        public CreateUser()
        {
            InitializeComponent();
            dateTimePickerEmploymentDate.Value = DateTime.Now;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void textBoxPositions_Click(object sender, EventArgs e)
        {
            Position department = new Position();
            department.ShowDialog();

            if (TextPosition != null)
                textBoxPositions.Text = TextPosition;
        }

        private void textBoxDepartment_Click(object sender, EventArgs e)
        {
            Department department = new Department();
            department.ShowDialog();

            if (TextDepartmemt != null)
                textBoxDepartment.Text = TextDepartmemt;
        }

        private async void buttonSubmit_Click(object sender, EventArgs e)
        {
            if(textBoxName.Text.Trim().Length > 0 
                && textBoxSurname.Text.Trim().Length > 0 
                && textBoxPatronymic.Text.Trim().Length > 0 && IdDepartment != -1 && IdPosition != -1)
            {
                await InsertUser(_cancellationTokenSource.Token);
            }
            else
            {
                string pole = "Заполните поля";
                await Console.Out.WriteLineAsync(pole);
                MessageBox.Show(pole);
            }

        }

        private async Task InsertUser(CancellationToken cancellationToken)
        {
            string mysqlData = dateTimePickerEmploymentDate.Value.ToString("yyyy-MM-dd");

            string query = $"INSERT People(`Name`, `Surname`, `Patronymic`, `EmploymentDate`, `idPositions`, `idDepartment`) " +
                $"VALUES ('{textBoxName.Text}', '{textBoxSurname.Text}', '{textBoxPatronymic.Text}', '{mysqlData}', '{IdPosition}', '{IdDepartment}' ); SELECT last_insert_id();";

            try
            {
                await _mCon.OpenAsync();

                await Task.Run(async () =>
                {
                    using (MySqlCommand command = new MySqlCommand(query, _mCon))
                    {
                        using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                _lastAddId = reader.GetInt32(0);
                            }

                            MessageBox.Show("Данные добавлены в базу данных");
                            reader.Close();
                        }
                    }

                }, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { _mCon.Close(); }
        }

        private async Task UpdateUser(CancellationToken cancellationToken)
        {
            string mysqlData = dateTimePickerEmploymentDate.Value.ToString("yyyy-MM-dd");

            string query = $"Update people set name = '{textBoxName.Text}', Surname = '{textBoxSurname.Text}', Patronymic = '{textBoxPatronymic.Text}', EmploymentDate = '{mysqlData}', idPositions = '{IdPosition}', idDepartment = '{IdDepartment}' where id = '{_lastAddId}';";

            try
            {
                await _mCon.OpenAsync();

                await Task.Run(async () =>
                {
                    using (MySqlCommand command = new MySqlCommand(query, _mCon))
                    {
                        await command.ExecuteNonQueryAsync();
                        MessageBox.Show("Данные изменены");
                    }

                }, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { _mCon.Close(); }
        }

        private void CreateUser_FormClosing(object sender, FormClosingEventArgs e)
        {
            _cancellationTokenSource.Cancel();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await UpdateUser(_cancellationTokenSource.Token);
        }
    }
}
