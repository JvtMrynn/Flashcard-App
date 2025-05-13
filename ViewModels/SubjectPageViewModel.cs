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
using FlashcardApp.Views;

namespace FlashcardApp.ViewModels
{
    [QueryProperty(nameof(SubjectId), "subjectId")]
    public class SubjectPageViewModel : ObservableObject
    {
        private readonly SubjectService _subjectService;
        
        private int _subjectId;
        public int SubjectId
        {
            get => _subjectId;
            set => SetProperty(ref _subjectId, value);
        }

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
            try
            {
                var editorPage = new SubjectEditorPage();
                await Application.Current.MainPage.Navigation.PushAsync(editorPage);
                
                if (editorPage.BindingContext is SubjectEditorPageViewModel vm)
                {
                    if (subject != null)
                    {
                        vm.SubjectId = subject.Id;
                    }
                    vm.ShowAlert = async (title, message, cancel) =>
                    {
                        await Application.Current.MainPage.DisplayAlert(title, message, cancel);
                    };
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to open editor: " + ex.Message, "OK");
            }
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
            var flashcardPage = new FlashcardPage();
            if (flashcardPage.BindingContext is FlashcardPageViewModel vm)
            {
                vm.SubjectId = subject.Id;
            }
            await Application.Current.MainPage.Navigation.PushAsync(flashcardPage);
        }

        private async Task GoToDashboard()
        {
            await Application.Current.MainPage.Navigation.PopToRootAsync();
        }
    }
}
