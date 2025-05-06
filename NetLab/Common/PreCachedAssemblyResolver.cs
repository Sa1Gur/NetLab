using ICSharpCode.Decompiler.Metadata;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetLab.Common;

public class PreCachedAssemblyResolver : IAssemblyResolver
{
    private static readonly Task<MetadataFile> NullFileTask = Task.FromResult<MetadataFile>(null);

    private readonly ConcurrentDictionary<string, (PEFile file, Task<MetadataFile> task)> _peFileCache = [];

    public PreCachedAssemblyResolver(IEnumerable<MetadataReference> references) =>
        AddToCaches(references.OfType<PortableExecutableReference>().Select(x => x.FilePath).OfType<string>());

    private void AddToCaches(IEnumerable<string> assemblyPaths)
    {
        foreach (string path in assemblyPaths)
        {
            PEFile file = new(path);
            _ = _peFileCache.TryAdd(file.Name, (file, Task.FromResult<MetadataFile>(file)));
        }
    }

    public MetadataFile Resolve(IAssemblyReference reference) => ResolveFromCacheForDecompilation(reference).file;

    public Task<MetadataFile> ResolveAsync(IAssemblyReference reference) => ResolveFromCacheForDecompilation(reference).task;

    public MetadataFile ResolveModule(MetadataFile mainModule, string moduleName) => throw new NotSupportedException();

    public Task<MetadataFile> ResolveModuleAsync(MetadataFile mainModule, string moduleName) => throw new NotSupportedException();

    private (PEFile file, Task<MetadataFile> task) ResolveFromCacheForDecompilation(IAssemblyReference reference) =>
        !_peFileCache.TryGetValue(reference.Name, out (PEFile file, Task<MetadataFile> task) cached)
            ? ((PEFile file, Task<MetadataFile> task))(null, NullFileTask)
            : (cached.file, cached.task);
}
