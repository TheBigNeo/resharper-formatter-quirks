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

    // ── Constructor argument alignment ──────────────────────────────────────────

    private void INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR_SpaceBeforeComma()
        => BuildIntAlignArgumentsRule(new ArgumentAlignVariant(
            SettingName: nameof(QuirkyFormattingSettingsKey.INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR),
            ExternalKey: x => x.INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR,
            ParentPredicate: node => node is IArgumentList { Parent: IObjectCreationExpression },
            StatementType: typeof(IDeclarationStatement),
            AlignKeyGroup: "Params",
            Position: AlignCommaPosition.SpaceBeforeComma
        ));

    private void INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR_SpaceAfterComma()
        => BuildIntAlignArgumentsRule(new ArgumentAlignVariant(
            SettingName: nameof(QuirkyFormattingSettingsKey.INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR),
            ExternalKey: x => x.INT_ALIGN_ARGUMENTS_IN_CONSTRUCTOR,
            ParentPredicate: node => node is IArgumentList { Parent: IObjectCreationExpression },
            StatementType: typeof(IDeclarationStatement),
            AlignKeyGroup: "Params",
            Position: AlignCommaPosition.SpaceAfterComma
        ));

    // ── Function call argument alignment ────────────────────────────────────────

    private void INT_ALIGN_ARGUMENTS_IN_FUNCTION_SpaceBeforeComma()
        => BuildIntAlignArgumentsRule(new ArgumentAlignVariant(
            SettingName: nameof(QuirkyFormattingSettingsKey.INT_ALIGN_ARGUMENTS_IN_FUNCTION),
            ExternalKey: x => x.INT_ALIGN_ARGUMENTS_IN_FUNCTION,
            ParentPredicate: node => node is IArgumentList { Parent: IInvocationExpression },
            StatementType: typeof(IExpressionStatement),
            AlignKeyGroup: "Func",
            Position: AlignCommaPosition.SpaceBeforeComma
        ));

    private void INT_ALIGN_ARGUMENTS_IN_FUNCTION_SpaceAfterComma()
        => BuildIntAlignArgumentsRule(new ArgumentAlignVariant(
            SettingName: nameof(QuirkyFormattingSettingsKey.INT_ALIGN_ARGUMENTS_IN_FUNCTION),
            ExternalKey: x => x.INT_ALIGN_ARGUMENTS_IN_FUNCTION,
            ParentPredicate: node => node is IArgumentList { Parent: IInvocationExpression },
            StatementType: typeof(IExpressionStatement),
            AlignKeyGroup: "Func",
            Position: AlignCommaPosition.SpaceAfterComma
        ));

    // ── Shared builder ───────────────────────────────────────────────────────────


    /// <summary>Describes what makes each alignment variant unique; everything else is shared logic.</summary>
    private record ArgumentAlignVariant(
        string SettingName,
        System.Linq.Expressions.Expression<System.Func<QuirkyFormattingSettingsKey, object>> ExternalKey,
        System.Func<ITreeNode, bool> ParentPredicate,
        System.Type StatementType,
        string AlignKeyGroup,
        AlignCommaPosition Position
    );

    private void BuildIntAlignArgumentsRule(ArgumentAlignVariant variant)
    {
        var isSpaceBefore = variant.Position == AlignCommaPosition.SpaceBeforeComma;
        var nameSuffix = isSpaceBefore ? "_SpaceBefore" : "_SpaceAfter";
        var commaKeyPart = isSpaceBefore ? "ArgComma" : "ArgAfterComma";

        var leftCondition = isSpaceBefore
            ? Left().Satisfies((node, _) => node.NodeOrNull is ICSharpArgument)
            : Left().HasType(CSharpTokenType.COMMA);

        var rightCondition = isSpaceBefore
            ? Right().HasType(CSharpTokenType.COMMA)
            : Right().Satisfies((node, _) => node.NodeOrNull is ICSharpArgument);

        DescribeWithExternalKey<QuirkyFormattingSettingsKey, IntAlignRule>()
            .Name(variant.SettingName + nameSuffix)
            .Where(
                leftCondition,
                rightCondition,
                Parent().Satisfies((node, _) => variant.ParentPredicate(node.NodeOrNull))
            )
            .SwitchOnExternalKey(
                variant.ExternalKey,
                When(variant.Position).Calculate((formattingRangeContext, _) =>
                    {
                        if (formattingRangeContext is null)
                        {
                            return null;
                        }

                        var ctx = (FormattingRangeContext)formattingRangeContext;

                        if (ctx.Parent.NodeOrNull is not IArgumentList argList)
                        {
                            return null;
                        }

                        // Find the index of the argument adjacent to a comma
                        var argIndex = 0;
                        var arguments = argList.Arguments;
                        for (var i = 0; i < arguments.Count; i++)
                        {
                            var sibling = isSpaceBefore ? arguments[i].NextSibling : arguments[i].PrevSibling;

                            while (sibling is IWhitespaceNode)
                            {
                                sibling = isSpaceBefore ? sibling.NextSibling : sibling.PrevSibling;
                            }

                            if (sibling is ITokenNode token && token.GetTokenType() == CSharpTokenType.COMMA)
                            {
                                argIndex = i;
                                break;
                            }
                        }

                        // Walk up to find the enclosing statement and use its block offset as the alignment group
                        for (ITreeNode n = ctx.Parent.NodeOrNull; n != null; n = n.Parent)
                        {
                            if (!variant.StatementType.IsInstanceOfType(n)) continue;

                            var blockOffset = GetContainingBlockOffset(n);

                            if (blockOffset == null)
                            {
                                return null;
                            }

                            return new IntAlignOptionValue(
                                $"{variant.AlignKeyGroup}${commaKeyPart}{argIndex}$Block{blockOffset}",
                                QuirkyPriority
                            );
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
                Left().Satisfies((node, _) => node.NodeOrNull is ITypeUsage)
                    .Or().HasType(CSharpTokenType.NEW_KEYWORD),
                Right().HasType(CSharpTokenType.LPARENTH),
                Parent().Satisfies((node, _) => node.NodeOrNull is IObjectCreationExpression)
            )
            .SwitchOnExternalKey(
                x => x.INT_ALIGN_NEW_LPARENTH,
                When(true).Calculate((formattingRangeContext, _) =>
                    {
                        if (formattingRangeContext == null) return null;
                        var ctx = (FormattingRangeContext)formattingRangeContext;
                        // Walk up to find the var declaration statement
                        ITreeNode n = ctx.Parent.NodeOrNull;
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
                Right().Satisfies((node, _) => node.NodeOrNull is IObjectInitializer),
                Parent().Satisfies((node, _) => node.NodeOrNull is IObjectCreationExpression)
            )
            .SwitchOnExternalKey(
                x => x.INT_ALIGN_INITIALIZER_LBRACE,
                When(true).Calculate((formattingRangeContext, _) =>
                    {
                        if (formattingRangeContext == null) return null;
                        var ctx = (FormattingRangeContext)formattingRangeContext;
                        ITreeNode n4 = ctx.Parent.NodeOrNull;
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
                Left().Satisfies((node, _) => node.NodeOrNull is ITokenNode t && t.Parent is IMemberInitializer),
                Right().HasType(CSharpTokenType.EQ),
                Parent().Satisfies((node, _) => node.NodeOrNull is IMemberInitializer)
            )
            .SwitchOnExternalKey(
                x => x.INT_ALIGN_MEMBER_INIT_EQ,
                When(true).Calculate((formattingRangeContext, _) =>
                    {
                        if (formattingRangeContext == null) return null;
                        var ctx = (FormattingRangeContext)formattingRangeContext;
                        ITreeNode n5 = ctx.Parent.NodeOrNull;
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