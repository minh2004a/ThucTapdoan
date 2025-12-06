# HỆ THỐNG NHIỆM VỤ - THIẾT KẾ CHI TIẾT

## 1. TỔNG QUAN HỆ THỐNG

Hệ thống nhiệm vụ tích hợp với các hệ thống hiện có: Economy, Shop, Inventory, Farming, Mining để tạo vòng gameplay liên tục khuyến khích người chơi tương tác với NPC và shop.

---

## 2. PHÂN LOẠI NHIỆM VỤ

### 2.1. Nhiệm Vụ Chính (Story Quest)
**Mục đích:** Hướng dẫn cốt truyện chính, mở khóa các khu vực/tính năng mới

**Đặc điểm:**
- Chỉ làm 1 lần
- Thưởng lớn (tiền + vật phẩm hiếm)
- Yêu cầu hoàn thành tuần tự
- Mở khóa NPC/khu vực mới

**Vai trò kinh tế:**
- Cung cấp vốn ban đầu cho người chơi
- Giới thiệu hệ thống mua bán
- Mở khóa vendor mới → mở rộng danh mục shop

### 2.2. Nhiệm Vụ Phụ (Side Quest)
**Mục đích:** Nội dung bổ sung, khám phá nhân vật NPC

**Đặc điểm:**
- Làm 1 lần, không bắt buộc
- Thưởng vừa phải
- Có thể làm theo thứ tự tự do
- Mở khóa công thức/trang bị đặc biệt

**Vai trò kinh tế:**
- Tạo nhu cầu mua nguyên liệu từ shop
- Thưởng công thức chế tạo → khuyến khích mua vật liệu
- Tăng uy tín với vendor → giảm giá

### 2.3. Nhiệm Vụ Hàng Ngày (Daily Quest)
**Mục đích:** Tạo mục tiêu ngắn hạn, khuyến khích chơi đều

**Đặc điểm:**
- Reset mỗi ngày
- Thưởng nhỏ nhưng ổn định
- Dễ hoàn thành (10-15 phút)
- 3-5 nhiệm vụ/ngày

**Vai trò kinh tế:**
- Nguồn thu nhập ổn định hàng ngày
- Khuyến khích mua công cụ/consumable
- Tạo chu kỳ: làm quest → nhận tiền → mua đồ → làm quest

### 2.4. Nhiệm Vụ Đặc Biệt (Special Quest)
**Mục đích:** Liên quan hoạt động farming/mining, khuyến khích chuyên môn hóa

#### A. Nhiệm Vụ Canh Tác (Farming Quest)
**Đặc điểm:**
- Trồng/thu hoạch số lượng cây cụ thể
- Thưởng: hạt giống hiếm, phân bón, công cụ farming

**Vai trò kinh tế:**
- Tạo nhu cầu mua hạt giống từ Seed Vendor
- Khuyến khích nâng cấp công cụ (hoe, watering can, scythe)
- Sản phẩm farm → bán cho vendor → mua hạt mới

#### B. Nhiệm Vụ Khai Thác (Mining Quest)
**Đặc điểm:**
- Đào khoáng sản, phá đá
- Thưởng: công thức chế tạo, pickaxe tốt hơn

**Vai trò kinh tế:**
- Tạo nhu cầu mua/nâng cấp pickaxe
- Khoáng sản → bán hoặc dùng chế tạo
- Khuyến khích mua consumable (HP/stamina potion)

---

## 3. VÒNG KINH TẾ VÀ HỆ THỐNG MUA BÁN

### 3.1. Chu Trình Kinh Tế Cơ Bản
```
QUEST → Nhận Nhiệm Vụ
  ↓
MUA ĐỒ → Chuẩn bị (công cụ, hạt giống, consumable)
  ↓
HOÀN THÀNH → Thu thập nguyên liệu/sản phẩm
  ↓
BÁN ĐỒ → Bán sản phẩm phụ cho vendor
  ↓
NHẬN THƯỞNG → Tiền + vật phẩm từ quest
  ↓
NÂNG CẤP → Mua trang bị tốt hơn
  ↓
QUEST MỚI → Nhiệm vụ khó hơn
```

