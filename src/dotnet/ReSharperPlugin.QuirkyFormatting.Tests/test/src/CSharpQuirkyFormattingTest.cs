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


    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.WRAP_LINES), false)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.INT_ALIGN_VARIABLES), true)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.INT_ALIGN_INVOCATIONS), true)]
    [Test]
    public void TestMyReference()
    {
        DoNamedTest2();
    }

    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.WRAP_LINES), false)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.INT_ALIGN_VARIABLES), true)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.INT_ALIGN_INVOCATIONS), true)]
    [Test]
    public void TestMyReference2()
    {
        DoNamedTest2();
    }


    [TestSetting(typeof(QuirkyFormattingSettingsKey), nameof(QuirkyFormattingSettingsKey.INT_ALIGN_NEW_LPARENTH), true)]
    [TestSetting(typeof(QuirkyFormattingSettingsKey), nameof(QuirkyFormattingSettingsKey.INT_ALIGN_ARG_COMMA), true)]
    [TestSetting(typeof(QuirkyFormattingSettingsKey), nameof(QuirkyFormattingSettingsKey.INT_ALIGN_INITIALIZER_LBRACE), true)]
    [TestSetting(typeof(QuirkyFormattingSettingsKey), nameof(QuirkyFormattingSettingsKey.INT_ALIGN_MEMBER_INIT_EQ), true)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.WRAP_LINES), false)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.INT_ALIGN_VARIABLES), true)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.INT_ALIGN_INVOCATIONS), true)]
    [Test]
    public void TestAlignParameters()
    {
        DoNamedTest2();
    }

    [TestSetting(typeof(QuirkyFormattingSettingsKey), nameof(QuirkyFormattingSettingsKey.INT_ALIGN_NEW_LPARENTH), true)]
    [TestSetting(typeof(QuirkyFormattingSettingsKey), nameof(QuirkyFormattingSettingsKey.INT_ALIGN_ARG_COMMA), true)]
    [TestSetting(typeof(QuirkyFormattingSettingsKey), nameof(QuirkyFormattingSettingsKey.INT_ALIGN_INITIALIZER_LBRACE), true)]
    [TestSetting(typeof(QuirkyFormattingSettingsKey), nameof(QuirkyFormattingSettingsKey.INT_ALIGN_MEMBER_INIT_EQ), true)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.WRAP_LINES), false)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.INT_ALIGN_VARIABLES), true)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.INT_ALIGN_INVOCATIONS), true)]
    [Test]
    public void TestAlignParameters2()
    {
        DoNamedTest2();
    }

    [TestSetting(typeof(QuirkyFormattingSettingsKey), nameof(QuirkyFormattingSettingsKey.INT_ALIGN_NEW_LPARENTH), true)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.WRAP_LINES), false)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.INT_ALIGN_VARIABLES), true)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.INT_ALIGN_INVOCATIONS), true)]
    [Test]
    public void TestAlignNewLParenth()
    {
        DoNamedTest2();
    }

    [TestSetting(typeof(QuirkyFormattingSettingsKey), nameof(QuirkyFormattingSettingsKey.INT_ALIGN_ARG_COMMA), true)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.WRAP_LINES), false)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.INT_ALIGN_VARIABLES), true)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.INT_ALIGN_INVOCATIONS), true)]
    [Test]
    public void TestAlignArgComma()
    {
        DoNamedTest2();
    }

    [TestSetting(typeof(QuirkyFormattingSettingsKey), nameof(QuirkyFormattingSettingsKey.INT_ALIGN_INITIALIZER_LBRACE), true)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.WRAP_LINES), false)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.INT_ALIGN_VARIABLES), true)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.INT_ALIGN_INVOCATIONS), true)]
    [Test]
    public void TestAlignInitializerLBrace()
    {
        DoNamedTest2();
    }

    [TestSetting(typeof(QuirkyFormattingSettingsKey), nameof(QuirkyFormattingSettingsKey.INT_ALIGN_MEMBER_INIT_EQ), true)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.WRAP_LINES), false)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.INT_ALIGN_VARIABLES), true)]
    [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.INT_ALIGN_INVOCATIONS), true)]
    [Test]
    public void TestAlignMemberInitEq()
    {
        DoNamedTest2();
    }
}