// @formatter:empty_block_style together_same_line

// ReSharper disable UnusedParameter.Global

namespace ReSharperPlugin.QuirkyFormatting.Tests.test.data;

internal abstract class AbstractSetting
{
  public int HtmlFieldWidth { get; set; }
  public string Description { get; set; }
}

internal sealed class StringSetting : AbstractSetting
{
  public StringSetting(object o1, object o2 = null, object o3 = null, object o4 = null, object o5 = null, object o6 = null) { }
}

internal sealed class UShortSetting : AbstractSetting
{
  public UShortSetting(object o1, object o2 = null, object o3 = null, object o4 = null, object o5 = null, object o6 = null) { }
}

internal sealed class IntSetting : AbstractSetting
{
  public IntSetting(object o1, object o2 = null, object o3 = null, object o4 = null, object o5 = null, object o6 = null) { }
}

internal sealed class BoolSetting : AbstractSetting
{
  public BoolSetting(object o1, object o2 = null, object o3 = null, object o4 = null, object o5 = null, object o6 = null) { }
}

internal enum HtmlFieldType
{
  Text = 6,
  Url = 8,
}

public static class Caller
{
  public static void F1(object o1, object o2 = null, object o3 = null, object o4 = null, object o5 = null, object o6 = null) { }
  public static void F2(object o1, object o2 = null, object o3 = null, object o4 = null, object o5 = null, object o6 = null) { }
  public static void F3(object o1, object o2 = null, object o3 = null, object o4 = null, object o5 = null, object o6 = null) { }
  public static void F4(object o1, object o2 = null, object o3 = null, object o4 = null, object o5 = null, object o6 = null) { }
}