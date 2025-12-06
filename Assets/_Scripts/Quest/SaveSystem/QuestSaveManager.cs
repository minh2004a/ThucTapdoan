using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace QuestSystem.SaveSystem
{
    public class QuestSaveManager : MonoBehaviour
    {
        public static QuestSaveManager Instance { get; private set; }

        private const string SaveFileName = "quest_save.json";
        private string SaveFilePath => Path.Combine(Application.persistentDataPath, SaveFileName);

        private QuestManager questManager;
        private DailyQuestManager dailyQuestManager;
        private NPCReputationManager reputationManager;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            questManager = QuestManager.Instance;
            dailyQuestManager = DailyQuestManager.Instance;
            reputationManager = NPCReputationManager.Instance;
        }

        public void SaveQuests()
        {
            var saveData = CreateSaveData();
            string json = JsonUtility.ToJson(saveData, true);

            try
            {
                File.WriteAllText(SaveFilePath, json);
                Debug.Log($"Quests saved to {SaveFilePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save quests: {e.Message}");
            }
        }

        public void LoadQuests()
        {
            if (!File.Exists(SaveFilePath))
            {
                Debug.Log("No save file found");
                return;
            }

            try
            {
                string json = File.ReadAllText(SaveFilePath);
                var saveData = JsonUtility.FromJson<QuestSaveData>(json);

                ApplySaveData(saveData);
                Debug.Log("Quests loaded successfully");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load quests: {e.Message}");
            }
        }

        private QuestSaveData CreateSaveData()
        {
            var saveData = new QuestSaveData();

            if (questManager != null)
            {
                var activeQuests = questManager.GetAllActiveQuests();
                foreach (var quest in activeQuests)
                {
                    saveData.activeQuests.Add(new ActiveQuestSave
                    {
                        questId = quest.questId,
                        state = quest.state.ToString(),
                        startTimeTicks = quest.startTime.Ticks,
                        objectives = quest.objectives.Select(obj => new ObjectiveSave
                        {
                            objectiveIndex = obj.objectiveIndex,
                            currentProgress = obj.currentProgress,
                            isComplete = obj.isComplete,
                            completionTimeTicks = obj.completionTime?.Ticks
                        }).ToList()
                    });
                }
            }

            if (dailyQuestManager != null)
            {
                saveData.dailyQuestState = new DailyQuestSaveData
                {
                    lastResetTimeTicks = System.DateTime.Now.Ticks,
                    streakDays = dailyQuestManager.ConsecutiveDays,
                    currentSlots = dailyQuestManager.GetAllSlots().Select(slot => new DailySlotSave
                    {
                        slotIndex = slot.slotIndex,
                        questId = slot.quest?.questId ?? "",
                        state = slot.state.ToString(),
                        rerollsUsed = slot.rerollsUsed
                    }).ToList()
                };
            }

            if (reputationManager != null)
            {
            }

            return saveData;
        }

        private void ApplySaveData(QuestSaveData saveData)
        {
            if (saveData == null || questManager == null) return;

            foreach (var activeSave in saveData.activeQuests)
            {
                if (questManager.StartQuest(activeSave.questId))
                {
                    var quest = questManager.GetActiveQuest(activeSave.questId);
                    if (quest != null)
                    {
                        for (int i = 0; i < activeSave.objectives.Count && i < quest.objectives.Count; i++)
                        {
                            var objSave = activeSave.objectives[i];
                            quest.objectives[i].currentProgress = objSave.currentProgress;
                            quest.objectives[i].isComplete = objSave.isComplete;

                            if (objSave.completionTimeTicks.HasValue)
                            {
                                quest.objectives[i].completionTime = new System.DateTime(objSave.completionTimeTicks.Value);
                            }
                        }
                    }
                }
            }

            if (saveData.npcReputation != null && reputationManager != null)
            {
                foreach (var kvp in saveData.npcReputation)
                {
                    reputationManager.AddReputation(kvp.Key, kvp.Value);
                }
            }
        }

        public void DeleteSave()
        {
            if (File.Exists(SaveFilePath))
            {
                File.Delete(SaveFilePath);
                Debug.Log("Save file deleted");
            }
        }

        public bool SaveExists()
        {
            return File.Exists(SaveFilePath);
        }

        private void OnApplicationQuit()
        {
            SaveQuests();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SaveQuests();
            }
        }
    }
}
