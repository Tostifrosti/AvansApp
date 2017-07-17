using System;
using AvansApp.Models.ServerModels;

namespace AvansApp.ViewModels
{
    public class ClassroomAvailabilityVM
    {
        private ClassroomAvailability _classroomAvailability;

        public ClassroomAvailabilityVM()
        {
            _classroomAvailability = new ClassroomAvailability();
        }
        public ClassroomAvailabilityVM(ClassroomAvailability ca)
        {
            _classroomAvailability = ca;
        }

        public DateTime DateTime
        {
            get { return _classroomAvailability.datum; }
            set { _classroomAvailability.datum = value; }
        }
        public string ScheduleTime
        {
            get { return _classroomAvailability.lesuur; }
            set { _classroomAvailability.lesuur = value; }
        }
        public string Classroom
        {
            get { return _classroomAvailability.ruimte; }
            set { _classroomAvailability.ruimte = value; }
        }
        public string ClassroomSpace
        {
            get { return _classroomAvailability.grootte; }
            set { _classroomAvailability.grootte = value; }
        }
        public string ClassroomType
        {
            get { return _classroomAvailability.type; }
            set { _classroomAvailability.type = value; }
        }
        public bool Occupied
        {
            get { return _classroomAvailability.bezet; }
            set { _classroomAvailability.bezet = value; }
        }

    }
}
