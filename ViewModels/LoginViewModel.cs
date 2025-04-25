using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FlashcardApp.Services;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using FlashcardApp.Views;

namespace FlashcardApp.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string email;
        private string password;
        private bool isBusy;

        public string Email
        {
            get => email;
            set { email = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => password;
            set { password = value; OnPropertyChanged(); }
        }

        public bool IsBusy
        {
            get => isBusy;
            set { isBusy = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }

        public ICommand GoToRegisterCommand { get; }
        public Action<string, string, string> ShowAlert { get; set; }

        private readonly FirebaseAuthService authService = new();

        public LoginViewModel()
        {
            LoginCommand = new Command(async () => await Login(), () => !IsBusy);
            GoToRegisterCommand = new Command(async () => await Shell.Current.GoToAsync("//RegisterPage"));
        }

        private async Task Login()
        {
            if (IsBusy) return;

            IsBusy = true;

            try
            {
                if(Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                {
                    ShowAlert?.Invoke("Network Error", "No Internet Connection. Please check your network and try again.", "OK");
                    return;
                }

                var response = await authService.LoginAsync(Email, Password);
                await SecureStorage.SetAsync("firebase_token", response.idToken);
                

                // Update UI after successful login
                if (Application.Current.MainPage is AppShell appShell)
                {
                    appShell.ShowMainContent();
                    appShell.HideAuthPages();
                }

                await Shell.Current.GoToAsync("///MainPage"); // Navigates and clears backstack
            }
            catch (Exception ex)
            {
                ShowAlert?.Invoke("Login Failed", "Invalid Email or Password", "Try Again");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
