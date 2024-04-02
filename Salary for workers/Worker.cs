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

        public void AddDayWork(int id, DateTime date, int idDay, int day,string dayName, int idNight, int night, string nightName)
        {
            mounthWork.Add(new MounthWork(id ,date, idDay, day, dayName, idNight, night, nightName));
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

        public int GetDayId(DateTime date)
        {
            int dayId = -1;

            foreach (MounthWork item in mounthWork)
            {
                if(item.GetDate() == date)
                {
                    dayId = item.GetDayId();
                }
            }

            return dayId;
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

        public string GetDayAbbreviation(DateTime date)
        {
            string dayAbbreviation = "-1";

            foreach (MounthWork item in mounthWork)
            {
                if (item.GetDate() == date)
                {
                    dayAbbreviation = item.GetDayAbbreviation();
                }
            }

            return dayAbbreviation;
        }

        public int GetNightId(DateTime date)
        {
            int nightId = -1;

            foreach (MounthWork item in mounthWork)
            {
                if (item.GetDate() == date)
                {
                    nightId = item.GetNightId();
                }
            }

            return nightId;
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

        public string GetNightAbbreviation(DateTime date)
        {
            string nightAbbreviation = "-1";

            foreach (MounthWork item in mounthWork)
            {
                if (item.GetDate() == date)
                {
                    nightAbbreviation = item.GetNightAbbreviation();
                }
            }

            return nightAbbreviation;
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

        private int _idDay;
        private int _day = 0;
        private string _dayName;

        private int _idNight;
        private int _night = 0;
        private string _nightName;

        public MounthWork(int id ,DateTime date, int idDay, int day, string dayName, int idNight , int night, string nightName) 
        {
            _id = id;
            _date = date;
            _idDay = idDay;
            _day = day;
            _dayName = dayName;
            _idNight = idNight;
            _night = night;
            _nightName = nightName;
        }

        public int GetId() { return _id; }

        public DateTime GetDate() { return _date; }

        public int GetDayId() { return _idDay; }

        public int GetDay() { return _day; }

        public string GetDayAbbreviation() { return _dayName; }

        public int GetNightId() { return _idNight; }

        public int GetNight() { return _night; }

        public string GetNightAbbreviation() { return _nightName; }

        public void UpdateDay(int day) { _day = day; }
        
        public void UpdateNight(int night) { _night = night;}

        public void DeliteDay() { _day = 0; }
        
        public void DeliteNight() { _night = 0; }
    }
}
