using Microsoft.Extensions.Logging;
using NetLab.Common;

namespace NetLab.Tests;

public class RoslynCodeSessionTests
{
    private readonly Mock<ILogger<RoslynCodeSession>> _mockLogger;
    private readonly RoslynOptions _options;
    private readonly string _testCode;
    private readonly ITestOutputHelper _output;

    public RoslynCodeSessionTests(ITestOutputHelper output)
    {
        _output = output;
        _mockLogger = new Mock<ILogger<RoslynCodeSession>>();
        _options = new CSharpInputOptions();
        _testCode = "using System; class Program { static void Main() { Console.WriteLine(\"Hello World\"); } }";
    }

    [Fact(Skip = "Requires WebAssembly runtime")]
    public void Constructor_InitializesWithProvidedValues()
    {
        // Arrange & Act
        RoslynCodeSession session = new RoslynCodeSession(_testCode, _options, true, _mockLogger.Object);

        // Assert
        Assert.NotNull(session);
        Assert.NotNull(session.Workspace);
        Assert.Equal(_testCode, session.SourceCode);
    }

    [Fact(Skip = "Requires WebAssembly runtime")]
    public void SetSourceText_UpdatesSourceCode()
    {
        // Arrange
        RoslynCodeSession session = new RoslynCodeSession("", _options, true, _mockLogger.Object);
        string newCode = "class Test {}";

        // Act
        session.SetSourceText(newCode);

        // Assert
        Assert.Equal(newCode, session.SourceCode);
    }

    [Fact(Skip = "Requires WebAssembly runtime")]
    public async Task GetDiagnosticsAsync_WithValidCode_ReturnsEmptyList()
    {
        // Skip this test if RoslynCodeSession.References is not initialized
        if (RoslynCodeSession.References == null || RoslynCodeSession.References.Count == 0)
        {
            _output.WriteLine("Skipping test because RoslynCodeSession.References is not initialized");
            return;
        }

        // Arrange
        RoslynCodeSession session = new RoslynCodeSession(_testCode, _options, true, _mockLogger.Object);
        List<NetLab.Common.Diagnostic> results = new List<NetLab.Common.Diagnostic>();

        // Act
        await session.GetDiagnosticsAsync(results);

        // Assert
        Assert.Empty(results);
    }

    [Fact(Skip = "Requires WebAssembly runtime")]
    public async Task GetDiagnosticsAsync_WithInvalidCode_ReturnsDiagnostics()
    {
        // Skip this test if RoslynCodeSession.References is not initialized
        if (RoslynCodeSession.References == null || RoslynCodeSession.References.Count == 0)
        {
            _output.WriteLine("Skipping test because RoslynCodeSession.References is not initialized");
            return;
        }

        // Arrange
        string invalidCode = "class Program { void Main() { undefinedVariable = 10; } }";
        RoslynCodeSession session = new RoslynCodeSession(invalidCode, _options, true, _mockLogger.Object);
        List<NetLab.Common.Diagnostic> results = new List<NetLab.Common.Diagnostic>();

        // Act
        await session.GetDiagnosticsAsync(results);

        // Assert
        Assert.NotEmpty(results);
    }
}