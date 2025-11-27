using UnityEngine;

public class DialogueClickToClose : MonoBehaviour
{
    [SerializeField] GameObject root;
    [SerializeField] TypewriterText typewriter;
    [SerializeField] PlayerController player;

    bool firstClick;

    void Awake()
    {
        if (!root) root = gameObject;
        if (!typewriter) typewriter = GetComponentInChildren<TypewriterText>(true);
        if (!player) player = FindObjectOfType<PlayerController>(true);
    }

    void OnEnable()
    {
        firstClick = false;
    }

    void Update()
    {
        if (!root || !root.activeInHierarchy) return;

        bool down =
            Input.GetMouseButtonDown(0) ||
            Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.E);

        if (!down) return;

        if (!firstClick)
        {
            // Lần 1: skip chữ chạy
            if (typewriter != null)
                typewriter.SkipToFull();
            firstClick = true;
        }
        else
        {
            // Lần 2: tắt panel + mở khoá di chuyển
            if (player != null)
                player.SetMoveLock(false);

            root.SetActive(false);
        }
    }
}
