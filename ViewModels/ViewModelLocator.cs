using Microsoft.Practices.ServiceLocation;
using GalaSoft.MvvmLight.Ioc;

using AvansApp.Services;
using AvansApp.Views;
using AvansApp.ViewModels.Pages;

namespace AvansApp.ViewModels
{
    public class ViewModelLocator
    {
        NavigationService _navigationService = new NavigationService();

        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register(() => _navigationService);
            SimpleIoc.Default.Register<MainPageViewModel>();

            Register<SplashPageViewModel, SplashPage>();
            Register<LoginPageViewModel, LoginPage>();

            Register<SchedulePageViewModel, SchedulePage>();
            Register<AnnouncementPageViewModel, AnnouncementsPage>();
            Register<AnnouncementPageDetailViewModel, AnnouncementsPageDetail>();
            Register<ResultPageViewModel, ResultsPage>();
            Register<ResultPageDetailViewModel, ResultsPageDetail>();
            Register<ExamPageViewModel, ExamsPage>();
            Register<ClassroomAvailabilityPageViewModel, ClassroomAvailabilityPage>();
            Register<ClassroomAvailabilityPageDetailViewModel, ClassroomAvailabilitySinglePage>();
            Register<EmployeePageViewModel, EmployeesPage>();
            Register<EmployeeSinglePageViewModel, EmployeesSinglePage>();
            Register<EmployeeScheduleSinglePageViewModel, EmployeesScheduleSinglePage>();
            Register<DisruptionPageViewModel, DisruptionsPage>();
            Register<DisruptionSinglePageViewModel, DisruptionsSinglePage>();
            Register<SettingsPageViewModel, SettingsPage>();

            Register<ProfilePageViewModel, MyProfilePage>();
        }

        // Single pages
        public LoginPageViewModel LoginPageViewModel => ServiceLocator.Current.GetInstance<LoginPageViewModel>();
        public SplashPageViewModel SplashPageViewModel => ServiceLocator.Current.GetInstance<SplashPageViewModel>();

        // Schedule
        public SchedulePageViewModel SchedulePageViewModel => ServiceLocator.Current.GetInstance<SchedulePageViewModel>();

        // Announcements
        public AnnouncementPageDetailViewModel AnnouncementPageDetailViewModel => ServiceLocator.Current.GetInstance<AnnouncementPageDetailViewModel>();
        public AnnouncementPageViewModel AnnouncementPageViewModel => ServiceLocator.Current.GetInstance<AnnouncementPageViewModel>();

        // Results
        public ResultPageViewModel ResultPageViewModel => ServiceLocator.Current.GetInstance<ResultPageViewModel>();
        public ResultPageDetailViewModel ResultPageDetailViewModel => ServiceLocator.Current.GetInstance<ResultPageDetailViewModel>();

        // Exams
        public ExamPageViewModel ExamPageViewModel => ServiceLocator.Current.GetInstance<ExamPageViewModel>();

        // Classroom Availability
        public ClassroomAvailabilityPageViewModel ClassroomAvailabilityPageViewModel => ServiceLocator.Current.GetInstance<ClassroomAvailabilityPageViewModel>();
        public ClassroomAvailabilityPageDetailViewModel ClassroomAvailabilityPageDetailViewModel => ServiceLocator.Current.GetInstance<ClassroomAvailabilityPageDetailViewModel>();

        // Employees
        public EmployeePageViewModel EmployeePageViewModel => ServiceLocator.Current.GetInstance<EmployeePageViewModel>();
        public EmployeeSinglePageViewModel EmployeeSinglePageViewModel => ServiceLocator.Current.GetInstance<EmployeeSinglePageViewModel>();
        public EmployeeScheduleSinglePageViewModel EmployeeScheduleSinglePageViewModel => ServiceLocator.Current.GetInstance<EmployeeScheduleSinglePageViewModel>();

        // Disruptions
        public DisruptionPageViewModel DisruptionPageViewModel => ServiceLocator.Current.GetInstance<DisruptionPageViewModel>();
        public DisruptionSinglePageViewModel DisruptionSinglePageViewModel => ServiceLocator.Current.GetInstance<DisruptionSinglePageViewModel>();

        // Settings
        public SettingsPageViewModel SettingsPageViewModel => ServiceLocator.Current.GetInstance<SettingsPageViewModel>();

        // My Profile
        public ProfilePageViewModel ProfilePageViewModel => ServiceLocator.Current.GetInstance<ProfilePageViewModel>();
        
        // Main Page \ Shell
        public MainPageViewModel MainPageViewModel => ServiceLocator.Current.GetInstance<MainPageViewModel>();

        public void Register<VM, V>() where VM : class
        {
            SimpleIoc.Default.Register<VM>();
            _navigationService.Configure(typeof(VM).FullName, typeof(V));
        }
    }
}
