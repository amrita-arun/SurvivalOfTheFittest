using System.Collections.Generic;
using UnityEngine;

namespace SurvivalOfTheFittest
{
    public class MazeBuilder : MonoBehaviour
    {
        [Header("Colors")]
        [SerializeField] private Color wallColor = new Color(0.35f, 0.1f, 0.45f, 1f);
        [SerializeField] private Color floorColor = new Color(0.08f, 0.08f, 0.14f, 1f);
        [SerializeField] private Color exitColor = new Color(0.2f, 1f, 0.75f, 1f);
        [SerializeField] private Color gemColor = new Color(0.25f, 0.9f, 0.4f, 1f);

        public Transform WallsRoot { get; private set; }
        public Transform FloorRoot { get; private set; }
        public Transform EntitiesRoot { get; private set; }
        public Vector2 PlayerStartWorld { get; private set; }
        public Vector2 ExitWorld { get; private set; }
        public List<Vector2> GemPositions { get; private set; } = new();
        public List<(char id, Vector2 pos)> EncounterPositions { get; private set; } = new();

        private readonly List<GameObject> _built = new();

        public void Build()
        {
            ClearBuilt();

            WallsRoot = CreateRoot("Walls");
            FloorRoot = CreateRoot("Floor");
            EntitiesRoot = CreateRoot("Entities");

            for (var y = 0; y < MazeData.Height; y++)
            {
                for (var x = 0; x < MazeData.Width; x++)
                {
                    var cell = MazeData.GetCell(x, y);
                    var world = MazeData.CellToWorld(x, y);

                    if (cell == '#')
                    {
                        CreateTile("Wall", world, wallColor, WallsRoot, true, 0);
                    }
                    else
                    {
                        CreateTile("Floor", world, floorColor, FloorRoot, false, -1);

                        switch (cell)
                        {
                            case 'P':
                                PlayerStartWorld = world;
                                break;
                            case 'X':
                                ExitWorld = world;
                                CreateExit(world);
                                break;
                            case 'G':
                                GemPositions.Add(world);
                                break;
                            case 'A':
                            case 'B':
                            case 'C':
                            case 'D':
                            case 'E':
                                EncounterPositions.Add((cell, world));
                                break;
                        }
                    }
                }
            }

            if (!MazeData.TryFind('P', out _, out _))
                Debug.LogError("[MazeBuilder] Player start 'P' missing from maze layout.");
            if (!MazeData.TryFind('X', out _, out _))
                Debug.LogError("[MazeBuilder] Exit 'X' missing from maze layout.");
        }

        public void ClearBuilt()
        {
            foreach (var go in _built)
            {
                if (go != null)
                    Destroy(go);
            }

            _built.Clear();
            GemPositions.Clear();
            EncounterPositions.Clear();
            WallsRoot = FloorRoot = EntitiesRoot = null;
        }

        private Transform CreateRoot(string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform, false);
            _built.Add(go);
            return go.transform;
        }

        private GameObject CreateTile(string name, Vector2 world, Color color, Transform parent, bool solid, int sorting)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = world;
            go.transform.localScale = Vector3.one * MazeData.CellSize * 0.98f;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteFactory.WhiteSquare;
            sr.color = color;
            sr.sortingOrder = sorting;

            if (solid)
            {
                var col = go.AddComponent<BoxCollider2D>();
                col.size = Vector2.one;
            }

            _built.Add(go);
            return go;
        }

        private void CreateExit(Vector2 world)
        {
            var go = new GameObject("Exit");
            go.transform.SetParent(EntitiesRoot, false);
            go.transform.position = world;
            go.transform.localScale = Vector3.one * 0.85f;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteFactory.WhiteSquare;
            sr.color = exitColor;
            sr.sortingOrder = 2;

            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = Vector2.one * 0.8f;

            go.AddComponent<ExitTrigger>();
            _built.Add(go);

            var label = CreateWorldLabel(go.transform, "EXIT", exitColor);
            _built.Add(label);
        }

        private GameObject CreateWorldLabel(Transform parent, string text, Color color)
        {
            var go = new GameObject("Label");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = new Vector3(0f, 0.55f, 0f);

            var tm = go.AddComponent<TextMesh>();
            tm.text = text;
            tm.characterSize = 0.12f;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.fontSize = 32;
            tm.color = color;
            tm.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
                      ?? Resources.GetBuiltinResource<Font>("Arial.ttf");

            var mr = go.GetComponent<MeshRenderer>();
            if (mr != null)
                mr.sortingOrder = 20;

            return go;
        }

        public GameObject SpawnGem(Vector2 world, System.Action onCollected)
        {
            var go = new GameObject("Gem");
            go.transform.SetParent(EntitiesRoot, false);
            go.transform.position = world;
            go.transform.localScale = Vector3.one * 0.45f;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteFactory.GreenFlag;
            sr.color = gemColor;
            sr.sortingOrder = 3;

            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.45f;

            var gem = go.AddComponent<GemCollectible>();
            gem.Initialize(onCollected, gemColor);

            _built.Add(go);
            return go;
        }
    }
}
