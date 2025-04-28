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
using FlashcardApp.Services;

namespace FlashcardApp.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly SubjectService _subjectService;

        public ObservableCollection<SubjectModel> Subjects { get; set; } = new();
        public ObservableCollection<SubjectModel> RecentSubjects { get; set; } = new();

        public ICommand GoToSubjectCommand { get; }
        public ICommand AddSubjectCommand { get; }

        public ICommand LogoutCommand { get; }
        public Action<string, string, string> ShowAlert { get; set; }

        public MainPageViewModel()
        {
            _subjectService = new SubjectService();
            GoToSubjectCommand = new Command<SubjectModel>(async (subject) => await NavigateToSubjectPage(subject));
            AddSubjectCommand = new Command(async () => await NavigateToSubjectEditorPage());

            LoadSubjects();

            MessagingCenter.Subscribe<SubjectEditorPageViewModel>(this, "ReloadSubjects", (sender) => LoadSubjects());

            LogoutCommand = new Command(async () => await Logout());
        }

        private async void LoadSubjects()
        {
            var subjects = await _subjectService.GetAllSubjectsAsync();
            Subjects.Clear();
            foreach (var subject in subjects)
                Subjects.Add(subject);

            // For now, simulate recent ones
            RecentSubjects.Clear();
            foreach (var subject in subjects.Take(3))
                RecentSubjects.Add(subject);
        }

        private async Task NavigateToSubjectPage(SubjectModel subject)
        {
            await Shell.Current.GoToAsync($"///SubjectPage?subjectId={subject.Id}");
        }

        private async Task NavigateToSubjectEditorPage()
        {
            await Shell.Current.GoToAsync("SubjectEditorPage");
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

        public async void ReloadSubjects()
        {
            var subjects = await _subjectService.GetAllSubjectsAsync();
            Subjects = new ObservableCollection<SubjectModel>(subjects);
        }
    }
}