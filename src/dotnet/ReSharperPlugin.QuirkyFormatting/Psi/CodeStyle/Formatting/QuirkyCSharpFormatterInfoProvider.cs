using System.Collections.Generic;
using System.Linq;
using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Calculated.Interface;
using JetBrains.Application.Threading;
using JetBrains.Lifetimes;
using JetBrains.ReSharper.Plugins.QuirkyFormatting.Psi.CodeStyle.Formatting;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Impl.CodeStyle.Formatter;
using JetBrains.ReSharper.Psi.CSharp.Impl.Tree;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Impl.CodeStyle;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;

namespace JetBrains.ReSharper.Plugins.QuirkyFormattings.Psi.CodeStyle.Formatting;

[Language(typeof(CSharpLanguage))]
public class QuirkyCSharpFormatterInfoProvider : CSharpFormatterInfoProviderPart
{
    private const int WeakQuirkyPriority = 99_999;
    private const int QuirkyPriority = 100_000;

    public QuirkyCSharpFormatterInfoProvider(
        ISettingsSchema settingsSchema,
        ICalculatedSettingsSchema calculatedSettingsSchema,
        IThreading threading, Lifetime lifetime
    ) : base(settingsSchema, calculatedSettingsSchema, threading, lifetime)
    {
    }

    protected override void Initialize()
    {
        base.Initialize();

        // Put the descriptions of the rules here

        // Some possible rule types to put into Describe<TRuleType> or DescribeWithExternalKey<TKey, TRuleType>:
        // FormattingRule  -> To describe a calculation of line breaks between some two tree nodes.
        // ElementListRule -> To describe how a sequence of elements should be formatted (i.e. the list in "int a, b, c = 3, d;") 
        // IntAlignRule    -> To describe some kind of column-based formatting
        // ...

        INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR_SpaceBeforeComma();
        INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR_SpaceAfterComma();
        INT_ALIGN_ARGUMENTS_IN_FUNCTION_SpaceBeforeComma();
        INT_ALIGN_ARGUMENTS_IN_FUNCTION_SpaceAfterComma();
        AddALIGN_PARAMETERS_LPARENTH();
        AddALIGN_PARAMETERS_INITIALIZER_LBRACE();
        AddALIGN_PARAMETERS_MEMBER_INIT_EQ();
    }

    private void INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR_SpaceBeforeComma()
        => BuildIntAlignArgumentsInConstructorRule(AlignCommaPosition.SpaceBeforeComma);

    private void INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR_SpaceAfterComma()
        => BuildIntAlignArgumentsInConstructorRule(AlignCommaPosition.SpaceAfterComma);

    private void BuildIntAlignArgumentsInConstructorRule(AlignCommaPosition position)
    {
        var isSpaceBefore = position == AlignCommaPosition.SpaceBeforeComma;
        var nameSuffix = isSpaceBefore ? "" : "_SpaceAfter";
        var keyPrefix = isSpaceBefore ? "ArgComma" : "ArgAfterComma";

        var leftCondition = isSpaceBefore
            ? Left().Satisfies((node, _) => node is ICSharpArgument)
            : Left().HasType(CSharpTokenType.COMMA);

        var rightCondition = isSpaceBefore
            ? Right().HasType(CSharpTokenType.COMMA)
            : Right().Satisfies((node, _) => node is ICSharpArgument);

        DescribeWithExternalKey<QuirkyFormattingSettingsKey, IntAlignRule>()
            .Name(nameof(QuirkyFormattingSettingsKey.INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR) + nameSuffix)
            .Where(
                leftCondition,
                rightCondition,
                Parent().Satisfies((node, _) => node is IArgumentList)
            )
            .SwitchOnExternalKey(
                x => x.INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR,
                When(position).Calculate((formattingRangeContext, _) =>
                    {
                        if (formattingRangeContext == null) return null;
                        var ctx = (FormattingRangeContext)formattingRangeContext;

                        if (ctx.Parent is not IArgumentList argList) return null;
                        var arguments = argList.Arguments;
                        var argIndex = 0;
                        for (var i = 0; i < arguments.Count; i++)
                        {
                            var sibling = isSpaceBefore ? arguments[i].NextSibling : arguments[i].PrevSibling;
                            while (sibling != null && sibling is IWhitespaceNode)
                                sibling = isSpaceBefore ? sibling.NextSibling : sibling.PrevSibling;
                            if (sibling is ITokenNode token && token.GetTokenType() == CSharpTokenType.COMMA)
                            {
                                argIndex = i;
                                break;
                            }
                        }

                        ITreeNode n = ctx.Parent;
                        while (n != null)
                        {
                            if (n is IDeclarationStatement)
                            {
                                var blockOffset = GetContainingBlockOffset(n);
                                if (blockOffset == null) return null;
                                return new IntAlignOptionValue($"Params${keyPrefix}{argIndex}$Block{blockOffset}", QuirkyPriority);
                            }

                            n = n.Parent;
                        }

                        return null;
                    }
                )
            )
            .Build();
    }

