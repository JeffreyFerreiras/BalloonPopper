using BalloonPopper.Maui.Utilities;

namespace BalloonPopper.Maui.Services;

/// <summary>
/// Error Handler Service.
/// </summary>
public interface IErrorHandler
{
    /// <summary>
    /// Handle error in UI.
    /// </summary>
    /// <param name="ex">Exception being thrown.</param>
    void HandleError(Exception ex);
}

/// <summary>
/// Modal Error Handler.
/// </summary>
public class ModalErrorHandler : IErrorHandler
{
    SemaphoreSlim _semaphore = new(1, 1);

    /// <summary>
    /// Handle error in UI.
    /// </summary>
    /// <param name="ex">Exception.</param>
    public void HandleError(Exception ex)
    {
        DisplayAlert(ex).FireAndForgetSafeAsync();
    }

    async Task DisplayAlert(Exception ex)
    {
        try
        {
            await _semaphore.WaitAsync();
            if (Shell.Current is Shell shell)
                await shell.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