### 3.2. Cơ Chế Khuyến Khích Mua Bán

#### Loại 1: Yêu Cầu Mua Nguyên Liệu
```
VD: "Mang Đá Thương Cho Tôi"
- Yêu cầu: 10 Stone (khoáng sản)
- Người chơi KHÔNG đủ → phải đi đào hoặc MUA từ vendor
- Nếu mua: cần 10 x buyPrice
- Thưởng: 500g + Iron Ore x5
```

#### Loại 2: Tiêu Hao Công Cụ
```
VD: "Thu Hoạch 50 Cà Rốt"
- Yêu cầu: Hoe (xới đất), Watering Can (tưới), Scythe (thu hoạch)
- Công cụ hỏng → phải MUA/SỬA ở Equipment Vendor
- Tạo nhu cầu mua công cụ backup
```

#### Loại 3: Consumable Requirement
```
VD: "Khám Phá Hang Sâu"
- Khu vực nguy hiểm → cần HP potion, stamina food
- Người chơi BẮT BUỘC mua consumable từ vendor
- Thưởng > chi phí → tạo lợi nhuận
```

#### Loại 4: Craft Requirement
```
VD: "Chế Tạo Kiếm Sắt"
- Yêu cầu: 5 Iron Bar + 2 Wood
- Người chơi thiếu nguyên liệu → MUA từ vendor
- Hoặc làm mining quest → lấy iron ore → đến NPC chế tạo (mất phí)
```

### 3.3. Hệ Thống Uy Tín Vendor (Reputation)
```
Level 0 (Stranger): Giá gốc
Level 1 (Acquaintance): -5% giá mua, +5% giá bán (sau 3 quest)
Level 2 (Friend): -10% giá mua, +10% giá bán (sau 7 quest)
Level 3 (Best Friend): -15% giá mua, +15% giá bán, mở khóa vật phẩm đặc biệt (sau 15 quest)
```

**Tác động:**
- Khuyến khích làm nhiều quest cho cùng vendor
- Tạo động lực mua bán lâu dài
- Vendor khác nhau → uy tín riêng → đa dạng hóa gameplay

---

## 4. BẢNG ÁNH XẠ NPC - LOẠI NHIỆM VỤ

### 4.1. Hướng Dẫn Thiết Kế NPC

**Nguyên tắc:**
1. Mỗi NPC chuyên về 1 lĩnh vực (farming/mining/combat/crafting)
2. NPC = Vendor + Quest Giver
3. Quest của NPC liên quan đến hàng họ bán → tạo nhu cầu mua

### 4.2. Bảng Ánh Xạ Chi Tiết

