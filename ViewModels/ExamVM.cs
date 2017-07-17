using System;
using AvansApp.Models.ServerModels;

namespace AvansApp.ViewModels
{
    public class ExamVM
    {
        private Exam _exam;
        
        public ExamVM()
        {
            _exam = new Exam();
        }
        public ExamVM(Exam e)
        {
            _exam = e;
        }
        
        public string StudentExamScheduleID
        {
            get { return _exam.TentamenRoosterStudentID; }
            set { _exam.TentamenRoosterStudentID = value; }
        }
        public int StudentNumber
        {
            get { return _exam.studentnummer; }
            set { _exam.studentnummer = value; }
        }
        public string StudentName
        {
            get { return _exam.naam; }
            set { _exam.naam = value; }
        }
        public string Course
        {
            get { return _exam.cursus; }
            set { _exam.cursus = value; }
        }
        public string ExamDescription
        {
            get { return _exam.Toetsomschrijving; }
            set { _exam.Toetsomschrijving = value; }
        }
        public DateTime DateTime
        {
            get { return _exam.sortbegin; }
            set { _exam.sortbegin = value; }
        }
        public string StartDate
        {
            get { return _exam.Begindatum; }
            set { _exam.Begindatum = value; }
        }
        public string StartTime
        {
            get { return _exam.Begintijd; }
            set { _exam.Begintijd = value; }
        }
        public string Deadline
        {
            get { return _exam.Einddatum; }
            set { _exam.Einddatum = value; }
        }
        public string Classroom
        {
            get { return _exam.Lokaal; }
            set { _exam.Lokaal = value; }
        }
        public string Location
        {
            get { return _exam.Locatie; }
            set { _exam.Locatie = value; }
        }
    }
}
