using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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

        public void AddDayWork(int id, DateTime date, int? idDay, int? day, string dayName, int? idNight, int? night, string nightName)
        {
            mounthWork.Add(new MounthWork(id, date, idDay, day, dayName, idNight, night, nightName));
        }

        public int GetId(DateTime date)
        {
            int id = -1;

            foreach (var item in mounthWork)
            {
                if (item.GetDate() == date)
                {
                    id = item.GetId();
                }
            }

            return id;
        }

        public int? GetDayId(DateTime date)
        {
            int? dayId = null;

            foreach (MounthWork item in mounthWork)
            {
                if (item.GetDate() == date)
                {
                    dayId = item.GetDayId();
                    break;
                }
            }

            return dayId;
        }

        public int? GetDay(DateTime date)
        {
            int? day = null;

            foreach (MounthWork item in mounthWork)
            {
                if (item.GetDate() == date)
                {
                    day = item.GetDay();
                    break;
                }
            }

            return day;
        }

        public string GetDayAbbreviation(DateTime date)
        {
            string dayAbbreviation = null;

            foreach (MounthWork item in mounthWork)
            {
                if (item.GetDate() == date)
                {
                    dayAbbreviation = item.GetDayAbbreviation();
                    break;
                }
            }

            return dayAbbreviation;
        }

        public int? GetNightId(DateTime date)
        {
            int? nightId = null;

            foreach (MounthWork item in mounthWork)
            {
                if (item.GetDate() == date)
                {
                    nightId = item.GetNightId();
                    break;
                }
            }

            return nightId;
        }

        public int? GetNight(DateTime date)
        {
            int? night = null;

            foreach (MounthWork item in mounthWork)
            {
                if (item.GetDate() == date)
                {
                    night = item.GetNight();
                    break;
                }
            }

            return night;
        }

        public string GetNightAbbreviation(DateTime date)
        {
            string nightAbbreviation = null;

            foreach (MounthWork item in mounthWork)
            {
                if (item.GetDate() == date)
                {
                    nightAbbreviation = item.GetNightAbbreviation();
                    break;
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

        public void ClearListDayWork()
        {
            mounthWork.Clear();
        }
    }
}