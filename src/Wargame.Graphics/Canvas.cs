// Copyright (c) Microsoft Corporation.
// SPDX-License-Identifier: MIT
namespace Wargame.Graphics;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// RGBA pixel canvas with drawing primitives.
/// </summary>
public class Canvas
{
    public static readonly (byte R, byte G, byte B, byte A) Transparent = (0, 0, 0, 0);

    private readonly (byte R, byte G, byte B, byte A)[][] _pixels;
    public int Width { get; }
    public int Height { get; }

    public Canvas(int width, int height, (byte R, byte G, byte B, byte A) fillColor)
    {
        Width = width;
        Height = height;
        _pixels = new (byte R, byte G, byte B, byte A)[height][];
        for (int row = 0; row < height; row++)
        {
            _pixels[row] = new (byte R, byte G, byte B, byte A)[width];
            for (int col = 0; col < width; col++)
                _pixels[row][col] = fillColor;
        }
    }

    public (byte R, byte G, byte B, byte A)[][] Pixels => _pixels;

    public void SetPixel(int x, int y, (byte R, byte G, byte B, byte A) color)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
            _pixels[y][x] = color;
    }

    public void DrawRect(int x, int y, int width, int height, (byte R, byte G, byte B, byte A) color)
    {
        var maxX = Math.Min(x + width, Width);
        var maxY = Math.Min(y + height, Height);
        for (int row = Math.Max(0, y); row < maxY; row++)
            for (int col = Math.Max(0, x); col < maxX; col++)
                _pixels[row][col] = color;
    }

    public void DrawPolygon(List<(int X, int Y)> points, (byte R, byte G, byte B, byte A) color)
    {
        if (points.Count == 0) return;

        var minRow = Math.Max(0, points.ConvertAll(p => p.Y).Min());
        var maxRow = Math.Min(Height - 1, points.ConvertAll(p => p.Y).Max());

        for (int row = minRow; row <= maxRow; row++)
        {
            var intersections = new List<int>();
            var (prevX, prevY) = points[^1];

            foreach (var (curX, curY) in points)
            {
                bool crosses = (curY <= row && row < prevY) || (prevY <= row && row < curY);
                if (crosses)
                {
                    var span = prevY - curY;
                    var col = curX + (row - curY) * (prevX - curX) / (double)span;
                    intersections.Add((int)Math.Round(col));
                }
                (prevX, prevY) = (curX, curY);
            }

            intersections.Sort();
            for (int i = 0; i < intersections.Count - 1; i += 2)
            {
                var left = intersections[i];
                var right = intersections[i + 1];
                for (int col = left; col <= right; col++)
                    SetPixel(col, row, color);
            }
        }
    }

    public void DrawEllipse(int centerX, int centerY, int radiusX, int radiusY, (byte R, byte G, byte B, byte A) color)
    {
        for (int row = centerY - radiusY; row <= centerY + radiusY; row++)
        {
            for (int col = centerX - radiusX; col <= centerX + radiusX; col++)
            {
                var nx = radiusX == 0 ? 0 : (col - centerX) / (double)radiusX;
                var ny = radiusY == 0 ? 0 : (row - centerY) / (double)radiusY;
                if (nx * nx + ny * ny <= 1)
                    SetPixel(col, row, color);
            }
        }
    }

    public void DrawDither(int x, int y, int width, int height, (byte R, byte G, byte B, byte A) color1, (byte R, byte G, byte B, byte A) color2, int step)
    {
        for (int row = y; row < y + height; row++)
        {
            for (int col = x; col < x + width; col++)
            {
                if (row >= 0 && row < Height && col >= 0 && col < Width)
                {
                    if ((row + col) % step == 0)
                        _pixels[row][col] = color1;
                    else if ((row + col + 2) % step == 0)
                        _pixels[row][col] = color2;
                }
            }
        }
    }

    public void CopyNonTransparent(Canvas source, int offsetX, int offsetY)
    {
        for (int row = 0; row < source.Height; row++)
        {
            for (int col = 0; col < source.Width; col++)
            {
                var color = source._pixels[row][col];
                if (color.A != 0)
                    SetPixel(offsetX + col, offsetY + row, color);
            }
        }
    }
}
