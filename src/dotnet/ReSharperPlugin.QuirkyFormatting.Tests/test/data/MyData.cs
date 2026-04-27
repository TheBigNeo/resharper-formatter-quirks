namespace ReSharperPlugin.QuirkyFormatting.Tests.test.data;

internal sealed class StringSetting
{
    public StringSetting(object o1, object o2 = null, object o3 = null, object o4 = null, object o5 = null, object o6 = null)
    {
    }

    public int HtmlFieldWidth { get; set; }
    public string Description { get; set; }
}

internal sealed class UShortSetting
{
    public UShortSetting(object o1, object o2 = null, object o3 = null, object o4 = null, object o5 = null, object o6 = null)
    {
    }

    public int HtmlFieldWidth { get; set; }
    public string Description { get; set; }
}

internal sealed class IntSetting
{
    public IntSetting(object o1, object o2 = null, object o3 = null, object o4 = null, object o5 = null, object o6 = null)
    {
    }

    public int HtmlFieldWidth { get; set; }
    public string Description { get; set; }
}

internal sealed class BoolSetting
{
    public BoolSetting(object o1, object o2 = null, object o3 = null, object o4 = null, object o5 = null, object o6 = null)
    {
    }

    public int HtmlFieldWidth { get; set; }
    public string Description { get; set; }
}

internal enum HtmlFieldType
{
    Text = 6,
    Url = 8,
}