| NPC | Loại Vendor | Hàng Bán | Loại Quest | Mục Đích Quest | Vai Trò Kinh Tế |
|-----|-------------|----------|------------|----------------|-----------------|
| **Ông Martin** (Nông dân già) | Seed Vendor | - Hạt giống mùa<br>- Phân bón<br>- Hoe, Watering Can | - Story Quest: Hướng dẫn farming<br>- Daily: Trồng/thu hoạch<br>- Special: Farming challenges | - Giới thiệu hệ thống farming<br>- Khuyến khích trồng trọt | - Bán hạt giống<br>- Mua farm product<br>- Tạo chu kỳ: mua hạt → trồng → bán sản phẩm |
| **Bà Rosa** (Đầu bếp) | Food Vendor | - Consumable (HP/Stamina food)<br>- Công thức nấu ăn | - Daily: Giao thực phẩm<br>- Side: Tìm nguyên liệu hiếm<br>- Repeatable: Đổi sản phẩm farm lấy consumable | - Tạo nhu cầu consumable<br>- Giá trị hóa farm product | - Mua farm product giá cao<br>- Bán consumable<br>- Động lực farm để đổi đồ ăn |
| **Thợ Rèn Kane** | Equipment Vendor | - Weapon, Tool, Armor<br>- Khoáng sản (Iron Bar, etc.)<br>- Upgrade service | - Story: Mở khóa crafting<br>- Side: Thu thập khoáng sản<br>- Daily: Chế tạo vật phẩm | - Khuyến khích mining<br>- Nâng cấp trang bị | - Bán công cụ/vũ khí<br>- Mua khoáng sản<br>- Thu phí craft/upgrade |
| **Cô Lily** (Nhà thám hiểm) | General Vendor | - Đa dạng (tool, consumable, seeds)<br>- Backpack, Ring | - Story: Khám phá khu vực mới<br>- Daily: Thu thập items<br>- Special: Challenge quest | - Mở rộng bản đồ<br>- Thử thách người chơi | - Vendor tổng hợp<br>- Mua mọi loại item<br>- Giá trung bình |
| **Cụ già Biển Wise** (Fisherman) | Fish/Boat Vendor | - Cần câu<br>- Mồi câu<br>- Thuyền/lưới | - Daily: Câu cá<br>- Special: Câu cá hiếm<br>- Side: Khám phá biển | - Thêm hoạt động fishing<br>- Đa dạng hóa thu nhập | - Bán đồ câu cá<br>- Mua cá giá cao<br>- Mở khóa fishing system |
| **Phù Thủy Morgana** (Witch) | Magic Vendor | - Potion đặc biệt<br>- Magic ring/amulet<br>- Rare seed | - Side: Thu thập rare material<br>- Repeatable: Đổi item hiếm<br>- Special: Boss quest | - Late-game content<br>- Trang bị hiếm | - Giá cao<br>- Vật phẩm mạnh<br>- Yêu cầu uy tín level 2+ |

### 4.3. Ví Dụ Quest Theo NPC

#### **Ông Martin - Farmer Vendor**

**[STORY-01] Bài Học Đầu Tiên**
```yaml
ID: MARTIN_STORY_01
Tên: "Bài Học Đầu Tiên"
NPC: Ông Martin
Loại: Story Quest
Yêu cầu: Không
Mục tiêu:
  - Mua 5 Carrot Seed từ Martin (10g/hạt = 50g)
  - Trồng và thu hoạch 5 cây cà rốt
Thưởng:
  - 200g
  - Hoe x1
  - Watering Can x1
Đối thoại mở:
  - "Chào cháu! Cháu mới đến làng à? Ta là Martin, nông dân lâu năm ở đây."
  - "Nếu cháu muốn sinh sống ở đây, hãy học cách tự trồng thức ăn."
  - "Hãy mua ít hạt cà rốt từ ta và thử trồng xem sao!"
Đối thoại hoàn thành:
  - "Tuyệt vời! Cháu đã thu hoạch được cà rốt rồi!"
  - "Đây là tiền thưởng và bộ công cụ. Giữ gìn chúng cẩn thận nhé!"
Ghi chú: Mở khóa Daily Quest của Martin
```

**[DAILY-01] Nông Sản Hàng Ngày**
```yaml
ID: MARTIN_DAILY_01
Tên: "Nông Sản Hàng Ngày"
NPC: Ông Martin
Loại: Daily Quest (reset mỗi ngày)
Yêu cầu: Hoàn thành MARTIN_STORY_01
Mục tiêu:
  - Giao 10 Farm Product bất kỳ (Carrot/Wheat/Corn/etc.)
Thưởng:
  - 150g
  - 20% chance: Rare Seed x1
Kinh tế:
  - Khuyến khích trồng nhiều → mua hạt từ Martin
  - 10 sản phẩm có thể bán được ~100g → quest thêm 150g = lời 50g
```

#### **Thợ Rèn Kane - Equipment Vendor**

