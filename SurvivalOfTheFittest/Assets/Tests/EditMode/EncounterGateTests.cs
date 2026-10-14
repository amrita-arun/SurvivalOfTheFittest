using System.Collections.Generic;
using NUnit.Framework;
using SurvivalOfTheFittest;
using UnityEngine;

namespace SurvivalOfTheFittest.Tests
{
    public class EncounterGateTests
    {
        [Test]
        public void Every_Encounter_Is_A_Required_Gate()
        {
            Assert.IsNull(ShortestPath('X', block: new[] { 'A' }));
            Assert.IsNull(ShortestPath('X', block: new[] { 'B' }));
            Assert.IsNull(ShortestPath('X', block: new[] { 'C' }));
            Assert.IsNotNull(ShortestPath('X', block: System.Array.Empty<char>()));
        }

        [Test]
        public void Some_Flags_Are_Before_The_Fights_And_Not_All_Of_Them()
        {
            var before = Reachable(block: new[] { 'A', 'B', 'C' });
            var gemsBefore = 0;
            var gems = 0;
            for (var y = 0; y < MazeData.Height; y++)
            for (var x = 0; x < MazeData.Width; x++)
            {
                if (MazeData.GetCell(x, y) != 'G')
                    continue;
                gems++;
                if (before.Contains(new Vector2Int(x, y)))
                    gemsBefore++;
            }

            Assert.GreaterOrEqual(gemsBefore, 4);
            Assert.Less(gemsBefore, gems);
            Assert.IsFalse(before.Contains(Find('X')));
        }

        [Test]
        public void Rush_Is_Long_And_Full_Clear_Does_Not_Fit_The_Clock()
        {
            var rush = ShortestPath('X', block: System.Array.Empty<char>());
            var toA = ShortestPath('A', block: new[] { 'B', 'C' });
            var toB = ShortestPath('B', block: new[] { 'C' });
            var toC = ShortestPath('C', block: System.Array.Empty<char>());

            Assert.IsNotNull(rush);
            Assert.IsNotNull(toA);
            Assert.IsNotNull(toB);
            Assert.IsNotNull(toC);
            Assert.Less(toA.Value, toB.Value);
            Assert.Less(toB.Value, toC.Value);
            Assert.Less(toC.Value, rush.Value);
            Assert.GreaterOrEqual(rush.Value, 100);

            var tour = FlagTourToExit();
            Assert.GreaterOrEqual(tour[3], rush.Value + 50, "Three flags should cost a real detour.");
            Assert.GreaterOrEqual(tour[tour.Length - 1], 280, "Every flag plus the exit is a long detour.");
        }

        [Test]
        public void The_Braid_Has_Many_Junctions_And_No_Highway()
        {
            var junctions = 0;
            var longest = 1;
            for (var y = 0; y < MazeData.Height; y++)
            {
                var run = 0;
                for (var x = 0; x < MazeData.Width; x++)
                {
                    if (MazeData.GetCell(x, y) == '#')
                    {
                        run = 0;
                        continue;
                    }

                    run++;
                    if (run > longest) longest = run;
                    if (Degree(x, y) >= 3) junctions++;
                }
            }

            for (var x = 0; x < MazeData.Width; x++)
            {
                var run = 0;
                for (var y = 0; y < MazeData.Height; y++)
                {
                    run = MazeData.GetCell(x, y) == '#' ? 0 : run + 1;
                    if (run > longest) longest = run;
                }
            }

            Assert.GreaterOrEqual(junctions, 40);
            Assert.LessOrEqual(longest, 5);
        }

        private static int Degree(int x, int y)
        {
            var n = 0;
            if (MazeData.GetCell(x + 1, y) != '#') n++;
            if (MazeData.GetCell(x - 1, y) != '#') n++;
            if (MazeData.GetCell(x, y + 1) != '#') n++;
            if (MazeData.GetCell(x, y - 1) != '#') n++;
            return n;
        }

