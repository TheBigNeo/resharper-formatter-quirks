// ReSharper disable InconsistentNaming

using JetBrains.Application.Settings;
using JetBrains.ReSharper.Psi.CodeStyle;

namespace JetBrains.ReSharper.Plugins.QuirkyFormatting.Psi.CodeStyle.Formatting;

[SettingsKey(typeof(CodeFormattingSettingsKey), "Quirky formatter settings")]
public class QuirkyFormattingSettingsKey
{
    // [SettingsEntry(false, "Indent right brace banner-style")]
    // public bool BANNER_STYLE_RIGHT_BRACE;
    //
    // [SettingsEntry(true, "Enforce line breaks after left braces")]
    // public bool ENFORCE_LINE_BREAKS_AFTER_LEFT_BRACES;
    //
    // [SettingsEntry(true, "Enforce line breaks between statements")]
    // public bool ENFORCE_LINE_BREAKS_BETWEEN_STATEMENTS;
    //
    // [SettingsEntry(true, "Enforce line breaks between a statement in which a local function is used and it's declaration")]
    // public bool ENFORCE_LOCAL_FUNCTION_DECLARATION_AND_INVOCATION_LINEBREAKS;
    //
    // [SettingsEntry(false, "Align commas in attribute usages")]
    // public bool INT_ALIGN_ATTRIBUTE_COMMAS;

    [SettingsEntry(false, "Align '(' after type name in 'new T(' constructor calls")]
    public bool INT_ALIGN_NEW_LPARENTH;

    [SettingsEntry(false, "Align argument commas by position across sibling declarations")]
    public bool INT_ALIGN_ARG_COMMA;

    [SettingsEntry(false, "Align object initializer '{' after constructor ')'")]
    public bool INT_ALIGN_INITIALIZER_LBRACE;

    [SettingsEntry(false, "Align '=' in object initializer member assignments")]
    public bool INT_ALIGN_MEMBER_INIT_EQ;
}
