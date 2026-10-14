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
        public bool CountsTowardExit = true;

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

        public static EncounterDefinition CreateFamilyFriend()
        {
            return new EncounterDefinition
            {
                MazeId = 'D',
                DisplayName = "The Family Friend",
                Dialogue = "Your mom gave me your number... I already added you on LinkedIn.",
                Color = new Color(1f, 0.6f, 0.3f, 1f),
                CountsTowardExit = false,
                SafeAction = new EncounterActionDef
                {
                    Label = "Politely Decline",
                    Risk = EncounterRisk.Safe,
                    SuccessReward = EncounterRewardType.EscapeOnly,
                    FailExtra = EncounterFailExtra.None,
                    SuccessFlavor = "You dodged it. Mom's disappointed but you're free.",
                    FailFlavor = "Now it's a whole family group chat. -1 heart.",
                    RewardHint = "Escape safely"
                },
                RiskyAction = new EncounterActionDef
                {
                    Label = "Give Him a Chance",
                    Risk = EncounterRisk.Risky,
                    SuccessReward = EncounterRewardType.BonusGem,
                    FailExtra = EncounterFailExtra.LoseTime,
                    FailTimePenalty = 5f,
                    SuccessFlavor = "Plot twist: he's actually lovely. +1 green flag, mom's thrilled.",
                    FailFlavor = "Three hours of small talk about his startup. -1 heart, -5 seconds.",
                    RewardHint = "+1 green flag"
                }
            };
        }

        public static EncounterDefinition CreateFriendsEx()
        {
            return new EncounterDefinition
            {
                MazeId = 'E',
                DisplayName = "Your Friend's Ex",
                Dialogue = "I know I'm your bestie's ex... but we'd be kind of perfect, no?",
                Color = new Color(0.7f, 0.4f, 0.95f, 1f),
                CountsTowardExit = false,
                SafeAction = new EncounterActionDef
                {
                    Label = "Respect the Code",
                    Risk = EncounterRisk.Safe,
                    SuccessReward = EncounterRewardType.EscapeOnly,
                    FailExtra = EncounterFailExtra.None,
                    SuccessFlavor = "Loyalty upheld. Your friend never has to know. Clean exit.",
                    FailFlavor = "You overthought it and sent a weird text. -1 heart.",
                    RewardHint = "Escape safely"
                },
                RiskyAction = new EncounterActionDef
                {
                    Label = "Shoot Your Shot",
                    Risk = EncounterRisk.Risky,
                    SuccessReward = EncounterRewardType.BonusTime,
                    SuccessTimeBonus = 5f,
                    FailExtra = EncounterFailExtra.ReturnLastSafe,
                    SuccessFlavor = "Forbidden spark, undeniable chemistry. +5 seconds of chaos.",
                    FailFlavor = "The group chat found out. Friendship imploded. Back you go.",
                    RewardHint = "+5 seconds"
                }
            };
        }

    }
}
