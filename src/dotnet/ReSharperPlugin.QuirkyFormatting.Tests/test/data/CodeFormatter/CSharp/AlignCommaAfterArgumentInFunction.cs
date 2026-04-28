using JetBrains.ReSharper.Plugins.QuirkyFormatting.Psi.CodeStyle.Formatting;
using ReSharperPlugin.QuirkyFormatting.Tests.test.data;

/// <seealso cref="QuirkyFormattingSettingsKey.INT_ALIGN_ARGUMENTS_IN_FUNCTION"/>
internal class AlignCommaAfterArgumentInFunction
{
  static void CallFunctions()
  {
    Caller.F1("T-Rex",  "Raptor",        "Moros",         "Triceratops");
    Caller.F1("Ankylo", "Tyrannosaurus", "Indominus rex", "Brachio");
    Caller.F2("Gallimimus", "Indor", "Carno", "Spino");
    Caller.F1("Veloci", "Baryo", "Moso", "Dilophosaurus");
    Caller.F3("T-Rex", "Raptor", "Moros", "Triceratops");
    Caller.F1("Ankylo", "Tyrannosaurus", "Indominus rex", "Brachio");
    Caller.F4("Gallimimus", "Indor", "Carno", "Spino");
    Caller.F1("Veloci", "Baryo", "Moso", "Dilophosaurus");
  }
}