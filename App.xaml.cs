namespace FlashcardApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        protected override async void OnStart()
        {
            var token = await SecureStorage.GetAsync("firebase_token");
            if (!string.IsNullOrEmpty(token))
            {
                if (MainPage is AppShell appShell)
                {
                    appShell.ShowMainContent();
                    appShell.HideAuthPages();
                    await Shell.Current.GoToAsync("//MainPage");
                }
            }
        }
    }
}