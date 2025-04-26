using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using Microsoft.Maui.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlashcardApp.Services;
using FlashcardApp.Models;
using FlashcardApp.ViewModels;

namespace FlashcardApp.ViewModels
{
    [QueryProperty(nameof(SubjectId), "subjectId")]
    public partial class SubjectEditorPageViewModel : ObservableObject
    {
        private readonly SubjectService _subjectService;

        [ObservableProperty] private string name;
        [ObservableProperty] private string description;
        [ObservableProperty] private string colorTag;

        [ObservableProperty] private int subjectId;
        [ObservableProperty] private bool isEditMode;

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public Action<string, string, string> ShowAlert { get; set; }

        public SubjectEditorPageViewModel()
        {
            _subjectService = new SubjectService();
            SaveCommand = new Command(async () => await SaveSubject());
            CancelCommand = new Command(async () => await Shell.Current.GoToAsync("///SubjectPage"));
        }

        partial void OnSubjectIdChanged(int value)
        {
            LoadSubject(value);
        }

        private async void LoadSubject(int id)
        {
            var subject = await _subjectService.GetSubjectByIdAsync(id);
            if (subject != null)
            {
                Name = subject.Name;
                Description = subject.Description;
                ColorTag = subject.ColorTag;
                IsEditMode = true;
            }
        }

        private async Task SaveSubject()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                ShowAlert?.Invoke("Validation Error", "Subject name is required.", "OK");
                return;
            }

            if (!IsEditMode && !await _subjectService.IsSubjectNameUniqueAsync(Name))
            {
                ShowAlert?.Invoke("Duplicate", "Subject name must be unique.", "OK");
                return;
            }

            var subject = new SubjectModel
            {
                Id = IsEditMode ? SubjectId : 0,
                Name = Name.Trim(),
                Description = Description?.Trim(),
                ColorTag = ColorTag?.Trim()
            };

            if (IsEditMode)
                await _subjectService.UpdateSubjectAsync(subject);
            else
                await _subjectService.AddSubjectAsync(subject);

            await Shell.Current.GoToAsync("///SubjectPage");
        }
    }
}
