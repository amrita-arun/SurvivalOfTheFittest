using UnityEngine;

namespace SurvivalOfTheFittest
{
    public enum EncounterRewardType
    {
        None,
        BonusGem,
        BonusTime,
        EscapeOnly
    }

    public enum EncounterFailExtra
    {
        None,
        CheckpointReturn,
        LoseTime,
        ReturnLastSafe
    }

    [System.Serializable]
    public class EncounterActionDef
    {
        public string Label;
        public EncounterRisk Risk;
        public EncounterRewardType SuccessReward;
        public float SuccessTimeBonus;
        public EncounterFailExtra FailExtra;
        public float FailTimePenalty;
        public string SuccessFlavor;
        public string FailFlavor;
        public string RewardHint;
    }

    [System.Serializable]
    public class EncounterDefinition
    {
        public char MazeId;
        public string DisplayName;
        public string Dialogue;
        public Color Color;
        public EncounterActionDef SafeAction;
        public EncounterActionDef RiskyAction;

        public static EncounterDefinition CreateEx()
        {
            return new EncounterDefinition
            {
                MazeId = 'A',
                DisplayName = "The Ex-Situationship",
                Dialogue = "Hey stranger… I've changed. (The playlist says otherwise.)",
                Color = new Color(1f, 0.35f, 0.45f, 1f),
                SafeAction = new EncounterActionDef
                {
                    Label = "Block and Run",
                    Risk = EncounterRisk.Safe,
                    SuccessReward = EncounterRewardType.EscapeOnly,
                    FailExtra = EncounterFailExtra.None,
                    SuccessFlavor = "Blocked. Muted. Spiritually unsubscribed.",
                    FailFlavor = "You hesitated. Feelings tax: 1 heart.",
                    RewardHint = "Escape safely"
                },
                RiskyAction = new EncounterActionDef
                {
                    Label = "Hear Them Out",
                    Risk = EncounterRisk.Risky,
                    SuccessReward = EncounterRewardType.BonusGem,
                    FailExtra = EncounterFailExtra.CheckpointReturn,
                    SuccessFlavor = "Closure AND a shiny green flag. Growth looks good on you.",
                    FailFlavor = "Emotional ambush! Heart lost — back to safer ground.",
                    RewardHint = "+1 green flag"
                }
            };
        }

        public static EncounterDefinition CreateApp()
        {
            return new EncounterDefinition
            {
                MazeId = 'B',
                DisplayName = "The Dating App",
                Dialogue = "There are definitely more options if you keep swiping.",
                Color = new Color(0.4f, 0.75f, 1f, 1f),
                SafeAction = new EncounterActionDef
                {
                    Label = "Delete the App",
                    Risk = EncounterRisk.Safe,
                    SuccessReward = EncounterRewardType.EscapeOnly,
                    FailExtra = EncounterFailExtra.None,
                    SuccessFlavor = "Deleted. Your thumb is free. For now.",
                    FailFlavor = "One more notification got you. -1 heart.",
                    RewardHint = "Escape safely"
                },
                RiskyAction = new EncounterActionDef
                {
                    Label = "One More Swipe",
                    Risk = EncounterRisk.Risky,
                    SuccessReward = EncounterRewardType.BonusGem,
                    FailExtra = EncounterFailExtra.LoseTime,
                    FailTimePenalty = 5f,
                    SuccessFlavor = "Miraculously, a green flag instead of a red flag.",
                    FailFlavor = "Doom-swiped. -1 heart and 5 seconds of your life.",
                    RewardHint = "+1 green flag"
                }
            };
        }

        public static EncounterDefinition CreateMixed()
        {
            return new EncounterDefinition
            {
                MazeId = 'C',
                DisplayName = "Mixed Signals",
                Dialogue = "I'm not looking for anything serious… but where are you?",
                Color = new Color(1f, 0.85f, 0.25f, 1f),
                SafeAction = new EncounterActionDef
                {
                    Label = "Ask for Clarity",
                    Risk = EncounterRisk.Safe,
                    SuccessReward = EncounterRewardType.EscapeOnly,
                    FailExtra = EncounterFailExtra.None,
                    SuccessFlavor = "You asked. They vanished. Win?",
                    FailFlavor = "They replied 'haha yeah'. -1 heart.",
                    RewardHint = "Escape safely"
                },
                RiskyAction = new EncounterActionDef
                {
                    Label = "Match Their Energy",
                    Risk = EncounterRisk.Risky,
                    SuccessReward = EncounterRewardType.BonusTime,
                    SuccessTimeBonus = 5f,
                    FailExtra = EncounterFailExtra.ReturnLastSafe,
                    SuccessFlavor = "Chaotic synchronization. +5 seconds of borrowed confidence.",
                    FailFlavor = "You matched the vibe… into a dead end. Back you go.",
                    RewardHint = "+5 seconds"
                }
            };
        }
    }
}
