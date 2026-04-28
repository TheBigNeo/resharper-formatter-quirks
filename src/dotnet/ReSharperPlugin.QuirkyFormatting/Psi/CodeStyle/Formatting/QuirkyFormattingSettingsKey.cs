// ReSharper disable InconsistentNaming

using JetBrains.Application.Settings;
using JetBrains.ReSharper.Psi.CodeStyle;

namespace JetBrains.ReSharper.Plugins.QuirkyFormatting.Psi.CodeStyle.Formatting;

[SettingsKey(typeof(CodeFormattingSettingsKey), "Quirky formatter settings")]
public class QuirkyFormattingSettingsKey
{
    [SettingsEntry(AlignCommaPosition.DoNotChange, "Align argument commas by position across sibling declarations")]
    public AlignCommaPosition INT_ALIGN_COMMA_AFTER_ARGUMENT_IN_CONSTRUCTOR;

    [SettingsEntry(false, "Align argument commas by position across sibling declarations")]
    public bool INT_ALIGN_COMMA_AFTER_ARGUMENT_IN_FUNCTION;

    [SettingsEntry(false, "Align '(' after type name in 'new T(' constructor calls")]
    public bool INT_ALIGN_NEW_LPARENTH;


    [SettingsEntry(false, "Align object initializer '{' after constructor ')'")]
    public bool INT_ALIGN_INITIALIZER_LBRACE;

    [SettingsEntry(false, "Align '=' in object initializer member assignments")]
    public bool INT_ALIGN_MEMBER_INIT_EQ;
}