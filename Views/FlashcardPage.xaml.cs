using FlashcardApp.ViewModels;

namespace FlashcardApp.Views;

public partial class FlashcardPage : ContentPage
{
	private int? _pendingFlashcardId = null;

	public FlashcardPage()
	{
		InitializeComponent();
		BindingContext = new FlashcardPageViewModel();
		
		// Subscribe to messages from FlashcardEditorPageViewModel with flashcard ID
		MessagingCenter.Subscribe<FlashcardEditorPageViewModel, int>(this, "ReloadFlashcards", (sender, flashcardId) => {
			if (BindingContext is FlashcardPageViewModel vm)
			{
				Console.WriteLine($"Received message to reload flashcards with ID: {flashcardId}");
				_pendingFlashcardId = flashcardId;
				vm.LoadFlashcards(flashcardId);
			}
		});
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		
		// Check if we have a pending flashcard ID to display
		if (_pendingFlashcardId.HasValue && BindingContext is FlashcardPageViewModel vm)
		{
			Console.WriteLine($"OnAppearing with pending flashcard ID: {_pendingFlashcardId}");
			vm.LoadFlashcards(_pendingFlashcardId);
			_pendingFlashcardId = null;
		}
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		
		// Unsubscribe when page disappears
		MessagingCenter.Unsubscribe<FlashcardEditorPageViewModel, int>(this, "ReloadFlashcards");
	}
}