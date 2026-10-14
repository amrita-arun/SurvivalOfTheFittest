using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace SurvivalOfTheFittest.Tests
{
    public class RandomUsageAuditTests
    {
        [Test]
        public void Only_DiceRoller_May_Call_UnityEngine_Random()
        {
            var assets = Path.GetFullPath(Path.Combine(Application.dataPath));
            var scriptsRoot = Path.Combine(assets, "Scripts");
            Assert.IsTrue(Directory.Exists(scriptsRoot), "Scripts folder missing");

            var offenders = new System.Collections.Generic.List<string>();
            foreach (var file in Directory.GetFiles(scriptsRoot, "*.cs", SearchOption.AllDirectories))
            {
                if (file.Replace('\\', '/').EndsWith("/Dice/DiceRoller.cs"))
                    continue;

                var text = File.ReadAllText(file);
                if (text.Contains("UnityEngine.Random") || text.Contains("Random.Range") || text.Contains("Random.value"))
                {
                    // Allow comments mentioning Random in docs-like comments only if clearly not a call —
                    // flag any file that isn't DiceRoller.
                    offenders.Add(Path.GetRelativePath(assets, file));
                }
            }

            Assert.IsEmpty(offenders,
                "Random usage found outside DiceRoller:\n" + string.Join("\n", offenders));
        }
    }
}
