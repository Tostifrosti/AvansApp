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
                    if (i > 0 && scheduleList[i].Count > 0 && scheduleList[i - 1].Count > 0 && 
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
            string type = string.Empty;
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

        public void EmptySchedule()
        {
            ScheduleWithBlanks = null;
            ScheduleWithoutBlanks = null;
            TodayIndex = 0;
        }

    }
}
