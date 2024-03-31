using Mysqlx.Crud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Salary_for_workers
{
    public class Worker
    {
        public readonly int Id;
        public readonly string Name;
        public readonly string Surname;
        public readonly string Patronymic;
        public readonly DateTime EmploymentDate;
        private List<MounthWork> mounthWork;

        public List<MounthWork> GetMounthWork() => mounthWork;

        public Worker(int id, string name, string surname, string patronymic, DateTime emplomentDate)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Patronymic = patronymic;
            EmploymentDate = emplomentDate;
            mounthWork = new List<MounthWork>();
        }

        public void AddDayWork(int id, DateTime date, int day, int night)
        {
            mounthWork.Add(new MounthWork(id ,date, day, night));
        }

        public int GetId(DateTime date)
        {
            int id = -1;

            foreach (var item in mounthWork)
            {
                if(item.GetDate() == date)
                {
                    id = item.GetId();
                }
            }

            return id;
        }

        public int GetDay(DateTime date)
        {
            int day = -1;

            foreach (MounthWork item in mounthWork)
            {
                if(item.GetDate() == date)
                {
                    day = item.GetDay();
                }
            }

            return day;
        }

        public int GetNight(DateTime date)
        {
            int night = -1;

            foreach (MounthWork item in mounthWork)
            {
                if (item.GetDate() == date)
                {
                    night = item.GetNight();
                }
            }

            return night;
        }

        public bool UpdateDay(DateTime date, int day)
        {
            bool update = false;

            foreach (MounthWork item in mounthWork)
            {
                if (item.GetDate() == date)
                {
                    item.UpdateNight(day);
                    update = true;
                }
            }

            return update;
        }

        public bool UpdateNight(DateTime date, int night)
        {
            bool update = false;

            foreach (MounthWork item in mounthWork)
            {
                if (item.GetDate() == date)
                {
                    item.UpdateNight(night);
                    update = true;
                }
            }

            return update;
        }

        public bool DeliteDay(DateTime date)
        {
            bool delite = false;

            foreach (MounthWork item in mounthWork)
            {
                if (item.GetDate() == date)
                {
                    item.DeliteDay();
                    delite = true;
                }
            }

            return delite;
        }

        public bool DeliteNight(DateTime date)
        {
            bool delite = false;

            foreach (MounthWork item in mounthWork)
            {
                if (item.GetDate() == date)
                {
                    item.DeliteNight();
                    delite = true;
                }
            }

            return delite;
        }
    }

    public class MounthWork
    {
        private int _id;
        private DateTime _date;
        private int _day = 0;
        private int _night = 0;

        public MounthWork(int id ,DateTime date, int day, int night) 
        {
            _id = id;
            _date = date;
            _day = day;
            _night = night;
        }

        public int GetId() { return _id; }

        public DateTime GetDate() { return _date; }

        public int GetDay() { return _day; }
        
        public int GetNight() { return _night; }

        public void UpdateDay(int day) { _day = day; }
        
        public void UpdateNight(int night) { _night = night;}

        public void DeliteDay() { _day = 0; }
        
        public void DeliteNight() { _night = 0; }
    }
}
