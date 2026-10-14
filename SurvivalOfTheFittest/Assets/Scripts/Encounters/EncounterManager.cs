using System.Collections.Generic;
using UnityEngine;

namespace SurvivalOfTheFittest
{
    public class EncounterManager : MonoBehaviour
    {
        public static EncounterManager Instance { get; private set; }

        private readonly List<EncounterTrigger> _triggers = new();
        private EncounterTrigger _active;
        private EncounterActionDef _chosenAction;
        private bool _awaitingAck;
        private bool _rollStarted;

        public EncounterTrigger ActiveTrigger => _active;
        public bool IsEncounterOpen => _active != null;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void Register(EncounterTrigger trigger)
        {
            if (!_triggers.Contains(trigger))
                _triggers.Add(trigger);
        }

        public void ClearAll()
        {
            _triggers.Clear();
            _active = null;
            _chosenAction = null;
            _awaitingAck = false;
            _rollStarted = false;
        }

        public void BeginEncounter(EncounterTrigger trigger)
        {
            if (_active != null || trigger == null || trigger.IsResolved)
                return;

            _active = trigger;
            _chosenAction = null;
            _awaitingAck = false;
            _rollStarted = false;

            GameManager.Instance?.EnterEncounter();
            GameAudio.Instance?.PlayEncounter();
            UI.EncounterUI.Instance?.ShowEncounter(trigger.Definition, GetLifetimeGems());
        }

        public void ChooseAction(EncounterActionDef action)
        {
            if (_active == null || action == null || _rollStarted || _awaitingAck)
                return;
            if (DiceRoller.Instance != null && DiceRoller.Instance.IsRolling)
                return;

            _chosenAction = action;
            _rollStarted = true;

            var gems = GetLifetimeGems();
            var required = GemOddsCalculator.GetRequiredRoll(gems, action.Risk);

            UI.EncounterUI.Instance?.ShowRolling(action, required, gems);
            GameAudio.Instance?.PlayDice();

            DiceRoller.Instance.Roll(
                onComplete: roll => ResolveRoll(roll, required),
                onPreviewFace: face => UI.EncounterUI.Instance?.SetDieFace(face));
        }

        private void ResolveRoll(int roll, int required)
        {
            if (_active == null || _chosenAction == null)
                return;

            var success = GemOddsCalculator.IsSuccess(roll, required);
            var resultLine = GemOddsCalculator.FormatResultLine(roll, required, success);

            if (success)
            {
                GameAudio.Instance?.PlaySuccess();
                ApplySuccess(_chosenAction);
            }
            else
            {
                GameAudio.Instance?.PlayFailure();
                ApplyFailure(_chosenAction);
            }

            var gemsNow = GetLifetimeGems();
            UI.EncounterUI.Instance?.ShowResult(resultLine, success, gemsNow, _chosenAction);
            _awaitingAck = true;
        }

        private void ApplySuccess(EncounterActionDef action)
        {
            switch (action.SuccessReward)
            {
                case EncounterRewardType.BonusGem:
                    GameManager.Instance?.GrantBonusGem();
                    break;
                case EncounterRewardType.BonusTime:
                    GameManager.Instance?.AddTime(action.SuccessTimeBonus);
                    break;
            }
        }

        private void ApplyFailure(EncounterActionDef action)
        {
            GameManager.Instance?.Damage(1);

            switch (action.FailExtra)
            {
                case EncounterFailExtra.CheckpointReturn:
                case EncounterFailExtra.ReturnLastSafe:
                    GameManager.Instance?.ReturnPlayerToLastSafe();
                    break;
                case EncounterFailExtra.LoseTime:
                    GameManager.Instance?.AddTime(-Mathf.Abs(action.FailTimePenalty));
                    break;
            }
        }

        public void AcknowledgeResult()
        {
            if (!_awaitingAck || _active == null)
                return;

            _active.MarkResolved();
            _active = null;
            _chosenAction = null;
            _awaitingAck = false;
            _rollStarted = false;

            UI.EncounterUI.Instance?.Hide();

            if (GameManager.Instance != null &&
                GameManager.Instance.State != GameState.Won &&
                GameManager.Instance.State != GameState.Lost)
            {
                GameManager.Instance.ExitEncounter();
            }
        }

        private static int GetLifetimeGems()
        {
            return GameManager.Instance != null ? GameManager.Instance.LifetimeGems : 0;
        }

        public void ResetEncounters()
        {
            foreach (var t in _triggers)
            {
                if (t != null)
                    t.ResetEncounter();
            }

            _active = null;
            _chosenAction = null;
            _awaitingAck = false;
            _rollStarted = false;
        }

        /// <summary>
        /// Closes any open encounter UI/state without resolving rewards again
        /// (used when the run ends mid-encounter).
        /// </summary>
        public void ForceClose()
        {
            DiceRoller.Instance?.CancelRoll();
            _active = null;
            _chosenAction = null;
            _awaitingAck = false;
            _rollStarted = false;
            UI.EncounterUI.Instance?.Hide();
        }
    }
}