        private static int[] FlagTourToExit()
        {
            var gems = new List<Vector2Int>();
            for (var y = 0; y < MazeData.Height; y++)
            for (var x = 0; x < MazeData.Width; x++)
            {
                if (MazeData.GetCell(x, y) == 'G')
                    gems.Add(new Vector2Int(x, y));
            }

            var index = new Dictionary<Vector2Int, int>();
            for (var i = 0; i < gems.Count; i++)
                index[gems[i]] = i;

            var start = Find('P');
            var best = new HashSet<(int x, int y, int mask)>();
            var heap = new SortedSet<(int d, int x, int y, int mask)>(Comparer<(int d, int x, int y, int mask)>.Create((a, b) =>
            {
                var c = a.d.CompareTo(b.d);
                if (c != 0) return c;
                c = a.x.CompareTo(b.x);
                if (c != 0) return c;
                c = a.y.CompareTo(b.y);
                if (c != 0) return c;
                return a.mask.CompareTo(b.mask);
            }));
            heap.Add((0, start.x, start.y, 0));
            var ans = new int[gems.Count + 1];
            for (var i = 0; i < ans.Length; i++)
                ans[i] = -1;

            while (heap.Count > 0)
            {
                var node = heap.Min;
                heap.Remove(heap.Min);
                if (!best.Add((node.x, node.y, node.mask)))
                    continue;

                if (MazeData.GetCell(node.x, node.y) == 'X')
                {
                    var count = 0;
                    var bits = node.mask;
                    while (bits > 0)
                    {
                        count += bits & 1;
                        bits >>= 1;
                    }

                    for (var k = 0; k <= count; k++)
                    {
                        if (ans[k] < 0)
                            ans[k] = node.d;
                    }

                    var done = true;
                    for (var k = 0; k < ans.Length; k++)
                    {
                        if (ans[k] < 0)
                            done = false;
                    }

                    if (done)
                        break;
                }

                Try(node.x + 1, node.y);
                Try(node.x - 1, node.y);
                Try(node.x, node.y + 1);
                Try(node.x, node.y - 1);

                void Try(int x, int y)
                {
                    if (MazeData.GetCell(x, y) == '#')
                        return;
                    var mask = node.mask;
                    var key = new Vector2Int(x, y);
                    if (index.TryGetValue(key, out var bit))
                        mask |= 1 << bit;
                    if (best.Contains((x, y, mask)))
                        return;
                    heap.Add((node.d + 1, x, y, mask));
                }
            }

            return ans;
        }

        private static Vector2Int Find(char symbol)
        {
            Assert.IsTrue(MazeData.TryFind(symbol, out var x, out var y));
            return new Vector2Int(x, y);
        }

        private static HashSet<Vector2Int> Reachable(char[] block)
        {
            var blocked = new HashSet<char>(block);
            var start = Find('P');
            var queue = new Queue<Vector2Int>();
            var seen = new HashSet<Vector2Int> { start };
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var p = queue.Dequeue();
                Try(p.x + 1, p.y);
                Try(p.x - 1, p.y);
                Try(p.x, p.y + 1);
                Try(p.x, p.y - 1);

                void Try(int x, int y)
                {
                    var key = new Vector2Int(x, y);
                    if (!seen.Add(key))
                        return;
                    var c = MazeData.GetCell(x, y);
                    if (c == '#' || blocked.Contains(c))
                        return;
                    queue.Enqueue(key);
                }
            }

            return seen;
        }

        private static int? ShortestPath(char goal, char[] block)
        {
            var blocked = new HashSet<char>(block);
            var start = Find('P');
            var queue = new Queue<(Vector2Int p, int d)>();
            var seen = new HashSet<Vector2Int> { start };
            queue.Enqueue((start, 0));

            while (queue.Count > 0)
            {
                var (p, d) = queue.Dequeue();
                if (MazeData.GetCell(p.x, p.y) == goal && p != start)
                    return d;

                Try(p.x + 1, p.y, d + 1);
                Try(p.x - 1, p.y, d + 1);
                Try(p.x, p.y + 1, d + 1);
                Try(p.x, p.y - 1, d + 1);

                void Try(int x, int y, int nd)
                {
                    var key = new Vector2Int(x, y);
                    if (!seen.Add(key))
                        return;
                    var c = MazeData.GetCell(x, y);
                    if (c == '#' || blocked.Contains(c))
                        return;
                    queue.Enqueue((key, nd));
                }
            }

            return null;
        }
    }
}