    private void INT_ALIGN_ARGUMENTS_IN_FUNCTION_SpaceBeforeComma()
        => BuildIntAlignArgumentsInFunctionRule(AlignCommaPosition.SpaceBeforeComma);

    private void INT_ALIGN_ARGUMENTS_IN_FUNCTION_SpaceAfterComma()
        => BuildIntAlignArgumentsInFunctionRule(AlignCommaPosition.SpaceAfterComma);

    private void BuildIntAlignArgumentsInFunctionRule(AlignCommaPosition position)
    {
        var isSpaceBefore = position == AlignCommaPosition.SpaceBeforeComma;
        var nameSuffix = isSpaceBefore ? "" : "_SpaceAfter";
        var keyPrefix = isSpaceBefore ? "ArgComma" : "ArgAfterComma";

        var leftCondition = isSpaceBefore
            ? Left().Satisfies((node, _) => node is ICSharpArgument)
            : Left().HasType(CSharpTokenType.COMMA);
        var rightCondition = isSpaceBefore
            ? Right().HasType(CSharpTokenType.COMMA)
            : Right().Satisfies((node, _) => node is ICSharpArgument);

        DescribeWithExternalKey<QuirkyFormattingSettingsKey, IntAlignRule>()
            .Name(nameof(QuirkyFormattingSettingsKey.INT_ALIGN_ARGUMENTS_IN_FUNCTION) + nameSuffix)
            .Where(
                leftCondition,
                rightCondition,
                Parent().Satisfies((node, _) => node is IArgumentList)
            )
            .SwitchOnExternalKey(
                x => x.INT_ALIGN_ARGUMENTS_IN_FUNCTION,
                When(position).Calculate((formattingRangeContext, _) =>
                    {
                        if (formattingRangeContext == null) return null;
                        var ctx = (FormattingRangeContext)formattingRangeContext;

                        if (ctx.Parent is not IArgumentList argList) return null;
                        var arguments = argList.Arguments;
                        var argIndex = 0;
                        for (var i = 0; i < arguments.Count; i++)
                        {
                            var sibling = isSpaceBefore ? arguments[i].NextSibling : arguments[i].PrevSibling;
                            while (sibling != null && sibling is IWhitespaceNode)
                                sibling = isSpaceBefore ? sibling.NextSibling : sibling.PrevSibling;
                            if (sibling is ITokenNode token && token.GetTokenType() == CSharpTokenType.COMMA)
                            {
                                argIndex = i;
                                break;
                            }
                        }

                        ITreeNode n = ctx.Parent;
                        while (n != null)
                        {
                            if (n is IExpressionStatement)
                            {
                                var blockOffset = GetContainingBlockOffset(n);
                                if (blockOffset == null) return null;

                                // Derive a stable method-name key from the invocation so that only
                                // calls to the same method are grouped into the same alignment column.
                                var methodName = "";
                                if (argList.Parent is IInvocationExpression invocation)
                                    methodName = invocation.InvokedExpression.GetText();

                                return new IntAlignOptionValue($"Func${keyPrefix}{argIndex}${methodName}$Block{blockOffset}", QuirkyPriority);
                            }

                            n = n.Parent;
                        }

                        return null;
                    }
                )
            )
            .Build();
    }


    private void AddALIGN_PARAMETERS_LPARENTH()
    {
        // ============================================
        // ======= Align Different Method Names at (
        // ============================================
        // Column 2: align '(' after type name in "new TypeName(" inside a local var declaration
        // The left of '(' in 'new TypeName(' is ITypeUsage (the type reference), not IReferenceName
        DescribeWithExternalKey<QuirkyFormattingSettingsKey, IntAlignRule>()
            .Name("ALIGN_PARAMETERS_LPARENTH")
            .Where(
                Left().Satisfies((node, _) => node is ITypeUsage)
                    .Or().HasType(CSharpTokenType.NEW_KEYWORD),
                Right().HasType(CSharpTokenType.LPARENTH),
                Parent().Satisfies((node, _) => node is IObjectCreationExpression)
            )
            .SwitchOnExternalKey(
                x => x.INT_ALIGN_NEW_LPARENTH,
                When(true).Calculate((formattingRangeContext, _) =>
                    {
                        if (formattingRangeContext == null) return null;
                        var ctx = (FormattingRangeContext)formattingRangeContext;
                        // Walk up to find the var declaration statement
                        ITreeNode n = ctx.Parent;
                        while (n != null)
                        {
                            if (n is IDeclarationStatement)
                            {
                                var blockOffset = GetContainingBlockOffset(n);
                                if (blockOffset == null) return null;
                                return new IntAlignOptionValue($"Params$LParen$Block{blockOffset}", QuirkyPriority);
                            }

                            n = n.Parent;
                        }

                        return null;
                    }
                )
            )
            .Build();
    }

