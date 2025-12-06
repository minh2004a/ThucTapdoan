using UnityEngine;
using System.Collections.Generic;
using System;

namespace QuestSystem
{
    public class NPCReputationManager : MonoBehaviour
    {
        public static NPCReputationManager Instance { get; private set; }

        private Dictionary<string, int> reputationPoints;
        private Dictionary<string, ReputationLevel> reputationLevels;

        public event Action<string, int, ReputationLevel> OnReputationChanged;
        public event Action<string, ReputationLevel> OnReputationLevelUp;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            reputationPoints = new Dictionary<string, int>();
            reputationLevels = new Dictionary<string, ReputationLevel>();
        }

        public void AddReputation(string npcId, int amount)
        {
            if (string.IsNullOrEmpty(npcId) || amount == 0) return;

            if (!reputationPoints.ContainsKey(npcId))
            {
                reputationPoints[npcId] = 0;
                reputationLevels[npcId] = ReputationLevel.Stranger;
            }

            ReputationLevel oldLevel = reputationLevels[npcId];
            reputationPoints[npcId] += amount;
            ReputationLevel newLevel = CalculateLevel(reputationPoints[npcId]);

            reputationLevels[npcId] = newLevel;

            OnReputationChanged?.Invoke(npcId, reputationPoints[npcId], newLevel);

            if (newLevel != oldLevel)
            {
                OnReputationLevelUp?.Invoke(npcId, newLevel);
                Debug.Log($"Reputation with {npcId} increased to {newLevel}!");
            }
        }

        public int GetReputation(string npcId)
        {
            if (reputationPoints.TryGetValue(npcId, out int points))
            {
                return points;
            }
            return 0;
        }

        public ReputationLevel GetReputationLevel(string npcId)
        {
            if (reputationLevels.TryGetValue(npcId, out ReputationLevel level))
            {
                return level;
            }
            return ReputationLevel.Stranger;
        }

        private ReputationLevel CalculateLevel(int points)
        {
            if (points >= (int)ReputationLevel.Soulmate) return ReputationLevel.Soulmate;
            if (points >= (int)ReputationLevel.BestFriend) return ReputationLevel.BestFriend;
            if (points >= (int)ReputationLevel.Friend) return ReputationLevel.Friend;
            if (points >= (int)ReputationLevel.Acquaintance) return ReputationLevel.Acquaintance;
            return ReputationLevel.Stranger;
        }

        public float GetShopDiscount(string npcId)
        {
            var level = GetReputationLevel(npcId);
            return level switch
            {
                ReputationLevel.Stranger => 0f,
                ReputationLevel.Acquaintance => 0.05f,
                ReputationLevel.Friend => 0.10f,
                ReputationLevel.BestFriend => 0.15f,
                ReputationLevel.Soulmate => 0.20f,
                _ => 0f
            };
        }

        public int GetPointsToNextLevel(string npcId)
        {
            int current = GetReputation(npcId);
            var currentLevel = GetReputationLevel(npcId);

            int nextThreshold = currentLevel switch
            {
                ReputationLevel.Stranger => (int)ReputationLevel.Acquaintance,
                ReputationLevel.Acquaintance => (int)ReputationLevel.Friend,
                ReputationLevel.Friend => (int)ReputationLevel.BestFriend,
                ReputationLevel.BestFriend => (int)ReputationLevel.Soulmate,
                _ => 0
            };

            if (nextThreshold == 0) return 0;
            return nextThreshold - current;
        }
    }
}
