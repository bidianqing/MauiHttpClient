using System.Text.Json.Nodes;

namespace MauiHttpClient;

public partial class MainPage : ContentPage
{
    int count = 0;
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnCounterClicked(object sender, EventArgs e)
    {
        var json = new JsonObject
        {
            ["phone"] = this.Phone.Text,
            ["code"] = "0000"
        }.ToJsonString();

        var result = await App.RequestService.PostAsync<LoginResult>(Urls.Login, json);

        LabelToken.Text = result?.Token;

        count++;

        if (count == 1)
            CounterBtn.Text = $"Clicked {count} time";
        else
            CounterBtn.Text = $"Clicked {count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);
    }
}

