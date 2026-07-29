using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using NetLab.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

WebAssemblyHost current = WebAssemblyHostBuilder.CreateDefault(args).Build();
Compiler = new Compiler(current.Services.GetRequiredService<ILoggerFactory>());
JSRuntime = current.Services.GetRequiredService<IJSRuntime>();
await JSRuntime.InvokeVoidAsync("initNetLab", DotNetObjectReference.Create(new Program()));
await current.RunAsync();

public partial class Program
{
    private static Compiler Compiler { get; set; }
    private static IJSRuntime JSRuntime { get; set; }

    [JSInvokable]
    public Task InitAsync(string baseUrl) => RoslynCodeSession.InitAsync(baseUrl).AsTask();

    [JSInvokable("InitWithUrlsAsync")]
    public Task InitAsync(string baseUrl, IDictionary<string, string> assemblyUrls) =>
        RoslynCodeSession.InitAsync(
            baseUrl,
            assemblyUrls,
            (name, completed, total) => JSRuntime.InvokeVoidAsync("updateNetLabProgress", name, completed, total)).AsTask();

    [JSInvokable]
    public Task<CompileResult> ProcessAsync(string code) =>
        Compiler.ProcessAsync(
            code,
            stage => JSRuntime.InvokeVoidAsync("updateNetLabStage", stage)).AsTask();

    [JSInvokable]
    public Task<List<Diagnostic>> GetDiagnosticsAsync(string code) => Compiler.GetDiagnosticsAsync(code).AsTask();

    [JSInvokable]
    public Task<IEnumerable<CompletionItem>> GetCompletionsAsync(string code, int position) => Compiler.GetCompletionsAsync(code, position).AsTask();

    [JSInvokable]
    public Task<InfoTipItem> GetInfoTipAsync(string code, int position) => Compiler.GetInfoTipAsync(code, position).AsTask();

    [JSInvokable]
    public IEnumerable<string> GetLanguageTypes() => Compiler.LanguageTypes.Select(x => x.ToString());

    [JSInvokable]
    public void SetLanguageType(string type) => Compiler.LanguageType = Enum.Parse<LanguageType>(type, true);

    [JSInvokable]
    public IEnumerable<string> GetOutputTypes() => Compiler.OutputTypes.Select(x => x.ToString());

    [JSInvokable]
    public void SetOutputType(string type) => Compiler.OutputType = Enum.Parse<OutputType>(type, true);

    [JSInvokable]
    public IEnumerable<string> GetInputLanguageVersions()
    {
        if (((IInputOptions)Compiler.InputOptions).LanguageVersions is not Array array) yield break;

        foreach (object @enum in array)
        {
            yield return @enum.ToString();
        }
    }

    [JSInvokable]
    public void SetInputLanguageVersion(string type)
    {
        if (((IInputOptions)Compiler.InputOptions).LanguageVersion?.GetType() is Type @enum)
        {
            ((IInputOptions)Compiler.InputOptions).LanguageVersion = (Enum)Enum.Parse(@enum, type, true);
        }
    }

    [JSInvokable]
    public IEnumerable<string> GetOutputLanguageVersions()
    {
        if (!((IOutputOptions)Compiler.OutputOptions).IsCSharp) yield break;

        foreach (object @enum in CSharpOutputOptions.LanguageVersions)
        {
            yield return @enum.ToString();
        }
    }

    [JSInvokable]
    public void SetOutputLanguageVersion(string type)
    {
        if (((IOutputOptions)Compiler.OutputOptions).LanguageVersion?.GetType() is Type @enum)
        {
            ((IOutputOptions)Compiler.OutputOptions).LanguageVersion = (Enum)Enum.Parse(@enum, type, true);
        }
    }
}