**[STORY-02] Vũ Khí Đầu Tay**
```yaml
ID: KANE_STORY_01
Tên: "Vũ Khí Đầu Tay"
NPC: Thợ Rèn Kane
Loại: Story Quest
Yêu cầu: Player level 3
Mục tiêu:
  - Thu thập 10 Stone (đào từ mine hoặc MUA từ Kane)
  - Giao cho Kane
Thưởng:
  - Iron Sword x1 (Dame 15)
  - 300g
  - Mở khóa: Crafting system
Đối thoại mở:
  - "Này! Cậu trông có vẻ khỏe mạnh đấy."
  - "Muốn vũ khí à? Không có gì miễn phí đâu!"
  - "Mang cho tôi 10 viên đá, tôi sẽ rèn cho cậu một thanh kiếm."
Đối thoại hoàn thành:
  - "Tốt lắm! Đây, kiếm của cậu đây."
  - "Từ giờ cậu có thể nhờ tôi chế tạo vũ khí nếu có đủ vật liệu."
Kinh tế:
  - Nếu chưa có Stone → phải mua (buyPrice: 5g/viên = 50g)
  - Hoặc đi mine → tốn stamina → cần consumable
  - Thưởng 300g + Iron Sword (giá trị ~400g) → lời
```

**[DAILY-02] Đơn Hàng Thợ Rèn**
```yaml
ID: KANE_DAILY_01
Tên: "Đơn Hàng Thợ Rèn"
NPC: Thợ Rèn Kane
Loại: Daily Quest
Yêu cầu: Hoàn thành KANE_STORY_01
Mục tiêu:
  - Chế tạo 1 Iron Bar (cần 5 Iron Ore)
  - Giao cho Kane
Thưởng:
  - 200g
  - 30% chance: Pickaxe durability restore
Kinh tế:
  - 5 Iron Ore: tự đào hoặc mua (15g/viên = 75g)
  - Phí chế tạo: 20g
  - Tổng chi phí: ~95g
  - Thưởng: 200g → lời 105g
  - Khuyến khích mining → hỏng pickaxe → mua/sửa từ Kane
```

#### **Bà Rosa - Food Vendor**

**[SIDE-01] Công Thức Bí Mật**
```yaml
ID: ROSA_SIDE_01
Tên: "Công Thức Bí Mật"
NPC: Bà Rosa
Loại: Side Quest
Yêu cầu: Friendship Level 1 với Rosa (3 daily quest)
Mục tiêu:
  - Mang 5 Mushroom (rare drop từ forest)
  - 3 Wheat
  - 2 Milk (mua từ vendor khác)
Thưởng:
  - Recipe: "Mushroom Soup" (HP +50, Stamina +30)
  - 400g
Đối thoại mở:
  - "À, cháu đến rồi! Ta có việc nhờ cháu đây."
  - "Ta muốn thử làm món súp nấm mới, nhưng thiếu nguyên liệu."
  - "Cháu có thể kiếm giúp ta không?"
Kinh tế:
  - Mushroom: phải tìm (khó) hoặc mua giá cao
  - Wheat: tự trồng hoặc mua
  - Milk: bắt buộc mua từ vendor khác (50g/chai)
  - Tổng chi phí: ~200g
  - Thưởng: Recipe (giá trị cao) + 400g
```

**[REPEATABLE-01] Trao Đổi Nông Sản**
```yaml
ID: ROSA_REPEAT_01
Tên: "Trao Đổi Nông Sản"
NPC: Bà Rosa
Loại: Repeatable (không giới hạn)
Yêu cầu: Không
Mục tiêu:
  - Đổi 10 Farm Product (Carrot/Corn/Wheat)
Thưởng:
  - HP Potion x3 (giá trị 150g)
  - Hoặc Stamina Food x5 (giá trị 200g)
Kinh tế:
  - 10 sản phẩm farm (giá bán ~100g)
  - Đổi lấy consumable (giá trị 150-200g)
  - Lợi nhuận: 50-100g tương đương
  - Khuyến khích farming → mua hạt từ Martin
```

---

## 5. MẪU CẤU TRÚC QUEST DATA

### 5.1. Quest Data Structure (C# ScriptableObject)

