using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Salary_for_workers
{
    public partial class MainForm : Form
    {
        private MySqlConnection _mCon;
        private readonly List<Worker> _workers;

        public MainForm(List<Worker> workers, MySqlConnection mCon)
        {
            _mCon = mCon;
            _workers = workers;
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            dateTimePicker1.CustomFormat = "MM-yyyy";
        }

        private void GetDataWorkers()
        {

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
    }
}
