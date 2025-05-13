using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using FlashcardApp.Models;
using FlashcardApp.Services;
using FlashcardApp.Views;

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
        [ObservableProperty] private int score = 0;
        [ObservableProperty] private int totalFlashcards;
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

        public async void LoadFlashcards(int? scrollToFlashcardId = null)
        {
            var cards = await _flashcardService.GetFlashcardsBySubjectIdAsync(SubjectId);
            Flashcards = cards.ToList();
            
            // If there are no cards, just reset everything
            if (Flashcards.Count == 0)
            {
                CurrentIndex = 0;
                Score = 0;
                TotalFlashcards = 0;
                CurrentFlashcard = null;
                ProgressText = "No Cards";
                return;
            }
            
            // Set the total count
            TotalFlashcards = Flashcards.Count;
            
            // If we need to scroll to a specific flashcard, find its index
            if (scrollToFlashcardId.HasValue)
            {
                var index = Flashcards.FindIndex(f => f.Id == scrollToFlashcardId.Value);
                if (index >= 0)
                {
                    CurrentIndex = index;
                }
                else
                {
                    // If the specific card wasn't found, go to the last card (likely the newly added one)
                    CurrentIndex = Flashcards.Count - 1;
                }
            }
            else
            {
                // Default to first card
                CurrentIndex = 0;
                Score = 0;
            }
            
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

            if (CurrentIndex < 0 || CurrentIndex >= Flashcards.Count)
            {
                CurrentIndex = 0;
            }

            CurrentFlashcard = Flashcards[CurrentIndex];
            ProgressText = $"Card {CurrentIndex + 1}/{Flashcards.Count}";
        }

        private void NextCard()
        {
            if (Flashcards == null || Flashcards.Count == 0)
                return;

            CurrentIndex = (CurrentIndex + 1) % Flashcards.Count;
            UpdateCurrentCard();
        }

        private void PreviousCard()
        {
            if (Flashcards == null || Flashcards.Count == 0)
                return;

            CurrentIndex = (CurrentIndex - 1 + Flashcards.Count) % Flashcards.Count;
            UpdateCurrentCard();
        }

        private void ToggleShuffle()
        {
            if (Flashcards == null)
                return;

            if (IsShuffled)
                Flashcards = Flashcards.OrderBy(f => f.Id).ToList();
            else
                Flashcards = Flashcards.OrderBy(_ => Guid.NewGuid()).ToList();

            IsShuffled = !IsShuffled;
            CurrentIndex = 0;
            UpdateCurrentCard();
        }

        private async void SelectAnswer(string choiceLetter)
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

            if (isCorrect)
                score++;

            Application.Current.MainPage.DisplayAlert(
                isCorrect ? "Correct!" : "Incorrect!",
                isCorrect ? "You chose the correct answer." : $"Correct Answer: {CurrentFlashcard.CorrectAnswer}",
                "OK");

            CurrentIndex++;

            if (CurrentIndex < Flashcards.Count)
            {
                UpdateCurrentCard();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Quiz Completed",
                    $"You scored {Score} out of {Flashcards.Count}!",
                    "OK");

                await Application.Current.MainPage.Navigation.PopAsync();
            }
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
            var editorPage = new FlashcardEditorPage();
            if (editorPage.BindingContext is FlashcardEditorPageViewModel vm)
            {
                vm.SubjectId = SubjectId;
                vm.FlashcardId = 0;
                vm.OnFlashcardSaved = (newId) => {
                    Console.WriteLine($"Flashcard saved with ID: {newId}");
                    LoadFlashcards(newId);
                };
            }
            await Application.Current.MainPage.Navigation.PushAsync(editorPage);
        }

        private async Task EditCurrentFlashcard()
        {
            if (CurrentFlashcard == null)
                return;

            var editorPage = new FlashcardEditorPage();
            if (editorPage.BindingContext is FlashcardEditorPageViewModel vm)
            {
                vm.SubjectId = SubjectId;
                vm.FlashcardId = CurrentFlashcard.Id;
                vm.OnFlashcardSaved = (updatedId) => {
                    Console.WriteLine($"Flashcard updated with ID: {updatedId}");
                    LoadFlashcards(updatedId);
                };
            }
            await Application.Current.MainPage.Navigation.PushAsync(editorPage);
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
                Flashcards.RemoveAt(CurrentIndex);

                if (CurrentIndex >= Flashcards.Count)
                    CurrentIndex = Math.Max(0, Flashcards.Count - 1);

                UpdateCurrentCard();
            }
        }

        private async Task GoToDashboard()
        {
            await Application.Current.MainPage.Navigation.PopToRootAsync();
        }

        private async Task GoToSubjects()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}