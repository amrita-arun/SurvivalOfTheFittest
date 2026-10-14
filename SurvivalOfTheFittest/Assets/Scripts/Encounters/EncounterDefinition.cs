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
                Dialogue = "Hey... run it back?",
                Color = new Color(1f, 0.35f, 0.45f, 1f),
                SafeAction = new EncounterActionDef
                {
                    Label = "Block and Run",
                    Risk = EncounterRisk.Safe,
                    SuccessReward = EncounterRewardType.EscapeOnly,
                    FailExtra = EncounterFailExtra.None,
                    SuccessFlavor = "Blocked! Nice job moving on.",
                    FailFlavor = "You hesitated. Feelings tax: 1 heart.",
                    RewardHint = "Escape safely"
                },
                RiskyAction = new EncounterActionDef
                {
                    Label = "Hear Them Out",
                    Risk = EncounterRisk.Risky,
                    SuccessReward = EncounterRewardType.BonusGem,
                    FailExtra = EncounterFailExtra.CheckpointReturn,
                    SuccessFlavor = "Closure AND a green flag. Would have never guessed...",
                    FailFlavor = "You got gaslit and lowkey believed them... Heart lost!",
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
                    SuccessFlavor = "Deleted, you beat the paradox of choice!",
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
                Dialogue = "I'm not looking for anything serious… but wya",
                Color = new Color(1f, 0.85f, 0.25f, 1f),
                SafeAction = new EncounterActionDef
                {
                    Label = "Ask for Clarity",
                    Risk = EncounterRisk.Safe,
                    SuccessReward = EncounterRewardType.EscapeOnly,
                    FailExtra = EncounterFailExtra.None,
                    SuccessFlavor = "Confrontation scared them away. A win is a win..?",
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
                    SuccessFlavor = "Okay nonchalant!! +5 seconds of borrowed confidence.",
                    FailFlavor = "You matched their vibe… into a dead end.",
                    RewardHint = "+5 seconds"
                }
            };
        }
    }
}
