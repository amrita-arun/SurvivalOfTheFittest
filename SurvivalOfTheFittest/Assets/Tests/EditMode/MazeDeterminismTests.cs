using NUnit.Framework;
using SurvivalOfTheFittest;
using UnityEngine;

namespace SurvivalOfTheFittest.Tests
{
    public class MazeDeterminismTests
    {
        [Test]
        public void Maze_Layout_Is_Fixed_And_Contains_Required_Pieces()
        {
            Assert.IsTrue(MazeData.TryFind('P', out var px, out var py));
            Assert.IsTrue(MazeData.TryFind('X', out var xx, out var xy));
            Assert.IsTrue(MazeData.TryFind('A', out _, out _));
            Assert.IsTrue(MazeData.TryFind('B', out _, out _));
            Assert.IsTrue(MazeData.TryFind('C', out _, out _));

            var gems = 0;
            for (var y = 0; y < MazeData.Height; y++)
            for (var x = 0; x < MazeData.Width; x++)
            {
                if (MazeData.GetCell(x, y) == 'G')
                    gems++;
            }

            Assert.GreaterOrEqual(gems, 6);
            Assert.AreNotEqual(new Vector2Int(px, py), new Vector2Int(xx, xy));
        }

        [Test]
        public void CellToWorld_Is_Deterministic()
        {
            var a = MazeData.CellToWorld(3, 4);
            var b = MazeData.CellToWorld(3, 4);
            Assert.AreEqual(a, b);
        }
    }
}
