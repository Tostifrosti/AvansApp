﻿using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using AvansApp.ViewModels;
using AvansApp.ViewModels.Pages;

namespace AvansApp.Views
{
    public sealed partial class ClassroomAvailabilitySinglePage : Page
    {
        private ClassroomAvailabilityPageDetailViewModel ViewModel
        {
            get { return DataContext as ClassroomAvailabilityPageDetailViewModel; }
        }
        public ClassroomAvailabilitySinglePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.Item = e.Parameter as ClassroomVM;
            ViewModel.Initialize();
        }
    }
}
