# QUEST SYSTEM - C# IMPLEMENTATION

## ĐÃ TẠO CÁC SCRIPT

### Core Scripts
- `QuestEnums.cs` - Tất cả enums cho hệ thống
- `QuestValidator.cs` - Validation logic
- `QuestManager.cs` - Manager chính (Singleton)
- `ObjectiveTracker.cs` - Tracking objectives qua events
- `RewardProcessor.cs` - Xử lý rewards

### Data Scripts
- `QuestData.cs` - ScriptableObject cho quest data
- `QuestObjectiveData.cs` - 9 loại objectives (Collect, Kill, TalkTo, Buy, Sell, Plant, Harvest, Deliver, EarnMoney, Mine)
- `RewardData.cs` - Reward structures và interfaces

### Runtime Scripts
- `QuestInstance.cs` - Quest state runtime
- `ObjectiveInstance.cs` - Objective progress runtime

### Event System
- `QuestEvents.cs` - Tất cả event types
- `EventDispatcher.cs` - Event bus (Singleton)
- `QuestEventIntegration.cs` - Integration helper

### NPC System
- `NPCQuestGiver.cs` - Component cho NPC
- `NPCReputationManager.cs` - Reputation system

### Daily Quest System
- `DailyQuestManager.cs` - Daily quest manager
- `DailyQuestSlot.cs` - Slot data structure

### Save System
- `QuestSaveData.cs` - Save data structures
- `QuestSaveManager.cs` - Save/Load manager

### UI Scripts
- `QuestUIController.cs` - Main quest UI
- `QuestTrackerUI.cs` - HUD tracker
- `DailyQuestUI.cs` - Daily quest UI

## CÁCH SỬ DỤNG

### 1. Setup Initial

```csharp
// Tạo GameObject trong scene
GameObject questSystem = new GameObject("QuestSystem");
questSystem.AddComponent<QuestManager>();
questSystem.AddComponent<DailyQuestManager>();
questSystem.AddComponent<NPCReputationManager>();
questSystem.AddComponent<QuestSaveManager>();
questSystem.AddComponent<EventDispatcher>();
questSystem.AddComponent<QuestEventIntegration>();
```

### 2. Tạo Quest (ScriptableObject)

Unity Editor → Right Click → Create → Quest System → Quest Data

### 3. Setup NPC

```csharp
// Add component vào NPC GameObject
NPCQuestGiver questGiver = npcObject.AddComponent<NPCQuestGiver>();
// Assign quests trong Inspector
```

### 4. Dispatch Events từ Game Systems

```csharp
// Trong PlayerInventory khi add item
void AddItem(ItemSO item, int amount) {
    // ... inventory logic

    EventDispatcher.Instance.Dispatch(new ItemCollectedEvent {
        item = item,
        amount = amount,
        newCount = GetItemCount(item)
    });
}

// Trong Combat system khi kill monster
void OnMonsterDeath(Monster monster) {
    EventDispatcher.Instance.Dispatch(new MonsterKilledEvent {
        monsterId = monster.id,
        monsterType = monster.type,
        weaponUsed = currentWeapon.weaponType,
        location = monster.transform.position
    });
}

// Trong Shop system khi buy
void BuyItem(ItemSO item, int count, string vendorId) {
    // ... purchase logic

    EventDispatcher.Instance.Dispatch(new ItemBoughtEvent {
        item = item,
        amount = count,
        price = totalPrice,
        vendorId = vendorId
    });
}
```

### 5. Integration với Existing Systems

```csharp
// EconomyManager integration đã có sẵn
// PlayerInventory integration đã có sẵn
// Chỉ cần dispatch events từ các systems hiện có
```

## CẤU TRÚC THƯ MỤC

```
Assets/_Scripts/Quest/
├── Core/
│   ├── QuestEnums.cs
│   ├── QuestManager.cs
│   ├── QuestValidator.cs
│   ├── ObjectiveTracker.cs
│   └── RewardProcessor.cs
├── Data/
│   ├── QuestData.cs
│   ├── QuestObjectiveData.cs
│   └── RewardData.cs
├── Runtime/
│   ├── QuestInstance.cs
│   └── ObjectiveInstance.cs
├── Events/
│   ├── QuestEvents.cs
│   └── EventDispatcher.cs
├── NPC/
│   ├── NPCQuestGiver.cs
│   └── NPCReputationManager.cs
├── Daily/
│   └── DailyQuestManager.cs
├── SaveSystem/
│   ├── QuestSaveData.cs
│   └── QuestSaveManager.cs
├── UI/
│   ├── QuestUIController.cs
│   ├── QuestTrackerUI.cs
│   └── DailyQuestUI.cs
└── Integration/
    └── QuestEventIntegration.cs
```

## API EXAMPLES

```csharp
// Start quest
QuestManager.Instance.StartQuest("MARTIN_STORY_01");

// Get available quests for NPC
var quests = QuestManager.Instance.GetAvailableQuestsForNPC("MARTIN");

// Check quest status
bool isActive = QuestManager.Instance.IsQuestActive("QUEST_ID");
bool isComplete = QuestManager.Instance.IsQuestCompleted("QUEST_ID");

// Get reputation
int rep = NPCReputationManager.Instance.GetReputation("MARTIN");
ReputationLevel level = NPCReputationManager.Instance.GetReputationLevel("MARTIN");
float discount = NPCReputationManager.Instance.GetShopDiscount("MARTIN");

// Daily quests
DailyQuestManager.Instance.StartDailyQuest(0); // Start slot 0
DailyQuestManager.Instance.RerollSlot(1); // Reroll slot 1

// Save/Load
QuestSaveManager.Instance.SaveQuests();
QuestSaveManager.Instance.LoadQuests();
```

## FEATURES ĐÃ IMPLEMENT

✅ Quest lifecycle management
✅ 9 objective types
✅ Event-driven tracking
✅ Reward system với handlers
✅ NPC quest giver
✅ Reputation system với shop discount
✅ Daily quest system với reroll
✅ Streak bonus
✅ Save/Load system
✅ Quest UI
✅ Quest tracker UI
✅ Daily quest UI
✅ Validation system
✅ Prerequisites
✅ Cooldown
✅ Time limits

## NEXT STEPS

1. Tạo Quest Data assets trong Unity
2. Setup UI prefabs
3. Hook events vào existing systems
4. Test và balance
5. Tạo sample quests
