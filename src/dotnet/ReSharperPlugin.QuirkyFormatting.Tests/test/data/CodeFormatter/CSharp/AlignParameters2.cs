using ReSharperPlugin.QuirkyFormatting.Tests.test.data;

public class AlignParameters2
{
    static void Main(string[] args)
    {
        int settingsSortIndex = 1;

        StringSetting coreName             = new("-1657-4d40-9c17-dcade0c6415d", "Core Name", ++settingsSortIndex, true, HtmlFieldType.Text, "Core 1");
        StringSetting apiAddress           = new("a111683e--443d-b8fc-4407a5fb14ba", "Core IP-Address or Hostname", ++settingsSortIndex, true, HtmlFieldType.Url, "127.0.0.1") { HtmlFieldWidth = 6 };
        UShortSetting apiPort              = new("f831819e-61c4-4915--9b6adceb46fd", "Listen Port API", ++settingsSortIndex, true, 8085) { HtmlFieldWidth = 6 };
        IntSetting    pulseDuration        = new("bfdde1fe-311f-47d0-9ad3-", "Pulse Duration", ++settingsSortIndex, true, 250) { Description = "In ms" };
        BoolSetting   useWebSocketsOverTls = new("-4c86-a54d-0b64a40fe8d5", "Use WebSockets over TLS", ++settingsSortIndex, false) { Description = "WebSocket Secure" };
        UShortSetting webSocketPort        = new("c89bc913-", "WebSocket Port", ++settingsSortIndex, true, 4444);
        StringSetting shutdownActions      = new("9acd45ec-e8b9-45ad-b136-ecdeecd87074", "Shutdown Actions", ++settingsSortIndex, false, HtmlFieldType.Text, null);
        StringSetting startupActions       = new("caf5ff1a5983", "Startup Actions", ++settingsSortIndex, false, HtmlFieldType.Text, null);
    }
}