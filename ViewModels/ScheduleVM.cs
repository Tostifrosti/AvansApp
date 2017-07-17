using System;
using AvansApp.Models.ServerModels;

namespace AvansApp.ViewModels
{
    public class ScheduleVM
    {
        private Schedule _schedule;

        public ScheduleVM()
        {
            _schedule = new Schedule();
        }
        public ScheduleVM(Schedule s)
        {
            _schedule = s;
        }
        
        public int Id
        {
            get { return _schedule.id; }
            set { _schedule.id = value; }
        }
        public DateTime StartTijd
        {
            get { return _schedule.start; }
            set { _schedule.start = value; }
        }
        public DateTime EindTijd
        {
            get { return _schedule.end; }
            set { _schedule.end = value; }
        }
        public string StartTijdString
        {
            get { return _schedule.starttijd; }
            set { _schedule.starttijd = value; }
        }
        public string EindTijdString
        {
            get { return _schedule.eindtijd; }
            set { _schedule.eindtijd = value; }
        }
        public string Vak
        {
            get { return _schedule.vak; }
            set { _schedule.vak = value; }
        }
        public string Ruimte
        {
            get { return _schedule.ruimte; }
            set { _schedule.ruimte = value; }
        }
        public string Groep
        {
            get { return _schedule.groep; }
            set { _schedule.groep = value; }
        }
        public string Docent
        {
            get { return _schedule.docent; }
            set { _schedule.docent = value; }
        }
        public string Details
        {
            get { return _schedule.details; }
            set { _schedule.details = value; }
        }
        public string Uurlabel
        {
            get { return _schedule.uurlabel; }
            set { _schedule.uurlabel = value; }
        }
        public string Weeklabel
        {
            get { return _schedule.weeklabel; }
            set { _schedule.weeklabel = value; }
        }
        public DateTime Datum
        {
            get { return _schedule.datum; }
            set { _schedule.datum = value; }
        }
    }
}
