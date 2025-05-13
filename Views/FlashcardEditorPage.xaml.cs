using FlashcardApp.ViewModels;

namespace FlashcardApp.Views;

public partial class FlashcardEditorPage : ContentPage
{
	private FlashcardEditorPageViewModel _viewModel;

	public FlashcardEditorPage()
	{
		InitializeComponent();
		
		_viewModel = new FlashcardEditorPageViewModel();
		_viewModel.ShowAlert = async (title, message, cancel) =>
		{
			await DisplayAlert(title, message, cancel);
		};
		
		BindingContext = _viewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel.FlashcardId == 0)
        {
            _viewModel.PrepareNewFlashcard();
        }
    }
}