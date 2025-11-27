using System;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [Serializable]
    public class DialogueEntry
    {
        [Header("Điều kiện")]
        public QuestData requiredQuest;  // có thể để trống
        public QuestState minState = QuestState.NotAccepted; // mặc định từ NotAccepted
        public QuestState maxState = QuestState.Completed; // đến Completed

        [Header("Nội dung thoại")]
        [TextArea] public string text;
    }

    [Header("Danh sách thoại theo điều kiện")]
    [SerializeField] List<DialogueEntry> entries = new();

    [Header("UI thoại")]
    [SerializeField] GameObject dialogueRoot;      // panel giống dùng cho trưởng làng
    [SerializeField] TypewriterText dialogueUI;    // TypewriterText trên DialogueText

    void Awake()
    {
        if (!dialogueUI)
            dialogueUI = FindObjectOfType<TypewriterText>(true);

        if (!dialogueRoot && dialogueUI)
            dialogueRoot = dialogueUI.transform.parent.gameObject;
    }

    DialogueEntry PickEntry()
    {
        if (entries == null || entries.Count == 0) return null;

        var qm = QuestManager.Instance;

        foreach (var e in entries)
        {
            if (e == null) continue;

            // Không cần quest → luôn match, dùng làm default
            if (!e.requiredQuest)
                return e;

            if (!qm) continue;

            if (!qm.GetState(e.requiredQuest, out var st))
                st = QuestState.NotAccepted;

            // Nếu state nằm trong [min, max] → dùng entry này
            if (st >= e.minState && st <= e.maxState)
                return e;
        }

        return null;
    }

    void ShowText(string text)
    {
        if (!dialogueUI) return;

        if (dialogueRoot && !dialogueRoot.activeSelf)
            dialogueRoot.SetActive(true);

        if (!dialogueUI.gameObject.activeSelf)
            dialogueUI.gameObject.SetActive(true);

        dialogueUI.ShowText(text);
    }

    // Hàm này sẽ được gọi khi player tương tác NPC
    public void Interact()
    {
        var entry = PickEntry();
        if (entry == null || string.IsNullOrWhiteSpace(entry.text))
            return;

        ShowText(entry.text);
    }
}
