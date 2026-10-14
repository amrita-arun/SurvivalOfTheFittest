using UnityEngine;

namespace SurvivalOfTheFittest
{
    /// <summary>
    /// Creates simple code-generated sprites so the prototype needs no external art.
    /// </summary>
    public static class SpriteFactory
    {
        private static Sprite _whiteSquare;
        private static Sprite _whiteCircle;

        public static Sprite WhiteSquare
        {
            get
            {
                if (_whiteSquare == null)
                    _whiteSquare = CreateFilledSprite(32, 32, Color.white, circle: false);
                return _whiteSquare;
            }
        }

        public static Sprite WhiteCircle
        {
            get
            {
                if (_whiteCircle == null)
                    _whiteCircle = CreateFilledSprite(64, 64, Color.white, circle: true);
                return _whiteCircle;
            }
        }

        private static Sprite _greenFlag;

        public static Sprite GreenFlag
        {
            get
            {
                if (_greenFlag == null)
                    _greenFlag = CreateFlagSprite(64);
                return _greenFlag;
            }
        }

        /// <summary>
        /// White flag silhouette (pole + right-pointing pennant). Tinted by the renderer.
        /// </summary>
        public static Sprite CreateFlagSprite(int size)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
                name = "SOTF_Flag"
            };

            var clear = new Color(0f, 0f, 0f, 0f);
            for (var y = 0; y < size; y++)
                for (var x = 0; x < size; x++)
                    tex.SetPixel(x, y, clear);

            const float poleX0 = 0.30f, poleX1 = 0.40f, poleY0 = 0.08f, poleY1 = 0.92f;
            const float flagBaseX = 0.40f, flagTipX = 0.86f;
            const float flagTopY = 0.90f, flagMidY = 0.74f, flagBotY = 0.58f;

            for (var y = 0; y < size; y++)
            {
                var ny = y / (float)(size - 1);
                for (var x = 0; x < size; x++)
                {
                    var nx = x / (float)(size - 1);
                    var on = false;

                    if (nx >= poleX0 && nx <= poleX1 && ny >= poleY0 && ny <= poleY1)
                        on = true;

                    if (nx >= flagBaseX && nx <= flagTipX)
                    {
                        var t = (nx - flagBaseX) / (flagTipX - flagBaseX);
                        var halfTop = Mathf.Lerp(flagTopY - flagMidY, 0f, t);
                        var halfBot = Mathf.Lerp(flagMidY - flagBotY, 0f, t);
                        if (ny <= flagMidY + halfTop && ny >= flagMidY - halfBot)
                            on = true;
                    }

                    if (on)
                        tex.SetPixel(x, y, Color.white);
                }
            }

            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 32f);
        }

        public static Sprite CreateFilledSprite(int width, int height, Color color, bool circle)
        {
            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
                name = circle ? "SOTF_Circle" : "SOTF_Square"
            };

            var cx = (width - 1) * 0.5f;
            var cy = (height - 1) * 0.5f;
            var radius = Mathf.Min(cx, cy);

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    if (!circle)
                    {
                        tex.SetPixel(x, y, color);
                        continue;
                    }

                    var dx = x - cx;
                    var dy = y - cy;
                    var dist = Mathf.Sqrt(dx * dx + dy * dy);
                    var a = Mathf.Clamp01(radius - dist + 0.5f);
                    tex.SetPixel(x, y, new Color(color.r, color.g, color.b, color.a * a));
                }
            }

            tex.Apply();
            var pivot = new Vector2(0.5f, 0.5f);
            return Sprite.Create(tex, new Rect(0, 0, width, height), pivot, 32f);
        }

        /// <summary>
        /// Soft fog mask: opaque black outside, transparent soft circle in the center.
        /// </summary>
        public static Sprite CreateFogHoleSprite(int size, float innerClearRadiusNormalized, float feather)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
                name = "SOTF_FogHole"
            };

            var cx = (size - 1) * 0.5f;
            var cy = (size - 1) * 0.5f;
            var clearR = size * 0.5f * innerClearRadiusNormalized;
            var outerR = clearR + size * 0.5f * feather;

            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var dx = x - cx;
                    var dy = y - cy;
                    var dist = Mathf.Sqrt(dx * dx + dy * dy);
                    float alpha;
                    if (dist <= clearR)
                        alpha = 0f;
                    else if (dist >= outerR)
                        alpha = 1f;
                    else
                        alpha = Mathf.SmoothStep(0f, 1f, (dist - clearR) / Mathf.Max(0.001f, outerR - clearR));

                    tex.SetPixel(x, y, new Color(0f, 0f, 0f, alpha));
                }
            }

            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 32f);
        }

        public static Material CreateSpriteMaterial(Color color)
        {
            var shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit")
                         ?? Shader.Find("Sprites/Default");
            var mat = new Material(shader) { color = color };
            return mat;
        }
    }
}
