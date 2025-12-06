using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestSystem
{
    [Serializable]
    public class QuestInstance
    {
        public QuestData questData;
        public string questId;
        public QuestState state;
        public DateTime startTime;
        public DateTime? completionTime;
        public List<ObjectiveInstance> objectives;
        public Dictionary<string, object> metadata;

        public event Action<QuestState> OnStateChanged;

        public QuestInstance(QuestData data)
        {
            questData = data;
            questId = data.questId;
            state = QuestState.Available;
            startTime = DateTime.Now;
            objectives = new List<ObjectiveInstance>();
            metadata = new Dictionary<string, object>();

            InitializeObjectives();
        }

        private void InitializeObjectives()
        {
            var allObjectives = questData.GetAllObjectives();
            for (int i = 0; i < allObjectives.Count; i++)
            {
                objectives.Add(new ObjectiveInstance(allObjectives[i], questId, i));
            }
        }

        public void Start()
        {
            if (state != QuestState.Available) return;
            TransitionTo(QuestState.Active);
            startTime = DateTime.Now;
        }

        public void TransitionTo(QuestState newState)
        {
            if (state == newState) return;
            state = newState;
            OnStateChanged?.Invoke(newState);

            if (newState == QuestState.Completed)
            {
                completionTime = DateTime.Now;
            }
        }

        public bool CanTransitionTo(QuestState targetState)
        {
            switch (state)
            {
                case QuestState.Locked:
                    return targetState == QuestState.Available;
                case QuestState.Available:
                    return targetState == QuestState.Active;
                case QuestState.Active:
                    return targetState == QuestState.Completed || targetState == QuestState.Failed;
                case QuestState.Completed:
                    return targetState == QuestState.Archived;
                case QuestState.Failed:
                    return targetState == QuestState.Active || targetState == QuestState.Archived;
                default:
                    return false;
            }
        }

        public ObjectiveInstance GetObjective(int index)
        {
            if (index < 0 || index >= objectives.Count) return null;
            return objectives[index];
        }

        public void UpdateObjective(int index, int progress)
        {
            var objective = GetObjective(index);
            if (objective == null) return;

            objective.UpdateProgress(progress);

            if (AreAllObjectivesComplete())
            {
                TransitionTo(QuestState.Completed);
            }
        }

        public void SetObjectiveProgress(int index, int value)
        {
            var objective = GetObjective(index);
            if (objective == null) return;

            objective.SetProgress(value);

            if (AreAllObjectivesComplete())
            {
                TransitionTo(QuestState.Completed);
            }
        }

        public bool IsObjectiveComplete(int index)
        {
            var objective = GetObjective(index);
            return objective != null && objective.isComplete;
        }

        public int GetCompletedObjectiveCount()
        {
            return objectives.Count(o => o.isComplete);
        }

        public bool AreAllObjectivesComplete()
        {
            if (objectives.Count == 0) return false;
            return objectives.All(o => o.isComplete || o.objectiveData.optional);
        }

        public float GetOverallProgress()
        {
            if (objectives.Count == 0) return 0f;
            int requiredObjectives = objectives.Count(o => !o.objectiveData.optional);
            if (requiredObjectives == 0) return 1f;

            int completed = objectives.Count(o => o.isComplete && !o.objectiveData.optional);
            return (float)completed / requiredObjectives;
        }

        public string GetProgressText()
        {
            int completed = GetCompletedObjectiveCount();
            int total = objectives.Count;
            return $"{completed}/{total} objectives";
        }

        public TimeSpan? GetTimeRemaining()
        {
            if (questData.timeLimit <= 0) return null;

            var elapsed = DateTime.Now - startTime;
            var limit = TimeSpan.FromHours(questData.timeLimit);
            var remaining = limit - elapsed;

            return remaining.TotalSeconds > 0 ? remaining : TimeSpan.Zero;
        }

        public bool IsExpired()
        {
            var remaining = GetTimeRemaining();
            return remaining.HasValue && remaining.Value.TotalSeconds <= 0;
        }
    }
}
