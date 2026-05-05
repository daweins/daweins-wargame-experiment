namespace Wargame.Graphics;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

/// <summary>
/// Reads non-interlaced 8-bit RGB and RGBA PNG files into RGBA canvases.
/// </summary>
public static class PngReader
{
    private static readonly byte[] Signature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    /// <summary>
    /// Reads a PNG file into a <see cref="Canvas"/>.
    /// </summary>
    /// <param name="path">The PNG file path.</param>
    /// <returns>The decoded RGBA canvas.</returns>
    public static Canvas ReadRgbaPng(string path)
    {
        var bytes = File.ReadAllBytes(path);
        if (!bytes.Take(Signature.Length).SequenceEqual(Signature))
        {
            throw new InvalidOperationException($"Not a PNG file: {path}");
        }

        var offset = Signature.Length;
        int width = 0;
        int height = 0;
        byte bitDepth = 0;
        byte colorType = 0;
        byte interlace = 0;
        var idat = new MemoryStream();

        while (offset < bytes.Length)
        {
            var length = ReadUInt32BE(bytes, offset);
            offset += 4;
            var chunkType = Encoding.ASCII.GetString(bytes, offset, 4);
            offset += 4;
            var chunkData = bytes.AsSpan(offset, checked((int)length)).ToArray();
            offset += checked((int)length) + 4;

            switch (chunkType)
            {
                case "IHDR":
                    width = checked((int)ReadUInt32BE(chunkData, 0));
                    height = checked((int)ReadUInt32BE(chunkData, 4));
                    bitDepth = chunkData[8];
                    colorType = chunkData[9];
                    interlace = chunkData[12];
                    break;
                case "IDAT":
                    idat.Write(chunkData, 0, chunkData.Length);
                    break;
                case "IEND":
                    return DecodeImage(path, width, height, bitDepth, colorType, interlace, idat.ToArray());
            }
        }

        throw new InvalidOperationException($"PNG missing IEND chunk: {path}");
    }

    private static Canvas DecodeImage(string path, int width, int height, byte bitDepth, byte colorType, byte interlace, byte[] compressedData)
    {
        if (bitDepth != 8)
        {
            throw new NotSupportedException($"Only 8-bit PNGs are supported: {path}");
        }

        if (interlace != 0)
        {
            throw new NotSupportedException($"Interlaced PNGs are not supported: {path}");
        }

        var bytesPerPixel = colorType switch
        {
            2 => 3,
            6 => 4,
            _ => throw new NotSupportedException($"Only RGB and RGBA PNGs are supported: {path}")
        };

        var inflated = InflateZlib(compressedData);
        var stride = width * bytesPerPixel;
        var expectedBytes = height * (stride + 1);
        if (inflated.Length < expectedBytes)
        {
            throw new InvalidOperationException($"PNG data is shorter than expected: {path}");
        }

        var canvas = new Canvas(width, height, Canvas.Transparent);
        var previous = new byte[stride];
        var current = new byte[stride];
        var inputOffset = 0;

        for (var row = 0; row < height; row++)
        {
            var filter = inflated[inputOffset++];
            Array.Copy(inflated, inputOffset, current, 0, stride);
            inputOffset += stride;
            UnfilterRow(current, previous, bytesPerPixel, filter);

            for (var col = 0; col < width; col++)
            {
                var pixelOffset = col * bytesPerPixel;
                var alpha = colorType == 6 ? current[pixelOffset + 3] : (byte)255;
                canvas.SetPixel(col, row, (current[pixelOffset], current[pixelOffset + 1], current[pixelOffset + 2], alpha));
            }

            (previous, current) = (current, previous);
        }

        return canvas;
    }

    private static byte[] InflateZlib(byte[] compressedData)
    {
        if (compressedData.Length < 6)
        {
            throw new InvalidOperationException("Compressed PNG data is invalid.");
        }

        using var input = new MemoryStream(compressedData, 2, compressedData.Length - 6);
        using var deflate = new DeflateStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        deflate.CopyTo(output);
        return output.ToArray();
    }

    private static void UnfilterRow(byte[] current, byte[] previous, int bytesPerPixel, byte filter)
    {
        for (var index = 0; index < current.Length; index++)
        {
            var left = index >= bytesPerPixel ? current[index - bytesPerPixel] : 0;
            var up = previous[index];
            var upLeft = index >= bytesPerPixel ? previous[index - bytesPerPixel] : 0;
            var predictor = filter switch
            {
                0 => 0,
                1 => left,
                2 => up,
                3 => (left + up) / 2,
                4 => Paeth(left, up, upLeft),
                _ => throw new InvalidOperationException($"Unsupported PNG filter: {filter}")
            };

            current[index] = unchecked((byte)(current[index] + predictor));
        }
    }

    private static int Paeth(int left, int up, int upLeft)
    {
        var prediction = left + up - upLeft;
        var leftDistance = Math.Abs(prediction - left);
        var upDistance = Math.Abs(prediction - up);
        var upLeftDistance = Math.Abs(prediction - upLeft);
        if (leftDistance <= upDistance && leftDistance <= upLeftDistance)
        {
            return left;
        }

        return upDistance <= upLeftDistance ? up : upLeft;
    }

    private static uint ReadUInt32BE(byte[] bytes, int offset) =>
        ((uint)bytes[offset] << 24) |
        ((uint)bytes[offset + 1] << 16) |
        ((uint)bytes[offset + 2] << 8) |
        bytes[offset + 3];
}