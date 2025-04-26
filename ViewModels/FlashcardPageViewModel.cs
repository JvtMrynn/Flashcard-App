using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using FlashcardApp.Models;
using FlashcardApp.Services;

namespace FlashcardApp.ViewModels
{
    [QueryProperty(nameof(SubjectId), "subjectId")]
    public partial class FlashcardPageViewModel : ObservableObject
    {
        private readonly FlashcardService _flashcardService;

        [ObservableProperty] private List<FlashcardModel> flashcards;
        [ObservableProperty] private FlashcardModel currentFlashcard;

        [ObservableProperty] private int subjectId;
        [ObservableProperty] private int currentIndex;
        [ObservableProperty] private bool isShuffled;
        [ObservableProperty] private string progressText;
        [ObservableProperty] private string learnedButtonText = "Mark as Learned";

        public ICommand NextCommand { get; }
        public ICommand PreviousCommand { get; }
        public ICommand ShuffleCommand { get; }
        public ICommand SelectAnswerCommand { get; }
        public ICommand ToggleLearnedCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand GoToDashboardCommand { get; }
        public ICommand GoToSubjectsCommand { get; }

        public FlashcardPageViewModel()
        {
            _flashcardService = new FlashcardService();

            NextCommand = new Command(NextCard);
            PreviousCommand = new Command(PreviousCard);
            ShuffleCommand = new Command(ToggleShuffle);
            SelectAnswerCommand = new Command<string>(SelectAnswer);
            ToggleLearnedCommand = new Command(ToggleLearned);
            EditCommand = new Command(async () => await EditCurrentFlashcard());
            DeleteCommand = new Command(async () => await DeleteCurrentFlashcard());
            AddCommand = new Command(async () => await AddFlashcard());
            GoToDashboardCommand = new Command(async () => await GoToDashboard());
            GoToSubjectsCommand = new Command(async () => await GoToSubjects());

            LoadFlashcards();
        }

        

        partial void OnSubjectIdChanged(int value)
        {
            LoadFlashcards();
        }

        private async void LoadFlashcards()
        {
            var cards = await _flashcardService.GetFlashcardsBySubjectIdAsync(SubjectId);
            Flashcards = cards.ToList();
            CurrentIndex = 0;  // Use the property instead of the field
            UpdateCurrentCard();
        }

        private void UpdateCurrentCard()
        {
            if (Flashcards == null || Flashcards.Count == 0)
            {
                CurrentFlashcard = null;
                ProgressText = "No Cards";
                return;
            }

            // Make sure we have a valid index
            if (CurrentIndex < 0 || CurrentIndex >= Flashcards.Count)
            {
                CurrentIndex = 0;
            }

            // Use the property instead of the field
            CurrentFlashcard = Flashcards[CurrentIndex];
            ProgressText = $"Card {CurrentIndex + 1}/{Flashcards.Count}";
        }

        private void NextCard()
        {
            if (Flashcards == null || Flashcards.Count == 0)
                return;

            // Use the property instead of the field
            CurrentIndex = (CurrentIndex + 1) % Flashcards.Count;
            UpdateCurrentCard();
        }

        private void PreviousCard()
        {
            if (Flashcards == null || Flashcards.Count == 0)
                return;

            // Use the property instead of the field
            CurrentIndex = (CurrentIndex - 1 + Flashcards.Count) % Flashcards.Count;
            UpdateCurrentCard();
        }

        private void ToggleShuffle()
        {
            if (Flashcards == null)
                return;

            if (IsShuffled)
                Flashcards = Flashcards.OrderBy(f => f.Id).ToList(); // Restore original order
            else
                Flashcards = Flashcards.OrderBy(_ => Guid.NewGuid()).ToList(); // Shuffle

            IsShuffled = !IsShuffled;
            CurrentIndex = 0;  // Use the property instead of the field
            UpdateCurrentCard();
        }

        private void SelectAnswer(string choiceLetter)
        {
            if (CurrentFlashcard == null)
                return;

            string selectedAnswer = choiceLetter switch
            {
                "A" => CurrentFlashcard.ChoiceA,
                "B" => CurrentFlashcard.ChoiceB,
                "C" => CurrentFlashcard.ChoiceC,
                "D" => CurrentFlashcard.ChoiceD,
                _ => null
            };

            if (selectedAnswer == null)
                return;

            bool isCorrect = selectedAnswer == CurrentFlashcard.CorrectAnswer;

            Application.Current.MainPage.DisplayAlert(
                isCorrect ? "Correct!" : "Incorrect!",
                isCorrect ? "You chose the correct answer." : $"Correct Answer: {CurrentFlashcard.CorrectAnswer}",
                "OK");
        }

        private void ToggleLearned()
        {
            if (CurrentFlashcard == null)
                return;

            CurrentFlashcard.IsLearned = !CurrentFlashcard.IsLearned;
            LearnedButtonText = CurrentFlashcard.IsLearned ? "Unmark Learned" : "Mark as Learned";
            _flashcardService.UpdateFlashcardAsync(CurrentFlashcard);
        }

        private async Task AddFlashcard()
        {
            await Shell.Current.GoToAsync($"///FlashcardEditorPage?subjectId={SubjectId}&flashcardId=0");
        }

        private async Task EditCurrentFlashcard()
        {
            if (CurrentFlashcard == null)
                return;

            await Shell.Current.GoToAsync($"///FlashcardEditorPage?flashcardId={CurrentFlashcard.Id}&subjectId={SubjectId}");
        }


        private async Task DeleteCurrentFlashcard()
        {
            if (CurrentFlashcard == null)
                return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Delete Flashcard",
                "Are you sure you want to delete this flashcard?",
                "Yes", "No");

            if (confirm)
            {
                await _flashcardService.DeleteFlashcardByIdAsync(CurrentFlashcard.Id);
                Flashcards.RemoveAt(CurrentIndex);  // Use the property instead of the field

                if (CurrentIndex >= Flashcards.Count)
                    CurrentIndex = Math.Max(0, Flashcards.Count - 1);

                UpdateCurrentCard();
            }
        }

        public async void ReloadFlashcards()
        {
            var cards = await _flashcardService.GetFlashcardsBySubjectIdAsync(SubjectId);
            Flashcards = cards.ToList();
            CurrentIndex = 0;
            UpdateCurrentCard();
        }

        private async Task GoToDashboard()
        {
            await Shell.Current.GoToAsync("///MainPage");
        }

        private async Task GoToSubjects()
        {
            await Shell.Current.GoToAsync("///SubjectPage");
        }
    }
}