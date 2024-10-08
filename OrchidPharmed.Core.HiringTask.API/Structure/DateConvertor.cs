namespace OrchidPharmed.Core.HiringTask.API.Structure
{
    public static class DateConvertor
    {
        public class PersianDateTimeElements
        {
            public int Hour { get; set; }
            public int Minute { get; set; }
            public int Second { get; set; }
            public int Year { get; set; }
            public int Month { get; set; }
            public int Day { get; set; }
            public PersianDateTimeElements(DateTime date)
            {
                System.Globalization.PersianCalendar pc = new System.Globalization.PersianCalendar();
                Year = pc.GetYear(date);
                Month = pc.GetMonth(date);
                Day = pc.GetDayOfMonth(date);
                Hour = pc.GetHour(date);
                Minute = pc.GetMinute(date);
                Second = pc.GetSecond(date);
            }
            public PersianDateTimeElements(string date)
            {
                string txtdate, txttime;
                if (date.Contains(' '))
                {
                    txtdate = date.Split()[0];
                    txttime = date.Split()[1];
                }
                else
                {
                    txtdate = date;
                    txttime = null;
                }
                char schar = _GetDateTimeSeperator(txtdate);
                if (char.IsWhiteSpace(schar))
                {
                    txtdate = _TryConvertToLong(txtdate);
                    schar = '-';
                }
                var els = txtdate.Split(schar);
                if (els.Length != 3)
                    throw new Exception(string.Format("The input string datetime has invalid format \"{0}\"", date));
                Year = int.Parse(els[0]);
                Month = int.Parse(els[1]);
                Day = int.Parse(els[2]);
                if (string.IsNullOrEmpty(txttime))
                {
                    Hour = 0;
                    Minute = 0;
                    Second = 0;
                }
                else
                {
                    els = txttime.Split(':');
                    Hour = els[0] != null ? int.Parse(els[0]) : 0;
                    Minute = els[1] != null ? int.Parse(els[1]) : 0;
                    Second = els[2] != null ? int.Parse(els[2]) : 0;
                }
            }
            private string _TryConvertToLong(string date)
            {
                long l;
                if (!long.TryParse(date, out l))
                    throw new Exception(string.Format("The input string datetime has invalid letter \"{0}\"", date));
                else
                    return l.ToString("0000-00-00");
            }
            private char _GetDateTimeSeperator(string date)
            {
                char[] seps = "-/.".ToArray();
                char[] res = seps.Where(e => date.Contains(e)).ToArray();
                if (res.Length == 0)
                    return ' ';
                else
                {
                    if (res.Length > 1)
                        throw new Exception(string.Format("The input string datetime has more than 1 separator \"{0}\"", date));
                    else
                        return res.Single();
                }
            }
        }
        public class GregorianDateTimeElements
        {
            public int Hour { get; set; }
            public int Minute { get; set; }
            public int Second { get; set; }
            public int Year { get; set; }
            public int Month { get; set; }
            public int Day { get; set; }
            public GregorianDateTimeElements(DateTime date)
            {
                Year = date.Year;
                Month = date.Month;
                Day = date.Day;
                Hour = date.Hour;
                Minute = date.Minute;
                Second = date.Second;
            }
            public GregorianDateTimeElements(string date)
            {
                string txtdate, txttime;
                if (date.Contains(' '))
                {
                    txtdate = date.Split()[0];
                    txttime = date.Split()[1];
                }
                else
                {
                    txtdate = date;
                    txttime = null;
                }
                char schar = _GetDateTimeSeperator(txtdate);
                if (char.IsWhiteSpace(schar))
                {
                    txtdate = _TryConvertToLong(txtdate);
                    schar = '-';
                }
                var els = txtdate.Split(schar);
                if (els.Length != 3)
                    throw new Exception(string.Format("The input string datetime has invalid format \"{0}\"", date));
                Year = int.Parse(els[0]);
                Month = int.Parse(els[1]);
                Day = int.Parse(els[2]);
                if (string.IsNullOrEmpty(txttime))
                {
                    Hour = 0;
                    Minute = 0;
                    Second = 0;
                }
                else
                {
                    els = txttime.Split(':');
                    Hour = els[0] != null ? int.Parse(els[0]) : 0;
                    Minute = els.Length > 1 ? int.Parse(els[1]) : 0;
                    Second = els.Length > 2 ? int.Parse(els[2]) : 0;
                }
            }
            private string _TryConvertToLong(string date)
            {
                long l;
                if (!long.TryParse(date, out l))
                    throw new Exception(string.Format("The input string datetime has invalid letter \"{0}\"", date));
                else
                    return l.ToString("0000-00-00");
            }
            private char _GetDateTimeSeperator(string date)
            {
                char[] seps = "-/.".ToArray();
                char[] res = seps.Where(e => date.Contains(e)).ToArray();
                if (res.Length == 0)
                    return ' ';
                else
                {
                    if (res.Length > 1)
                        throw new Exception(string.Format("The input string datetime has more than 1 separator \"{0}\"", date));
                    else
                        return res.Single();
                }
            }
        }
        public static string ToPersianDateTime(this DateTime date)
        {
            var pd = new PersianDateTimeElements(date);
            return string.Format("{3}/{4}/{5} {0}:{1}:{2}", _GetDatepartString(pd.Hour),
                                                            _GetDatepartString(pd.Minute),
                                                            _GetDatepartString(pd.Second),
                                                            pd.Year,
                                                            _GetDatepartString(pd.Month),
                                                            _GetDatepartString(pd.Day));
        }
        public static string ToGregorianDateTime(this DateTime date)
        {
            return string.Format("{0}/{1}/{2} {3}:{4}:{5}",
                                                              date.Year,
                                                            _GetDatepartString(date.Month),
                                                            _GetDatepartString(date.Day),
                                                            _GetDatepartString(date.Hour),
                                                            _GetDatepartString(date.Minute),
                                                            _GetDatepartString(date.Second)
                                                           );
        }
        public static DateTime FromPersianStringDate(this string date)
        {
            var pd = new PersianDateTimeElements(date);
            var pc = new System.Globalization.PersianCalendar();
            return new DateTime(pd.Year, pd.Month, pd.Day, pd.Hour, pd.Minute, pd.Second, pc);
        }
        public static DateTime FromGregorianStringDate(this string date)
        {
            var pd = new GregorianDateTimeElements(date);
            return new DateTime(pd.Year, pd.Month, pd.Day, pd.Hour, pd.Minute, pd.Second);
        }
        public static string ToTimeString(this DateTime date)
        {
            var pd = new PersianDateTimeElements(date);
            return string.Format("{0}:{1}:{2}", _GetDatepartString(pd.Hour),
                                                            _GetDatepartString(pd.Minute),
                                                            _GetDatepartString(pd.Second));
        }
        public static string ToPersianDate(this DateTime date)
        {
            var pd = new PersianDateTimeElements(date);
            return string.Format("{0}/{1}/{2}", pd.Year,
                                                            _GetDatepartString(pd.Month),
                                                            _GetDatepartString(pd.Day));
        }
        public static string ToGreGorianDate(this DateTime date)
        {
            return string.Format("{0}/{1}/{2}", date.Year,
                                                            _GetDatepartString(date.Month),
                                                            _GetDatepartString(date.Day));
        }
        private static string _GetDatepartString(int i)
        {
            return i.ToString().PadLeft(2, '0');
        }
    }
}
