# KIẾN TRÚC HỆ THỐNG NHIỆM VỤ - UNITY C#
## Quest System Architecture Document

---

## MỤC LỤC

1. [Tổng Quan Hệ Thống](#1-tổng-quan-hệ-thống)
2. [Kiến Trúc Tổng Thể](#2-kiến-trúc-tổng-thể)
3. [Core Components](#3-core-components)
4. [Data Layer](#4-data-layer)
5. [Quest Types Architecture](#5-quest-types-architecture)
6. [NPC Quest System](#6-npc-quest-system)
7. [Daily Quest System](#7-daily-quest-system)
8. [Objective System](#8-objective-system)
9. [Reward System](#9-reward-system)
10. [Event & Messaging](#10-event--messaging)
11. [Save/Load System](#11-saveload-system)
12. [Integration Points](#12-integration-points)
13. [Scalability & Extension](#13-scalability--extension)
14. [Performance Considerations](#14-performance-considerations)

---

## 1. TỔNG QUAN HỆ THỐNG

### 1.1. Mục Tiêu Thiết Kế

**Nguyên Tắc Cốt Lõi:**
- **Single Responsibility**: Mỗi class chỉ đảm nhiệm 1 nhiệm vụ rõ ràng
- **Open/Closed**: Mở để mở rộng, đóng để sửa đổi
- **Dependency Inversion**: Phụ thuộc vào abstraction, không phải concrete
- **Separation of Concerns**: Tách biệt data, logic, presentation

**Yêu Cầu Chức Năng:**
1. Quản lý nhiệm vụ theo cốt truyện (Story Quest Chain)
2. Nhiệm vụ thu nhập (Resource Collection, Monster Hunting)
3. Nhiệm vụ NPC (Vendor quests: seeds, equipment, weapons)
4. Hệ thống Daily Quest với NPC chuyên biệt
5. Tracking tiến độ real-time
6. Save/Load state
7. Event-driven architecture

### 1.2. Design Patterns Sử Dụng

```
┌─────────────────────────────────────────────────────┐
│ DESIGN PATTERNS                                     │
├─────────────────────────────────────────────────────┤
│ 1. Factory Pattern      → Quest creation            │
│ 2. Strategy Pattern     → Objective behaviors       │
│ 3. Observer Pattern     → Event system              │
│ 4. Command Pattern      → Reward execution          │
│ 5. State Pattern        → Quest lifecycle           │
│ 6. Singleton Pattern    → QuestManager              │
│ 7. Builder Pattern      → Quest construction        │
│ 8. Template Method      → Quest base behavior       │
└─────────────────────────────────────────────────────┘
```

---

## 2. KIẾN TRÚC TỔNG THỂ

### 2.1. High-Level Architecture Diagram

```
┌──────────────────────────────────────────────────────────────────┐
│                        PRESENTATION LAYER                        │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐          │
│  │  Quest UI    │  │  Dialog UI   │  │ Tracker UI   │          │
│  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘          │
└─────────┼──────────────────┼──────────────────┼─────────────────┘
          │                  │                  │
┌─────────┼──────────────────┼──────────────────┼─────────────────┐
│         │          BUSINESS LOGIC LAYER       │                 │
│  ┌──────▼───────┐  ┌──────▼───────┐  ┌───────▼──────┐          │
│  │QuestManager  │  │NPCQuestGiver │  │EventDispatcher│         │
│  └──────┬───────┘  └──────┬───────┘  └───────┬──────┘          │
│         │                  │                  │                 │
│  ┌──────▼──────────────────▼──────────────────▼──────┐          │
│  │           Quest Processor Pipeline                │          │
│  │  • Validator • Tracker • Completer • Rewarder    │          │
│  └──────┬────────────────────────────────────────────┘          │
└─────────┼──────────────────────────────────────────────────────┘
          │
┌─────────┼──────────────────────────────────────────────────────┐
│         │              DATA LAYER                              │
│  ┌──────▼───────┐  ┌──────────────┐  ┌──────────────┐          │
│  │  Quest Data  │  │  Save Data   │  │  Configs     │          │
│  │(ScriptableObj)│  │   (JSON)     │  │  (Settings)  │          │
│  └──────────────┘  └──────────────┘  └──────────────┘          │
└──────────────────────────────────────────────────────────────────┘
          │
┌─────────┼──────────────────────────────────────────────────────┐
│         │           INTEGRATION LAYER                          │
│  ┌──────▼───────┐  ┌──────────────┐  ┌──────────────┐          │
│  │  Inventory   │  │   Economy    │  │    Combat    │          │
│  │   System     │  │   Manager    │  │   System     │          │
│  └──────────────┘  └──────────────┘  └──────────────┘          │
└──────────────────────────────────────────────────────────────────┘
```

### 2.2. Data Flow Architecture

```
QUEST LIFECYCLE FLOW:

[Quest Created]
      │
      ▼
[Validation] ──────► [Prerequisites Check]
      │                    │
      ├─── FAIL ──────────►[Reject & Notify]
      │
      ▼ PASS
[Quest Activated]
      │
      ▼
[Objective Tracking] ◄──── [Game Events]
      │                         │
      │                    [Inventory Changed]
      │                    [Monster Killed]
      │                    [NPC Talked]
      │                    [Item Bought/Sold]
      │
      ▼
[Progress Update] ──────► [UI Notification]
      │
      ▼
[Check Completion]
      │
      ├─── NOT COMPLETE ───► [Continue Tracking]
      │
      ▼ COMPLETE
[Quest Completed]
      │
      ▼
[Reward Processing]
      │
      ├─► [Gold Reward]
      ├─► [Item Reward]
      ├─► [Reputation Gain]
      └─► [Unlock New Quests]
      │
      ▼
[Save State]
      │
      ▼
[Cleanup & Archive]
```

---

## 3. CORE COMPONENTS

### 3.1. Quest Manager (Singleton)

**Trách Nhiệm:**
- Quản lý toàn bộ quest lifecycle
- Registry của tất cả quest data
- Điều phối giữa các subsystems
- Save/Load quest state

**Cấu Trúc:**

```
QuestManager
├── Properties
│   ├── AllQuests (Dictionary<string, QuestData>)
│   ├── ActiveQuests (Dictionary<string, QuestInstance>)
│   ├── CompletedQuests (HashSet<string>)
│   ├── AvailableQuests (List<QuestData>)
│   └── DailyQuestSlots (List<DailyQuestSlot>)
│
├── Core Methods
│   ├── Initialize()
│   ├── RegisterQuest(QuestData)
│   ├── StartQuest(string questId)
│   ├── UpdateQuest(string questId, ObjectiveProgress)
│   ├── CompleteQuest(string questId)
│   ├── CancelQuest(string questId)
│   └── GetAvailableQuestsFor(NPCData npc)
│
├── Query Methods
│   ├── IsQuestActive(string questId)
│   ├── IsQuestCompleted(string questId)
│   ├── CanStartQuest(QuestData quest)
│   ├── GetActiveQuestCount()
│   └── GetQuestProgress(string questId)
│
└── Event Handlers
    ├── OnInventoryChanged()
    ├── OnMonsterKilled(string monsterId)
    ├── OnItemBought(ItemSO item, int count)
    ├── OnNPCInteraction(string npcId)
    └── OnTimeChanged(GameTime time)
```

**Quan Hệ:**
- **Depends On**: QuestData, QuestInstance, ObjectiveTracker, RewardProcessor
- **Used By**: NPCQuestGiver, UI Controllers, Save System
- **Communicates With**: EventDispatcher, InventorySystem, EconomyManager

### 3.2. Quest Instance (Runtime State)

**Trách Nhiệm:**
- Lưu trạng thái runtime của 1 quest đang active
- Tracking objectives progress
- Quest state machine

**Cấu Trúc:**

```
QuestInstance
├── Data
│   ├── questData (Reference to QuestData SO)
│   ├── questId (string)
│   ├── state (QuestState enum)
│   ├── startTime (DateTime)
│   ├── objectives (List<ObjectiveInstance>)
│   └── metadata (Dictionary<string, object>)
│
├── State Management
│   ├── CurrentState { get; }
│   ├── TransitionTo(QuestState newState)
│   ├── CanTransitionTo(QuestState target)
│   └── OnStateChanged (event)
│
├── Objective Tracking
│   ├── GetObjective(int index)
│   ├── UpdateObjective(int index, int progress)
│   ├── IsObjectiveComplete(int index)
│   ├── GetCompletedObjectiveCount()
│   └── AreAllObjectivesComplete()
│
└── Progress Calculation
    ├── GetOverallProgress() // Returns 0-1
    ├── GetProgressText() // "2/3 objectives"
    └── GetTimeRemaining() // For timed quests
```

**Quest State Machine:**

```
┌─────────────┐
│  AVAILABLE  │ (Đủ điều kiện nhận)
└──────┬──────┘
       │ StartQuest()
       ▼
┌─────────────┐
│   ACTIVE    │ (Đang làm)
└──────┬──────┘
       │ UpdateProgress()
       ├──► FAILED (Nếu có fail condition)
       │
       ▼ AllObjectivesComplete()
┌─────────────┐
│  COMPLETED  │ (Hoàn thành, chưa claim)
└──────┬──────┘
       │ ClaimReward()
       ▼
┌─────────────┐
│  ARCHIVED   │ (Đã claim, lưu vào history)
└─────────────┘
```

### 3.3. Quest Validator

**Trách Nhiệm:**
- Kiểm tra điều kiện trước khi start quest
- Validate quest chain logic
- Prevent invalid quest states

**Cấu Trúc:**

```
QuestValidator
├── Validation Rules
│   ├── CheckPlayerLevel(int required)
│   ├── CheckPrerequisites(List<string> questIds)
│   ├── CheckReputation(string npcId, int required)
│   ├── CheckInventorySpace(List<ItemReward>)
│   ├── CheckMaxActiveQuests(int limit)
│   └── CheckCooldown(QuestData quest)
│
├── Validation Pipeline
│   ├── ValidateAll(QuestData quest, PlayerData player)
│   ├── GetFailureReasons(QuestData quest)
│   └── CanStartQuest(QuestData quest) → ValidationResult
│
└── Result Types
    ├── ValidationResult
    │   ├── isValid (bool)
    │   ├── failureReason (string)
    │   └── failureType (ValidationType enum)
    │
    └── ValidationType (enum)
        ├── LevelTooLow
        ├── PrerequisiteNotMet
        ├── InsufficientReputation
        ├── InventoryFull
        ├── MaxActiveQuestsReached
        └── OnCooldown
```

### 3.4. Quest Processor Pipeline

**Trách Nhiệm:**
- Xử lý các bước của quest lifecycle
- Chain of responsibility pattern

**Cấu Trúc:**

```
QuestProcessor (Abstract Base)
├── Process(QuestInstance quest) → ProcessResult
├── next (QuestProcessor)
└── SetNext(QuestProcessor processor)

ProcessorPipeline:
    ┌───────────────┐
    │  Validator    │ → Check can start
    └───────┬───────┘
            ▼
    ┌───────────────┐
    │   Activator   │ → Initialize quest instance
    └───────┬───────┘
            ▼
    ┌───────────────┐
    │    Tracker    │ → Monitor objectives
    └───────┬───────┘
            ▼
    ┌───────────────┐
    │   Completer   │ → Mark as complete
    └───────┬───────┘
            ▼
    ┌───────────────┐
    │   Rewarder    │ → Give rewards
    └───────┬───────┘
            ▼
    ┌───────────────┐
    │   Archiver    │ → Save to history
    └───────────────┘
```

**Lợi Ích:**
- Dễ thêm/bớt steps trong pipeline
- Mỗi processor test độc lập
- Có thể skip steps cho special quests
- Clear separation of concerns

---

## 4. DATA LAYER

### 4.1. Quest Data (ScriptableObject)

**Thiết Kế:**

```
QuestData : ScriptableObject
├── Identity
│   ├── questId (string) [Primary Key]
│   ├── questName (LocalizedString)
│   ├── description (LocalizedString)
│   └── icon (Sprite)
│
├── Classification
│   ├── questType (QuestType enum)
│   ├── category (QuestCategory enum)
│   ├── tier (QuestTier enum)
│   └── tags (List<string>)
│
├── Requirements
│   ├── minPlayerLevel (int)
│   ├── prerequisites (List<QuestData>)
│   ├── requiredReputation (NPCReputationRequirement[])
│   ├── requiredItems (ItemRequirement[])
│   └── seasonRestriction (Season enum)
│
├── Objectives
│   ├── objectives (List<QuestObjective>)
│   ├── objectiveOrder (ObjectiveOrder enum)
│   │   ├── Parallel (all at once)
│   │   └── Sequential (one by one)
│   └── failConditions (List<FailCondition>)
│
├── Rewards
│   ├── goldReward (int)
│   ├── itemRewards (ItemReward[])
│   ├── reputationRewards (NPCReputation[])
│   ├── unlocks (UnlockData[])
│   └── experienceReward (int)
│
├── Dialog & Narrative
│   ├── npcGiverId (string)
│   ├── dialogueOnOffer (DialogueTree)
│   ├── dialogueOnProgress (DialogueTree)
│   ├── dialogueOnComplete (DialogueTree)
│   └── storyBeat (StoryBeatData)
│
├── Behavior Settings
│   ├── isRepeatable (bool)
│   ├── cooldownHours (float)
│   ├── timeLimit (float)
│   ├── autoComplete (bool)
│   ├── trackInUI (bool)
│   └── showInJournal (bool)
│
└── Advanced
    ├── onStartActions (QuestAction[])
    ├── onCompleteActions (QuestAction[])
    ├── customData (SerializedDictionary)
    └── editorNotes (string)
```

**Quest Type Enum:**

```
QuestType
├── Story           // Cốt truyện chính
├── Side            // Phụ
├── Daily           // Hàng ngày
├── Income          // Thu nhập (farming/resource)
├── Combat          // Tiêu diệt quái
├── Delivery        // Giao hàng
├── Collection      // Thu thập
└── Tutorial        // Hướng dẫn
```

**Quest Category Enum:**

```
QuestCategory
├── MainStory       // Chuỗi chính
├── NPCPersonal     // Câu chuyện NPC
├── ResourceGather  // Thu thập tài nguyên
├── MonsterHunt     // Săn quái
├── ShopMission     // Nhiệm vụ vendor
├── Exploration     // Khám phá
└── Seasonal        // Theo mùa
```

### 4.2. Quest Objective (Polymorphic Design)

**Base Structure:**

```
QuestObjective (Abstract Base)
├── Common Properties
│   ├── objectiveId (string)
│   ├── description (LocalizedString)
│   ├── targetAmount (int)
│   ├── optional (bool)
│   └── hidden (bool)
│
├── Abstract Methods
│   ├── GetProgress() → int
│   ├── IsComplete() → bool
│   ├── Validate() → bool
│   └── GetTrackingKey() → string
│
└── Virtual Methods
    ├── OnActivate()
    ├── OnUpdate(EventData)
    └── OnComplete()
```

**Objective Types Hierarchy:**

```
CollectObjective : QuestObjective
├── targetItem (ItemSO)
├── targetAmount (int)
└── consumeOnComplete (bool)

KillObjective : QuestObjective
├── targetMonsterType (string)
├── targetAmount (int)
└── requiresSpecificWeapon (WeaponType)

TalkToObjective : QuestObjective
├── targetNPCId (string)
└── specificDialogueId (string)

BuyObjective : QuestObjective
├── targetItem (ItemSO)
├── targetAmount (int)
└── fromSpecificVendor (string)

SellObjective : QuestObjective
├── targetItem (ItemSO)
├── targetAmount (int)
└── toSpecificVendor (string)

PlantObjective : QuestObjective
├── targetSeed (SeedSO)
├── targetAmount (int)
└── anySeasonalCrop (bool)

HarvestObjective : QuestObjective
├── targetCrop (ItemSO)
├── targetAmount (int)
└── qualityRequired (CropQuality)

ExploreObjective : QuestObjective
├── targetLocation (string)
└── revealArea (bool)

DeliverObjective : QuestObjective
├── itemToDeliver (ItemSO)
├── targetNPC (string)
└── removeItemOnDeliver (bool)

EquipObjective : QuestObjective
├── requiredEquipment (ItemSO)
└── mustKeepEquipped (bool)

EarnMoneyObjective : QuestObjective
├── targetAmount (int)
└── trackingMode (MoneyTrackingMode)
    ├── Total (tổng số hiện có)
    └── Earned (kiếm được trong quest)
```

**Strategy Pattern Application:**

```
IObjectiveStrategy
├── CheckProgress(GameEvent event) → bool
├── UpdateProgress(GameEvent event)
└── GetCurrentValue() → int

ObjectiveStrategyFactory
├── CreateStrategy(ObjectiveType type) → IObjectiveStrategy
└── RegisterStrategy(ObjectiveType, IObjectiveStrategy)
```

### 4.3. Reward System Data

**Cấu Trúc:**

```
RewardData
├── Immediate Rewards (Claim ngay)
│   ├── GoldReward
│   │   └── amount (int)
│   │
│   ├── ItemReward
│   │   ├── item (ItemSO)
│   │   ├── amount (int)
│   │   ├── dropChance (float)
│   │   └── quality (ItemQuality)
│   │
│   └── ExperienceReward
│       ├── amount (int)
│       └── skillType (SkillType)
│
├── Progression Rewards (Unlock/Unlock)
│   ├── ReputationReward
│   │   ├── npcId (string)
│   │   └── amount (int)
│   │
│   ├── UnlockReward
│   │   ├── unlockType (UnlockType)
│   │   ├── targetId (string)
│   │   └── unlockData (object)
│   │
│   └── RecipeUnlock
│       └── recipeData (RecipeData)
│
└── Special Rewards
    ├── StoryProgression
    │   └── storyBeatId (string)
    │
    ├── QuestUnlock
    │   └── questIds (List<string>)
    │
    └── CustomReward
        └── rewardHandler (IRewardHandler)
```

**Reward Processor:**

```
RewardProcessor
├── ValidateReward(RewardData) → bool
├── CanGiveReward(RewardData, PlayerData) → bool
├── GiveReward(RewardData, PlayerData) → RewardResult
└── RollRandomRewards(RewardData[]) → RewardData[]

RewardResult
├── success (bool)
├── givenRewards (List<RewardData>)
├── failedRewards (List<RewardData>)
└── failureReason (string)
```

---

## 5. QUEST TYPES ARCHITECTURE

### 5.1. Story Quest System

**Đặc Điểm:**
- Linear hoặc branching narrative
- Chain của nhiều quests
- Gate cho progression
- Cinematic/dialogue heavy

**Cấu Trúc:**

```
StoryQuestChain
├── chainId (string)
├── chainName (LocalizedString)
├── quests (List<QuestData>)
├── branchPoints (List<BranchPoint>)
└── progression (ChainProgression)

BranchPoint
├── questId (string)
├── condition (BranchCondition)
└── nextQuests (List<string>)

BranchCondition
├── type (ConditionType)
│   ├── PlayerChoice
│   ├── QuestOutcome
│   └── ReputationThreshold
└── evaluator (IConditionEvaluator)

StoryQuestManager : IQuestTypeManager
├── GetCurrentStoryQuest() → QuestData
├── GetNextInChain(string questId) → QuestData
├── EvaluateBranch(BranchPoint) → string
└── GetChainProgress(string chainId) → float
```

**Example Story Flow:**

```
[Prolog: Farm Inheritance]
      │
      ▼
[Chapter 1: Learning Basics]
   ├─► Quest 1.1: Meet NPCs
   ├─► Quest 1.2: First Harvest
   └─► Quest 1.3: First Sale
      │
      ▼
[Chapter 2: Expansion]
   ├─► Quest 2.1: Clear Forest (unlock area)
   ├─► Quest 2.2: Upgrade Tools
   └─► Quest 2.3: First Monster
      │
      ├──► BRANCH: Help Vendor A
      │         │
      │         ├─► Quest 3A.1: Gather Seeds
      │         └─► Quest 3A.2: Plant Garden
      │
      └──► BRANCH: Help Vendor B
                │
                ├─► Quest 3B.1: Hunt Monsters
                └─► Quest 3B.2: Get Equipment
```

### 5.2. Income Quest System

**Đặc Điểm:**
- Focus vào kiếm tiền/resources
- Repeatable hoặc one-time
- Tie vào economy loop

**Cấu Trúc:**

```
IncomeQuestData : QuestData
├── incomeType (IncomeType enum)
│   ├── Farming
│   ├── ResourceGathering
│   ├── Selling
│   └── Mixed
│
├── profitTarget (int)
├── timeLimit (float)
└── bonusRewardThreshold (int)

IncomeQuestTracker
├── startingMoney (int)
├── currentProfit (int)
├── resourcesCollected (Dictionary<ItemSO, int>)
└── CalculateProfit() → int

Example Income Quests:
├── "Harvest 50 Crops" → Get base reward
├── "Earn 1000g in 3 days" → Any method
├── "Sell 20 Resources to Vendor" → Specific action
└── "Collect 100 Wood + 50 Stone" → Specific resources
```

### 5.3. Combat/Monster Hunt Quest System

**Đặc Điểm:**
- Tiêu diệt số lượng monster
- Có thể yêu cầu vũ khí cụ thể
- Unlock khu vực mới

**Cấu Trúc:**

```
CombatQuestData : QuestData
├── targetMonsterType (MonsterType enum)
├── targetCount (int)
├── specificArea (string)
├── requiredWeapon (WeaponType)
├── timeLimit (float)
└── allowOverkill (bool)

MonsterKillTracker
├── monsterType (string)
├── killCount (int)
├── killsByWeapon (Dictionary<WeaponType, int>)
└── OnMonsterKilled(MonsterKilledEvent)

Integration với Combat System:
    Combat System
         │
         ├─► OnMonsterKilled event
         │        │
         ▼        ▼
    QuestManager.OnMonsterKilled(monsterId, weaponType)
         │
         ▼
    Update all active combat quests
         │
         ▼
    Check completion
```

### 5.4. NPC Vendor Quest System

**Đặc Điểm:**
- Quests từ NPCs bán hàng
- Tie vào shop economy
- Unlock items/discounts

**Cấu Trúc:**

```
VendorQuestData : QuestData
├── vendorId (string)
├── questRewardType (VendorRewardType)
│   ├── Discount
│   ├── UnlockItem
│   ├── SpecialOffer
│   └── Reputation
│
├── vendorSpecificData
│   ├── itemsToUnlock (List<ItemSO>)
│   ├── discountPercent (float)
│   └── specialOfferDuration (float)
│
└── purchaseRequirements
    ├── mustBuyFrom (string vendorId)
    ├── mustBuyItem (ItemSO)
    └── mustSpendAmount (int)

VendorQuestTypes:
├── Seed Collection Quest
│   ├── Objective: Buy/collect X seeds
│   └── Reward: Rare seed unlock
│
├── Equipment Upgrade Quest
│   ├── Objective: Collect materials
│   └── Reward: Discount on upgrade service
│
├── Weapon Retrieval Quest
│   ├── Objective: Defeat monster, get weapon
│   └── Reward: Unlock better weapons in shop
│
└── Resource Delivery Quest
    ├── Objective: Deliver X resources
    └── Reward: Permanent discount
```

---

## 6. NPC QUEST SYSTEM

### 6.1. NPC Quest Giver Component

**Trách Nhiệm:**
- Attach vào NPC GameObject
- Manage quests offered by NPC
- Handle dialogue integration

**Cấu Trúc:**

```
NPCQuestGiver : MonoBehaviour
├── NPC Identity
│   ├── npcId (string)
│   ├── npcName (LocalizedString)
│   └── npcType (NPCType enum)
│
├── Quest Pool
│   ├── availableQuests (List<QuestData>)
│   ├── activeQuestsFromThisNPC (List<string>)
│   └── completedQuestsFromThisNPC (HashSet<string>)
│
├── Quest Management
│   ├── GetAvailableQuests() → List<QuestData>
│   ├── CanOfferQuest(QuestData) → bool
│   ├── OfferQuest(QuestData)
│   ├── OnQuestAccepted(QuestData)
│   └── OnQuestCompleted(QuestData)
│
├── Dialogue Integration
│   ├── GetGreetingDialogue() → DialogueNode
│   ├── GetQuestOfferDialogue(QuestData) → DialogueNode
│   ├── GetInProgressDialogue(QuestData) → DialogueNode
│   └── GetCompletionDialogue(QuestData) → DialogueNode
│
└── Reputation
    ├── currentReputation (int)
    ├── reputationLevel (ReputationLevel enum)
    └── OnReputationChanged (event)
```

**NPC Interaction Flow:**

```
Player Interacts with NPC
    │
    ▼
[Check Active Quests from this NPC]
    │
    ├─► Has completable quest?
    │   └─► Show "Complete Quest" dialogue
    │
    ├─► Has in-progress quest?
    │   └─► Show "Progress Check" dialogue
    │
    └─► No active quest
        │
        ▼
   [Check Available Quests]
        │
        ├─► Has available quests?
        │   └─► Show "Quest Offer" dialogue
        │
        └─► No quests
            └─► Show "Generic Greeting" dialogue
```

### 6.2. NPC Reputation System

**Cấu Trúc:**

```
NPCReputation
├── npcId (string)
├── currentPoints (int)
├── level (ReputationLevel enum)
│   ├── Stranger (0-99)
│   ├── Acquaintance (100-299)
│   ├── Friend (300-599)
│   ├── BestFriend (600-999)
│   └── Soulmate (1000+)
│
├── benefits (List<ReputationBenefit>)
└── thresholds (ReputationThreshold[])

ReputationBenefit
├── unlockLevel (ReputationLevel)
├── benefitType (BenefitType)
│   ├── ShopDiscount
│   ├── QuestUnlock
│   ├── SpecialDialogue
│   └── UniqueReward
└── value (float)

ReputationManager
├── GetReputation(string npcId) → int
├── AddReputation(string npcId, int amount)
├── GetReputationLevel(string npcId) → ReputationLevel
├── GetBenefitsAt(ReputationLevel) → List<ReputationBenefit>
└── OnReputationLevelUp (event)
```

**Tích Hợp Với Shop:**

```
When player shops with vendor:
    │
    ▼
Get current reputation level
    │
    ▼
Apply discount benefits
    │
    ├─► Stranger: 0%
    ├─► Acquaintance: -5%
    ├─► Friend: -10%
    ├─► BestFriend: -15%
    └─► Soulmate: -20% + special items
```

---

## 7. DAILY QUEST SYSTEM

### 7.1. Daily Quest Manager

**Đặc Điểm:**
- Reset mỗi ngày (game time)
- Limited slots (3-5 quests/day)
- Random selection from pool
- Dedicated NPC

**Cấu Trúc:**

```
DailyQuestManager : MonoBehaviour
├── Configuration
│   ├── dailyQuestNPCId (string)
│   ├── maxDailyQuests (int) // 3-5
│   ├── resetHour (int) // Game time
│   └── questPool (List<QuestData>)
│
├── State
│   ├── currentDailyQuests (List<DailyQuestSlot>)
│   ├── lastResetTime (DateTime)
│   ├── completedToday (HashSet<string>)
│   └── rerollsRemaining (int)
│
├── Core Methods
│   ├── Initialize()
│   ├── GenerateDailyQuests()
│   ├── CheckForReset()
│   ├── RerollQuest(int slotIndex)
│   └── GetAvailableDailyQuests() → List<QuestData>
│
└── Selection Logic
    ├── SelectRandomQuests(int count) → List<QuestData>
    ├── FilterByDifficulty(DifficultyRange)
    ├── EnsureVariety() // No duplicates
    └── ApplyWeights(QuestWeight[])
```

**Daily Quest Slot:**

```
DailyQuestSlot
├── slotIndex (int)
├── quest (QuestData)
├── state (SlotState enum)
│   ├── Available
│   ├── Active
│   ├── Completed
│   └── Locked
├── rerollCost (int)
└── expiryTime (DateTime)
```

**Reset Logic:**

```
Time System tick
    │
    ▼
Check if current hour == resetHour
    │
    ▼ YES
Clear all daily quest slots
    │
    ▼
Generate new set of daily quests
    │
    ├─► Select from weighted pool
    ├─► Ensure difficulty variety
    ├─► No repeats from yesterday
    └─► Assign to slots
    │
    ▼
Notify player (if online)
    │
    ▼
Save state
```

### 7.2. Daily Quest Pool Configuration

**Cấu Trúc:**

```
DailyQuestPool : ScriptableObject
├── poolId (string)
├── quests (List<DailyQuestEntry>)
└── selectionRules (PoolSelectionRules)

DailyQuestEntry
├── quest (QuestData)
├── weight (float) // Higher = more likely
├── difficulty (QuestDifficulty enum)
├── minimumDay (int) // Unlock after X days
└── seasonRestriction (Season enum)

PoolSelectionRules
├── mustIncludeDifficulty (DifficultyDistribution)
│   ├── Easy: 40%
│   ├── Medium: 40%
│   └── Hard: 20%
│
├── categoryVariety (bool) // Mix quest types
├── preventRecentRepeats (int) // Days to wait
└── playerLevelScaling (bool)

Example Pool:
├── Easy Quests (40%)
│   ├── "Harvest 10 Crops" (weight: 1.0)
│   ├── "Sell 5 Items" (weight: 1.0)
│   └── "Talk to 3 NPCs" (weight: 0.8)
│
├── Medium Quests (40%)
│   ├── "Earn 500g" (weight: 1.0)
│   ├── "Kill 5 Slimes" (weight: 0.9)
│   └── "Buy 10 Seeds" (weight: 1.0)
│
└── Hard Quests (20%)
    ├── "Harvest 50 Crops" (weight: 0.7)
    ├── "Kill 10 Monsters" (weight: 0.8)
    └── "Earn 2000g" (weight: 0.6)
```

### 7.3. Daily Quest NPC

**Special NPC "Quest Board" hoặc "Town Crier":**

```
DailyQuestNPC : NPCQuestGiver
├── Override Methods
│   ├── GetAvailableQuests() → DailyQuestManager.GetDailyQuests()
│   ├── OnInteract() → Show daily quest UI
│   └── GetDialogue() → Daily-specific dialogue
│
├── UI Integration
│   ├── ShowDailyQuestBoard()
│   ├── AllowReroll(int slotIndex)
│   └── DisplayTimeUntilReset()
│
└── Special Features
    ├── streakBonus (int) // Consecutive days
    ├── weeklyChallenge (QuestData)
    └── leaderboard (bool)
```

**UI Layout:**

```
┌─────────────────────────────────────────┐
│  DAILY QUESTS - Reset in: 4h 23m        │
│  Streak: 7 days 🔥 (+10% gold bonus)    │
├─────────────────────────────────────────┤
│  SLOT 1: [EASY]                         │
│  ┌───────────────────────────────────┐  │
│  │ Harvest 10 Crops                  │  │
│  │ Progress: 3/10                    │  │
│  │ Reward: 150g                      │  │
│  │ [IN PROGRESS]      [REROLL: 50g] │  │
│  └───────────────────────────────────┘  │
│                                          │
│  SLOT 2: [MEDIUM]                       │
│  ┌───────────────────────────────────┐  │
│  │ Kill 5 Slimes                     │  │
│  │ Reward: 300g + Slime x5           │  │
│  │ [ACCEPT]           [REROLL: 50g]  │  │
│  └───────────────────────────────────┘  │
│                                          │
│  SLOT 3: [HARD]                         │
│  ┌───────────────────────────────────┐  │
│  │ ✓ COMPLETED                       │  │
│  │ Earned 1000g                      │  │
│  │ Reward: 500g + Rare Seed          │  │
│  │ [CLAIM REWARD]                    │  │
│  └───────────────────────────────────┘  │
│                                          │
│  [CLOSE]                                │
└─────────────────────────────────────────┘
```

---

## 8. OBJECTIVE SYSTEM

### 8.1. Objective Tracker

**Trách Nhiệm:**
- Monitor game events
- Update objective progress
- Notify quest manager

**Cấu Trúc:**

```
ObjectiveTracker
├── Registration
│   ├── RegisterObjective(QuestInstance, ObjectiveInstance)
│   ├── UnregisterObjective(string objectiveId)
│   └── GetTrackedObjectives() → List<ObjectiveInstance>
│
├── Event Subscriptions
│   ├── SubscribeToInventoryEvents()
│   ├── SubscribeToCombatEvents()
│   ├── SubscribeToNPCEvents()
│   ├── SubscribeToEconomyEvents()
│   └── SubscribeToTimeEvents()
│
├── Progress Updates
│   ├── OnGameEvent(GameEvent event)
│   ├── UpdateRelevantObjectives(GameEvent)
│   ├── CheckCompletion(ObjectiveInstance)
│   └── NotifyQuestManager(string questId)
│
└── Query Methods
    ├── GetObjectivesForQuest(string questId)
    ├── GetProgressForObjective(string objId) → float
    └── GetCompletedObjectives() → List<ObjectiveInstance>
```

**Event-to-Objective Mapping:**

```
Event Dispatcher
    │
    ├─► InventoryChangedEvent
    │   └─► Update: CollectObjective, SellObjective
    │
    ├─► MonsterKilledEvent
    │   └─► Update: KillObjective
    │
    ├─► NPCInteractionEvent
    │   └─► Update: TalkToObjective, DeliverObjective
    │
    ├─► ItemBoughtEvent
    │   └─► Update: BuyObjective
    │
    ├─► CropPlantedEvent
    │   └─► Update: PlantObjective
    │
    ├─► CropHarvestedEvent
    │   └─► Update: HarvestObjective
    │
    └─► LocationDiscoveredEvent
        └─► Update: ExploreObjective
```

### 8.2. Objective Instance (Runtime)

**Cấu Trúc:**

```
ObjectiveInstance
├── Data Reference
│   ├── objectiveData (QuestObjective)
│   ├── questId (string)
│   └── objectiveIndex (int)
│
├── Progress Tracking
│   ├── currentProgress (int)
│   ├── targetProgress (int)
│   ├── isComplete (bool)
│   └── completionTime (DateTime?)
│
├── State
│   ├── isActive (bool)
│   ├── isFailed (bool)
│   └── failReason (string)
│
└── Methods
    ├── UpdateProgress(int amount)
    ├── SetProgress(int value)
    ├── CheckCompletion() → bool
    ├── Reset()
    └── GetProgressPercent() → float
```

### 8.3. Objective Validators

**Purpose:** Validate progress before updating

```
IObjectiveValidator
├── CanUpdate(ObjectiveInstance, GameEvent) → bool
├── ValidateProgress(int newProgress) → bool
└── GetValidationError() → string

InventorySpaceValidator
├── Check if player has space for rewards
└── Used for: CollectObjective

LocationValidator
├── Check if player in correct area
└── Used for: HarvestObjective, KillObjective

TimeValidator
├── Check if within time window
└── Used for: Timed objectives

EquipmentValidator
├── Check if correct tool/weapon equipped
└── Used for: KillObjective (specific weapon)
```

---

## 9. REWARD SYSTEM

### 9.1. Reward Processor Architecture

**Cấu Trúc:**

```
RewardProcessor
├── Validation
│   ├── ValidateReward(RewardData) → ValidationResult
│   ├── CanPlayerReceive(RewardData, PlayerData) → bool
│   └── CheckInventorySpace(ItemReward[]) → bool
│
├── Processing
│   ├── ProcessReward(RewardData) → RewardResult
│   ├── ProcessGoldReward(int amount)
│   ├── ProcessItemReward(ItemReward)
│   ├── ProcessReputationReward(NPCReputation)
│   └── ProcessUnlockReward(UnlockData)
│
├── Random Rewards
│   ├── RollRewards(RandomRewardTable) → RewardData[]
│   ├── ApplyDropChance(ItemReward) → bool
│   └── SelectFromPool(RewardPool, int count)
│
└── Events
    ├── OnRewardProcessed (event)
    ├── OnGoldAdded (event)
    └── OnItemAdded (event)
```

**Reward Processing Pipeline:**

```
Quest Completed
    │
    ▼
[Validate Rewards]
    │
    ├─► Check inventory space
    ├─► Check gold capacity
    └─► Validate unlock conditions
    │
    ▼ All Valid
[Process Immediate Rewards]
    │
    ├─► Add Gold → EconomyManager
    ├─► Add Items → InventorySystem
    └─► Add Experience → (if exists)
    │
    ▼
[Process Progression Rewards]
    │
    ├─► Add Reputation → ReputationManager
    ├─► Unlock Quests → QuestManager
    ├─► Unlock Items → ShopManager
    └─► Unlock Areas → WorldManager
    │
    ▼
[Trigger Reward Events]
    │
    ├─► UI Notification
    ├─► Sound/VFX
    └─► Save State
    │
    ▼
[Execute Post-Reward Actions]
    │
    ├─► Start follow-up quests
    ├─► Trigger cutscene
    └─► Update world state
```

### 9.2. Reward Handlers (Strategy Pattern)

**Interface:**

```
IRewardHandler
├── CanHandle(RewardData) → bool
├── Handle(RewardData, PlayerData) → RewardResult
└── GetHandlerType() → RewardType

Implementations:

GoldRewardHandler : IRewardHandler
├── Integrates with: EconomyManager
└── Adds gold to player wallet

ItemRewardHandler : IRewardHandler
├── Integrates with: InventorySystem
├── Handles: Item addition, quality rolls
└── Handles overflow (mail system?)

ReputationRewardHandler : IRewardHandler
├── Integrates with: ReputationManager
└── Updates NPC reputation

QuestUnlockHandler : IRewardHandler
├── Integrates with: QuestManager
└── Makes new quests available

ShopUnlockHandler : IRewardHandler
├── Integrates with: VendorSystem
└── Unlocks items in shops

RecipeUnlockHandler : IRewardHandler
├── Integrates with: CraftingSystem (future)
└── Unlocks crafting recipes

AreaUnlockHandler : IRewardHandler
├── Integrates with: WorldManager
└── Opens new map areas
```

**Handler Registration:**

```
RewardHandlerRegistry
├── handlers (Dictionary<RewardType, IRewardHandler>)
├── RegisterHandler(IRewardHandler)
├── GetHandler(RewardType) → IRewardHandler
└── ProcessReward(RewardData) → delegate to handler
```

### 9.3. Random Reward Tables

**Cấu Trúc:**

```
RandomRewardTable : ScriptableObject
├── tableId (string)
├── entries (List<RewardEntry>)
├── guaranteedRewards (List<RewardData>)
└── rollCount (int) // How many to roll

RewardEntry
├── reward (RewardData)
├── weight (float)
├── minRoll (float) [0-100]
├── maxRoll (float) [0-100]
└── conditions (RewardCondition[])

Example Table:
├── Guaranteed: 200 Gold
├── Roll 2 items from:
│   ├── Iron Ore x5 (40% chance, weight: 4.0)
│   ├── Rare Seed x1 (20% chance, weight: 2.0)
│   ├── HP Potion x3 (30% chance, weight: 3.0)
│   └── Nothing (10% chance, weight: 1.0)
```

---

## 10. EVENT & MESSAGING

### 10.1. Event Dispatcher

**Trách Nhiệm:**
- Central event bus
- Decouple systems
- Type-safe events

**Cấu Trúc:**

```
EventDispatcher : MonoBehaviour (Singleton)
├── Event Registry
│   └── events (Dictionary<Type, Delegate>)
│
├── Core Methods
│   ├── Subscribe<T>(Action<T> handler)
│   ├── Unsubscribe<T>(Action<T> handler)
│   ├── Dispatch<T>(T eventData)
│   └── ClearAll()
│
└── Lifecycle
    ├── Initialize()
    └── Cleanup()

Usage Example:
    // Subscribe
    EventDispatcher.Instance.Subscribe<MonsterKilledEvent>(OnMonsterKilled);

    // Dispatch
    EventDispatcher.Instance.Dispatch(new MonsterKilledEvent {
        monsterId = "slime_001",
        weaponUsed = WeaponType.Sword
    });

    // Unsubscribe
    EventDispatcher.Instance.Unsubscribe<MonsterKilledEvent>(OnMonsterKilled);
```

### 10.2. Quest-Related Events

**Event Types:**

```
QuestStartedEvent
├── questId (string)
├── questData (QuestData)
└── timestamp (DateTime)

QuestProgressUpdatedEvent
├── questId (string)
├── objectiveIndex (int)
├── oldProgress (int)
├── newProgress (int)
└── isComplete (bool)

QuestCompletedEvent
├── questId (string)
├── questData (QuestData)
├── rewards (RewardData[])
└── completionTime (DateTime)

QuestFailedEvent
├── questId (string)
├── failReason (string)
└── canRetry (bool)

QuestUnlockedEvent
├── questId (string)
└── unlockedBy (string)

DailyQuestsRefreshedEvent
├── newQuests (List<QuestData>)
└── resetTime (DateTime)
```

### 10.3. Game Events (for Objective Tracking)

**Event Definitions:**

```
InventoryChangedEvent
├── itemAdded (ItemSO)
├── itemRemoved (ItemSO)
├── changeAmount (int)
└── newCount (int)

MonsterKilledEvent
├── monsterId (string)
├── monsterType (MonsterType)
├── weaponUsed (WeaponType)
├── location (Vector3)
└── damageDealt (int)

NPCInteractionEvent
├── npcId (string)
├── interactionType (InteractionType)
│   ├── Talk
│   ├── Trade
│   └── QuestTurnIn
└── timestamp (DateTime)

ItemBoughtEvent
├── item (ItemSO)
├── amount (int)
├── price (int)
└── vendorId (string)

ItemSoldEvent
├── item (ItemSO)
├── amount (int)
├── earnings (int)
└── vendorId (string)

CropPlantedEvent
├── seed (SeedSO)
├── location (Vector2Int)
└── season (Season)

CropHarvestedEvent
├── crop (ItemSO)
├── amount (int)
├── quality (CropQuality)
└── location (Vector2Int)

MoneyEarnedEvent
├── amount (int)
├── source (MoneySource enum)
│   ├── QuestReward
│   ├── Selling
│   ├── Harvest
│   └── Other
└── newTotal (int)

LocationDiscoveredEvent
├── locationId (string)
├── locationName (string)
└── timestamp (DateTime)
```

---

## 11. SAVE/LOAD SYSTEM

### 11.1. Quest Save Data

**Cấu Trúc:**

```
QuestSaveData
├── activeQuests (List<ActiveQuestSave>)
├── completedQuests (List<string>) // Quest IDs
├── dailyQuestState (DailyQuestSave)
├── npcReputation (Dictionary<string, int>)
├── unlockedQuests (List<string>)
└── questHistory (List<QuestHistoryEntry>)

ActiveQuestSave
├── questId (string)
├── state (QuestState)
├── startTime (long) // Ticks
├── objectives (List<ObjectiveSave>)
└── customData (string) // JSON

ObjectiveSave
├── objectiveIndex (int)
├── currentProgress (int)
├── isComplete (bool)
└── completionTime (long?)

DailyQuestSave
├── lastResetTime (long)
├── currentSlots (List<DailySlotSave>)
├── completedToday (List<string>)
└── streakDays (int)

DailySlotSave
├── slotIndex (int)
├── questId (string)
├── state (SlotState)
└── rerollsUsed (int)

QuestHistoryEntry
├── questId (string)
├── completedTime (long)
├── rewardsReceived (string) // JSON
└── wasDaily (bool)
```

### 11.2. Save/Load Manager Integration

**Interface:**

```
IQuestSaveProvider
├── SaveQuests() → QuestSaveData
├── LoadQuests(QuestSaveData)
├── ResetProgress()
└── GetSaveVersion() → int

QuestSaveManager : IQuestSaveProvider
├── Serialization
│   ├── Serialize(QuestSaveData) → string (JSON)
│   ├── Deserialize(string) → QuestSaveData
│   └── Compress(string) → byte[]
│
├── Persistence
│   ├── SaveToFile(QuestSaveData, string path)
│   ├── LoadFromFile(string path) → QuestSaveData
│   └── DeleteSave(string path)
│
└── Migration
    ├── MigrateFromVersion(int oldVersion, QuestSaveData)
    └── ValidateSaveData(QuestSaveData) → bool
```

**Save Timing:**

```
Auto-Save Triggers:
├── Quest started
├── Quest objective completed
├── Quest completed
├── Daily quests refreshed
├── Game paused/quit
└── Every X minutes (configurable)

Save Flow:
    Trigger event
        │
        ▼
    Gather quest state
        │
        ▼
    Serialize to JSON
        │
        ▼
    Write to file (async)
        │
        ▼
    Backup previous save
```

---

## 12. INTEGRATION POINTS

### 12.1. Inventory System Integration

**Interface Contract:**

```
IInventorySystem
├── HasItem(ItemSO item, int amount) → bool
├── AddItem(ItemSO item, int amount) → bool
├── RemoveItem(ItemSO item, int amount) → bool
├── GetItemCount(ItemSO item) → int
├── HasSpace(int slotsNeeded) → bool
└── OnInventoryChanged (event)

Quest System Usage:
├── CollectObjective → Subscribe to OnInventoryChanged
├── Reward System → AddItem for item rewards
├── BuyObjective → Track AddItem events
└── SellObjective → Track RemoveItem events
```

**Event Flow:**

```
Player picks up item
    │
    ▼
InventorySystem.AddItem(item, count)
    │
    ▼
Dispatch InventoryChangedEvent
    │
    ▼
ObjectiveTracker receives event
    │
    ▼
Check active CollectObjectives
    │
    ▼
Update matching objectives
    │
    ▼
Check quest completion
```

### 12.2. Economy System Integration

**Interface Contract:**

```
IEconomyManager
├── GetMoney() → int
├── AddMoney(int amount) → bool
├── TrySpend(int amount) → bool
├── CanAfford(int amount) → bool
└── OnMoneyChanged (event)

Quest System Usage:
├── Reward System → AddMoney for gold rewards
├── EarnMoneyObjective → Track OnMoneyChanged
├── BuyObjective → Track TrySpend
└── Quest prerequisites → CanAfford check
```

### 12.3. Combat System Integration

**Interface Contract:**

```
ICombatSystem
├── GetLastKilledMonster() → MonsterData
├── GetKillCount(string monsterType) → int
├── RegisterDamage(int amount, MonsterData target)
└── OnMonsterKilled (event)

Quest System Usage:
├── KillObjective → Subscribe to OnMonsterKilled
├── Combat quests → Track kill counts
└── Special quests → Check weapon used

Event Structure:
    OnMonsterKilled(MonsterKilledEvent {
        monsterId: "slime_blue_01",
        monsterType: "slime",
        weaponUsed: WeaponType.Sword,
        location: Vector3,
        overkill: bool
    })
```

### 12.4. NPC/Dialogue System Integration

**Interface Contract:**

```
IDialogueSystem
├── StartDialogue(DialogueTree tree)
├── GetCurrentNode() → DialogueNode
├── SelectChoice(int choiceIndex)
├── EndDialogue()
└── OnDialogueEvent (event)

Quest System Usage:
├── NPCQuestGiver → Trigger dialogue trees
├── Quest offer → Show quest details in dialogue
├── Quest turn-in → Show completion dialogue
└── TalkToObjective → Track dialogue completion

DialogueNode Quest Extensions:
├── questOfferId (string) // Show quest offer
├── questTurnInId (string) // Allow turn in
├── questProgressCheckId (string) // Show progress
└── onNodeComplete → TriggerObjectiveUpdate
```

### 12.5. Time System Integration

**Interface Contract:**

```
ITimeSystem
├── GetCurrentGameTime() → GameTime
├── GetCurrentHour() → int
├── GetCurrentDay() → int
├── GetCurrentSeason() → Season
└── OnHourChanged (event)

Quest System Usage:
├── Daily reset → Check hour == resetHour
├── Timed quests → Track time remaining
├── Season quests → Filter by current season
└── Time-limited objectives → Check deadlines

GameTime Structure:
    GameTime {
        day: int,
        hour: int,
        minute: int,
        season: Season,
        year: int
    }
```

### 12.6. Shop/Vendor System Integration

**Interface Contract:**

```
IShopSystem
├── BuyItem(ItemSO item, int count, string vendorId) → bool
├── SellItem(ItemSO item, int count, string vendorId) → bool
├── GetVendorStock(string vendorId) → List<VendorItem>
├── UnlockItem(string vendorId, ItemSO item)
└── OnPurchaseEvent (event)

Quest System Usage:
├── BuyObjective → Track OnPurchaseEvent
├── SellObjective → Track OnSellEvent
├── Vendor quests → Unlock shop items as rewards
└── Reputation → Modify prices

Integration with Reputation:
    Get base price → Apply reputation discount

    float GetFinalPrice(ItemSO item, string vendorId) {
        int basePrice = item.buyPrice;
        int reputation = ReputationManager.Get(vendorId);
        float discount = GetDiscountFor(reputation);
        return basePrice * (1 - discount);
    }
```

---

## 13. SCALABILITY & EXTENSION

### 13.1. Adding New Quest Types

**Steps:**

```
1. Create New Quest Data Type
   ├── Inherit from QuestData
   ├── Add type-specific fields
   └── Create ScriptableObject menu

2. Create New Objective Type
   ├── Inherit from QuestObjective
   ├── Implement abstract methods
   └── Register with ObjectiveFactory

3. Create Tracker (if needed)
   ├── Subscribe to relevant game events
   ├── Implement tracking logic
   └── Register with ObjectiveTracker

4. Update UI (if needed)
   ├── Add specific UI elements
   ├── Create progress displays
   └── Add to QuestUI factory

5. Test & Balance
   ├── Create test quests
   ├── Verify save/load
   └── Balance rewards
```

**Example: Adding "Taming Quest":**

```
TamingQuestData : QuestData
├── targetAnimalType (AnimalType)
├── tamingMethod (TamingMethod enum)
└── requiredItems (List<ItemSO>)

TameAnimalObjective : QuestObjective
├── targetAnimal (string)
├── targetCount (int)
└── mustBeFriendly (bool)

TamingTracker
├── Subscribe to OnAnimalTamed event
├── Update TameAnimalObjective
└── Validate taming conditions
```

### 13.2. Plugin Architecture

**Purpose:** Allow external systems to extend quest functionality

```
IQuestPlugin
├── PluginName { get; }
├── Initialize(QuestManager manager)
├── OnQuestStarted(QuestData quest)
├── OnQuestCompleted(QuestData quest)
└── Shutdown()

QuestPluginManager
├── plugins (List<IQuestPlugin>)
├── RegisterPlugin(IQuestPlugin)
├── UnregisterPlugin(string name)
└── NotifyPlugins(QuestEvent event)

Example Plugins:
├── AchievementPlugin
│   └── Award achievements for quest milestones
│
├── StatisticsPlugin
│   └── Track quest completion stats
│
├── LeaderboardPlugin
│   └── Post quest scores
│
└── AnalyticsPlugin
    └── Send quest data to analytics
```

### 13.3. Mod Support Considerations

**Design for Moddability:**

```
QuestModLoader
├── LoadCustomQuests(string modPath)
├── ValidateQuestData(QuestData)
├── RegisterCustomObjectives(Type objectiveType)
└── UnloadMod(string modId)

Modding API:
├── Quest creation via ScriptableObject
├── Custom objectives via C# scripts
├── Custom rewards via IRewardHandler
├── Custom validators via IObjectiveValidator
└── Asset bundles for icons/UI

Mod Structure:
MyQuestMod/
├── Quests/
│   ├── my_quest_1.asset
│   └── my_quest_2.asset
├── Scripts/
│   └── CustomObjective.cs
├── Assets/
│   └── Icons/
└── mod_info.json
```

### 13.4. Localization Support

**Architecture:**

```
LocalizedString
├── key (string)
├── defaultText (string)
└── Get(Language) → string

LocalizationManager
├── currentLanguage (Language enum)
├── LoadLanguage(Language)
├── GetText(string key) → string
└── OnLanguageChanged (event)

Quest Localization:
├── Quest names → "quest.{questId}.name"
├── Quest descriptions → "quest.{questId}.desc"
├── Objective text → "objective.{objId}.text"
└── Dialogue → "dialogue.{npcId}.{nodeId}"

Language Files (JSON):
{
  "quest.martin_story_01.name": "First Lesson",
  "quest.martin_story_01.desc": "Learn farming basics",
  "objective.harvest_carrot.text": "Harvest {0} carrots"
}
```

---

## 14. PERFORMANCE CONSIDERATIONS

### 14.1. Optimization Strategies

**1. Object Pooling:**

```
QuestInstancePool
├── pool (Queue<QuestInstance>)
├── active (HashSet<QuestInstance>)
├── Get() → QuestInstance
└── Return(QuestInstance)

Benefits:
├── Reduce GC pressure
├── Faster quest activation
└── Controlled memory usage
```

**2. Lazy Loading:**

```
Quest Data Loading:
├── Load core quests at startup
├── Load NPC quests when area entered
├── Load daily quests when NPC opened
└── Unload completed quest instances

QuestDataCache
├── loadedQuests (Dictionary<string, QuestData>)
├── Load(string questId) → QuestData
├── Unload(string questId)
└── Preload(List<string> questIds)
```

**3. Event Batching:**

```
Instead of:
    Every item pickup → Dispatch event → Update 10 quests

Use:
    Collect events for 1 frame → Batch dispatch → Update quests once

EventBatcher
├── pendingEvents (List<GameEvent>)
├── QueueEvent(GameEvent)
├── FlushEvents() // Called once per frame
└── ProcessBatch(List<GameEvent>)
```

**4. Objective Filtering:**

```
Only check relevant objectives:

ObjectiveTracker
├── objectivesByType (Dictionary<ObjectiveType, List<ObjectiveInstance>>)
├── OnInventoryChanged(event)
│   └── Only check CollectObjective, not ALL objectives
└── OnMonsterKilled(event)
    └── Only check KillObjective

Speedup: O(n) → O(k) where k << n
```

**5. Progress Caching:**

```
Cache frequently-queried data:

QuestProgressCache
├── progressByQuest (Dictionary<string, float>)
├── completionStatus (Dictionary<string, bool>)
├── Invalidate(string questId) // When progress updates
└── Rebuild() // Periodic refresh

UI queries cache instead of live calculation
```

### 14.2. Memory Management

**Limits:**

```
QuestSystemConfig
├── maxActiveQuests (int) = 10
├── maxDailyQuests (int) = 5
├── maxQuestHistory (int) = 100
├── objectiveTrackingLimit (int) = 30
└── eventQueueSize (int) = 100

Cleanup Strategy:
├── Archive old completed quests
├── Unload distant NPC quests
├── Limit quest history size
└── Clear event queue regularly
```

**Profiling Points:**

```
Key Areas to Monitor:
├── QuestManager.Update() frame time
├── ObjectiveTracker event processing time
├── Quest save/load time
├── Memory allocated for quest instances
└── Event queue size
```

### 14.3. Scalability Targets

**Performance Targets:**

```
Target Specs:
├── 100+ quests in database: OK
├── 10 active quests simultaneously: < 0.5ms/frame
├── 50 objectives being tracked: < 1ms/frame
├── Quest save/load: < 100ms
├── Daily quest generation: < 50ms
└── Memory footprint: < 10MB
```

**Stress Test Scenarios:**

```
1. Spam item pickup (1000 items/sec)
   → Event batching should handle

2. Complete 10 quests simultaneously
   → Reward processing should not block

3. Load save with 50 completed quests
   → Load time should be acceptable

4. Have 100 active objectives
   → Filtering should keep update fast
```

---

## 15. TESTING STRATEGY

### 15.1. Unit Tests

**Test Coverage:**

```
QuestManager Tests:
├── CanStartQuest_ValidQuest_ReturnsTrue()
├── CanStartQuest_MissingPrereq_ReturnsFalse()
├── StartQuest_ValidQuest_AddsToActive()
├── CompleteQuest_GivesCorrectRewards()
└── SaveLoad_PreservesState()

QuestValidator Tests:
├── ValidatePrerequisites_AllMet_ReturnsValid()
├── ValidateLevel_TooLow_ReturnsInvalid()
├── ValidateInventory_NoSpace_ReturnsInvalid()

ObjectiveTracker Tests:
├── UpdateProgress_CorrectEvent_IncrementsProgress()
├── UpdateProgress_WrongEvent_NoChange()
├── CheckCompletion_AllDone_ReturnsTrue()

RewardProcessor Tests:
├── GiveGoldReward_AddsToWallet()
├── GiveItemReward_NoSpace_ReturnsFalse()
├── RollRandomReward_RespectsDropChance()
```

### 15.2. Integration Tests

**Scenarios:**

```
Full Quest Flow Test:
1. Start quest
2. Complete objectives
3. Turn in quest
4. Receive rewards
5. Verify state

Multi-Quest Test:
1. Start 3 quests simultaneously
2. Complete in different orders
3. Verify no interference

Daily Quest Test:
1. Generate daily quests
2. Complete one
3. Advance time
4. Verify reset
```

### 15.3. Debug Tools

**In-Game Debug UI:**

```
QuestDebugPanel
├── List all active quests
├── Force complete quest
├── Reset quest
├── Add quest without prerequisites
├── Modify objective progress
├── Trigger daily reset
└── View quest history

QuestLogger
├── Log all quest events
├── Log objective updates
├── Log reward distribution
└── Export to file
```

---

## 16. IMPLEMENTATION ROADMAP

### Phase 1: Core Foundation (Week 1-2)
```
├── QuestData ScriptableObject
├── QuestManager singleton
├── Basic quest lifecycle (start/complete)
├── Simple objective types (Collect, Kill, TalkTo)
└── Event system foundation
```

### Phase 2: Objective System (Week 3)
```
├── ObjectiveTracker
├── All objective types implemented
├── Event integration (Inventory, Combat, NPC)
└── Progress tracking UI
```

### Phase 3: Reward System (Week 4)
```
├── RewardProcessor
├── All reward handlers
├── Random reward tables
└── Reward UI/VFX
```

### Phase 4: NPC Integration (Week 5)
```
├── NPCQuestGiver component
├── Reputation system
├── Dialogue integration
└── Vendor quest types
```

### Phase 5: Daily Quests (Week 6)
```
├── DailyQuestManager
├── Quest pool system
├── Daily reset logic
└── Daily quest UI
```

### Phase 6: Story Quests (Week 7)
```
├── Quest chains
├── Branching logic
├── Story progression tracking
└── Cutscene triggers
```

### Phase 7: Save/Load (Week 8)
```
├── Quest save data structures
├── Serialization
├── Load validation
└── Migration support
```

### Phase 8: Polish & Testing (Week 9-10)
```
├── Bug fixes
├── Performance optimization
├── Balance tuning
├── UI polish
└── Documentation
```

---

## 17. TÀI LIỆU THAM KHẢO

**Code Templates:**
- `Assets/_Scripts/Quests/Templates/`

**Architecture Diagrams:**
- `Docs/Architecture/quest_system_flow.png`

**Sample Quests:**
- `Assets/GameData/Quests/Samples/`

**Test Scenarios:**
- `Tests/QuestSystem/Scenarios/`

**API Documentation:**
- Auto-generated XML docs
- `Docs/API/quest_system_api.html`

---

**Phiên bản:** 1.0
**Ngày:** 2025-12-06
**Tác giả:** Technical Design Team
