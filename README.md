# NetLab

A .NET WebAssembly application for code compilation, diagnostics, and completion services.

## Project Structure

- **NetLab**: Main WebAssembly application
  - **Common**: Core functionality for compilation and code analysis
- **NetLab.Tests**: Test project for the application

## Testing

The test project is set up with xUnit and includes tests for all major components. Due to the WebAssembly target framework, tests are divided into two categories:

1. **Standard Tests**: These tests can run in any environment and don't require WebAssembly runtime.
2. **WebAssembly-dependent Tests**: These tests are marked with `[Fact(Skip = "Requires WebAssembly runtime")]` and require a WebAssembly runtime to execute.

### Running Tests

To run the tests that don't require WebAssembly:

```bash
dotnet test --filter "FullyQualifiedName~MockTests"
```

### Test Categories

- **CompilerTests**: Tests for the Compiler class
- **RoslynCodeSessionTests**: Tests for the RoslynCodeSession class
- **ProgramTests**: Tests for the Program class
- **DiagnosticTests**: Tests for the Diagnostic class
- **CompilationResultsTests**: Tests for the CompilationResults class
- **InputOptionsTests**: Tests for the InputOptions classes
- **OutputOptionsTests**: Tests for the OutputOptions classes
- **MockTests**: Tests that don't require WebAssembly runtime

## Running WebAssembly Tests

To run tests that require WebAssembly runtime, you need to:

1. Set up a browser-based test runner
2. Use a tool like Playwright or Selenium to automate browser testing
3. Configure the test runner to execute the WebAssembly tests in a browser context

## Future Improvements

- Extract core logic into a separate class library that doesn't depend on WebAssembly
- Create a test-specific build configuration that targets standard .NET
- Implement browser-based testing for WebAssembly components