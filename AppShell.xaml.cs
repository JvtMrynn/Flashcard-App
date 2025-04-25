namespace FlashcardApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register simplified routes for easier navigation
            Routing.RegisterRoute("LoginPage", typeof(Views.LoginPage));
            Routing.RegisterRoute("RegisterPage", typeof(Views.RegisterPage));
            Routing.RegisterRoute("MainPage", typeof(MainPage));
            //Routing.RegisterRoute("FlashcardPage", typeof(Views.FlashcardPage));

            HideMainContent();

            // Check login state on app start
            CheckLoggedInStatus();
        }

        private async void CheckLoggedInStatus()
        {
            var token = await SecureStorage.GetAsync("firebase_token");

            if (!string.IsNullOrEmpty(token))
            {
                // ✅ User is logged in
                ShowMainContent();
                HideAuthPages();
                await GoToAsync("//MainPage"); // Optional: go to main page
            }
            else
            {
                // ❌ Not logged in, send to LoginPage
                await GoToAsync("//LoginPage");
            }
        }

        public void HideMainContent()
        {
            if (MainPageFlyoutItem != null && Items.Contains(MainPageFlyoutItem))
                Items.Remove(MainPageFlyoutItem);
        }

        public void ShowMainContent()
        {
            if (MainPageFlyoutItem != null && !Items.Contains(MainPageFlyoutItem))
                Items.Add(MainPageFlyoutItem);
        }

        public void HideAuthPages()
        {
            if (LoginFlyoutItem != null && Items.Contains(LoginFlyoutItem))
                Items.Remove(LoginFlyoutItem);

            if (RegisterFlyoutItem != null && Items.Contains(RegisterFlyoutItem))
                Items.Remove(RegisterFlyoutItem);
        }

        public void ShowAuthPages()
        {
            if (LoginFlyoutItem != null && !Items.Contains(LoginFlyoutItem))
                Items.Insert(0, LoginFlyoutItem);

            if (RegisterFlyoutItem != null && !Items.Contains(RegisterFlyoutItem))
                Items.Insert(1, RegisterFlyoutItem);
        }
    }
}
