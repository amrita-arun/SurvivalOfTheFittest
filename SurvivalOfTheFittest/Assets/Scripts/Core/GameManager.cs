using System.Collections;
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
        [Tooltip("Starting countdown in seconds.")]
        [SerializeField] private float startingTime = 90f;

        [SerializeField] private int startingHearts = 3;

        public GameState State { get; private set; } = GameState.Tutorial;
        public float TimeRemaining { get; private set; }
        public int Hearts { get; private set; }
        public int LifetimeGems => _revealMeter != null ? _revealMeter.LifetimeGems : 0;
        public int ClaritySegments => _revealMeter != null ? _revealMeter.SegmentFill : 0;

        public bool AllowsPlayerMovement =>
            State == GameState.Playing || State == GameState.ClarityReveal;

        public bool AllowsGameplayMotion =>
            State == GameState.Playing || State == GameState.ClarityReveal;

        public bool TimerRunning => State == GameState.Playing || State == GameState.ClarityReveal;

        private MazeBuilder _maze;
        private PlayerController _player;
        private FogOfWarController _fog;
        private RevealMeter _revealMeter;
        private readonly List<GameObject> _spawnedGems = new();
        private string _lossReason = "";
        private bool _clarityBusy;

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
            _clarityBusy = false;
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
            var clarity = _revealMeter.RegisterGem(1);
            RefreshHud();

            if (clarity && !_clarityBusy)
                StartCoroutine(ClarityRoutine());
        }

        public void GrantBonusGem()
        {
            if (_revealMeter == null)
                return;

            GameAudio.Instance?.PlayGem();
            var clarity = _revealMeter.RegisterGem(1);
            RefreshHud();
            UI.EncounterUI.Instance?.RefreshOdds(LifetimeGems);

            if (clarity && !_clarityBusy && State != GameState.Lost && State != GameState.Won)
                StartCoroutine(ClarityRoutine());
        }

        private IEnumerator ClarityRoutine()
        {
            _clarityBusy = true;
            var enteredFromPlaying = State == GameState.Playing;
            if (enteredFromPlaying)
                State = GameState.ClarityReveal;

            GameAudio.Instance?.PlayClarity();
            UI.HUDController.Instance?.ShowBanner("CLARITY ACTIVATED!", 2f);

            var done = false;
            _fog?.PlayFullReveal(_revealMeter != null ? _revealMeter.RevealDuration : 2f, () => done = true);
            while (!done)
                yield return null;

            // Clarity segments already cleared when the 3rd gem filled the meter.
            // Resetting again must never touch LifetimeGems.
            _revealMeter?.ResetClaritySegmentsOnly();

            if (enteredFromPlaying && State == GameState.ClarityReveal)
                State = GameState.Playing;

            _clarityBusy = false;
            RefreshHud();
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
            RefreshHud();
            if (Hearts <= 0)
                Lose("You lost all your hearts. Emotional damage: critical.");
        }

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
            if (State != GameState.Playing && State != GameState.ClarityReveal)
                return;

            State = GameState.Won;
            _player?.SetCanMove(false);
            EncounterManager.Instance?.ForceClose();
            GameAudio.Instance?.PlayWin();
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
            StopAllCoroutines();
            _clarityBusy = false;
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
            UI.HUDController.Instance?.UpdateHud(TimeRemaining, Hearts, LifetimeGems, ClaritySegments);
        }
    }
}
