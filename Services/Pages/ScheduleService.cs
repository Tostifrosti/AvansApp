using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using AvansApp.Models;
using AvansApp.ViewModels;
using AvansApp.Models.Enums;
using AvansApp.Models.ServerModels;

namespace AvansApp.Services.Pages
{
    public class ScheduleService
    {
        private const string base_url = "https://publicapi.avans.nl/oauth";
        private static List<List<ScheduleVM>> ScheduleWithBlanks { get; set; }
        private static List<List<ScheduleVM>> ScheduleWithoutBlanks { get; set; }
        public int TodayIndex { get; private set; }
        
        public ScheduleService()
        {
        }

        /*
            INPUT:
            type: G, D of R voor resp. een groeprooster, docentrooster of ruimterooster
            schedulecode: resp. de groepcode, docentcode of ruimtecode waarvoor het rooster opgehaald moet worden
            start: datum vanaf, in nl - volgorde met - (bv. 14 - 9 - 2011) of us-volgorde met / (bv. 9 / 14 / 2011)
            end: datum tot en met, in nl - volgorde met - (bv. 20 - 9 - 2011) of us-volgorde met / (bv. 9 / 20 / 2011)
            NOTE: ook UTC-strings en andere ENGELSE tekstuele datumrepresentaties worden geaccepteerd(specifically: any format recognized by PHP strtotime function) 
        */
        public async Task<List<List<ScheduleVM>>> GetSchedule(ScheduleType type, string schedulecode, DateTime startdate, DateTime enddate, bool blanks)
        {
            if (blanks && ScheduleWithBlanks != null)
                return ScheduleWithBlanks;
            if (!blanks && ScheduleWithoutBlanks != null)
                return ScheduleWithoutBlanks;

            ScheduleWithBlanks = new List<List<ScheduleVM>>();
            ScheduleWithoutBlanks = new List<List<ScheduleVM>>();
            
            List<List<ScheduleVM>> scheduleList = new List<List<ScheduleVM>>();

            string startdateString = startdate.Day + "-" + startdate.Month + "-" + startdate.Year;
            string enddateString = enddate.Day + "-" + enddate.Month + "-" + enddate.Year;
            
            List<Schedule> data = await OAuth.GetInstance().RequestJSON<List<Schedule>>(base_url + "/rooster/v2/", new List<string>() { "type=" + GetScheduleType(type), "param=" + schedulecode, "start=" + startdateString, "end=" + enddateString }, HttpMethod.GET);


            if (data != null && data.Count > 0)
            {
                List<ScheduleVM> day = new List<ScheduleVM>();
                TodayIndex = 0;

                foreach (Schedule s in data)
                {
                    if (day.Count > 0 && day[day.Count - 1].Datum != s.datum)
                    {
                        day = day.OrderBy(d => d.StartTijd).ToList();
                        scheduleList.Add(day);
                        day = new List<ScheduleVM>();
                    }
                    else
                    {
                        day.Add(new ScheduleVM(s));
                    }
                }
            }

            // Set Schedule without blanks
            foreach(var item in scheduleList)
            {
                ScheduleWithoutBlanks.Add(item);
            }
             
            // Fill in the blanks
            for (int i=0; i < scheduleList.Count; i++)
            {
                if (scheduleList.Count > 0)
                {
                    if (scheduleList[i].Count > 0 && scheduleList[i - 1].Count > 0 && 
                        scheduleList[i][0].Datum > scheduleList[i - 1][0].Datum)
                    {
                        FillBlanks(ref scheduleList, scheduleList[i][0].Datum, scheduleList[i - 1][0].Datum, i); 
                    }
                }
            }

            // If the last date is still in the past
            if (scheduleList.Count > 0 && scheduleList[scheduleList.Count - 1].Count > 0 &&
                scheduleList[scheduleList.Count-1][0].Datum < enddate)
            {
                FillBlanks(ref scheduleList, scheduleList[scheduleList.Count - 1][0].Datum, enddate, (scheduleList.Count-1));
            }
            if (scheduleList.Count == 0)
            {
                FillBlanks(ref scheduleList, startdate, enddate, 0);
            }

            // Find index of Today
            var today = DateTime.Now;
            for (int i = 0; i < scheduleList.Count; i++)
            {
                if (scheduleList[i].Count > 0)
                {
                    if (scheduleList[i][0].Datum.Year == today.Year &&
                        scheduleList[i][0].Datum.Month == today.Month &&
                        scheduleList[i][0].Datum.Day == today.Day)
                    {
                        TodayIndex = i;
                        break;
                    }
                }
            }

            // Set Schedule with blanks
            foreach (var item in scheduleList)
            {
                ScheduleWithBlanks.Add(item);
            }

            return blanks ? ScheduleWithBlanks : ScheduleWithoutBlanks;
        }

