using MauiHttpClient.Services.Request;

namespace MauiHttpClient;

public partial class App : Application
{
	public static IRequestService RequestService { get; private set; }

    public App(IRequestService requestService)
	{
		InitializeComponent();

		MainPage = new AppShell();

		RequestService = requestService;
    }
}
