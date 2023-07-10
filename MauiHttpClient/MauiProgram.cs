using MauiHttpClient.Services.Request;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Hosting;

namespace MauiHttpClient;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddSingleton(sp =>
		{
			var client = new HttpClient(new MauiHttpClientHandler());
			client.BaseAddress = new Uri(Urls.Domain);

            return client;
        });
        builder.Services.AddSingleton<IRequestService, RequestService>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
