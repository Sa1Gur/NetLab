using System.Buffers.Binary;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

if (args.Length != 1)
{
    Console.Error.WriteLine("Usage: CoreLibPatcher <System.Private.CoreLib.dll>");
    return 2;
}

var path = Path.GetFullPath(args[0]);
var image = File.ReadAllBytes(path);

using var stream = new MemoryStream(image, writable: false);
using var peReader = new PEReader(stream);
var metadata = peReader.GetMetadataReader();

MethodDefinitionHandle readBarrierHandle = default;
foreach (var typeHandle in metadata.TypeDefinitions)
{
    var type = metadata.GetTypeDefinition(typeHandle);
    if (metadata.GetString(type.Namespace) != "System.Threading" ||
        metadata.GetString(type.Name) != "Volatile")
    {
        continue;
    }

    foreach (var methodHandle in type.GetMethods())
    {
        var method = metadata.GetMethodDefinition(methodHandle);
        if (metadata.GetString(method.Name) == "ReadBarrier")
        {
            if (!readBarrierHandle.IsNil)
                throw new InvalidDataException("System.Threading.Volatile has more than one ReadBarrier method.");

            readBarrierHandle = methodHandle;
        }
    }
}

if (readBarrierHandle.IsNil)
    throw new InvalidDataException("System.Threading.Volatile.ReadBarrier was not found.");

var readBarrier = metadata.GetMethodDefinition(readBarrierHandle);
if (readBarrier.RelativeVirtualAddress == 0)
    throw new InvalidDataException("System.Threading.Volatile.ReadBarrier has no IL body.");

var methodOffset = RvaToFileOffset(peReader.PEHeaders, readBarrier.RelativeVirtualAddress);
var (ilOffset, codeSize) = ReadMethodBody(image, methodOffset);
var il = image.AsSpan(ilOffset, codeSize);

// The browser Mono interpreter does not substitute this runtime intrinsic and
// executes its recursive reference IL. AOT does substitute it. In Debug builds
// we make the intrinsic a no-op so Roslyn's emit path can run under the debugger.
if (il.Length >= 1 && il[0] == 0x2A && IsAllZero(il[1..]))
{
    Console.WriteLine($"CoreLib patch already present: {path}");
    return 0;
}

if (il.Length != 6 || il[0] != 0x28 || il[5] != 0x2A)
    throw new InvalidDataException($"Unexpected ReadBarrier IL ({Convert.ToHexString(il)}); refusing to patch an unknown runtime.");

var calledToken = BinaryPrimitives.ReadInt32LittleEndian(il[1..5]);
var calledHandle = MetadataTokens.EntityHandle(calledToken);
if (calledHandle.Kind != HandleKind.MethodDefinition ||
    (MethodDefinitionHandle)calledHandle != readBarrierHandle)
{
    throw new InvalidDataException($"ReadBarrier does not call itself (token 0x{calledToken:X8}); refusing to patch.");
}

il.Clear();
il[0] = 0x2A; // ret
File.WriteAllBytes(path, image);
Console.WriteLine($"Patched recursive System.Threading.Volatile.ReadBarrier in {path}");
return 0;

static int RvaToFileOffset(PEHeaders headers, int rva)
{
    foreach (var section in headers.SectionHeaders)
    {
        var size = Math.Max(section.VirtualSize, section.SizeOfRawData);
        if (rva >= section.VirtualAddress && rva < section.VirtualAddress + size)
            return checked(section.PointerToRawData + rva - section.VirtualAddress);
    }

    throw new InvalidDataException($"RVA 0x{rva:X8} is not contained in a PE section.");
}

static (int IlOffset, int CodeSize) ReadMethodBody(byte[] image, int methodOffset)
{
    var first = image[methodOffset];
    switch (first & 0x3)
    {
        case 0x2: // tiny header
            return (methodOffset + 1, first >> 2);

        case 0x3: // fat header
            var flagsAndSize = BinaryPrimitives.ReadUInt16LittleEndian(image.AsSpan(methodOffset, 2));
            var headerSize = (flagsAndSize >> 12) * 4;
            var codeSize = BinaryPrimitives.ReadInt32LittleEndian(image.AsSpan(methodOffset + 4, 4));
            if (headerSize < 12 || codeSize < 0)
                throw new InvalidDataException("Invalid fat method header.");
            return (checked(methodOffset + headerSize), codeSize);

        default:
            throw new InvalidDataException($"Unknown method header 0x{first:X2}.");
    }
}

static bool IsAllZero(ReadOnlySpan<byte> bytes)
{
    foreach (var value in bytes)
    {
        if (value != 0)
            return false;
    }

    return true;
}
