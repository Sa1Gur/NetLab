using NetLab.Common;

namespace NetLab.Tests;

public class CompilationResultsTests
{
    private readonly ITestOutputHelper _output;

    public CompilationResultsTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Constructor_InitializesProperties()
    {
        // Arrange
        MemoryStream assemblyStream = new MemoryStream();
        MemoryStream symbolStream = new MemoryStream();

        // Act
        CompilationResults results = new CompilationResults(assemblyStream, symbolStream);

        // Assert
        Assert.Same(assemblyStream, results.AssemblyStream);
        Assert.Same(symbolStream, results.SymbolStream);
    }

    [Fact]
    public void Dispose_DisposesStreams()
    {
        // Arrange
        MemoryStream assemblyStream = new MemoryStream();
        MemoryStream symbolStream = new MemoryStream();
        CompilationResults results = new CompilationResults(assemblyStream, symbolStream);

        // Act
        results.Dispose();

        // Assert - Verify streams are closed
        Assert.Throws<ObjectDisposedException>(() => assemblyStream.ReadByte());
        Assert.Throws<ObjectDisposedException>(() => symbolStream.ReadByte());
    }

    [Fact]
    public void Dispose_HandlesNullStreams()
    {
        // Arrange
        CompilationResults results = new CompilationResults(null, null);

        // Act & Assert - Should not throw
        results.Dispose();
    }
}