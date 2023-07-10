namespace MauiHttpClient.Services.Dialog
{
    public interface IDialogService
    {
        Task ShowAlertAsync(string title, string message, string buttonLabel);
    }
}
