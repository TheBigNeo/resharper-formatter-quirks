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

        QuirkyAlignParameters();
    }


    private void QuirkyAlignParameters()
    {
        // Helper to get the containing block offset for grouping alignment columns across var declarations
        static long? GetContainingBlockOffset(ITreeNode node)
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


        // ============================================
        // ======= Align Different Method Names at (
        // ============================================
        // Column 2: align '(' after type name in "new TypeName(" inside a local var declaration
        // The left of '(' in 'new TypeName(' is ITypeUsage (the type reference), not IReferenceName
        DescribeWithExternalKey<QuirkyFormattingSettingsKey, IntAlignRule>()
            .Name("ALIGN_PARAMETERS_LPARENTH")
            .Where(
                Left().Satisfies((node, _) => node is ITypeUsage),
                Right().HasType(CSharpTokenType.LPARENTH),
                Parent().Satisfies((node, _) => node is IObjectCreationExpression)
            )
            .SwitchOnExternalKey(
                x => x.INT_ALIGN_PARAMETERS,
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

        // Column 3: align ',' after each argument at position N — one rule for all indices.
        // The group key encodes both the argument position and the containing block, so commas
        // at the same position across sibling var-declarations are aligned as a column.
        // The argument index is determined by counting COMMA tokens in the IArgumentList that
        // precede the Right() COMMA token — i.e. commaIndex == argIndex of the left argument.
        DescribeWithExternalKey<QuirkyFormattingSettingsKey, IntAlignRule>()
            .Name("ALIGN_PARAMETERS_ARG_COMMA")
            .Where(
                Left().Satisfies((node, _) => node is ICSharpArgument),
                Right().HasType(CSharpTokenType.COMMA),
                Parent().Satisfies((node, _) => node is IArgumentList)
            )
            .SwitchOnExternalKey(
                x => x.INT_ALIGN_PARAMETERS,
                When(true).Calculate((formattingRangeContext, _) =>
                    {
                        if (formattingRangeContext == null) return null;
                        var ctx = (FormattingRangeContext)formattingRangeContext;

                        // ctx.Parent is IArgumentList; find the index of the argument to the left
                        // of this comma by walking the structured Arguments list.
                        // For each argument, find its index, then check if the next significant
                        // sibling token is a COMMA — the first match gives us the arg index.
                        if (ctx.Parent is not IArgumentList argList) return null;
                        var arguments = argList.Arguments;
                        var argIndex  = 0;
                        for (var i = 0; i < arguments.Count; i++)
                        {
                            // Walk right from this argument to find the next significant token
                            var sibling = arguments[i].NextSibling;
                            while (sibling != null && sibling is IWhitespaceNode)
                                sibling = sibling.NextSibling;
                            if (sibling is ITokenNode nextToken && nextToken.GetTokenType() == CSharpTokenType.COMMA)
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
                                return new IntAlignOptionValue($"Params$ArgComma{argIndex}$Block{blockOffset}", QuirkyPriority);
                            }

                            n = n.Parent;
                        }

                        return null;
                    }
                )
            )
            .Build();

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
                x => x.INT_ALIGN_PARAMETERS,
                When(true).Calculate((formattingRangeContext, _) =>
                    {
                        if (formattingRangeContext == null) return null;
                        var       ctx = (FormattingRangeContext)formattingRangeContext;
                        ITreeNode n4  = ctx.Parent;
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
                x => x.INT_ALIGN_PARAMETERS,
                When(true).Calculate((formattingRangeContext, _) =>
                    {
                        if (formattingRangeContext == null) return null;
                        var       ctx = (FormattingRangeContext)formattingRangeContext;
                        ITreeNode n5  = ctx.Parent;
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

    public override IEnumerable<IScalarSetting<bool>> PureIntAlignSettings()
    {
        // yield return CalculatedSettingsSchema.GetScalarSetting((QuirkyFormattingSettingsKey x) => x.INT_ALIGN_ATTRIBUTE_COMMAS);
        yield return CalculatedSettingsSchema.GetScalarSetting((QuirkyFormattingSettingsKey x) => x.INT_ALIGN_PARAMETERS);
    }
}