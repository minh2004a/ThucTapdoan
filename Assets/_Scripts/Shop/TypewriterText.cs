using System.Collections;
using TMPro;
using UnityEngine;

public class TypewriterText : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] TextMeshProUGUI textLabel;

    [Header("Tốc độ gõ")]
    [SerializeField] float charsPerSecond = 40f;

    [Header("Debug")]
    [TextArea]
    [SerializeField] string debugTextOnAwake;

    string fullText;
    Coroutine typingRoutine;

    public bool IsTyping => typingRoutine != null;

    void Awake()
    {
        if (!textLabel)
            textLabel = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        // Cho phép test nhanh: nếu điền sẵn text ở Inspector thì khi Play sẽ tự gõ
        if (!string.IsNullOrEmpty(debugTextOnAwake))
        {
            ShowText(debugTextOnAwake);
        }
    }

    public void ShowText(string content)
    {
        fullText = content ?? "";
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        typingRoutine = StartCoroutine(TypeRoutine());
    }

    IEnumerator TypeRoutine()
    {
        textLabel.text = "";
        float t = 0f;
        int charIndex = 0;

        while (charIndex < fullText.Length)
        {
            // dùng unscaled để sau này pause game vẫn gõ chữ được
            t += Time.unscaledDeltaTime * charsPerSecond;
            int newIndex = Mathf.FloorToInt(t);
            newIndex = Mathf.Clamp(newIndex, 0, fullText.Length);

            if (newIndex != charIndex)
            {
                charIndex = newIndex;
                textLabel.text = fullText.Substring(0, charIndex);
            }

            yield return null;
        }

        typingRoutine = null;
    }

    // Gọi hàm này nếu muốn skip gõ và hiện full text luôn
    public void SkipToFull()
    {
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        typingRoutine = null;
        textLabel.text = fullText;
    }
}