```csharp
[CreateAssetMenu(menuName = "Quest/Quest Data")]
public class QuestData : ScriptableObject
{
    [Header("Basic Info")]
    public string questID;
    public string questName;
    [TextArea(3, 5)] public string description;
    public QuestType questType;
    public string npcID; // NPC giao quest

    [Header("Requirements")]
    public int requiredPlayerLevel;
    public QuestData[] prerequisiteQuests; // Quest phải hoàn thành trước
    public int requiredReputation; // Uy tín với NPC

    [Header("Objectives")]
    public QuestObjective[] objectives;

    [Header("Rewards")]
    public int goldReward;
    public ItemReward[] itemRewards;
    public int experienceReward;
    public int reputationReward; // Tăng uy tín với NPC

    [Header("Dialogue")]
    [TextArea(2, 4)] public string[] dialogueStart;
    [TextArea(2, 4)] public string[] dialogueProgress;
    [TextArea(2, 4)] public string[] dialogueComplete;

    [Header("Settings")]
    public bool isRepeatable;
    public float resetHours; // Số giờ game để reset (24 = 1 ngày)
}

[System.Serializable]
public class QuestObjective
{
    public ObjectiveType type;
    public ItemSO targetItem; // Cho type = Collect/Craft
    public int targetAmount;
    public string targetEnemyID; // Cho type = Kill
    public string targetLocationID; // Cho type = GoTo

    [HideInInspector] public int currentProgress;
}

public enum QuestType
{
    Story,      // Nhiệm vụ chính
    Side,       // Nhiệm vụ phụ
    Daily,      // Hàng ngày
    Special     // Đặc biệt (farming/mining)
}

public enum ObjectiveType
{
    Collect,    // Thu thập vật phẩm
    Craft,      // Chế tạo vật phẩm
    Kill,       // Tiêu diệt kẻ địch
    GoTo,       // Đến địa điểm
    TalkTo,     // Nói chuyện với NPC
    Plant,      // Trồng cây
    Harvest,    // Thu hoạch
    Mine,       // Khai thác
    Buy,        // Mua vật phẩm từ shop
    Sell        // Bán vật phẩm
}

[System.Serializable]
public class ItemReward
{
    public ItemSO item;
    public int amount;
    [Range(0, 100)] public float dropChance; // % nhận được (100 = chắc chắn)
}
```

### 5.2. Quest Manager Script

```csharp
public class QuestManager : MonoBehaviour
{
    [SerializeField] List<QuestData> allQuests;

    // Active quests
    Dictionary<string, QuestProgress> activeQuests = new Dictionary<string, QuestProgress>();

    // Completed quests
    HashSet<string> completedQuests = new HashSet<string>();

    // NPC Reputation
    Dictionary<string, int> npcReputation = new Dictionary<string, int>();

    public void StartQuest(QuestData quest)
    {
        if (quest == null || activeQuests.ContainsKey(quest.questID)) return;
        if (completedQuests.Contains(quest.questID) && !quest.isRepeatable) return;

        // Kiểm tra yêu cầu
        if (!CanStartQuest(quest)) return;

        // Tạo quest progress
        QuestProgress progress = new QuestProgress(quest);
        activeQuests.Add(quest.questID, progress);

        // Hiển thị UI thông báo
        UIManager.Instance?.ShowQuestNotification($"Nhiệm vụ mới: {quest.questName}");
    }

    public void UpdateObjective(string questID, int objectiveIndex, int progress)
    {
        if (!activeQuests.TryGetValue(questID, out QuestProgress questProgress)) return;

        questProgress.UpdateObjective(objectiveIndex, progress);

        // Kiểm tra hoàn thành
        if (questProgress.IsComplete())
        {
            CompleteQuest(questProgress.questData);
        }
    }

    void CompleteQuest(QuestData quest)
    {
        // Gỡ khỏi active
        activeQuests.Remove(quest.questID);

        // Thêm vào completed
        if (!quest.isRepeatable)
        {
            completedQuests.Add(quest.questID);
        }

        // Trao thưởng
        GiveRewards(quest);

        // Tăng uy tín
        if (npcReputation.ContainsKey(quest.npcID))
            npcReputation[quest.npcID] += quest.reputationReward;
        else
            npcReputation[quest.npcID] = quest.reputationReward;

        // UI notification
        UIManager.Instance?.ShowQuestComplete(quest);
    }

    void GiveRewards(QuestData quest)
    {
        // Tiền
        EconomyManager.Instance?.AddMoney(quest.goldReward);

        // Vật phẩm
        PlayerInventory inv = PlayerInventory.Instance;
        foreach (var reward in quest.itemRewards)
        {
            float roll = Random.Range(0f, 100f);
            if (roll <= reward.dropChance)
            {
                inv?.AddItem(reward.item, reward.amount);
            }
        }

        // Exp (nếu có hệ thống level)
        // PlayerStats.Instance?.AddExp(quest.experienceReward);
    }

    bool CanStartQuest(QuestData quest)
    {
        // Check level
        // if (PlayerStats.Instance.Level < quest.requiredPlayerLevel) return false;

        // Check prerequisite quests
        foreach (var prereq in quest.prerequisiteQuests)
        {
            if (!completedQuests.Contains(prereq.questID)) return false;
        }

        // Check reputation
        if (npcReputation.TryGetValue(quest.npcID, out int rep))
        {
            if (rep < quest.requiredReputation) return false;
        }
        else if (quest.requiredReputation > 0)
        {
            return false;
        }

        return true;
    }

    public int GetReputation(string npcID)
    {
        return npcReputation.TryGetValue(npcID, out int rep) ? rep : 0;
    }
}

public class QuestProgress
{
    public QuestData questData;
    public int[] objectiveProgress;

    public QuestProgress(QuestData data)
    {
        questData = data;
        objectiveProgress = new int[data.objectives.Length];
    }

    public void UpdateObjective(int index, int progress)
    {
        if (index < 0 || index >= objectiveProgress.Length) return;
        objectiveProgress[index] = progress;
    }

    public bool IsComplete()
    {
        for (int i = 0; i < questData.objectives.Length; i++)
        {
            if (objectiveProgress[i] < questData.objectives[i].targetAmount)
                return false;
        }
        return true;
    }
}
```

