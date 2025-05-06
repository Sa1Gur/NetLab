using ICSharpCode.Decompiler.CSharp;
using NetLab.Common;

namespace NetLab.Tests;

public class OutputOptionsTests(ITestOutputHelper output)
{
    private readonly ITestOutputHelper _output = output;

    [Fact]
    public void CSharpOutputOptions_IsCSharp_ReturnsTrue()
    {
        // Arrange
        CSharpOutputOptions options = new();
        IOutputOptions iOutputOptions = options;

        // Act & Assert
        Assert.True(iOutputOptions.IsCSharp);
    }

    [Fact]
    public void ILOutputOptions_IsCSharp_ReturnsFalse()
    {
        // Arrange
        ILOutputOptions options = new ILOutputOptions();
        IOutputOptions iOutputOptions = (IOutputOptions)options;

        // Act & Assert
        Assert.False(iOutputOptions.IsCSharp);
    }

    [Fact]
    public void RunOutputOptions_IsCSharp_ReturnsFalse()
    {
        // Arrange
        RunOutputOptions options = new();

        // Act & Assert
        Assert.False(((IOutputOptions)options).IsCSharp);
    }

    [Fact]
    public void CSharpOutputOptions_LanguageVersions_ContainsAllEnumValues()
    {
        // Arrange & Act
        List<LanguageVersion> languageVersions = CSharpOutputOptions.LanguageVersions;
        IEnumerable<LanguageVersion> enumValues = Enum.GetValues<LanguageVersion>();

        // Assert
        foreach (LanguageVersion value in enumValues)
        {
            Assert.Contains(value, languageVersions);
        }
    }

    [Fact]
    public void CSharpOutputOptions_LanguageVersion_DefaultIsCSharp1()
    {
        // Arrange
        CSharpOutputOptions options = new CSharpOutputOptions();

        // Act & Assert
        Assert.Equal(LanguageVersion.CSharp1, options.LanguageVersion);
    }
}