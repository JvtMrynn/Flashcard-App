using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;
using FlashcardApp.Models;
using FlashcardApp.Services;

namespace FlashcardApp.ViewModels
{
    [QueryProperty(nameof(SubjectId), "subjectId")]
    [QueryProperty(nameof(FlashcardId), "flashcardId")]
    public partial class FlashcardEditorPageViewModel : ObservableObject
    {
        private readonly FlashcardService _flashcardService;

        [ObservableProperty] private string definition;
        [ObservableProperty] private string choiceA;
        [ObservableProperty] private string choiceB;
        [ObservableProperty] private string choiceC;
        [ObservableProperty] private string choiceD;
        [ObservableProperty] private string correctAnswer;

        [ObservableProperty] private int subjectId;
        [ObservableProperty] private int flashcardId;
        [ObservableProperty] private bool isEditMode;

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SetCorrectAnswerCommand { get; }

        public FlashcardEditorPageViewModel()
        {
            _flashcardService = new FlashcardService();

            SaveCommand = new Command(async () => await SaveFlashcard());
            CancelCommand = new Command(async () => await Shell.Current.GoToAsync($"///FlashcardPage?subjectId={SubjectId}"));
            SetCorrectAnswerCommand = new Command<string>(SetCorrectAnswer);
        }

        partial void OnFlashcardIdChanged(int value)
        {
            if (value != 0)
                LoadFlashcard();
        }

        partial void OnSubjectIdChanged(int value)
        {
            if (FlashcardId == 0)
            {
                ClearFields();
                IsEditMode = false;
            }
        }

        private async void LoadFlashcard()
        {
            var flashcard = await _flashcardService.GetFlashcardByIdAsync(FlashcardId);
            if (flashcard != null)
            {
                Definition = flashcard.Definition;
                ChoiceA = flashcard.ChoiceA;
                ChoiceB = flashcard.ChoiceB;
                ChoiceC = flashcard.ChoiceC;
                ChoiceD = flashcard.ChoiceD;
                CorrectAnswer = flashcard.CorrectAnswer;
                IsEditMode = true;
            }
        }

        private void ClearFields()
        {
            Definition = string.Empty;
            ChoiceA = string.Empty;
            ChoiceB = string.Empty;
            ChoiceC = string.Empty;
            ChoiceD = string.Empty;
            CorrectAnswer = string.Empty;
        }

        private void SetCorrectAnswer(string choiceKey)
        {
            switch (choiceKey)
            {
                case "A": CorrectAnswer = ChoiceA; break;
                case "B": CorrectAnswer = ChoiceB; break;
                case "C": CorrectAnswer = ChoiceC; break;
                case "D": CorrectAnswer = ChoiceD; break;
            }
        }

        private async Task SaveFlashcard()
        {
            if (string.IsNullOrWhiteSpace(Definition) || string.IsNullOrWhiteSpace(CorrectAnswer))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter a definition and select the correct answer.", "OK");
                return;
            }

            var flashcard = new FlashcardModel
            {
                Id = FlashcardId,
                SubjectId = SubjectId,
                Definition = Definition,
                ChoiceA = ChoiceA,
                ChoiceB = ChoiceB,
                ChoiceC = ChoiceC,
                ChoiceD = ChoiceD,
                CorrectAnswer = CorrectAnswer
            };

            if (IsEditMode)
                await _flashcardService.UpdateFlashcardAsync(flashcard);
            else
                await _flashcardService.AddFlashcardAsync(flashcard);

            await Shell.Current.GoToAsync($"///FlashcardPage?subjectId={SubjectId}");
        }

        public void PrepareNewFlashcard()
        {
            ClearFields();
            IsEditMode = false;
        }

    }
}
