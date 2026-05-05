// Copyright (c) Microsoft Corporation.
// SPDX-License-Identifier: MIT
namespace Wargame.Graphics;

using System;
using System.IO;
using System.IO.Compression;

/// <summary>
/// PNG writer using only .NET standard library.
/// Writes RGBA PNG files with struct packing and zlib compression.
/// </summary>
public static class PngWriter
{
    /// <summary>Write an RGBA PNG file from a pixel canvas.</summary>
    public static void WriteRgbaPng(string outputPath, (byte R, byte G, byte B, byte A)[][] pixels)
    {
        var height = pixels.Length;
        if (height == 0) throw new ArgumentException("Canvas cannot be empty", nameof(pixels));
        var width = pixels[0].Length;

        // Create directory if needed
        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        // Build PNG data
        var pngData = new MemoryStream();
        pngData.Write(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, 0, 8); // PNG signature

        // IHDR chunk
        var ihdr = new MemoryStream();
        WriteUInt32BE(ihdr, (uint)width);
        WriteUInt32BE(ihdr, (uint)height);
        ihdr.WriteByte(8); // bit depth
        ihdr.WriteByte(6); // color type (RGBA)
        ihdr.WriteByte(0); // compression method
        ihdr.WriteByte(0); // filter method
        ihdr.WriteByte(0); // interlace method
        WriteChunk(pngData, "IHDR", ihdr.ToArray());

        // IDAT chunk (compressed pixel data)
        var rawRows = new MemoryStream();
        foreach (var row in pixels)
        {
            rawRows.WriteByte(0); // filter type (none)
            foreach (var (r, g, b, a) in row)
            {
                rawRows.WriteByte(r);
                rawRows.WriteByte(g);
                rawRows.WriteByte(b);
                rawRows.WriteByte(a);
            }
        }

        var compressedData = CompressZlib(rawRows.ToArray());
        WriteChunk(pngData, "IDAT", compressedData);

        // IEND chunk
        WriteChunk(pngData, "IEND", Array.Empty<byte>());

        File.WriteAllBytes(outputPath, pngData.ToArray());
    }

    private static void WriteUInt32BE(Stream stream, uint value)
    {
        stream.WriteByte((byte)((value >> 24) & 0xFF));
        stream.WriteByte((byte)((value >> 16) & 0xFF));
        stream.WriteByte((byte)((value >> 8) & 0xFF));
        stream.WriteByte((byte)(value & 0xFF));
    }

    private static void WriteChunk(Stream stream, string kind, byte[] data)
    {
        // Write length
        var length = (uint)data.Length;
        WriteUInt32BE(stream, length);

        // Write chunk type and data
        var kindBytes = System.Text.Encoding.ASCII.GetBytes(kind);
        stream.Write(kindBytes, 0, 4);
        stream.Write(data, 0, data.Length);

        // Write CRC
        var crcData = new byte[kindBytes.Length + data.Length];
        Buffer.BlockCopy(kindBytes, 0, crcData, 0, 4);
        Buffer.BlockCopy(data, 0, crcData, 4, data.Length);
        var crc = Crc32(crcData) ^ 0xFFFFFFFF;
        WriteUInt32BE(stream, crc);
    }

    private static byte[] CompressZlib(byte[] data)
    {
        var output = new MemoryStream();
        // Zlib header
        output.WriteByte(0x78);
        output.WriteByte(0x9C);

        // Compress with DeflateStream
        using (var deflate = new DeflateStream(output, CompressionMode.Compress, leaveOpen: true))
        {
            deflate.Write(data, 0, data.Length);
        }

        // Adler-32 checksum
        var checksum = Adler32(data);
        output.WriteByte((byte)((checksum >> 24) & 0xFF));
        output.WriteByte((byte)((checksum >> 16) & 0xFF));
        output.WriteByte((byte)((checksum >> 8) & 0xFF));
        output.WriteByte((byte)(checksum & 0xFF));

        return output.ToArray();
    }

    private static uint Adler32(byte[] data)
    {
        const uint mod = 65521;
        uint a = 1, b = 0;
        foreach (var byte_ in data)
        {
            a = (a + byte_) % mod;
            b = (b + a) % mod;
        }
        return (b << 16) | a;
    }

    // CRC32 lookup table
    private static readonly uint[] CrcTable = InitCrcTable();

    private static uint[] InitCrcTable()
    {
        var table = new uint[256];
        for (int i = 0; i < 256; i++)
        {
            uint c = (uint)i;
            for (int k = 0; k < 8; k++)
                c = (c & 1) == 1 ? 0xEDB88320 ^ (c >> 1) : c >> 1;
            table[i] = c;
        }
        return table;
    }

    private static uint Crc32(byte[] data)
    {
        uint crc = 0xFFFFFFFF;
        foreach (var byte_ in data)
            crc = CrcTable[(crc ^ byte_) & 0xFF] ^ (crc >> 8);
        return crc;
    }
}
