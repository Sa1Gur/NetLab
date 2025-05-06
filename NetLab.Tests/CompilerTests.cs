using Microsoft.Extensions.Logging;
using NetLab.Common;

namespace NetLab.Tests;

public class CompilerTests
{
    private readonly Mock<ILoggerFactory> _mockLoggerFactory;
    private readonly Mock<ILogger<Compiler>> _mockLogger;
    private readonly ITestOutputHelper _output;

    public CompilerTests(ITestOutputHelper output)
    {
        _output = output;
        _mockLogger = new Mock<ILogger<Compiler>>();
        _mockLoggerFactory = new Mock<ILoggerFactory>();
        _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(_mockLogger.Object);
    }

    [Fact]
    public void Constructor_InitializesWithDefaultValues()
    {
        // Arrange & Act
        Compiler compiler = new Compiler(_mockLoggerFactory.Object);

        // Assert
        Assert.Equal(LanguageType.CSharp, compiler.LanguageType);
        Assert.Equal(OutputType.Run, compiler.OutputType);
        Assert.IsType<CSharpInputOptions>(compiler.InputOptions);
        Assert.IsType<RunOutputOptions>(compiler.OutputOptions);
    }

    [Fact]
    public void LanguageType_WhenChanged_UpdatesInputOptions()
    {
        // Arrange
        Compiler compiler = new Compiler(_mockLoggerFactory.Object);

        // Act
        compiler.LanguageType = LanguageType.VisualBasic;

        // Assert
        Assert.Equal(LanguageType.VisualBasic, compiler.LanguageType);
        Assert.IsType<VisualBasicInputOptions>(compiler.InputOptions);
    }

    [Fact]
    public void OutputType_WhenChanged_UpdatesOutputOptions()
    {
        // Arrange
        Compiler compiler = new Compiler(_mockLoggerFactory.Object);

        // Act
        compiler.OutputType = OutputType.CSharp;

        // Assert
        Assert.Equal(OutputType.CSharp, compiler.OutputType);
        Assert.IsType<CSharpOutputOptions>(compiler.OutputOptions);
    }

    [Fact(Skip = "Requires WebAssembly runtime")]
    public async Task GetDiagnosticsAsync_WithValidCSharpCode_ReturnsNoDiagnostics()
    {
        // Arrange
        Compiler compiler = new Compiler(_mockLoggerFactory.Object);
        string validCode = "using System; class Program { static void Main() { Console.WriteLine(\"Hello World\"); } }";

        // Act
        List<Diagnostic> diagnostics = await compiler.GetDiagnosticsAsync(validCode);

        // Assert
        Assert.Empty(diagnostics);
    }

    [Fact(Skip = "Requires WebAssembly runtime")]
    public async Task GetDiagnosticsAsync_WithInvalidCSharpCode_ReturnsDiagnostics()
    {
        // Arrange
        Compiler compiler = new Compiler(_mockLoggerFactory.Object);
        string invalidCode = "class Program { void Main() { undefinedVariable = 10; } }";

        // Act
        List<Diagnostic> diagnostics = await compiler.GetDiagnosticsAsync(invalidCode);

        // Assert
        Assert.NotEmpty(diagnostics);
        Assert.Contains(diagnostics, d => d.Message.Contains("undefinedVariable"));
    }

    [Fact(Skip = "Requires WebAssembly runtime")]
    public async Task GetCompletionsAsync_WithCSharpCode_ReturnsCompletions()
    {
        // Arrange
        Compiler compiler = new Compiler(_mockLoggerFactory.Object);
        string code = "using System; class Program { static void Main() { Console. } }";
        int position = code.IndexOf("Console.") + "Console.".Length;

        // Act
        IEnumerable<CompletionItem> completions = await compiler.GetCompletionsAsync(code, position);

        // Assert
        Assert.NotEmpty(completions);
        // Should contain common Console methods
        List<string> completionTexts = new List<string>();
        foreach (CompletionItem completion in completions)
        {
            completionTexts.Add(completion.DisplayText);
        }
        Assert.Contains(completionTexts, text => text == "WriteLine");
    }
}