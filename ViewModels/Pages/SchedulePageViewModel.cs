using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

using AvansApp.Helpers;
using AvansApp.Models.Enums;
using AvansApp.Services.Pages;

namespace AvansApp.ViewModels.Pages
{
    public class SchedulePageViewModel : ViewModelBase
    {
        private int currentDayIndex;
        private int todayIndex;

        public List<ObservableCollection<ScheduleVM>> Items { get; private set; }
        public ObservableCollection<ScheduleVM> CurrentDay { get; private set; }
        private string _header;
        private string _headerDay;
        public string Header {
            get { return _header; }
            set { Set(ref _header, value); }
        }
        public string HeaderDay {
            get { return _headerDay; }
            set { Set(ref _headerDay, value); }
        }
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { Set(ref _isLoading, value); }
        }
        private bool _hasNoResult;
        public bool HasNoResult
        {
            get { return _hasNoResult; }
            set { Set(ref _hasNoResult, value); }
        }
        private bool _hasNoScheduleCode;
        public bool HasNoScheduleCode
        {
            get { return _hasNoScheduleCode; }
            set { Set(ref _hasNoScheduleCode, value); }
        }
        public bool ButtonsEnabled { get { return !_isLoading && !_hasNoResult && !_hasNoScheduleCode; } }
        public ICommand OnPreviousDayClickCommand { get; private set; }
        public ICommand OnNextDayClickCommand { get; private set; }
        public ICommand OnTodayClickCommand { get; private set; }

        private ScheduleService Service { get; set; }
        private SettingsService Settings { get; set; }

        public SchedulePageViewModel()
        {
            Items = new List<ObservableCollection<ScheduleVM>>();
            CurrentDay = new ObservableCollection<ScheduleVM>();
            Service = Singleton<ScheduleService>.Instance;
            Settings = Singleton<SettingsService>.Instance;

            todayIndex = 0;
            currentDayIndex = 0;

            OnPreviousDayClickCommand = new RelayCommand<ItemClickEventArgs>(OnPreviousDayClick, (e) => { return true; });
            OnNextDayClickCommand = new RelayCommand<ItemClickEventArgs>(OnNextDayClick, (e) => { return true; });
            OnTodayClickCommand = new RelayCommand<ItemClickEventArgs>(OnTodayClick, (e) => { return true; });
            
            SetHeader();
        }
        public void Initialize()
        {
            MainPageViewModel.SetPageTitle("Shell_SchedulePage".GetLocalized());
        }
        public async Task LoadDataAsync()
        {
            if (Settings.KeyExists(SettingsService.ScheduleCodeKey))
            {
                IsLoading = true;
                HasNoResult = false;
                HasNoScheduleCode = false;

                string scheduleCode = await Settings.ReadScheduleCode();
                bool withoutBlanks = await Settings.ReadScheduleBlanks();
                List<List<ScheduleVM>> data = await Service.GetSchedule(ScheduleType.Classroom, scheduleCode, DateTime.Now.AddMonths(-3), DateTime.Now.AddMonths(3), withoutBlanks);

                todayIndex = (withoutBlanks == true) ? Service.TodayIndex : 0;
                currentDayIndex = todayIndex;

                Items.Clear();

                if (data.Count > 0)
                {
                    foreach (List<ScheduleVM> list in data)
                    {
                        var l = new ObservableCollection<ScheduleVM>();
                        foreach (ScheduleVM s in list)
                        {
                            l.Add(s);
                        }
                        Items.Add(l);
                    }
                    CurrentDay = Items[todayIndex];
                }
                IsLoading = false;
                HasNoResult = Items.Count <= 0;
                HasNoScheduleCode = false;
            }
            else
            {
                Items.Clear();
                todayIndex = 0;
                currentDayIndex = 0;

                IsLoading = false;
                HasNoResult = false;
                HasNoScheduleCode = true;
            }
            OnPropertyChanged("ButtonsEnabled");
        }

        private void OnPreviousDayClick(ItemClickEventArgs e)
        {
            if (currentDayIndex > 0)
            {
                currentDayIndex -= 1;
                CurrentDay = Items[currentDayIndex];

                SetCurrentDay();
                SetHeader();
            }

        }
        private void OnNextDayClick(ItemClickEventArgs e)
        {
            if (currentDayIndex < (Items.Count - 1))
            {
                currentDayIndex += 1;
                CurrentDay = Items[currentDayIndex];

                SetCurrentDay();
                SetHeader();
            }
        }
        private void OnTodayClick(ItemClickEventArgs e)
        {
            if (CurrentDay != null && CurrentDay.Count > 0)
            {
                CurrentDay = Items[todayIndex];
                currentDayIndex = todayIndex;
                SetCurrentDay();
                SetHeader();
            }
        }
        private void SetCurrentDay()
        {
            // TODO
            if (CurrentDay.Count <= 0 || (CurrentDay.Count == 1 && CurrentDay[0].Id < 0))
            {
                //ScheduleListView.ItemsSource = new List<ScheduleVM>();
                //ScheduleListView.Visibility = Visibility.Collapsed;
                //NoScheduleText.Visibility = Visibility.Visible;
            }
            else
            {
                //ScheduleListView.ItemsSource = CurrentDay;
                //ScheduleListView.Visibility = Visibility.Visible;
                //NoScheduleText.Visibility = Visibility.Collapsed;
            }
        }

        private void SetHeader()
        {
            if (CurrentDay != null && CurrentDay.Count > 0)
            {
                Header = CurrentDay[0].Datum.Day + "-" + CurrentDay[0].Datum.Month + "-" + CurrentDay[0].Datum.Year; // Pak de datum van de eerste item
                HeaderDay = CurrentDay[0].Datum.DayOfWeek.ToString();
            }
            else
            {
                Header = DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
                HeaderDay = DateTime.Now.DayOfWeek.ToString();
            }
        }
    }
}
