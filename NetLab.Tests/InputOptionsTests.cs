using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.VisualBasic;
using NetLab.Common;

namespace NetLab.Tests;

public class InputOptionsTests
{
    private readonly ITestOutputHelper _output;

    public InputOptionsTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void CSharpInputOptions_HasCorrectLanguageName()
    {
        // Arrange
        CSharpInputOptions options = new CSharpInputOptions();

        // Act & Assert
        Assert.Equal(LanguageNames.CSharp, options.LanguageName);
    }

    [Fact]
    public void VisualBasicInputOptions_HasCorrectLanguageName()
    {
        // Arrange
        VisualBasicInputOptions options = new VisualBasicInputOptions();

        // Act & Assert
        Assert.Equal(LanguageNames.VisualBasic, options.LanguageName);
    }

    [Fact]
    public void CSharpInputOptions_LanguageVersion_DefaultIsPreview()
    {
        // Arrange
        CSharpInputOptions options = new CSharpInputOptions();

        // Act & Assert
        Assert.Equal(Microsoft.CodeAnalysis.CSharp.LanguageVersion.Preview, options.LanguageVersion);
    }

    [Fact]
    public void VisualBasicInputOptions_LanguageVersion_DefaultIsLatest()
    {
        // Arrange
        VisualBasicInputOptions options = new VisualBasicInputOptions();

        // Act & Assert
        Assert.Equal(Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.Latest, options.LanguageVersion);
    }

    [Fact(Skip = "Requires WebAssembly runtime")]
    public void RoslynOptions_GetOptions_ForCSharp_ReturnsCorrectOptions()
    {
        // Arrange
        CSharpInputOptions options = new CSharpInputOptions();

        // Act
        options.GetOptions(true, out CompilationOptions compilation, out ParseOptions parse);

        // Assert
        Assert.IsType<CSharpCompilationOptions>(compilation);
        Assert.IsType<CSharpParseOptions>(parse);
        Assert.Equal(OutputKind.ConsoleApplication, ((CSharpCompilationOptions)compilation).OutputKind);
    }

    [Fact(Skip = "Requires WebAssembly runtime")]
    public void RoslynOptions_GetOptions_ForVisualBasic_ReturnsCorrectOptions()
    {
        // Arrange
        VisualBasicInputOptions options = new VisualBasicInputOptions();

        // Act
        options.GetOptions(true, out CompilationOptions compilation, out ParseOptions parse);

        // Assert
        Assert.IsType<VisualBasicCompilationOptions>(compilation);
        Assert.IsType<VisualBasicParseOptions>(parse);
        Assert.Equal(OutputKind.ConsoleApplication, ((VisualBasicCompilationOptions)compilation).OutputKind);
    }

    [Fact(Skip = "Requires WebAssembly runtime")]
    public void RoslynOptions_GetOptions_WithIsConsoleFalse_ReturnsLibraryOutputKind()
    {
        // Arrange
        CSharpInputOptions options = new CSharpInputOptions();

        // Act
        options.GetOptions(false, out CompilationOptions compilation, out _);

        // Assert
        Assert.Equal(OutputKind.DynamicallyLinkedLibrary, ((CSharpCompilationOptions)compilation).OutputKind);
    }
}