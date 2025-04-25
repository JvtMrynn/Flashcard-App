using FlashcardApp.ViewModels;

namespace FlashcardApp.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();

		var vm = new LoginViewModel();
		vm.ShowAlert = async (title, message, cancel) =>
		{
			await DisplayAlert(title, message, cancel);
		};

		BindingContext = vm;
	}
}