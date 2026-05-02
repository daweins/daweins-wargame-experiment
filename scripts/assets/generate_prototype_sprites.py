"""Generate prototype pixel-art sprite sheets for the Godot tactical slice."""

from __future__ import annotations

import struct
import sys
import zlib
from pathlib import Path


EXIT_SUCCESS = 0
SPRITE_SIZE = 64
ROOT = Path(__file__).resolve().parents[2]
OUTPUT_DIRECTORY = ROOT / "game" / "WargamePrototype" / "assets" / "sprites"

Color = tuple[int, int, int, int]
Canvas = list[list[Color]]

TRANSPARENT: Color = (0, 0, 0, 0)
OUTLINE: Color = (8, 13, 22, 255)
DEEP_SHADOW: Color = (12, 18, 29, 210)
SOFT_SHADOW: Color = (8, 12, 20, 92)


def create_canvas(width: int, height: int, color: Color = TRANSPARENT) -> Canvas:
    """Create an RGBA pixel canvas."""
    return [[color for _column in range(width)] for _row in range(height)]


def save_png(path: Path, pixels: Canvas) -> None:
    """Write an RGBA PNG using only the Python standard library."""
    height = len(pixels)
    width = len(pixels[0])
    raw_rows: list[bytes] = []
    for row_pixels in pixels:
        row_data = bytearray([0])
        for color in row_pixels:
            row_data.extend(color)
        raw_rows.append(bytes(row_data))

    def chunk(kind: bytes, data: bytes) -> bytes:
        checksum = zlib.crc32(kind + data) & 0xFFFFFFFF
        return struct.pack(">I", len(data)) + kind + data + struct.pack(">I", checksum)

    png_data = b"\x89PNG\r\n\x1a\n"
    png_data += chunk(b"IHDR", struct.pack(">IIBBBBB", width, height, 8, 6, 0, 0, 0))
    png_data += chunk(b"IDAT", zlib.compress(b"".join(raw_rows), level=9))
    png_data += chunk(b"IEND", b"")
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_bytes(png_data)


def set_pixel(pixels: Canvas, column: int, row: int, color: Color) -> None:
    """Set a pixel when it is inside the canvas."""
    if 0 <= row < len(pixels) and 0 <= column < len(pixels[0]):
        pixels[row][column] = color


def rect(pixels: Canvas, left: int, top: int, width: int, height: int, color: Color) -> None:
    """Draw a filled rectangle."""
    for row in range(max(0, top), min(len(pixels), top + height)):
        for column in range(max(0, left), min(len(pixels[0]), left + width)):
            pixels[row][column] = color


def horizontal_line(pixels: Canvas, left: int, right: int, row: int, color: Color) -> None:
    """Draw a horizontal pixel line."""
    for column in range(left, right + 1):
        set_pixel(pixels, column, row, color)


def vertical_line(pixels: Canvas, column: int, top: int, bottom: int, color: Color) -> None:
    """Draw a vertical pixel line."""
    for row in range(top, bottom + 1):
        set_pixel(pixels, column, row, color)


def polygon(pixels: Canvas, points: list[tuple[int, int]], color: Color) -> None:
    """Draw a filled polygon using a simple scanline fill."""
    if not points:
        return

    min_row = max(0, min(point[1] for point in points))
    max_row = min(len(pixels) - 1, max(point[1] for point in points))
    for row in range(min_row, max_row + 1):
        intersections: list[int] = []
        previous_column, previous_row = points[-1]
        for current_column, current_row in points:
            crosses = (current_row <= row < previous_row) or (previous_row <= row < current_row)
            if crosses:
                span = previous_row - current_row
                column = current_column + (row - current_row) * (previous_column - current_column) / span
                intersections.append(int(round(column)))
            previous_column, previous_row = current_column, current_row

        intersections.sort()
        for pair_index in range(0, len(intersections), 2):
            if pair_index + 1 >= len(intersections):
                break
            horizontal_line(pixels, intersections[pair_index], intersections[pair_index + 1], row, color)


def ellipse(pixels: Canvas, center_column: int, center_row: int, radius_column: int, radius_row: int, color: Color) -> None:
    """Draw a filled ellipse."""
    for row in range(center_row - radius_row, center_row + radius_row + 1):
        for column in range(center_column - radius_column, center_column + radius_column + 1):
            normalized_column = (column - center_column) / radius_column if radius_column else 0
            normalized_row = (row - center_row) / radius_row if radius_row else 0
            if normalized_column * normalized_column + normalized_row * normalized_row <= 1:
                set_pixel(pixels, column, row, color)