---

## 6. MẪU UI DIALOGUE

### 6.1. Vendor Quest UI Layout

```
┌─────────────────────────────────────────┐
│  [ICON] Ông Martin - Nông Dân           │
│  Uy tín: ★★☆☆☆ (Friend Level 1)        │
├─────────────────────────────────────────┤
│  [Tab: SHOP] [Tab: QUEST*]              │
├─────────────────────────────────────────┤
│  ┌───────────────────────────────────┐  │
│  │ NHIỆM VỤ ĐANG HOẠT ĐỘNG          │  │
│  ├───────────────────────────────────┤  │
│  │ [!] Nông Sản Hàng Ngày            │  │
│  │     Tiến độ: 3/10 Farm Product    │  │
│  │     Thưởng: 150g + Rare Seed?     │  │
│  │     [NỘP] [HUỶ]                   │  │
│  └───────────────────────────────────┘  │
│                                          │
│  ┌───────────────────────────────────┐  │
│  │ NHIỆM VỤ MỚI                      │  │
│  ├───────────────────────────────────┤  │
│  │ [+] Thử Thách Mùa Thu              │  │
│  │     Yêu cầu: Friend Level 2        │  │
│  │     Mục tiêu: Thu hoạch 50 Wheat   │  │
│  │     Thưởng: 500g + Recipe          │  │
│  │     [NHẬN] [CHI TIẾT]              │  │
│  └───────────────────────────────────┘  │
│                                          │
│  [ĐÓNG]                                 │
└─────────────────────────────────────────┘
```

### 6.2. Đoạn Dialogue Mẫu - Quest Start

**Scene: Người chơi click vào NPC Ông Martin**

