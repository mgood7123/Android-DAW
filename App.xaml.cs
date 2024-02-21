namespace _2DAW {
    public partial class App : Application {
        public App() {
            InitializeComponent();
            ContentPage contentPage = new();
            contentPage.Content = new XUI.Platform.Host() {
                Content = new XUI.Views.RepeatDrawView() {
                    Repeat = 1000,
                    Content = new XUI.Views.ColorView() {
                        Color = XUI.Graphics.Colors.DarkBlue
                    }
                }
            };
            MainPage = contentPage;
        }
    }
}
