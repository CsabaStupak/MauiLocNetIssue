using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Net.Sockets;

namespace MauiApp7;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

    private async void OnCounterClicked(object sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);

        // Local network connection fails
        await PingAsync("192.168.3.10");
        await ConnectAsync("192.168.3.10", 4356);

        // Public network is OK
        await PingAsync("8.8.8.8");

        await PingAsync("192.168.3.10");
    }

    public static async Task PingAsync(string ipAddr)
    {
        try
        {
            Ping ping = new Ping();
            PingReply reply = await ping.SendPingAsync(IPAddress.Parse(ipAddr));
            if (reply.Status == IPStatus.Success)
                Debug.WriteLine($"Ping to {ipAddr} OK");
        }
        catch (Exception ex)
        {
            var locIP = GetLocalIPAddress();
            Debug.WriteLine($"Ping to {ipAddr} from {locIP} failed: {ex}");
        }
    }

    public static async Task ConnectAsync(string ipAddr, int port)
    {
        try
        {
            TcpClient client = new TcpClient();
            await client.ConnectAsync(IPAddress.Parse(ipAddr), port);
            client.Close();
            Debug.WriteLine($"Connect to {ipAddr} OK");
        }
        catch (Exception ex)
        {
            var locIP = GetLocalIPAddress();
            Debug.WriteLine($"Connect to {ipAddr} from {locIP} failed: {ex}");
        }
    }

    public static IPAddress GetLocalIPAddress()
    {
        var candidates = NetworkInterface.GetAllNetworkInterfaces()
            .SelectMany(x => x.GetIPProperties().UnicastAddresses)
            .Select(x => x.Address)
            .Where(x => x.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(x))
            .ToArray();
        if (candidates.Length > 0)
            return candidates.First();

        return IPAddress.None;
    }
}

