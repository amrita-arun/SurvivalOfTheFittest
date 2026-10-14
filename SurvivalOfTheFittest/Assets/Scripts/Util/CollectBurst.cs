using System.Collections;
using UnityEngine;

namespace SurvivalOfTheFittest
{
    /// <summary>
    /// Simple deterministic particle-like spark burst for gem pickup juice.
    /// </summary>
    public class CollectBurst : MonoBehaviour
    {
        public static void Spawn(Vector3 position, Color color, Transform parent = null)
        {
            var root = new GameObject("CollectBurst");
            if (parent != null)
                root.transform.SetParent(parent, true);
            root.transform.position = position;
            var burst = root.AddComponent<CollectBurst>();
            burst.StartCoroutine(burst.Play(color));
        }

        private IEnumerator Play(Color color)
        {
            const int count = 8;
            var bits = new SpriteRenderer[count];
            var dirs = new Vector2[count];
            var speeds = new float[count];

            for (var i = 0; i < count; i++)
            {
                var angle = i / (float)count * Mathf.PI * 2f;
                dirs[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                speeds[i] = 2.2f + (i % 3) * 0.35f;

                var go = new GameObject("Bit");
                go.transform.SetParent(transform, false);
                go.transform.localScale = Vector3.one * 0.18f;
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = SpriteFactory.WhiteCircle;
                sr.color = color;
                sr.sortingOrder = 25;
                bits[i] = sr;
            }

            var t = 0f;
            const float life = 0.35f;
            while (t < life)
            {
                t += Time.unscaledDeltaTime;
                var n = t / life;
                for (var i = 0; i < count; i++)
                {
                    if (bits[i] == null)
                        continue;
                    bits[i].transform.localPosition = dirs[i] * (speeds[i] * n);
                    var c = color;
                    c.a = 1f - n;
                    bits[i].color = c;
                    bits[i].transform.localScale = Vector3.one * Mathf.Lerp(0.18f, 0.05f, n);
                }

                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
