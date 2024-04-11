using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Salary_for_workers
{
    public partial class Autorization : Form
    {
        MySqlConnection mCon = new MySqlConnection(ConfigurationManager.ConnectionStrings["local"].ConnectionString);
        Ciphertar ciphertar = new Ciphertar();

        public Autorization()
        {
            InitializeComponent();
        }

        private async void Submit_Click(object sender, System.EventArgs e)
        {
            Submit.Enabled = false;
            string sql = ("SELECT password FROM passwords where login = @login");
            bool isCheckLoginInput = CheckInput(textBoxLogin.Text);
            string encryptedPassword, decryptedPassword;

            if (isCheckLoginInput)
            {
                try
                {
                    encryptedPassword = await CheckInDBData(sql, textBoxLogin.Text);

                    if (encryptedPassword != null)
                    {
                        //string ovter = ciphertar.EncryptsData(encryptedPassword);
                        decryptedPassword = await ciphertar.DecryptsDataAsync(encryptedPassword);

                        if (decryptedPassword == textBoxPassword.Text)
                        {
                            int idPosition = await GetIdPositionsAsync(textBoxLogin.Text, encryptedPassword);
                            if (idPosition != -1)
                            {
                                List<Worker> workers = await GetWorkersAsync(idPosition, textBoxLogin.Text, encryptedPassword);
                                if (workers.Count > 0)
                                {
                                    MainForm mainForm;

                                    if (textBoxLogin.Text == "" && textBoxPassword.Text == "")
                                        mainForm = new MainForm(workers, mCon, idPosition);
                                    else
                                        mainForm = new MainForm(workers, mCon, idPosition);

                                    this.Visible = false;
                                    mainForm.ShowDialog();
                                    this.Close();
                                }
                                else
                                {
                                    throw new DllNotFoundException();
                                }
                            }
                            else
                            {
                                throw new InRowChangingEventException();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Пароли не совпадают");
                        }
                    }
                }
                catch (InRowChangingEventException ex)
                {
                    MessageBox.Show("Ошибка входа #44515");
                    Console.WriteLine("Нету idPosition" + ex.Message);
                }
                catch (DllNotFoundException ex)
                {
                    MessageBox.Show("Ошибка входа #44520");
                    Console.WriteLine("Под вашим пользователем нету подчиненных" + ex.Message);
                }
                catch (AggregateException ex)
                {
                    MessageBox.Show("Ошибка входа #44525");
                    await Console.Out.WriteLineAsync(ex.Message);
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Ошибка входа #44530");
                    await Console.Out.WriteLineAsync(ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка входа #44535");
                    Console.WriteLine(ex);
                }
            }
            else
            {
                MessageBox.Show("Неправельно введены данные");
            }

            Submit.Enabled = true;
        }

        private bool CheckInput(string text)
        {
            var textTrim = text.Trim();

            if (textTrim.Length > 0 && text != null)
                return true;
            else
                return false;
        }

        private async Task<string> CheckInDBData(string query, string login)
        {
            string password = "null";

            try
            {
                using (MySqlCommand command = new MySqlCommand(query, mCon))
                {
                    await mCon.OpenAsync();
                    command.Parameters.AddWithValue("@Login", login);

                    object passwordObj = await command.ExecuteScalarAsync();
                    if (passwordObj != null)
                    {
                        password = Convert.ToString(passwordObj);
                    }
                    else
                    {
                        throw new AggregateException("Логин не был найден");
                    }
                }
            }
            catch (AggregateException ex)
            {
                throw new AggregateException("Логин не был найден" + ex);
            }
            catch (MySqlException ex)
            {
                throw new Exception("Ошибка при открытии бд \n" + ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка n" + ex);
            }
            finally
            {
                mCon.Close();
            }
            return password;
        }

        private async Task<int> GetIdPositionsAsync(string login, string password)
        {
            int id = -1;
            string query = $"SELECT idPositions FROM authorization.people left join positions on positions.id = people.idPositions left join passwords on passwords.id =  people.idPassword WHERE people.idPositions  = (select people.idPositions from people left join passwords on passwords.id = people.idPassword where (login = @login AND password = @password) limit 1) limit 1;";
            try
            {
                using (MySqlCommand command = new MySqlCommand(query, mCon))
                {
                    command.Parameters.AddWithValue($"@login", login);
                    command.Parameters.AddWithValue($"@password", password);

                    await mCon.OpenAsync();

                    await Task.Run(() =>
                    {
                        object resul = command.ExecuteScalar();

                        if (resul != null)
                        {
                            id = Convert.ToInt32(resul);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { mCon.Close(); }

            return id;
        }

        private async Task<List<Worker>> GetWorkersAsync(int idPosition, string login, string password)
        {
            List<Worker> workers = new List<Worker>();
            string query = "SELECT people.Id, people.Name, people.Surname, people.Patronymic, people.EmploymentDate FROM people left join passwords ON people.idPassword = passwords.id left join positions ON people.idPositions = positions.id WHERE positions.id = @idPositions;";

            try
            {
                using (MySqlCommand command = new MySqlCommand(query, mCon))
                {
                    command.Parameters.AddWithValue($"@idPositions", idPosition);

                    await mCon.OpenAsync();

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
            finally { mCon.Close(); }

            return workers;
        }
    }
}
