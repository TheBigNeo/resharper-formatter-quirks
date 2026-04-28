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
[TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.WRAP_LINES),            false)]
[TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.INT_ALIGN_INVOCATIONS), true)]
public class CSharpQuirkyFormattingTest : CodeFormatterWithExplicitSettingsTestBase<CSharpLanguage>
{
  protected override string RelativeTestDataPath => "CodeFormatter/CSharp";


  [Test]
  public void TestMyReference()
  {
    DoNamedTest2();
  }


  [Test]
  [TestSetting(typeof(QuirkyFormattingSettingsKey), nameof(QuirkyFormattingSettingsKey.INT_ALIGN_NEW_LPARENTH), true)]
  public void TestAlignNewLParenth()
  {
    DoNamedTest2();
  }

  [Test]
  [TestSetting(typeof(QuirkyFormattingSettingsKey), nameof(QuirkyFormattingSettingsKey.INT_ALIGN_COMMA_AFTER_ARGUMENT_IN_CONSTRUCTOR), AlignCommaPosition.SpaceBeforeComma)]
  public void TestAlignCommaAfterArgumentInConstructor()
  {
    DoNamedTest2();
  }

  [Test]
  [TestSetting(typeof(QuirkyFormattingSettingsKey), nameof(QuirkyFormattingSettingsKey.INT_ALIGN_COMMA_AFTER_ARGUMENT_IN_CONSTRUCTOR), AlignCommaPosition.SpaceAfterComma)]
  public void TestAlignCommaAfterArgumentInConstructorSpaceAfterComma()
  {
    DoNamedTest2();
  }

  [Test]
  [TestSetting(typeof(QuirkyFormattingSettingsKey), nameof(QuirkyFormattingSettingsKey.INT_ALIGN_INITIALIZER_LBRACE), true)]
  public void TestAlignInitializerLBrace()
  {
    DoNamedTest2();
  }

  [Test]
  [TestSetting(typeof(QuirkyFormattingSettingsKey), nameof(QuirkyFormattingSettingsKey.INT_ALIGN_MEMBER_INIT_EQ), true)]
  public void TestAlignMemberInitEq()
  {
    DoNamedTest2();
  }

  [Test]
  [TestSetting(typeof(CSharpFormatSettingsKey), nameof(CSharpFormatSettingsKey.INT_ALIGN_VARIABLES),   true)]
  //
  [TestSetting(typeof(QuirkyFormattingSettingsKey), nameof(QuirkyFormattingSettingsKey.INT_ALIGN_NEW_LPARENTH),         true)]
  [TestSetting(typeof(QuirkyFormattingSettingsKey), nameof(QuirkyFormattingSettingsKey.INT_ALIGN_COMMA_AFTER_ARGUMENT_IN_CONSTRUCTOR), AlignCommaPosition.SpaceBeforeComma)]
  [TestSetting(typeof(QuirkyFormattingSettingsKey), nameof(QuirkyFormattingSettingsKey.INT_ALIGN_INITIALIZER_LBRACE),   true)]
  [TestSetting(typeof(QuirkyFormattingSettingsKey), nameof(QuirkyFormattingSettingsKey.INT_ALIGN_MEMBER_INIT_EQ),       true)]
  public void TestCombined()
  {
    DoNamedTest2();
  }
}