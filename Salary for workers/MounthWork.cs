using System;

namespace Salary_for_workers
{
    public class MounthWork
    {
        private int _id;
        private DateTime _date;

        private int? _idDay;
        private int? _day = 0;
        private string _dayName;

        private int? _idNight;
        private int? _night = 0;
        private string _nightName;

        public MounthWork(int id, DateTime date, int? idDay, int? day, string dayName, int? idNight, int? night, string nightName)
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

        public int? GetDayId() { return _idDay; }

        public int? GetDay() 
        {
            return _day; 
        }

        public string GetDayAbbreviation() { return _dayName; }

        public int? GetNightId() { return _idNight; }

        public int? GetNight() 
        { 
            return _night;
        }

        public string GetNightAbbreviation() { return _nightName; }

        public void UpdateDay(int day) { _day = day; }

        public void UpdateNight(int night) { _night = night; }

        public void DeliteDay() { _day = 0; }

        public void DeliteNight() { _night = 0; }
    }
}
