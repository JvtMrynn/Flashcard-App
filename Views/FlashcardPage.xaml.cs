using FlashcardApp.ViewModels;

namespace FlashcardApp.Views;

public partial class FlashcardPage : ContentPage
{
	public FlashcardPage()
	{
		InitializeComponent();
		BindingContext = new FlashcardPageViewModel();
	}

    
}