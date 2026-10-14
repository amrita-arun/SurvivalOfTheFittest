using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SurvivalOfTheFittest
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Run Settings")]
        [Tooltip("Starting countdown in seconds. Tighter for a harder 1–2 min run.")]
        [SerializeField] private float startingTime = 90f;

        [SerializeField] private int startingHearts = 3;

        [Tooltip("Exit stays locked until this many dating obstacles are cleared. The maze also forces a walk through all three.")]
        [SerializeField] private int encountersRequiredToExit = 3;

        public GameState State { get; private set; } = GameState.Tutorial;
        public float TimeRemaining { get; private set; }
        public int Hearts { get; private set; }
        public int LifetimeGems => _revealMeter != null ? _revealMeter.LifetimeGems : 0;

        public bool AllowsPlayerMovement => State == GameState.Playing;

        public bool AllowsGameplayMotion => State == GameState.Playing;

        public bool TimerRunning => State == GameState.Playing;

        private MazeBuilder _maze;
        private PlayerController _player;
        private FogOfWarController _fog;
        private RevealMeter _revealMeter;
        private readonly List<GameObject> _spawnedGems = new();
        private string _lossReason = "";

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void Bind(
            MazeBuilder maze,
            PlayerController player,
            FogOfWarController fog,
            RevealMeter revealMeter)
        {
            _maze = maze;
            _player = player;
            _fog = fog;
            _revealMeter = revealMeter;
        }

        public void BeginTutorial()
        {
            State = GameState.Tutorial;
            TimeRemaining = startingTime;
            Hearts = startingHearts;
            _revealMeter?.ResetMeters();
            _fog?.ResetFog();
            _lossReason = "";

            UI.TutorialUI.Instance?.Show();
            UI.HUDController.Instance?.SetVisible(false);
            UI.GameOverUI.Instance?.Hide();
            UI.EncounterUI.Instance?.Hide();
            _player?.SetCanMove(false);
            RefreshHud();
        }

        public void StartGameFromTutorial()
        {
            GameAudio.Instance?.PlayUiClick();
            UI.TutorialUI.Instance?.Hide();
            UI.HUDController.Instance?.SetVisible(true);
            State = GameState.Playing;
            _player?.SetCanMove(true);
            _player?.MarkSafePosition();
            RefreshHud();
        }

        private void Update()
        {
            HandleGlobalKeys();

            if (TimerRunning)
            {
                TimeRemaining -= Time.deltaTime;
                if (TimeRemaining <= 0f)
                {
                    TimeRemaining = 0f;
                    Lose("You ran out of time. The group chat moved on without you.");
                }
            }

            RefreshHud();
        }

        private void HandleGlobalKeys()
        {
            var kb = Keyboard.current;
            if (kb == null)
                return;

            if (kb.rKey.wasPressedThisFrame)
                RestartGame();

            if (kb.qKey.wasPressedThisFrame || kb.escapeKey.wasPressedThisFrame)
                QuitGame();
        }

        public void OnGemCollected()
        {
            if (_revealMeter == null)
                return;

            GameAudio.Instance?.PlayGem();
            ScreenFlash.Instance?.Flash(new Color(0.3f, 1f, 0.55f), 0.28f, 0.2f);
            CameraJuice.Instance?.Shake(0.08f, 0.12f);
            if (_player != null)
                CollectBurst.Spawn(_player.transform.position, new Color(0.35f, 1f, 0.55f));

            _revealMeter.RegisterGem(1);
            RefreshHud();
        }

        public void GrantBonusGem()
        {
            if (_revealMeter == null)
                return;

            GameAudio.Instance?.PlayGem();
            _revealMeter.RegisterGem(1);
            RefreshHud();
            UI.EncounterUI.Instance?.RefreshOdds(LifetimeGems);
        }

        public void EnterEncounter()
        {
            if (State == GameState.Won || State == GameState.Lost)
                return;
            State = GameState.Encounter;
            _player?.SetCanMove(false);
        }

        public void ExitEncounter()
        {
            if (State == GameState.Won || State == GameState.Lost)
                return;
            State = GameState.Playing;
            _player?.SetCanMove(true);
            _player?.MarkSafePosition();
        }

        public void Damage(int amount)
        {
            Hearts = Mathf.Max(0, Hearts - amount);
            _player?.PlayHurtPulse();
            ScreenFlash.Instance?.Flash(new Color(1f, 0.2f, 0.35f), 0.5f, 0.28f);
            CameraJuice.Instance?.Shake(0.28f, 0.22f);
            RefreshHud();
            if (Hearts <= 0)
                Lose("You lost all your hearts. Emotional damage: critical.");
        }

        public void OnEncounterResolved(int resolvedCount)
        {
            RefreshHud();
            if (resolvedCount >= encountersRequiredToExit)
                UI.HUDController.Instance?.ShowBanner("EXIT UNLOCKED!", 1.4f);
        }

        public int EncountersRequiredToExit => encountersRequiredToExit;

        public int EncountersResolved =>
            EncounterManager.Instance != null ? EncounterManager.Instance.ResolvedCount : 0;

        public bool ExitIsUnlocked => EncountersResolved >= encountersRequiredToExit;

        public void AddTime(float delta)
        {
            TimeRemaining = Mathf.Max(0f, TimeRemaining + delta);
            RefreshHud();
        }

        public void ReturnPlayerToLastSafe()
        {
            if (_player == null)
                return;
            _player.Teleport(_player.LastSafePosition);
        }

        public void TryWin()
        {
            if (State != GameState.Playing)
                return;

            if (!ExitIsUnlocked)
            {
                var need = encountersRequiredToExit - EncountersResolved;
                GameAudio.Instance?.PlayBlocked();
                CameraJuice.Instance?.Shake(0.12f, 0.15f);
                UI.HUDController.Instance?.ShowBanner(
                    $"EXIT LOCKED — clear {need} more obstacle{(need == 1 ? "" : "s")}!", 1.6f);
                return;
            }

            State = GameState.Won;
            _player?.SetCanMove(false);
            EncounterManager.Instance?.ForceClose();
            GameAudio.Instance?.PlayWin();
            ScreenFlash.Instance?.Flash(new Color(0.4f, 1f, 0.8f), 0.45f, 0.4f);
            UI.GameOverUI.Instance?.ShowWin(TimeRemaining, LifetimeGems);
        }

        public void Lose(string reason)
        {
            if (State == GameState.Won || State == GameState.Lost)
                return;

            _lossReason = reason;
            State = GameState.Lost;
            _player?.SetCanMove(false);
            EncounterManager.Instance?.ForceClose();
            GameAudio.Instance?.PlayLoss();
            UI.GameOverUI.Instance?.ShowLoss(_lossReason, LifetimeGems);
        }

        public void RestartGame()
        {
            GameBootstrap.Instance?.RebuildRun();
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void NotifyRunBuilt(PlayerController player)
        {
            _player = player;
        }

        private void RefreshHud()
        {
            UI.HUDController.Instance?.UpdateHud(
                TimeRemaining,
                Hearts,
                LifetimeGems,
                EncountersResolved,
                encountersRequiredToExit,
                _player != null ? _player.DashCooldownNormalized : 0f);
        }
    }
}
