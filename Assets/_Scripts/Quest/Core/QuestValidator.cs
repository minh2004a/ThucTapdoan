using UnityEngine;

namespace QuestSystem
{
    public class ValidationResult
    {
        public bool isValid;
        public ValidationType validationType;
        public string failureReason;

        public static ValidationResult Valid()
        {
            return new ValidationResult { isValid = true, validationType = ValidationType.Valid };
        }

        public static ValidationResult Invalid(ValidationType type, string reason)
        {
            return new ValidationResult { isValid = false, validationType = type, failureReason = reason };
        }
    }

    public class QuestValidator
    {
        private QuestManager questManager;
        private PlayerInventory inventory;

        public QuestValidator(QuestManager manager)
        {
            questManager = manager;
            inventory = Object.FindObjectOfType<PlayerInventory>();
        }

        public ValidationResult ValidateAll(QuestData quest)
        {
            var levelCheck = CheckPlayerLevel(quest.minPlayerLevel);
            if (!levelCheck.isValid) return levelCheck;

            var prereqCheck = CheckPrerequisites(quest.prerequisites);
            if (!prereqCheck.isValid) return prereqCheck;

            var reputationCheck = CheckReputation(quest.requiredNPCId, quest.requiredReputation);
            if (!reputationCheck.isValid) return reputationCheck;

            var inventoryCheck = CheckInventorySpace(quest.rewards);
            if (!inventoryCheck.isValid) return inventoryCheck;

            var activeCheck = CheckMaxActiveQuests();
            if (!activeCheck.isValid) return activeCheck;

            var cooldownCheck = CheckCooldown(quest);
            if (!cooldownCheck.isValid) return cooldownCheck;

            var completedCheck = CheckAlreadyCompleted(quest);
            if (!completedCheck.isValid) return completedCheck;

            return ValidationResult.Valid();
        }

        public ValidationResult CheckPlayerLevel(int requiredLevel)
        {
            if (requiredLevel <= 0) return ValidationResult.Valid();

            return ValidationResult.Valid();
        }

        public ValidationResult CheckPrerequisites(System.Collections.Generic.List<QuestData> prerequisites)
        {
            if (prerequisites == null || prerequisites.Count == 0)
                return ValidationResult.Valid();

            foreach (var prereq in prerequisites)
            {
                if (prereq == null) continue;

                if (!questManager.IsQuestCompleted(prereq.questId))
                {
                    return ValidationResult.Invalid(
                        ValidationType.PrerequisiteNotMet,
                        $"Yêu cầu hoàn thành: {prereq.questName}"
                    );
                }
            }

            return ValidationResult.Valid();
        }

        public ValidationResult CheckReputation(string npcId, int requiredReputation)
        {
            if (string.IsNullOrEmpty(npcId) || requiredReputation <= 0)
                return ValidationResult.Valid();

            int currentRep = questManager.GetReputation(npcId);
            if (currentRep < requiredReputation)
            {
                return ValidationResult.Invalid(
                    ValidationType.InsufficientReputation,
                    $"Cần {requiredReputation} uy tín với NPC"
                );
            }

            return ValidationResult.Valid();
        }

        public ValidationResult CheckInventorySpace(QuestReward rewards)
        {
            if (rewards == null || rewards.itemRewards == null || rewards.itemRewards.Count == 0)
                return ValidationResult.Valid();

            if (inventory == null)
                return ValidationResult.Valid();

            int slotsNeeded = rewards.itemRewards.Count;

            return ValidationResult.Valid();
        }

        public ValidationResult CheckMaxActiveQuests()
        {
            int maxActive = 10;
            int currentActive = questManager.GetActiveQuestCount();

            if (currentActive >= maxActive)
            {
                return ValidationResult.Invalid(
                    ValidationType.MaxActiveQuestsReached,
                    $"Đã đạt giới hạn {maxActive} nhiệm vụ cùng lúc"
                );
            }

            return ValidationResult.Valid();
        }

        public ValidationResult CheckCooldown(QuestData quest)
        {
            if (!quest.isRepeatable || quest.cooldownHours <= 0)
                return ValidationResult.Valid();

            var lastCompletion = questManager.GetLastCompletionTime(quest.questId);
            if (lastCompletion.HasValue)
            {
                var cooldownEnd = lastCompletion.Value.AddHours(quest.cooldownHours);
                if (System.DateTime.Now < cooldownEnd)
                {
                    var remaining = cooldownEnd - System.DateTime.Now;
                    return ValidationResult.Invalid(
                        ValidationType.OnCooldown,
                        $"Cooldown: {remaining.Hours}h {remaining.Minutes}m"
                    );
                }
            }

            return ValidationResult.Valid();
        }

        public ValidationResult CheckAlreadyCompleted(QuestData quest)
        {
            if (quest.isRepeatable)
                return ValidationResult.Valid();

            if (questManager.IsQuestCompleted(quest.questId))
            {
                return ValidationResult.Invalid(
                    ValidationType.AlreadyCompleted,
                    "Đã hoàn thành nhiệm vụ này"
                );
            }

            return ValidationResult.Valid();
        }

        public bool CanStartQuest(QuestData quest)
        {
            return ValidateAll(quest).isValid;
        }

        public string GetFailureReasons(QuestData quest)
        {
            var result = ValidateAll(quest);
            return result.isValid ? "" : result.failureReason;
        }
    }
}
