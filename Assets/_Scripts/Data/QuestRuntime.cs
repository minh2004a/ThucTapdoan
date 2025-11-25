using System;
using UnityEngine;

public enum QuestState
{
    NotAccepted,          // chưa nhận
    InProgress,           // đã nhận nhưng chưa đủ đồ
    ReadyToTurnIn,        // đủ đồ, có thể trả
    Completed             // đã nhận thưởng
}

[Serializable]
public class QuestInstance
{
    public QuestData data;
    public QuestState state = QuestState.NotAccepted;

    // Nếu sau này có quest kiểu giết quái / thu thập từ nhiều nguồn,
    // có thể dùng currentAmount để track tiến độ.
    public int currentAmount;

    public string Id => data != null ? data.id : string.Empty;
}