```
[UI hiện ảnh Ông Martin, text box xuất hiện dần]

Martin: "Chào cháu! Hôm nay trông cháu khỏe mạnh đấy."

[Lựa chọn xuất hiện]
→ "Chào ông, có việc gì cần giúp không ạ?"
→ "Cho xem hàng hóa" [→ Mở Shop]
→ "Tạm biệt"

[Nếu chọn option 1]

Martin: "À, thật ra ông đang cần người giúp đỡ."
Martin: "Mùa thu đang đến, ông cần thu hoạch lúa mì trước khi trời lạnh."
Martin: "Cháu có thể giúp ông thu 20 Wheat không?"

[Hiện thông tin quest]
╔════════════════════════════════════╗
║ NHIỆM VỤ: Mùa Thu Bận Rộn         ║
║ NPC: Ông Martin                    ║
║ Loại: Daily Quest                  ║
║────────────────────────────────────║
║ Mục tiêu:                          ║
║ • Thu hoạch 20 Wheat (0/20)        ║
║────────────────────────────────────║
║ Thưởng:                            ║
║ • 200 Gold                         ║
║ • Wheat Seed x10                   ║
║ • +5 Reputation với Martin         ║
╚════════════════════════════════════╝

[Lựa chọn]
→ "Được, để cháu giúp ông!" [NHẬN QUEST]
→ "Xin lỗi, cháu bận rồi" [TỪ CHỐI]

[Nếu nhận quest]

Martin: "Cảm ơn cháu! Ông tin tưởng cháu lắm đấy."
Martin: "Nhớ quay lại khi hoàn thành nhé!"

[UI thông báo]
✓ Đã nhận nhiệm vụ: "Mùa Thu Bận Rộn"
```

### 6.3. Dialogue Mẫu - Quest In Progress

**Scene: Người chơi quay lại Martin khi chưa hoàn thành**

```
Martin: "Cháu quay lại rồi à?"

[Kiểm tra tiến độ: 8/20 Wheat]

Martin: "Hừm... cháu vẫn chưa đủ số lúa mì mà ông cần."
Martin: "Cháu còn thiếu 12 bó nữa. Cố gắng lên nhé!"

[Lựa chọn]
→ "Vâng, cháu sẽ cố gắng" [ĐÓNG]
→ "Ông có thể cho cháu hạt giống không?" [→ MỞ SHOP]
→ "Cháu muốn bỏ cuộc" [HUỶ QUEST - warning]
```

### 6.4. Dialogue Mẫu - Quest Complete

**Scene: Người chơi hoàn thành quest và quay lại**

```
[Người chơi click Martin, game tự động detect quest hoàn thành]

Martin: "Ồ! Cháu đã thu đủ 20 bó lúa mì rồi sao?"

[Auto check inventory: có đủ 20 Wheat]

Martin: "Tuyệt vời! Cháu thật sự giúp ông một việc lớn đấy!"

[Animation: Items bay từ inventory vào NPC]
[Animation: NPC tỏa sáng, rewards bay ra]

[UI thông báo]
╔════════════════════════════════════╗
║ ✓ HOÀN THÀNH NHIỆM VỤ!             ║
║ "Mùa Thu Bận Rộn"                  ║
║────────────────────────────────────║
║ Phần thưởng nhận được:             ║
║ • +200 Gold                        ║
║ • Wheat Seed x10                   ║
║ • +5 Reputation với Martin         ║
║                                    ║
║ Uy tín mới: ★★☆☆☆ (Level 1)       ║
╚════════════════════════════════════╝

Martin: "Đây là tiền công và hạt giống cho cháu."
Martin: "Cứ đến gặp ông mỗi ngày, luôn có việc làm cho cháu đấy!"

[Lựa chọn]
→ "Cảm ơn ông!" [ĐÓNG]
→ "Có nhiệm vụ nào khác không?" [XEM QUEST LIST]
```

### 6.5. Dialogue Mẫu - Shop Integration

**Scene: Người chơi thiếu nguyên liệu cho quest**

