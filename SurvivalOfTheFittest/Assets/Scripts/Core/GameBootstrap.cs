using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using SurvivalOfTheFittest.UI;

namespace SurvivalOfTheFittest
{
    /// <summary>
    /// Runtime bootstrap: builds the entire playable prototype from code
    /// so minimal Inspector wiring is required.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class GameBootstrap : MonoBehaviour
    {
        public static GameBootstrap Instance { get; private set; }

        [Header("Auto Setup")]
        [Tooltip("If true, builds the game on Awake.")]
        [SerializeField] private bool buildOnAwake = true;

        private Transform _worldRoot;
        private MazeBuilder _maze;
        private GameManager _gameManager;
        private RevealMeter _revealMeter;
        private FogOfWarController _fog;
        private PlayerController _player;
        private readonly List<GameObject> _runtimeObjects = new();
        private bool _uiBuilt;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AutoCreateBootstrap()
        {
            if (FindFirstObjectByType<GameBootstrap>() != null)
                return;

            var go = new GameObject("GameBootstrap");
            go.AddComponent<GameBootstrap>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            if (buildOnAwake)
                BuildEverything();
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void BuildEverything()
        {
            EnsureEventSystem();
            EnsureCamera();

            var systems = GetOrCreate("Systems");
            _gameManager = GetOrAdd<GameManager>(systems);
            GetOrAdd<DiceRoller>(systems);
            GetOrAdd<EncounterManager>(systems);
            GetOrAdd<GameAudio>(systems);
            GetOrAdd<CameraJuice>(systems);
            _revealMeter = GetOrAdd<RevealMeter>(systems);

            _worldRoot = GetOrCreate("World").transform;
            _maze = GetOrAdd<MazeBuilder>(_worldRoot.gameObject);

            BuildUi();
            RebuildRun();
        }

        public void RebuildRun()
        {
            DiceRoller.Instance?.CancelRoll();
            ClearRuntimeEntities();

            _maze.Build();
            ConfigureCameraForMaze();

            SpawnPlayer();
            SpawnGems();
            SpawnEncounters();
            SpawnFog();

            _gameManager.Bind(_maze, _player, _fog, _revealMeter);
            _gameManager.BeginTutorial();
        }

        private void ClearRuntimeEntities()
        {
            EncounterManager.Instance?.ClearAll();

            if (_player != null)
            {
                Destroy(_player.gameObject);
                _player = null;
            }

            if (_fog != null)
            {
                Destroy(_fog.gameObject);
                _fog = null;
            }

            foreach (var go in _runtimeObjects)
            {
                if (go != null)
                    Destroy(go);
            }

            _runtimeObjects.Clear();
            _maze?.ClearBuilt();
        }

        private void SpawnPlayer()
        {
            var go = new GameObject("Player");
            go.transform.SetParent(_worldRoot, false);
            go.transform.position = _maze.PlayerStartWorld;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = SpriteFactory.WhiteCircle;
            sr.color = new Color(0.3f, 1f, 0.85f, 1f);
            sr.sortingOrder = 10;
            go.transform.localScale = Vector3.one * 0.55f;

            go.AddComponent<CircleCollider2D>();
            go.AddComponent<Rigidbody2D>();
            _player = go.AddComponent<PlayerController>();
            _player.Teleport(_maze.PlayerStartWorld);

            var glow = new GameObject("Glow");
            glow.transform.SetParent(go.transform, false);
            glow.transform.localScale = Vector3.one * 1.35f;
            var gsr = glow.AddComponent<SpriteRenderer>();
            gsr.sprite = SpriteFactory.WhiteCircle;
            gsr.color = new Color(0.3f, 1f, 0.85f, 0.25f);
            gsr.sortingOrder = 9;

            _player.SetVisualRefs(sr, glow.transform);
            _runtimeObjects.Add(go);
        }

        private void SpawnGems()
        {
            foreach (var pos in _maze.GemPositions)
            {
                var gem = _maze.SpawnGem(pos, () => _gameManager.OnGemCollected());
                _runtimeObjects.Add(gem);
            }
        }

        private void SpawnEncounters()
        {
            var defs = new Dictionary<char, EncounterDefinition>
            {
                { 'A', EncounterDefinition.CreateEx() },
                { 'B', EncounterDefinition.CreateApp() },
                { 'C', EncounterDefinition.CreateMixed() },
                { 'D', EncounterDefinition.CreateFamilyFriend() },
                { 'E', EncounterDefinition.CreateFriendsEx() }
            };

            foreach (var (id, pos) in _maze.EncounterPositions)
            {
                if (!defs.TryGetValue(id, out var def))
                    continue;

                var go = new GameObject(def.DisplayName);
                go.transform.SetParent(_maze.EntitiesRoot, false);
                go.transform.position = pos;
                var trigger = go.AddComponent<EncounterTrigger>();
                trigger.Initialize(def);
                EncounterManager.Instance?.Register(trigger);
                _runtimeObjects.Add(go);
            }
        }

        private void SpawnFog()
        {
            var go = new GameObject("FogOfWar");
            go.transform.SetParent(_worldRoot, false);
            _fog = go.AddComponent<FogOfWarController>();
            _fog.Initialize(_player.transform);
            _runtimeObjects.Add(go);
        }

        private void BuildUi()
        {
            if (_uiBuilt && GameObject.Find("GameCanvas") != null)
                return;

            var existing = GameObject.Find("GameCanvas");
            if (existing != null)
                Destroy(existing);

            var canvas = UiFactory.CreateCanvas("GameCanvas", 100);
            // Canvas is persistent across restarts — do not track in _runtimeObjects.

            var hud = canvas.gameObject.AddComponent<HUDController>();
            hud.Build(canvas.transform);

            var tutorial = canvas.gameObject.AddComponent<TutorialUI>();
            tutorial.Build(canvas.transform);

            var encounter = canvas.gameObject.AddComponent<EncounterUI>();
            encounter.Build(canvas.transform);

            var gameOver = canvas.gameObject.AddComponent<GameOverUI>();
            gameOver.Build(canvas.transform);

            var flash = canvas.gameObject.AddComponent<ScreenFlash>();
            flash.Build(canvas.transform);

            _uiBuilt = true;
        }

        private static void EnsureEventSystem()
        {
            if (FindFirstObjectByType<EventSystem>() != null)
                return;

            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<InputSystemUIInputModule>();
        }

        private void EnsureCamera()
        {
            var cam = Camera.main;
            if (cam == null)
            {
                var go = new GameObject("Main Camera");
                cam = go.AddComponent<Camera>();
                go.tag = "MainCamera";
                go.AddComponent<AudioListener>();
            }

            cam.orthographic = true;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;
            cam.transform.position = new Vector3(MazeData.MazeCenterWorld.x, MazeData.MazeCenterWorld.y, -10f);
            CameraJuice.Instance?.CacheBase();
        }

        private void ConfigureCameraForMaze()
        {
            var cam = Camera.main;
            if (cam == null)
                return;

            cam.transform.position = new Vector3(MazeData.MazeCenterWorld.x, MazeData.MazeCenterWorld.y, -10f);

            var aspect = cam.aspect > 0.01f ? cam.aspect : 16f / 9f;
            var halfH = MazeData.MazeSizeWorld.y * 0.5f + 0.5f;
            var halfW = MazeData.MazeSizeWorld.x * 0.5f + 0.5f;
            cam.orthographicSize = Mathf.Max(halfH, halfW / aspect);
            CameraJuice.Instance?.CacheBase();
        }

        private GameObject GetOrCreate(string name)
        {
            var t = transform.Find(name);
            if (t != null)
                return t.gameObject;

            var go = new GameObject(name);
            go.transform.SetParent(transform, false);
            return go;
        }

        private static T GetOrAdd<T>(GameObject go) where T : Component
        {
            var c = go.GetComponent<T>();
            if (c == null)
                c = go.AddComponent<T>();
            return c;
        }
    }
}
