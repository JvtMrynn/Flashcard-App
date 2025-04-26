using FlashcardApp.ViewModels;

namespace FlashcardApp.Views;

public partial class FlashcardEditorPage : ContentPage
{
	public FlashcardEditorPage()
	{
		InitializeComponent();
		BindingContext = new FlashcardEditorPageViewModel();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is FlashcardEditorPageViewModel vm)
        {
            if (vm.FlashcardId == 0)
            {
                vm.PrepareNewFlashcard();
            }
        }
    }
}