using AvansApp.Models.ServerModels;

namespace AvansApp.ViewModels
{
    public class AvailabilityVM
    {
        private Availability _availability;

        public AvailabilityVM(Availability a)
        {
            _availability = a;
        }
        public AvailabilityVM()
        {
            _availability = new Availability();
        }

        public string ScheduleTime
        {
            get { return _availability.lesuur; }
            set { _availability.lesuur = value; }
        }
        public bool Occupied
        {
            get { return _availability.bezet; }
            set { _availability.bezet = value; }
        }

    }
}