        private void FillBlanks(ref List<List<ScheduleVM>> schedule, DateTime start, DateTime end, int index)
        {
            int daysInBetween = 0;
            var diff = (end - start);
            daysInBetween = (int)Math.Ceiling(diff.TotalDays);

            if (daysInBetween > 1)
            {
                if (schedule.Count == 0)
                {
                    Schedule s = new Schedule() { id = -1, datum = start };
                    schedule.Add(new List<ScheduleVM> { new ScheduleVM(s) });
                }
                for (int i = 1; i < daysInBetween; i++) // In between, so not the first or last date
                {
                    Schedule s = new Schedule() { id = -1, datum = start.AddDays(i) };
                    schedule.Insert(index + i, new List<ScheduleVM> { new ScheduleVM(s) });
                }
            }
        }

        private string GetScheduleType(ScheduleType t)
        {
            string type;
            switch (t)
            {
                case ScheduleType.Classroom:
                    type = "R"; // Ruimterooster
                    break;
                case ScheduleType.Group:
                    type = "G"; // Groeprooster
                    break;
                case ScheduleType.Teacher:
                    type = "D"; // Docentenrooster
                    break;
                default:
                    type = "G";
                    break;
            }
            return type;
        }


        //////////////////////////////////////////
        /*private async void GetScheduleWithBlanks(string param)
        {
            List<Schedule> result = await OAuth.GetInstance().RequestJSON<List<Schedule>>("https://publicapi.avans.nl/oauth/rooster/v2/" + param, "GET");
            if (result != null && result.Count > 0)
            {
                CurrentDay = new List<ScheduleVM>();
                List<ScheduleVM> day = new List<ScheduleVM>();
                currentDayIndex = 0;
                todayIndex = 0;

                foreach (Schedule r in result)
                {
                    // If current date isnt the same as previous date, FILL IN THE BLANKS!
                    if (day.Count > 0 && day[day.Count - 1].Datum != r.datum && ((int)Math.Ceiling((r.datum - day[day.Count - 1].Datum).TotalDays)) > 1)
                    {
                        int daysInBetween = 0;
                        var diff = (r.datum - day[day.Count - 1].Datum);
                        daysInBetween = (int)Math.Ceiling(diff.TotalDays);

                        DateTime lastDay = day[day.Count - 1].Datum;
                        if (day.Count > 0)
                        {
                            ScheduleList.Add(day);
                            day = new List<ScheduleVM>();
                        }

                        if (daysInBetween > 1)
                        {
                            for (int i = 1; i < daysInBetween; i++) // In between, so not the first or last date
                            {
                                DateTime d = lastDay.AddDays(i);
                                Schedule s = new Schedule() { id = -1, datum = d };

                                // check if today is that day
                                if ((s.datum.Year == DateTime.Now.Year &&
                                    s.datum.Month == DateTime.Now.Month &&
                                    s.datum.Day == DateTime.Now.Day)
                                    && currentDayIndex == 0)
                                {
                                    currentDayIndex = ScheduleList.Count;
                                    todayIndex = currentDayIndex;
                                }

                                ScheduleList.Add(new List<ScheduleVM>() { new ScheduleVM(s) });
                            }
                            // Add last day (and check if today is that day)
                            if ((r.datum.Year == DateTime.Now.Year &&
                                    r.datum.Month == DateTime.Now.Month &&
                                    r.datum.Day == DateTime.Now.Day)
                                    && currentDayIndex == 0)
                            {
                                currentDayIndex = ScheduleList.Count;
                                todayIndex = currentDayIndex;
                            }
                            day.Add(new ScheduleVM(r));
                        }
                    }
                    // day is empty OR if the day is the same
                    else if (day.Count <= 0 || day[day.Count - 1].Datum == r.datum)
                    {
                        if ((r.datum.Year == DateTime.Now.Year &&
                            r.datum.Month == DateTime.Now.Month &&
                            r.datum.Day == DateTime.Now.Day)
                            && currentDayIndex == 0)
                        {
                            currentDayIndex = ScheduleList.Count;
                            todayIndex = currentDayIndex;
                        }
                        day.Add(new ScheduleVM(r));
                    }
                    // new start, new day
                    else if (day.Count > 0)
                    {
                        day = day.OrderBy(s => s.StartTijd).ToList();
                        ScheduleList.Add(day);

                        if ((r.datum.Year == DateTime.Now.Year &&
                            r.datum.Month == DateTime.Now.Month &&
                            r.datum.Day == DateTime.Now.Day)
                            && currentDayIndex == 0)
                        {
                            currentDayIndex = ScheduleList.Count;
                            todayIndex = currentDayIndex;
                        }

                        day = new List<ScheduleVM>();
                        day.Add(new ScheduleVM(r));
                    }

                    // if the last course is still back in the day -> Fill in till today..
                    if (result[result.Count - 1] == r)
                    {
                        if (day.Count > 0) // Last check of last course on the last day
                        {
                            ScheduleList.Add(day);
                            day = new List<ScheduleVM>();
                        }
                        DateTime next = DateTime.Now.AddMonths(3);
                        int daysInBetweenTillNext = 0;
                        var diffTillNext = (next - r.datum);
                        daysInBetweenTillNext = (int)Math.Ceiling(diffTillNext.TotalDays);
                        if (daysInBetweenTillNext > 1)
                        {
                            for (int i = 1; i < daysInBetweenTillNext; i++) // In between, so not the first or last date
                            {
                                DateTime d = r.datum.AddDays(i);
                                Schedule s = new Schedule() { id = -1, datum = d };

                                if ((s.datum.Year == DateTime.Now.Year &&
                                    s.datum.Month == DateTime.Now.Month &&
                                    s.datum.Day == DateTime.Now.Day)
                                    && currentDayIndex == 0)
                                {
                                    currentDayIndex = ScheduleList.Count;
                                    todayIndex = currentDayIndex;
                                }

                                ScheduleList.Add(new List<ScheduleVM>() { new ScheduleVM(s) });
                            }
                        }
                    }
                }


                CurrentDay = ScheduleList[currentDayIndex]; // Today
                ScheduleListView.ItemsSource = CurrentDay;

                //PreviousDayButton.IsEnabled = true;
                //NextDayButton.IsEnabled = true;
                //TodayButton.IsEnabled = true;

                //main.PageTitle = "Rooster";
                SetHeader();
                SetCurrentDay();
            }
            else
            {
                todayIndex = 0;
                currentDayIndex = 0;
                ScheduleList = new List<List<ScheduleVM>>();
                CurrentDay = new List<ScheduleVM>();

                PreviousDayButton.IsEnabled = false;
                NextDayButton.IsEnabled = false;
                TodayButton.IsEnabled = false;

                SetHeader();

                SetCurrentDay();
            }
        }
        private async void GetScheduleWithoutBlanks(string param)
        {
            List<Schedule> result = await OAuth.GetInstance().RequestJSON<List<Schedule>>("https://publicapi.avans.nl/oauth/rooster/v2/" + param, "GET");
            if (result != null && result.Count > 0)
            {
                CurrentDay = new List<ScheduleVM>();
                List<ScheduleVM> day = new List<ScheduleVM>();
                currentDayIndex = 0;
                todayIndex = 0;

                foreach (Schedule r in result)
                {
                    // If current date isnt the same as previous date, FILL IN THE BLANKS!
                    if (day.Count > 0 && day[day.Count - 1].Datum != r.datum && ((int)Math.Ceiling((r.datum - day[day.Count - 1].Datum).TotalDays)) > 1)
                    {
                        int daysInBetween = 0;
                        var diff = (r.datum - day[day.Count - 1].Datum);
                        daysInBetween = (int)Math.Ceiling(diff.TotalDays);

                        DateTime lastDay = day[day.Count - 1].Datum;
                        if (day.Count > 0)
                        {
                            ScheduleList.Add(day);
                            day = new List<ScheduleVM>();
                        }

                        if (daysInBetween > 1)
                        {
                            // Add last day (and check if today is that day)
                            if ((r.datum.Year >= DateTime.Now.Year &&
                                r.datum.Month >= DateTime.Now.Month &&
                                r.datum.Day >= DateTime.Now.Day)
                                && currentDayIndex == 0)
                            {
                                currentDayIndex = ScheduleList.Count;
                                todayIndex = currentDayIndex;
                            }
                            day.Add(new ScheduleVM(r));
                        }
                    }
                    // day is empty OR if the day is the same
                    else if (day.Count <= 0 || day[day.Count - 1].Datum == r.datum)
                    {
                        if ((r.datum.Year >= DateTime.Now.Year &&
                            r.datum.Month >= DateTime.Now.Month &&
                            r.datum.Day >= DateTime.Now.Day)
                            && currentDayIndex == 0)
                        {
                            currentDayIndex = ScheduleList.Count;
                            todayIndex = currentDayIndex;
                        }
                        day.Add(new ScheduleVM(r));
                    }
                    // new start, new day
                    else if (day.Count > 0)
                    {
                        day = day.OrderBy(s => s.StartTijd).ToList();
                        ScheduleList.Add(day);

                        if ((r.datum.Year >= DateTime.Now.Year &&
                            r.datum.Month >= DateTime.Now.Month &&
                            r.datum.Day >= DateTime.Now.Day)
                            && currentDayIndex == 0)
                        {
                            currentDayIndex = ScheduleList.Count;
                            todayIndex = currentDayIndex;
                        }

                        day = new List<ScheduleVM>();
                        day.Add(new ScheduleVM(r));
                    }

                    // if the last course is still back in the day -> Fill in till today..
                    if (result[result.Count - 1] == r)
                    {
                        if (day.Count > 0) // Last check of last course on the last day
                        {
                            if (currentDayIndex == 0)
                            {
                                currentDayIndex = ScheduleList.Count;
                                todayIndex = currentDayIndex;
                            }

                            ScheduleList.Add(day);
                            day = new List<ScheduleVM>();
                        }
                    }
                }


                CurrentDay = ScheduleList[currentDayIndex]; // Today
                ScheduleListView.ItemsSource = CurrentDay;

                //PreviousDayButton.IsEnabled = true;
                //NextDayButton.IsEnabled = true;
                //TodayButton.IsEnabled = true;

                //main.PageTitle = "Rooster";
                SetHeader();
                SetCurrentDay();
            }
            else
            {
                todayIndex = 0;
                currentDayIndex = 0;
                ScheduleList = new List<List<ScheduleVM>>();
                CurrentDay = new List<ScheduleVM>();

                //PreviousDayButton.IsEnabled = false;
                //NextDayButton.IsEnabled = false;
                //TodayButton.IsEnabled = false;

                //SetHeader();

                //SetCurrentDay();
            }
        }*/


    }
}