def copy_sprite(sheet: Canvas, sprite: Canvas, offset_column: int, offset_row: int) -> None:
    """Copy non-transparent pixels into a sprite sheet."""
    for row_index, row_pixels in enumerate(sprite):
        for column_index, color in enumerate(row_pixels):
            if color[3] != 0:
                sheet[offset_row + row_index][offset_column + column_index] = color


def dither(pixels: Canvas, first: Color, second: Color, step: int = 5) -> None:
    """Add sparse checker texture without losing crisp tile readability."""
    height = len(pixels)
    width = len(pixels[0])
    for row in range(0, height, step):
        for column in range((row // step) % 2, width, step * 2):
            if pixels[row][column][3] != 0:
                pixels[row][column] = first
            if row + 2 < height and column + 3 < width and pixels[row + 2][column + 3][3] != 0:
                pixels[row + 2][column + 3] = second


def draw_tile_border(pixels: Canvas, top: Color, bottom: Color) -> None:
    """Draw subtle pixel bevels around a terrain tile."""
    rect(pixels, 0, 0, SPRITE_SIZE, 2, top)
    rect(pixels, 0, SPRITE_SIZE - 3, SPRITE_SIZE, 3, bottom)
    rect(pixels, 0, 0, 2, SPRITE_SIZE, top)
    rect(pixels, SPRITE_SIZE - 2, 0, 2, SPRITE_SIZE, bottom)


def tile_plain() -> Canvas:
    """Create a grassy plain tile."""
    pixels = create_canvas(SPRITE_SIZE, SPRITE_SIZE, (89, 142, 91, 255))
    for row in range(SPRITE_SIZE):
        shade = row // 9
        color = (94 + shade * 3, 150 + shade * 2, 92 + shade, 255)
        rect(pixels, 0, row, SPRITE_SIZE, 1, color)
    dither(pixels, (118, 176, 104, 255), (67, 119, 77, 255), 6)
    for blade_column, blade_row in ((7, 13), (20, 9), (35, 18), (48, 12), (11, 43), (30, 38), (51, 49)):
        rect(pixels, blade_column, blade_row, 9, 2, (133, 191, 111, 255))
        rect(pixels, blade_column + 2, blade_row + 3, 6, 2, (54, 105, 68, 255))
    draw_tile_border(pixels, (132, 188, 116, 255), (58, 101, 68, 255))
    return pixels


def tile_road() -> Canvas:
    """Create a worn road tile."""
    pixels = create_canvas(SPRITE_SIZE, SPRITE_SIZE, (74, 127, 85, 255))
    polygon(pixels, [(0, 15), (63, 9), (63, 49), (0, 56)], (172, 135, 83, 255))
    polygon(pixels, [(0, 12), (63, 7), (63, 12), (0, 18)], (221, 181, 112, 255))
    polygon(pixels, [(0, 53), (63, 47), (63, 54), (0, 61)], (95, 76, 55, 255))
    for pebble_column, pebble_row in ((8, 28), (17, 37), (29, 26), (42, 34), (54, 24)):
        rect(pixels, pebble_column, pebble_row, 6, 2, (229, 197, 129, 255))
        rect(pixels, pebble_column + 2, pebble_row + 4, 5, 2, (112, 88, 59, 255))
    rect(pixels, 2, 2, 14, 4, (104, 160, 94, 255))
    rect(pixels, 45, 56, 13, 3, (47, 94, 65, 255))
    draw_tile_border(pixels, (108, 165, 99, 255), (54, 92, 68, 255))
    return pixels


def tile_cover() -> Canvas:
    """Create a wooded cover tile."""
    pixels = create_canvas(SPRITE_SIZE, SPRITE_SIZE, (42, 91, 76, 255))
    rect(pixels, 0, 45, SPRITE_SIZE, 19, (30, 63, 55, 255))
    for trunk_column, crown_column, crown_row, crown_color in (
        (13, 7, 14, (91, 158, 103, 255)),
        (30, 24, 8, (112, 182, 112, 255)),
        (45, 39, 18, (70, 129, 94, 255)),
    ):
        rect(pixels, trunk_column, 34, 7, 22, (84, 55, 38, 255))
        rect(pixels, trunk_column + 2, 34, 3, 22, (129, 84, 52, 255))
        ellipse(pixels, crown_column + 8, crown_row + 10, 13, 11, crown_color)
        ellipse(pixels, crown_column + 15, crown_row + 17, 13, 12, (48, 101, 76, 255))
        rect(pixels, crown_column + 4, crown_row + 8, 18, 3, (139, 205, 121, 255))
    rect(pixels, 4, 53, 53, 5, (20, 43, 40, 255))
    draw_tile_border(pixels, (76, 142, 97, 255), (22, 48, 44, 255))
    return pixels


def tile_hq() -> Canvas:
    """Create a sci-fi HQ tile."""
    pixels = create_canvas(SPRITE_SIZE, SPRITE_SIZE, (69, 56, 132, 255))
    dither(pixels, (91, 74, 163, 255), (48, 39, 94, 255), 8)
    rect(pixels, 8, 46, 48, 11, (39, 33, 80, 255))
    rect(pixels, 13, 26, 38, 23, OUTLINE)
    rect(pixels, 15, 24, 34, 24, (193, 185, 232, 255))
    rect(pixels, 18, 19, 28, 7, (247, 203, 92, 255))
    rect(pixels, 22, 13, 20, 7, (144, 112, 226, 255))
    rect(pixels, 29, 5, 7, 11, (247, 203, 92, 255))
    rect(pixels, 21, 32, 7, 11, (72, 55, 145, 255))
    rect(pixels, 36, 32, 7, 11, (72, 55, 145, 255))
    rect(pixels, 17, 27, 30, 3, (242, 235, 255, 255))
    rect(pixels, 13, 51, 38, 4, (243, 229, 151, 255))
    draw_tile_border(pixels, (130, 107, 220, 255), (40, 31, 83, 255))
    return pixels


def tile_ridge() -> Canvas:
    """Create a rocky ridge tile."""
    pixels = create_canvas(SPRITE_SIZE, SPRITE_SIZE, (32, 41, 56, 255))
    polygon(pixels, [(3, 52), (15, 33), (27, 48), (39, 23), (59, 54)], (17, 25, 38, 255))
    polygon(pixels, [(8, 47), (18, 24), (31, 48)], (58, 71, 91, 255))
    polygon(pixels, [(22, 49), (39, 14), (55, 50)], (75, 88, 111, 255))
    polygon(pixels, [(37, 50), (53, 30), (62, 52)], (43, 53, 71, 255))
    rect(pixels, 35, 17, 10, 3, (142, 154, 172, 255))
    rect(pixels, 15, 28, 8, 3, (111, 125, 145, 255))
    rect(pixels, 46, 34, 6, 3, (101, 114, 137, 255))
    dither(pixels, (45, 57, 76, 255), (25, 32, 46, 255), 7)
    draw_tile_border(pixels, (67, 80, 101, 255), (15, 22, 34, 255))
    return pixels


def team_palette(team: str) -> dict[str, Color]:
    """Return SNES-style colors for a team."""
    if team == "player":
        return {
            "light": (190, 227, 255, 255),
            "mid": (72, 161, 224, 255),
            "dark": (25, 80, 144, 255),
            "accent": (86, 225, 232, 255),
            "deep": (15, 43, 86, 255),
        }

    return {
        "light": (255, 201, 174, 255),
        "mid": (223, 90, 67, 255),
        "dark": (124, 36, 48, 255),
        "accent": (255, 154, 83, 255),
        "deep": (74, 20, 35, 255),
    }


def draw_common_shadow(pixels: Canvas) -> None:
    """Draw a soft contact shadow under a unit."""
    ellipse(pixels, 32, 54, 22, 6, SOFT_SHADOW)


def unit_infantry(palette: dict[str, Color]) -> Canvas:
    """Create a heavier 64x64 infantry sprite."""
    pixels = create_canvas(SPRITE_SIZE, SPRITE_SIZE)
    draw_common_shadow(pixels)
    rect(pixels, 20, 9, 24, 12, OUTLINE)
    rect(pixels, 22, 7, 20, 13, palette["mid"])
    rect(pixels, 25, 8, 14, 4, palette["light"])
    rect(pixels, 23, 17, 18, 8, (218, 226, 212, 255))
    rect(pixels, 26, 20, 12, 3, (76, 96, 112, 255))
    rect(pixels, 18, 25, 28, 20, OUTLINE)
    rect(pixels, 20, 24, 24, 20, palette["mid"])
    rect(pixels, 24, 26, 16, 15, palette["dark"])
    rect(pixels, 23, 25, 18, 3, palette["light"])
    rect(pixels, 13, 27, 9, 18, palette["deep"])
    rect(pixels, 42, 25, 8, 18, palette["deep"])
    rect(pixels, 47, 19, 4, 25, (206, 218, 211, 255))
    rect(pixels, 50, 18, 5, 6, OUTLINE)
    rect(pixels, 22, 44, 8, 13, OUTLINE)
    rect(pixels, 34, 44, 8, 13, OUTLINE)
    rect(pixels, 23, 44, 6, 11, palette["dark"])
    rect(pixels, 35, 44, 6, 11, palette["dark"])
    rect(pixels, 18, 56, 13, 4, OUTLINE)
    rect(pixels, 34, 56, 13, 4, OUTLINE)
    rect(pixels, 26, 30, 12, 3, palette["accent"])
    rect(pixels, 14, 31, 5, 5, palette["light"])
    rect(pixels, 43, 29, 4, 5, palette["light"])
    return pixels


def unit_armor(palette: dict[str, Color]) -> Canvas:
    """Create a 64x64 tank sprite."""
    pixels = create_canvas(SPRITE_SIZE, SPRITE_SIZE)
    draw_common_shadow(pixels)
    rect(pixels, 7, 39, 50, 12, OUTLINE)
    rect(pixels, 9, 36, 46, 12, palette["deep"])
    rect(pixels, 12, 34, 40, 10, palette["dark"])
    polygon(pixels, [(18, 25), (41, 22), (49, 35), (14, 36)], OUTLINE)
    polygon(pixels, [(20, 24), (40, 22), (46, 34), (16, 35)], palette["mid"])
    rect(pixels, 25, 17, 16, 8, palette["mid"])
    rect(pixels, 28, 15, 11, 5, palette["light"])
    rect(pixels, 43, 25, 17, 5, (212, 222, 214, 255))
    rect(pixels, 57, 23, 4, 6, OUTLINE)
    rect(pixels, 16, 37, 29, 4, palette["light"])
    rect(pixels, 21, 29, 17, 3, palette["accent"])
    for wheel_column in (14, 25, 36, 47):
        ellipse(pixels, wheel_column, 49, 5, 5, OUTLINE)
        ellipse(pixels, wheel_column, 49, 3, 3, (204, 214, 208, 255))
    rect(pixels, 11, 42, 42, 3, (228, 238, 230, 255))
    rect(pixels, 10, 51, 45, 3, (40, 51, 64, 255))
    return pixels


def unit_scout(palette: dict[str, Color]) -> Canvas:
    """Create a fast scout vehicle sprite."""
    pixels = create_canvas(SPRITE_SIZE, SPRITE_SIZE)
    draw_common_shadow(pixels)
    polygon(pixels, [(8, 42), (19, 28), (48, 28), (58, 40), (46, 48), (16, 49)], OUTLINE)
    polygon(pixels, [(11, 41), (21, 30), (46, 30), (54, 39), (44, 45), (17, 46)], palette["dark"])
    polygon(pixels, [(23, 22), (44, 25), (49, 31), (19, 31)], palette["mid"])
    polygon(pixels, [(31, 15), (46, 22), (43, 27), (25, 24)], palette["mid"])
    polygon(pixels, [(33, 17), (43, 22), (39, 24), (29, 22)], palette["light"])
    rect(pixels, 48, 32, 10, 5, (211, 222, 214, 255))
    rect(pixels, 14, 39, 30, 4, palette["accent"])
    ellipse(pixels, 18, 50, 7, 7, OUTLINE)
    ellipse(pixels, 45, 50, 7, 7, OUTLINE)
    ellipse(pixels, 18, 50, 4, 4, (205, 216, 210, 255))
    ellipse(pixels, 45, 50, 4, 4, (205, 216, 210, 255))
    rect(pixels, 13, 31, 7, 6, palette["light"])
    rect(pixels, 53, 38, 4, 3, palette["light"])
    return pixels


def build_terrain_sheet() -> Canvas:
    """Build the terrain sprite sheet."""
    sprites = [tile_plain(), tile_road(), tile_cover(), tile_hq(), tile_ridge()]
    sheet = create_canvas(SPRITE_SIZE * len(sprites), SPRITE_SIZE)
    for sprite_index, sprite in enumerate(sprites):
        copy_sprite(sheet, sprite, sprite_index * SPRITE_SIZE, 0)
    return sheet


def build_unit_sheet() -> Canvas:
    """Build the unit sprite sheet."""
    sheet = create_canvas(SPRITE_SIZE * 3, SPRITE_SIZE * 2)
    for row_index, team in enumerate(("player", "enemy")):
        palette = team_palette(team)
        sprites = [unit_infantry(palette), unit_armor(palette), unit_scout(palette)]
        for sprite_index, sprite in enumerate(sprites):
            copy_sprite(sheet, sprite, sprite_index * SPRITE_SIZE, row_index * SPRITE_SIZE)
    return sheet


def main() -> int:
    """Generate all prototype sprite sheets."""
    save_png(OUTPUT_DIRECTORY / "terrain.png", build_terrain_sheet())
    save_png(OUTPUT_DIRECTORY / "units.png", build_unit_sheet())
    print(f"Generated 64x64 sprite sheets in {OUTPUT_DIRECTORY}")
    return EXIT_SUCCESS


if __name__ == "__main__":
    sys.exit(main())