```
[Quest: "Chế Tạo Kiếm Sắt" - Yêu cầu: 5 Iron Ore]
[Người chơi chỉ có 2 Iron Ore]

Kane: "Cậu cần 5 Iron Ore để tôi rèn kiếm."
Kane: "Tôi thấy cậu chỉ có 2 thôi."

[Lựa chọn]
→ "Tôi có thể mua Iron Ore từ anh không?" [AUTO MỞ SHOP TAB - HIGHLIGHT IRON ORE]
→ "Để tôi đi đào thêm" [ĐÓNG]

[Nếu chọn option 1]

[UI Shop mở, Iron Ore được highlight]
╔════════════════════════════════════╗
║ KHO CỦA KANE                        ║
║ Tab: [MUA*] [BÁN]                   ║
║────────────────────────────────────║
║ ┌──────────────────────────────┐   ║
║ │ [IMG] Iron Ore      15g/viên │   ║
║ │ "Quặng sắt cơ bản"           │   ║
║ │                              │   ║
║ │ Cần mua: 3 viên (45g)        │ ◄─ Gợi ý tự động
║ │ [MUA 1] [MUA 3*] [MUA 5]     │   ║
║ └──────────────────────────────┘   ║
║                                     ║
║ Ví của bạn: 120g                   ║
╚════════════════════════════════════╝

[Sau khi mua]

Kane: "Tốt! Bây giờ cậu đã có đủ 5 Iron Ore rồi."
Kane: "Có muốn tôi rèn ngay không?"

→ "Vâng, làm ơn!" [COMPLETE QUEST]
→ "Để sau vậy" [ĐÓNG]
```

---

## 7. CHECKLIST TRIỂN KHAI

### Phase 1: Core System (Tuần 1-2)
- [ ] Tạo QuestData ScriptableObject
- [ ] Viết QuestManager script
- [ ] Tích hợp với Inventory/Economy hiện có
- [ ] Tạo 3 quest mẫu (Story/Daily/Side)

### Phase 2: UI (Tuần 3)
- [ ] Thiết kế Quest UI (list, detail, notification)
- [ ] Dialogue system (textbox, choices)
- [ ] Integration với VendorShopUI (thêm Quest tab)
- [ ] Quest tracker (HUD hiện progress)

### Phase 3: NPC Integration (Tuần 4)
- [ ] Thêm Quest Giver component cho NPC
- [ ] Reputation system
- [ ] Dynamic shop prices theo reputation
- [ ] NPC dialogue manager

### Phase 4: Content Creation (Tuần 5-6)
- [ ] Tạo 5 NPC với vendor + quest
- [ ] Viết 20+ quest (đầy đủ các loại)
- [ ] Viết dialogue cho tất cả quest
- [ ] Balance rewards/costs

### Phase 5: Testing & Polish (Tuần 7)
- [ ] Playtest economy loop
- [ ] Balance adjustments
- [ ] Bug fixes
- [ ] Sound/VFX cho quest complete

---

## 8. LƯU Ý KỸ THUẬT

1. **Quest Tracking:**
   - Lưu quest progress vào Save System
   - Reset daily quest vào 6h sáng (game time)
   - Track completed quest IDs để không lặp lại

2. **Economy Balance:**
   - Quest reward > cost để mua nguyên liệu
   - Tỷ lệ: Thưởng = Chi phí x 1.5 - 2.0
   - Daily quest: Nguồn thu chính (~200-300g/ngày)

3. **Performance:**
   - Cache quest data khi game start
   - Update objective chỉ khi cần (event-based)
   - Limit số active quest (max 5)

4. **Localization:**
   - Tách riêng text dialogue ra file JSON/CSV
   - Dễ dàng dịch sang ngôn ngữ khác
   - Hiện tại: Vietnamese

---

## 9. TÀI LIỆU THAM KHẢO

**Mẫu Quest Design Document:**
- File này là template, copy và edit cho mỗi quest mới
- Lưu tại: `Assets/GameDesign/Quests/`

**Script References:**
- QuestData.cs: `Assets/_Scripts/Data/QuestData.cs`
- QuestManager.cs: `Assets/_Scripts/Data/QuestManager.cs`
- VendorQuestUI.cs: `Assets/_Scripts/UI/VendorQuestUI.cs`

**Asset References:**
- Quest Icons: `Assets/UI/Icons/Quests/`
- NPC Portraits: `Assets/Texture2D/Character and Portrait/`
- Dialogue Box: `Assets/UI/DialogueSystem/`

---

**Phiên bản:** 1.0
**Ngày cập nhật:** 2025-12-06
**Người viết:** Game Design Team
