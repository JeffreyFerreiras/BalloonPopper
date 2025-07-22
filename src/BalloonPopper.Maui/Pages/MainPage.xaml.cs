using BalloonPopper.Maui.Models;
using BalloonPopper.Maui.PageModels;

namespace BalloonPopper.Maui.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}