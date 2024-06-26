﻿using System;

namespace AttendanceTracking.View.Entities
{
    public class Attendens
    {
        public int StudentIndex { get; private set; }
        public int Day { get; private set; }
        public int Hours { get; private set; }
        private bool _isExcused;
        public bool IsExcused => _isExcused ? Hours > 0 : false;
        public Attendens(int rowIndex, int day, int hours, bool isExcused)
        {
            if (rowIndex < 0)
                throw new ArgumentException("rowIndex must be more zero");
            if (day < 0)
                throw new ArgumentException("day must be more zero");
            if (hours < 0)
                throw new ArgumentException("hours must be more zero");
            StudentIndex = rowIndex;
            Day = day;
            Hours = hours;
            _isExcused = isExcused;
        }
    }
}
