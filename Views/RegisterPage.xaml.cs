using FlashcardApp.ViewModels;

namespace FlashcardApp.Views;

public partial class RegisterPage : ContentPage
{
	public RegisterPage()
	{
		InitializeComponent();

        var vm = new RegisterViewModel();
        vm.ShowAlert = async (title, message, cancel) =>
        {
            await DisplayAlert(title, message, cancel);
        };

        BindingContext = vm;
    }
}