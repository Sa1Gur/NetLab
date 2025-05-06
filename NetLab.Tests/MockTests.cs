using Microsoft.Extensions.Logging;
using Moq;
using NetLab.Common;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace NetLab.Tests;

/// <summary>
/// Tests that don't require WebAssembly runtime and can run in any environment
/// </summary>
public class MockTests
{
    private readonly ITestOutputHelper _output;
    private readonly Mock<ILoggerFactory> _mockLoggerFactory;
    private readonly Mock<ILogger<Compiler>> _mockLogger;

    public MockTests(ITestOutputHelper output)
    {
        _output = output;
        _mockLogger = new Mock<ILogger<Compiler>>();
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(_mockLogger.Object);
    }

    [Fact]
    public void CompileResult_Constructor_InitializesProperties()
    {
        // Arrange
        List<Diagnostic> diagnostics = new List<Diagnostic>();
        string decompiled = "decompiled code";
        List<string> outputs = new List<string> { "output1", "output2" };

        // Act
        CompileResult result = new CompileResult(diagnostics, decompiled, outputs);

        // Assert
        Assert.Same(diagnostics, result.Diagnostics);
        Assert.Equal(decompiled, result.Decompiled);
        Assert.Same(outputs, result.Outputs);
    }

    [Fact]
    public void CompletionItem_Constructor_InitializesProperties()
    {
        // Arrange
        string displayText = "DisplayText";
        string filterText = "FilterText";
        string sortText = "SortText";
        string inlineDescription = "InlineDescription";
        System.Collections.Immutable.ImmutableArray<string> tags = System.Collections.Immutable.ImmutableArray.Create("Tag1", "Tag2");
        Microsoft.CodeAnalysis.Text.TextSpan span = new Microsoft.CodeAnalysis.Text.TextSpan(0, 10);

        // Act
        CompletionItem item = new CompletionItem(displayText, filterText, sortText, inlineDescription, tags, span);

        // Assert
        Assert.Equal(displayText, item.DisplayText);
        Assert.Equal(filterText, item.FilterText);
        Assert.Equal(sortText, item.SortText);
        Assert.Equal(inlineDescription, item.InlineDescription);
        Assert.Equal(tags, item.Tags);
        Assert.Equal(span, item.Span);
    }

    [Fact]
    public void InfoTipItem_Constructor_InitializesProperties()
    {
        // Arrange
        System.Collections.Immutable.ImmutableArray<string> tags = System.Collections.Immutable.ImmutableArray.Create("Tag1", "Tag2");
        Microsoft.CodeAnalysis.Text.TextSpan span = new Microsoft.CodeAnalysis.Text.TextSpan(0, 10);
        InfoTipSection[] sections = new[] { new InfoTipSection("Kind1", new InfoTipTaggedText("Tag1", "Text1")) };

        // Act
        InfoTipItem item = new InfoTipItem(tags, span, sections);

        // Assert
        Assert.Equal(tags, item.Tags);
        Assert.Equal(span, item.Span);
        Assert.Equal(sections, item.Sections);
    }

    [Fact]
    public void InfoTipSection_Constructor_InitializesProperties()
    {
        // Arrange
        string kind = "Kind1";
        InfoTipTaggedText[] parts = new[] { new InfoTipTaggedText("Tag1", "Text1") };

        // Act
        InfoTipSection section = new InfoTipSection(kind, parts);

        // Assert
        Assert.Equal(kind, section.Kind);
        Assert.Equal(parts, section.Parts);
    }

    [Fact]
    public void InfoTipTaggedText_Constructor_InitializesProperties()
    {
        // Arrange
        string tag = "Tag1";
        string text = "Text1";

        // Act
        InfoTipTaggedText taggedText = new InfoTipTaggedText(tag, text);

        // Assert
        Assert.Equal(tag, tag);
        Assert.Equal(text, text);
    }

    [Fact]
    public void LanguageType_Enum_HasExpectedValues()
    {
        // Assert
        Assert.Equal(0b011, (int)LanguageType.CSharp);
        Assert.Equal(0b111, (int)LanguageType.VisualBasic);
        Assert.Equal(0b001, (int)LanguageType.IL);
    }

    [Fact]
    public void OutputType_Enum_HasExpectedValues()
    {
        // Assert
        Assert.Equal(0b011, (int)OutputType.CSharp);
        Assert.Equal(0b001, (int)OutputType.IL);
        Assert.Equal(0b100, (int)OutputType.Run);
    }
}