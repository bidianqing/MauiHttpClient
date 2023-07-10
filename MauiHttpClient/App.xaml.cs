using MauiHttpClient.Services.Request;

namespace MauiHttpClient;

public partial class App : Application
{
	public static HttpClient HttpClient { get; private set; }
	public static IRequestService RequestService { get; private set; }

    public App(HttpClient httpClient, IRequestService requestService)
	{
		InitializeComponent();

		MainPage = new AppShell();

		HttpClient = httpClient;
		RequestService = requestService;
    }
}
