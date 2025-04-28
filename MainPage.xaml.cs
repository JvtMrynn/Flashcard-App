using FlashcardApp.ViewModels;

namespace FlashcardApp
{
    public partial class MainPage : ContentPage
    {
        private MainPageViewModel _viewModel;
        public MainPage()
        {
            InitializeComponent();
            _viewModel = new MainPageViewModel();
            _viewModel.ShowAlert = async (title, message, cancel) =>
            {
                await DisplayAlert(title, message, cancel);
            };

            BindingContext = _viewModel;
        }

        //protected override void OnAppearing()
        //{
        //    base.OnAppearing();

        //    if (BindingContext is MainPageViewModel vm)
        //    {
        //        vm.ReloadSubjects();
        //    }
        //}
    }


}
