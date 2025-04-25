using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FlashcardApp.Models;
using FlashcardApp.Views;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace FlashcardApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public ICommand LogoutCommand { get; }
        public Action<string, string, string> ShowAlert { get; set; }

        public MainViewModel()
        {
            LogoutCommand = new Command(async () => await Logout());
        }

        private async Task Logout()
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                ShowAlert?.Invoke("Network Error", "No Internet Connection. User will not be logged out to preserve progress.", "OK");
                return;
            }

            SecureStorage.Remove("firebase_token");

            if (Shell.Current is AppShell appShell)
            {
                appShell.ShowAuthPages();
                appShell.HideMainContent();
            }
            else
            {
                Console.WriteLine("Warning: Shell.Current is not the expected AppShell type.");
            }

            await Shell.Current.GoToAsync("///LoginPage");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Object.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}