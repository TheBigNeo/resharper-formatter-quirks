using JetBrains.ReSharper.FeaturesTestFramework.Formatter;
using JetBrains.ReSharper.Plugins.QuirkyFormatting.Psi.CodeStyle.Formatting;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.CodeStyle.FormatSettings;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;

// ReSharper disable once CheckNamespace
namespace ReSharperPlugin.QuirkyFormatting.Tests;

[TestQuirks]
[Category("Formatting"), Category("CSharp")]
public class CSharpQuirkyFormattingTest : CodeFormatterWithExplicitSettingsTestBase<CSharpLanguage>
{
    protected override string RelativeTestDataPath => "CodeFormatter/CSharp";


    [TestSetting(typeof(QuirkyFormattingSettingsKey), nameof(QuirkyFormattingSettingsKey.INT_ALIGN_PARAMETERS), true)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.WRAP_LINES), false)]
    // [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.INDENT_SIZE), 4)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.INT_ALIGN_VARIABLES), true)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.INT_ALIGN_INVOCATIONS), true)]
    [Test]
    public void TestAlignParameters()
    {
        DoNamedTest2();
    }
}