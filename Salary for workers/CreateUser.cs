using MySql.Data.MySqlClient;
using MySql.Data.Types;
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
        private int _id;
        private bool isUpdate = false;

        public static int? IdPosition = -1;
        public static string TextPosition;
        public static int? IdDepartment = -1;
        public static string TextDepartmemt;

        private MySqlConnection _mCon = new MySqlConnection(ConfigurationManager.ConnectionStrings["local"].ConnectionString);
        private CancellationTokenSource _cancellationTokenSource;

        public CreateUser()
        {
            InitializeComponent();
            dateTimePickerEmploymentDate.Value = DateTime.Now;
            _cancellationTokenSource = new CancellationTokenSource();
            isUpdate = false;
            ChangeVisiblButton();
        }

        public CreateUser(int id)
        {
            InitializeComponent();
            _id = id;
            isUpdate = true;
            _cancellationTokenSource = new CancellationTokenSource();
            ChangeVisiblButton();
            GetData();
        }

        private void ChangeVisiblButton()
        {
            if (isUpdate)
            {
                buttonUpdate.Visible = true;
                buttonSubmit.Visible = false;
            }
            else
            {
                buttonSubmit.Visible = true;
                buttonUpdate.Visible = false;
            }
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
                $"VALUES ('{textBoxName.Text}', '{textBoxSurname.Text}', '{textBoxPatronymic.Text}', '{mysqlData}', '{IdPosition}', '{IdDepartment}' );";

            try
            {
                await _mCon.OpenAsync();

                await Task.Run(async () =>
                {
                    using (MySqlCommand command = new MySqlCommand(query, _mCon))
                    {
                        await command.ExecuteNonQueryAsync();
                        MessageBox.Show("Данные добавлены в базу данных");
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

            string query = $"Update people set name = '{textBoxName.Text}', Surname = '{textBoxSurname.Text}', Patronymic = '{textBoxPatronymic.Text}', EmploymentDate = '{mysqlData}', idPositions = '{IdPosition}', idDepartment = '{IdDepartment}' where id = '{_id}';";

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

        private void ChangeText(string name, string surname, string patronymic, MySqlDateTime? date)
        {
            textBoxName.Text = name;
            textBoxSurname.Text = surname;
            textBoxPatronymic.Text = patronymic;
            if(date != null)
            {
                dateTimePickerEmploymentDate.Value = (DateTime)date.Value;
            }

            textBoxDepartment.Text = TextDepartmemt;
            textBoxPositions.Text = TextPosition;
        }

        private async void GetData()
        {
            string query = $"SELECT people.name, Surname, Patronymic, EmploymentDate, idPositions, idDepartment, positions.name, department.Name FROM authorization.people left join positions on positions.id = people.idPositions left join department on department.id = people.idDepartment where (people.id = '{_id}')";

            string name = null;
            string surname = null;
            string patronymic = null;
            MySqlDateTime? date = null;

            try
            {
                if (_mCon.State == ConnectionState.Closed)
                    await _mCon.OpenAsync();


                using (MySqlCommand command = new MySqlCommand(query, _mCon))
                {

                    await Task.Run(async () =>
                    {
                        using (MySqlDataReader reader = (MySqlDataReader)await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                name = reader.GetString(0);
                                surname = reader.GetString(1);
                                patronymic = reader.GetString(2);
                                date = reader.GetMySqlDateTime(3);
                                IdPosition = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4);
                                IdDepartment = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5);
                                TextPosition = reader.IsDBNull(6) ? null : reader.GetString(6);
                                TextDepartmemt = reader.IsDBNull(7) ? null : reader.GetString(7);

                            }

                            reader.Close();
                        }
                    });
                }

                ChangeText(name, surname, patronymic, date);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Ошибка чтения данных");
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (_mCon.State == ConnectionState.Open)
                    _mCon.Close();
            }
        }
    }
}
