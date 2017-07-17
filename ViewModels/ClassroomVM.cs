using System;
using System.Collections.Generic;
using AvansApp.Models.ServerModels;

namespace AvansApp.ViewModels
{
    public class ClassroomVM
    {
        private Classroom _classroom;
        private List<AvailabilityVM> _availabilities;

        public ClassroomVM()
        {
            _classroom = new Classroom();
            _availabilities = new List<AvailabilityVM>();
        }
        public ClassroomVM(Classroom c)
        {
            _classroom = c;
            _availabilities = new List<AvailabilityVM>();
        }

        public DateTime DateTime
        {
            get { return _classroom.datum; }
            set { _classroom.datum = value; }
        }
        public string Classroom
        {
            get { return _classroom.ruimte; }
            set { _classroom.ruimte = value; }
        }
        public string ClassroomSpace
        {
            get { return _classroom.grootte; }
            set { _classroom.grootte = value; }
        }
        public string ClassroomType
        {
            get { return _classroom.type; }
            set { _classroom.type = value; }
        }
        public bool Occupied
        {
            get { return _classroom.isBezet; }
            set { _classroom.isBezet = value; }
        }
        public List<AvailabilityVM> Availability
        {
            get { return _availabilities; }
            set { _availabilities = value; }
        }
    }
}
