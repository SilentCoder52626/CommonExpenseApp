﻿using CommonNotesApp.ViewModel;

namespace CommonNotesApp
{
    public partial class MainPage : ContentPage
    {
        private readonly MainViewModel _vm;
        public MainPage(MainViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            BindingContext = vm;
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await _vm.LoadItemsAsync();
        }
       
    }

}