    private void AddALIGN_PARAMETERS_INITIALIZER_LBRACE()
    {
        // Column 4: align the object initializer '{...}' node after the closing ')' of constructor args
        // The Right() is IObjectInitializer (composite), not the '{' token directly
        DescribeWithExternalKey<QuirkyFormattingSettingsKey, IntAlignRule>()
            .Name("ALIGN_PARAMETERS_INITIALIZER_LBRACE")
            .Where(
                Left().HasType(CSharpTokenType.RPARENTH),
                Right().Satisfies((node, _) => node is IObjectInitializer),
                Parent().Satisfies((node, _) => node is IObjectCreationExpression)
            )
            .SwitchOnExternalKey(
                x => x.INT_ALIGN_INITIALIZER_LBRACE,
                When(true).Calculate((formattingRangeContext, _) =>
                    {
                        if (formattingRangeContext == null) return null;
                        var ctx = (FormattingRangeContext)formattingRangeContext;
                        ITreeNode n4 = ctx.Parent;
                        while (n4 != null)
                        {
                            if (n4 is IDeclarationStatement)
                            {
                                var blockOffset = GetContainingBlockOffset(n4);
                                if (blockOffset == null) return null;
                                return new IntAlignOptionValue($"Params$InitLBrace$Block{blockOffset}", QuirkyPriority);
                            }

                            n4 = n4.Parent;
                        }

                        return null;
                    }
                )
            )
            .Build();
    }

    private void AddALIGN_PARAMETERS_MEMBER_INIT_EQ()
    {
        // Align left hand side from = in object initializer
        // Column 5: align '=' (Operator) in object initializer member assignments
        // Parent is IMemberInitializer; Left() is the identifier token, Right() is the EQ token
        DescribeWithExternalKey<QuirkyFormattingSettingsKey, IntAlignRule>()
            .Name("ALIGN_PARAMETERS_MEMBER_INIT_EQ")
            .Where(
                Left().Satisfies((node, _) => node is ITokenNode t && t.Parent is IMemberInitializer),
                Right().HasType(CSharpTokenType.EQ),
                Parent().Satisfies((node, _) => node is IMemberInitializer)
            )
            .SwitchOnExternalKey(
                x => x.INT_ALIGN_MEMBER_INIT_EQ,
                When(true).Calculate((formattingRangeContext, _) =>
                    {
                        if (formattingRangeContext == null) return null;
                        var ctx = (FormattingRangeContext)formattingRangeContext;
                        ITreeNode n5 = ctx.Parent;
                        while (n5 != null)
                        {
                            if (n5 is IDeclarationStatement)
                            {
                                var blockOffset = GetContainingBlockOffset(n5);
                                if (blockOffset == null) return null;
                                return new IntAlignOptionValue($"Params$MemberEq$Block{blockOffset}", QuirkyPriority);
                            }

                            n5 = n5.Parent;
                        }

                        return null;
                    }
                )
            )
            .Build();
    }

    private static long? GetContainingBlockOffset(ITreeNode node)
    {
        var n = node;
        while (n != null)
        {
            if (n is IBlock block)
                return block.GetDocumentStartOffset().Offset;
            n = n.Parent;
        }

        return null;
    }


    public override IEnumerable<IScalarSetting<bool>> PureIntAlignSettings()
    {
        // yield return CalculatedSettingsSchema.GetScalarSetting((QuirkyFormattingSettingsKey x) => x.INT_ALIGN_ATTRIBUTE_COMMAS);
        // yield return CalculatedSettingsSchema.GetScalarSetting((QuirkyFormattingSettingsKey x) => x.INT_ALIGN_NEW_LPARENTH);
        // yield return CalculatedSettingsSchema.GetScalarSetting((QuirkyFormattingSettingsKey x) => x.INT_ALIGN_ARG_COMMA);
        // yield return CalculatedSettingsSchema.GetScalarSetting((QuirkyFormattingSettingsKey x) => x.INT_ALIGN_INITIALIZER_LBRACE);
        // yield return CalculatedSettingsSchema.GetScalarSetting((QuirkyFormattingSettingsKey x) => x.INT_ALIGN_MEMBER_INIT_EQ);

        yield break;
    }
}