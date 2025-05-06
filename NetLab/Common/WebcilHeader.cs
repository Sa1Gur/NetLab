using System.Runtime.InteropServices;

namespace NetLab.Common;

/// <summary>
/// The header of a WebCIL file.
/// </summary>
/// <remarks>The header is a subset of the PE, COFF and CLI headers that are needed by the mono runtime to load managed assemblies.</remarks>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct WebcilHeader
{
    public fixed byte id[4];
    public ushort version_major;
    public ushort version_minor;
    public ushort coff_sections;
    public ushort reserved0;
    public uint pe_cli_header_rva;
    public uint pe_cli_header_size;
    public uint pe_debug_rva;
    public uint pe_debug_size;
}