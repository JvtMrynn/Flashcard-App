using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FlashcardApp.Services;
using System.Windows.Input;

namespace FlashcardApp.ViewModels
{
    public class RegisterViewModel
    {
        private string email;
        private string password;
        private string confirmPassword;
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

        public string ConfirmPassword
        {
            get => confirmPassword;
            set { confirmPassword = value; OnPropertyChanged(); }
        }

        public bool IsBusy
        {
            get => isBusy;
            set { isBusy = value; OnPropertyChanged(); }
        }

        public ICommand RegisterCommand { get; }
        public ICommand GoToLoginCommand { get; }
        public Action<string, string, string> ShowAlert { get; set; }

        private readonly FirebaseAuthService authService = new();

        public RegisterViewModel()
        {
            RegisterCommand = new Command(async () => await Register(), () => !IsBusy);
            GoToLoginCommand = new Command(async () => await Shell.Current.GoToAsync("LoginPage"));
        }

        private async Task Register()
        {
            if (IsBusy) return;

            IsBusy = true;

            try
            {
                if (Password != ConfirmPassword)
                {
                    ShowAlert?.Invoke("Validation Error", "Passwords do not match.", "OK");
                    return;
                }

                if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                {
                    ShowAlert?.Invoke("Validation Error", "No Internet Connection. Please check your network and try again.", "OK");
                    return;
                }

                var response = await authService.RegisterAsync(Email, Password);
                await SecureStorage.SetAsync("firebase_token", response.idToken);
                
                await Shell.Current.GoToAsync("///MainPage");

            }
            catch (Exception ex)
            {
                ShowAlert?.Invoke("Registration Failed", ex.Message, "OK");
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
