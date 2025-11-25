using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Refs")]
    [SerializeField] PlayerInventory playerInventory;
    [SerializeField] PlayerWallet playerWallet;

    // key = questData.id
    readonly Dictionary<string, QuestInstance> activeQuests = new();
    readonly Dictionary<string, SaveStore.QuestStateDTO> loadedStates = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!playerInventory)
            playerInventory = FindObjectOfType<PlayerInventory>(true);
        if (!playerWallet)
            playerWallet = FindObjectOfType<PlayerWallet>(true);
        // THÊM: load quest từ SaveStore
        loadedStates.Clear();
        foreach (var dto in SaveStore.GetQuestStates())
        {
            if (dto != null && !string.IsNullOrEmpty(dto.id))
            {
                loadedStates[dto.id] = dto;
            }
        }
    }
    QuestInstance GetOrCreateInstance(QuestData data)
    {
        if (!data)
        {
            Debug.LogWarning("QuestManager: QuestData null.");
            return null;
        }

        if (string.IsNullOrEmpty(data.id))
        {
            Debug.LogWarning($"QuestManager: QuestData '{data.name}' chưa có id.");
            return null;
        }

        if (activeQuests.TryGetValue(data.id, out var existing))
            return existing;

        var inst = new QuestInstance
        {
            data = data,
            state = QuestState.NotAccepted,
            currentAmount = 0
        };

        // NẾU CÓ SAVE CŨ THÌ GÁN VÀO
        if (loadedStates.TryGetValue(data.id, out var dto))
        {
            inst.state = dto.state;
            inst.currentAmount = dto.currentAmount;
        }

        activeQuests[data.id] = inst;
        return inst;
    }
    public QuestState GetState(QuestData data)
    {
        var inst = GetOrCreateInstance(data);
        return inst != null ? inst.state : QuestState.NotAccepted;
    }

    public QuestInstance AcceptQuest(QuestData data)
    {
        var inst = GetOrCreateInstance(data);
        if (inst == null) return null;

        if (inst.state == QuestState.NotAccepted)
        {
            inst.state = QuestState.InProgress;
            inst.currentAmount = 0;
            Debug.Log($"QuestManager: Bắt đầu quest {data.id}");
            SaveQuests(); // Lưu ngay sau khi nhận quest
        }

        return inst;
    }

    void EnsureInventory()
    {
        if (!playerInventory)
            playerInventory = FindObjectOfType<PlayerInventory>(true);
    }
    void EnsureWallet()
    {
        if (!playerWallet)
            playerWallet = FindObjectOfType<PlayerWallet>(true);
    }
    void UpdateProgressFromInventory(QuestInstance inst)
    {
        if (inst == null || inst.data == null) return;
        EnsureInventory();
        if (!playerInventory) return;

        var q = inst.data;
        if (!q.requiredItem || q.requiredAmount <= 0) return;

        int have = playerInventory.CountItem(q.requiredItem);
        inst.currentAmount = Mathf.Clamp(have, 0, q.requiredAmount);

        if (have >= q.requiredAmount && inst.state == QuestState.InProgress)
        {
            inst.state = QuestState.ReadyToTurnIn;
        }
        else if (have < q.requiredAmount && inst.state == QuestState.ReadyToTurnIn)
        {
            // nếu mất bớt đồ đi thì quay lại trạng thái đang làm
            inst.state = QuestState.InProgress;
        }
    }

    public bool CanTurnIn(QuestData data)
    {
        var inst = GetOrCreateInstance(data);
        if (inst == null) return false;

        UpdateProgressFromInventory(inst);
        return inst.state == QuestState.ReadyToTurnIn;
    }

    public bool TryTurnIn(QuestData data)
    {
        var inst = GetOrCreateInstance(data);
        if (inst == null) return false;

        EnsureInventory();
        if (!playerInventory)
        {
            Debug.LogWarning("QuestManager: không tìm thấy PlayerInventory.");
            return false;
        }

        var q = data;
        if (!q.requiredItem || q.requiredAmount <= 0) return false;

        // check lần nữa cho chắc
        if (!playerInventory.HasItem(q.requiredItem, q.requiredAmount))
            return false;

        // 1. Trừ đồ yêu cầu
        bool removed = playerInventory.RemoveItem(q.requiredItem, q.requiredAmount);
        if (!removed)
        {
            Debug.LogWarning("QuestManager: RemoveItem fail dù HasItem true??");
            return false;
        }

        // 2. Thưởng
        if (q.rewardItem && q.rewardAmount > 0)
        {
            int remaining = playerInventory.AddItem(q.rewardItem, q.rewardAmount);
            if (remaining > 0)
            {
                Debug.LogWarning($"QuestManager: túi đầy, còn {remaining} reward chưa add được.");
                // sau này có thể drop xuống đất
            }
            // 3. Thưởng tiền
        EnsureWallet();
        if (playerWallet != null && q.moneyReward > 0)
        {
            playerWallet.AddMoney(q.moneyReward);
            Debug.Log($"QuestManager: thưởng thêm {q.moneyReward} tiền cho quest {q.id}.");
        }
        }

        // 3. Đánh dấu hoàn thành
        inst.state = QuestState.Completed;
        inst.currentAmount = q.requiredAmount;

        Debug.Log($"QuestManager: Quest {q.id} đã hoàn thành và phát thưởng.");
        SaveQuests(); // Lưu ngay sau khi hoàn thành quest
        return true;
    }

    public bool IsCompleted(QuestData data)
    {
        var inst = GetOrCreateInstance(data);
        return inst != null && inst.state == QuestState.Completed;
    }
    void SaveQuests()
    {
        var list = new List<SaveStore.QuestStateDTO>();

        foreach (var kv in activeQuests)
        {
            var inst = kv.Value;
            if (inst == null || inst.data == null) continue;

            list.Add(new SaveStore.QuestStateDTO
            {
                id = inst.data.id,
                state = inst.state,
                currentAmount = inst.currentAmount
            });
        }

        SaveStore.SetQuestStates(list);
    }
}
