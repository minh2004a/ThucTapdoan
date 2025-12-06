using System;

namespace QuestSystem
{
    [Serializable]
    public class ObjectiveInstance
    {
        public QuestObjectiveData objectiveData;
        public string questId;
        public int objectiveIndex;
        public int currentProgress;
        public bool isComplete;
        public DateTime? completionTime;

        public ObjectiveInstance(QuestObjectiveData data, string questId, int index)
        {
            this.objectiveData = data;
            this.questId = questId;
            this.objectiveIndex = index;
            this.currentProgress = 0;
            this.isComplete = false;
            this.completionTime = null;
        }

        public void UpdateProgress(int amount)
        {
            currentProgress += amount;
            CheckCompletion();
        }

        public void SetProgress(int value)
        {
            currentProgress = value;
            CheckCompletion();
        }

        public bool CheckCompletion()
        {
            if (isComplete) return true;

            if (objectiveData.ValidateCompletion(currentProgress))
            {
                isComplete = true;
                completionTime = DateTime.Now;
                return true;
            }
            return false;
        }

        public float GetProgressPercent()
        {
            if (objectiveData.targetAmount <= 0) return 0f;
            return (float)currentProgress / objectiveData.targetAmount;
        }

        public string GetProgressText()
        {
            return objectiveData.GetProgressText(currentProgress);
        }

        public void Reset()
        {
            currentProgress = 0;
            isComplete = false;
            completionTime = null;
        }
    }
}
