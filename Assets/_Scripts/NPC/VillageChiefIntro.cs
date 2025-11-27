using UnityEngine;

public class VillageChiefIntro : MonoBehaviour
{
    [Header("Quest mở khoá làng")]
    [SerializeField] QuestData villageIntroQuest;   // gán Q_GapTruongLang
    [Header("Lock player")]
    [SerializeField] PlayerController player;
    [SerializeField] BookToggle bookToggle;
    [Header("UI thoại")]
    [Tooltip("Panel gốc chứa text (GameObject có thể bật/tắt)")]
    [SerializeField] GameObject dialogueRoot;

    [Tooltip("TypewriterText dùng để gõ chữ")]
    [SerializeField] TypewriterText dialogueUI;

    [TextArea]
    [SerializeField] string firstTalkText =
        "Chào con, chào mừng đến ngôi làng này. Hãy đi làm quen với mọi người nhé!";

    [TextArea]
    [SerializeField] string repeatText =
        "Hãy tiếp tục làm quen với dân làng và giúp đỡ họ nhé.";

   void Awake()
    {
        // Auto-find nếu em lười gán tay
        if (!dialogueUI)
        {
            dialogueUI = FindObjectOfType<TypewriterText>(true);
        }

        if (!dialogueRoot && dialogueUI)
        {
            dialogueRoot = dialogueUI.transform.parent.gameObject;
        }

        if (!player)
            player = FindObjectOfType<PlayerController>(true);

        if (!bookToggle)
            bookToggle = FindObjectOfType<BookToggle>(true);
    }
    void ShowDialogue(string text)
    {
        if (!dialogueUI) return;

        // Đóng bag nếu đang mở
        bookToggle?.CloseBook();

        // Khoá di chuyển player
        if (player != null)
            player.SetMoveLock(true);

        // Bật panel gốc
        if (dialogueRoot && !dialogueRoot.activeSelf)
            dialogueRoot.SetActive(true);

        // Bật text object
        if (!dialogueUI.gameObject.activeSelf)
            dialogueUI.gameObject.SetActive(true);

        dialogueUI.ShowText(text);
    }
    public void Interact()
    {
        var qm = QuestManager.Instance;

        if (!qm || !villageIntroQuest)
        {
            ShowDialogue(firstTalkText);
            return;
        }

        qm.GetState(villageIntroQuest, out var state);

        if (state == QuestState.NotAccepted)
        {
            // lần đầu gặp trưởng làng
            qm.AcceptQuest(villageIntroQuest);
            Debug.Log("QuestManager: Bắt đầu quest " + villageIntroQuest.id);
            ShowDialogue(firstTalkText);
        }
        else
        {
            ShowDialogue(repeatText);
        }

    }
}
