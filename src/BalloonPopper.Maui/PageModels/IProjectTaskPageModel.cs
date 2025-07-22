using BalloonPopper.Maui.Models;
using CommunityToolkit.Mvvm.Input;

namespace BalloonPopper.Maui.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}