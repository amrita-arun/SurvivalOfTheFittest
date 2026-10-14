using UnityEngine;

namespace SurvivalOfTheFittest
{
    /// <summary>
    /// Fixed, hand-designed maze. Never procedurally generated.
    /// Legend: # wall, . floor, P player, X exit, G gem,
    /// A Ex-Situationship, B Dating App, C Mixed Signals.
    ///
    /// The left side is a flag garden. The right side is a braid of lanes
    /// that still has to pass A, then B, then C. Green flags sit on detours.
    /// Sweeping every flag is a long detour on the 90 second clock.
    /// </summary>
    public static class MazeData
    {
        public const float CellSize = 1f;

        private static readonly string[] Layout =
        {
            "########################################",
            "######G....#..P..#.#.....#.....#..######",
            "######..#..#.#.....#..#..#..#..#...#####",
            "######..#....#..#.....#.....#....#...###",
            "############.##################.####.###",
            "####G....#...#..#.....#.....#.....##.###",
            "####..#.....##.##..#..#..#..#..#..##..##",
            "####..#...#..#.....#.....#.....#..###.##",
            "############.##A####################G.##",
            "##.....#.....#.....#.....#.....#..######",
            "##..#.....#..#.##..#..#..#..#.....######",
            "##..#...#....#..#.....#.....#...#...####",
            "##########.####################B########",
            "##G....#.....####.....#.....#....#.....#",
            "##..#..#..#..####..#..#..#..#..#...#...#",
            "##..#.....#..#.....#.....#.....#..###..#",
            "############.##C#####################.##",
            "#G....#...#..##.....#.....#...#######..#",
            "#..#..#.....###..#.....#..#..#########G#",
            "#..#.....#...##..#.....#....X###########",
            "########################################",
        };

        public static int Width => Layout[0].Length;
        public static int Height => Layout.Length;

        public static char GetCell(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return '#';
            return Layout[y][x];
        }

        public static bool IsWalkable(int x, int y) => GetCell(x, y) != '#';

        public static Vector2 CellToWorld(int x, int y)
        {
            var worldY = (Height - 1 - y) * CellSize;
            return new Vector2(x * CellSize, worldY);
        }

        public static Vector2Int WorldToCell(Vector2 world)
        {
            var x = Mathf.RoundToInt(world.x / CellSize);
            var yFromBottom = Mathf.RoundToInt(world.y / CellSize);
            var y = Height - 1 - yFromBottom;
            return new Vector2Int(x, y);
        }

        public static Vector2 MazeCenterWorld =>
            new Vector2((Width - 1) * CellSize * 0.5f, (Height - 1) * CellSize * 0.5f);

        public static Vector2 MazeSizeWorld =>
            new Vector2(Width * CellSize, Height * CellSize);

        public static bool TryFind(char symbol, out int x, out int y)
        {
            for (var row = 0; row < Height; row++)
            {
                for (var col = 0; col < Width; col++)
                {
                    if (Layout[row][col] != symbol)
                        continue;
                    x = col;
                    y = row;
                    return true;
                }
            }

            x = y = 0;
            return false;
        }
    }
}
