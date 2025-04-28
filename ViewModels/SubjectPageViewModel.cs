using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlashcardApp.Services;
using FlashcardApp.Models;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;

namespace FlashcardApp.ViewModels
{
    public class SubjectPageViewModel : ObservableObject
    {
        private readonly SubjectService _subjectService;

        public ObservableCollection<SubjectModel> Subjects { get; set; } = new();


        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                FilterSubjects();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand TapSubjectCommand { get; }
        public ICommand GoToDashboardCommand { get; }

        private List<SubjectModel> _allSubjects = new();

        public SubjectPageViewModel()
        {
            _subjectService = new SubjectService();
            AddCommand = new Command(async () => await GoToEditorPage());
            EditCommand = new Command<SubjectModel>(async (subject) => await GoToEditorPage(subject));
            DeleteCommand = new Command<SubjectModel>(async (subject) => await DeleteSubject(subject));
            TapSubjectCommand = new Command<SubjectModel>(async (subject) => await GoToFlashcardPage(subject));
            GoToDashboardCommand = new Command(async () => await GoToDashboard());

            LoadSubjects();

            MessagingCenter.Subscribe<SubjectEditorPageViewModel>(this, "ReloadSubjects", (sender) => LoadSubjects());
        }

        public async void ReloadSubjects()
        {
            var subjects = await _subjectService.GetAllSubjectsAsync();
            Subjects = new ObservableCollection<SubjectModel>(subjects);
        }


        private async void LoadSubjects()
        {
            var subjects = await _subjectService.GetAllSubjectsAsync();
            Subjects.Clear();
            _allSubjects.Clear();

            foreach (var subject in subjects)
            {
                Subjects.Add(subject);
                _allSubjects.Add(subject);
            }
        }

        private void FilterSubjects()
        {
            var filtered = string.IsNullOrWhiteSpace(SearchText)
                ? _allSubjects
                : _allSubjects
                    .Where(s => s.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            Subjects.Clear();
            foreach (var subject in filtered)
                Subjects.Add(subject);
        }

        private async Task GoToEditorPage(SubjectModel subject = null)
        {
            string route = subject == null 
                ? "SubjectEditorPage" 
                : $"SubjectEditorPage?subjectId={subject.Id}";

            await Shell.Current.GoToAsync(route);
        }

        private async Task DeleteSubject(SubjectModel subject)
        {
            if (subject == null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Cannot delete subject: Invalid selection",
                    "OK");
                return;
            }

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Delete Subject",
                $"Are you sure you want to delete \"{subject.Name}\"?",
                "Yes", "No");

            if (confirm)
            {
                await _subjectService.DeleteSubjectAsync(subject.Id);
                LoadSubjects();
            }
        }

        private async Task GoToFlashcardPage(SubjectModel subject)
        {
            await Shell.Current.GoToAsync($"FlashcardPage?subjectId={subject.Id}");
        }

        private async Task GoToDashboard()
        {
            await Shell.Current.GoToAsync("///MainPage");
        }

    }
}
