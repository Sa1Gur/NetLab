using Microsoft.CodeAnalysis;

namespace NetLab.Tests;

public class DiagnosticTests
{
    private readonly ITestOutputHelper _output;

    public DiagnosticTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Constructor_WithSeverityAndMessage_InitializesCorrectly()
    {
        // Arrange
        DiagnosticSeverity severity = DiagnosticSeverity.Error;
        string message = "Test error message";

        // Act
        NetLab.Common.Diagnostic diagnostic = new NetLab.Common.Diagnostic(severity, message);

        // Assert
        Assert.Equal(message, diagnostic.Message);
        Assert.Equal(severity.ToString(), diagnostic.Severity);
        Assert.Empty(diagnostic.Actions);
    }

    [Fact]
    public void Constructor_WithException_InitializesCorrectly()
    {
        // Arrange
        InvalidOperationException exception = new InvalidOperationException("Test exception");

        // Act
        NetLab.Common.Diagnostic diagnostic = new NetLab.Common.Diagnostic(exception);

        // Assert
        Assert.Equal(exception.Message, diagnostic.Message);
        Assert.Equal(DiagnosticSeverity.Error.ToString(), diagnostic.Severity);
        Assert.Empty(diagnostic.Actions);
    }

    [Fact(Skip = "Requires WebAssembly runtime")]
    public void Constructor_WithRoslynDiagnostic_InitializesCorrectly()
    {
        // Arrange
        DiagnosticDescriptor descriptor = new DiagnosticDescriptor(
            "TEST001",
            "Test Diagnostic",
            "Test message",
            "Testing",
            DiagnosticSeverity.Warning,
            true);

        Microsoft.CodeAnalysis.Location location = Location.Create(
            "test.cs",
            new Microsoft.CodeAnalysis.Text.TextSpan(0, 10),
            new Microsoft.CodeAnalysis.Text.LinePositionSpan(
                new Microsoft.CodeAnalysis.Text.LinePosition(1, 0),
                new Microsoft.CodeAnalysis.Text.LinePosition(1, 10)));

        Microsoft.CodeAnalysis.Diagnostic roslynDiagnostic = Microsoft.CodeAnalysis.Diagnostic.Create(descriptor, location);

        // Act
        NetLab.Common.Diagnostic diagnostic = new NetLab.Common.Diagnostic(roslynDiagnostic);

        // Assert
        Assert.Equal("TEST001", diagnostic.ID);
        Assert.Equal(roslynDiagnostic.GetMessage(), diagnostic.Message);
        Assert.Equal(DiagnosticSeverity.Warning.ToString(), diagnostic.Severity);
        Assert.Equal(new Microsoft.CodeAnalysis.Text.LinePositionSpan(
            new Microsoft.CodeAnalysis.Text.LinePosition(1, 0),
            new Microsoft.CodeAnalysis.Text.LinePosition(1, 10)), diagnostic.Location);
        Assert.Empty(diagnostic.Actions);
    }
}