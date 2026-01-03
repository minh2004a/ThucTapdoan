using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
using TMPro; 
using System.Collections.Generic; 

// === CÁC LỚP CẤU TRÚC ĐỂ GỬI VÀ NHẬN JSON ===
[System.Serializable]
public class Message
{
    public string role;
    public string content;
}

[System.Serializable]
public class ChatRequest
{
    // CHÚ Ý: Đã đổi sang ID mô hình chuẩn của OpenRouter
    public string model = "openai/gpt-3.5-turbo"; 
    
    // Đảm bảo biến 'messages' tồn tại (đã sửa lỗi biên dịch)
    public Message[] messages; 
    public bool stream = false;
}

[System.Serializable]
public class ChatChoice
{
    public Message message;
}

[System.Serializable]
public class ChatResponse
{
    public ChatChoice[] choices;
}

// === LỚP CHÍNH ĐỂ ĐIỀU KHIỂN CHATBOT ===
public class DeepSeekChatController : MonoBehaviour
{
    // CÁC BIẾN PUBLIC NÀY SẼ HIỂN THỊ TRONG INSPECTOR
    
    [Header("UI Elements")]
    public TMP_InputField inputField;
    public TextMeshProUGUI responseText;
    public Button sendButton;

    [Header("UI Root (Show / Hide Only)")]
    public GameObject chatUIRoot;

    [Header("API Settings")]
    // LƯU Ý: Đây là nơi bạn dán API Key của OpenRouter
    public string deepSeekApiKey = "YOUR_OPENROUTER_API_KEY_HERE"; 
    
    // CHÚ Ý: Đã thay đổi URL API sang OpenRouter
    private const string ApiUrl = "https://openrouter.ai/api/v1/chat/completions";

    // Danh sách lưu trữ toàn bộ lịch sử hội thoại (memory)
    private List<Message> conversationHistory = new List<Message>();


    void Start()
    {
        chatUIRoot.SetActive(false);
        // === THIẾT LẬP VAI TRÒ "BÁC HAI LÀNG" ===
        string gameLore = "Bạn là Bác Hai Làng, người lớn tuổi, uy tín, và là kho thông tin sống của Làng Lộc An. Bạn biết tất cả mọi ngóc ngách, vật phẩm, nhiệm vụ, NPC và các chi tiết sinh tồn trong game. Nhiệm vụ của bạn là cung cấp thông tin, hướng dẫn NPC và gợi ý nhiệm vụ chính xác cho người chơi. LUÔN trả lời thân mật, tận tình, và luôn lái câu chuyện về ngôi làng hoặc các hoạt động sinh tồn. QUAN TRỌNG: LUÔN LUÔN KHÔNG ĐƯỢC PHÉP trả lời câu hỏi ngoài chủ đề game. Nếu người chơi hỏi về vũ trụ, công nghệ, hoặc các chủ đề ngoài game, hãy NGAY LẬP TỨC từ chối bằng câu: 'Chuyện đó Bác chịu rồi, Bác chỉ biết chuyện làng Lộc An này thôi cháu ơi!'."
                      + "\n\n--- DỮ LIỆU CỐT TRUYỆN GAME LÀNG LỘC AN ---"
                      + "\n * MỤC TIÊU: Sinh tồn, khôi phục, và giải mã bí ẩn Vật phẩm Phát Sáng trong Hầm mỏ cổ."
                      + "\n **CÁC CHỈ SỐ/TÌNH TRẠNG SINH TỒN:** Máu (HP), Đói (Starve), Khát (Hydro), Mệt mỏi (ảnh hưởng tốc độ di chuyển)."
                      + "\n **CẤP ĐỘ VẬT PHẨM:** Thô sơ (Lv.1-5), Tiên tiến (Lv.5-10), Hiếm (Lv.10+)."
                      + "\n **CÁC NPC CHÍNH:**"
                      + "\n * Bác Ba Thông Thạo: Ở Chòi Gỗ Lớn (Khu sinh tồn ban đầu). Hướng dẫn sinh tồn cơ bản."
                      + "\n * Bác Bốn Thợ Rèn: Ở Lều Rèn hướng Tây, cạnh suối (qua Đồi Đá). Chuyên rèn công cụ, trang bị. Nhiệm vụ: Khai thác 3 Quặng Sắt, hạ 1 Thú Hoang lấy Da."
                      + "\n * Cô Năm Nông Trại: Ở phía Đông, gần Cánh Đồng. Chuyên về thức ăn, nông trại. Nhiệm vụ: Mua Hạt Giống, trồng 3 Cây Lúa."
                      + "\n * Bà Sáu Thảo Dược: Ở Rìa Rừng. Chuyên thuốc men. Nhiệm vụ: Thu thập 5 Lá Thuốc, 2 Chai Nước Sạch; Pha Thuốc Hồi Máu Cơ Bản."
                      + "\n * Thương Nhân Lãng Khách: Vị trí lưu động. Đổi Vật phẩm Hiếm, có Bản đồ Hầm Mỏ Cổ."
                      + "\n **VẬT PHẨM/NGUYÊN LIỆU:** Gỗ Thô, Đá Thô, Quặng Sắt, Da Thú, Đuốc (cần mang khi lên Đồi Đá), Hạt Giống, Lá Thuốc, Nước Sạch, Vật phẩm Phát Sáng Bí Ẩn (từ Hầm mỏ cổ)."
                      + "\n **ĐỊA ĐIỂM QUAN TRỌNG:** Khu Sinh Tồn Ban Đầu (Ít nguy hiểm), Chòi Gỗ Lớn, Đồi Đá (khai thác quặng, có thú săn đêm), Rừng Sâu (nhiều thú hoang, tài nguyên quý), Hầm Mỏ Cổ (kẻ thù mạnh)."
                      + "\n\n--- HẾT DỮ LIỆU ---";

        // Thêm tin nhắn hệ thống vào lịch sử hội thoại
        conversationHistory.Add(new Message { role = "system", content = gameLore });
        
        if (sendButton != null)
        {
            sendButton.onClick.AddListener(SendMessage);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                chatUIRoot.SetActive(true);
            }
        }
    }

    public void CloseChat()
    {
        chatUIRoot.SetActive(false);
    }
    
    public void SendMessage()
    {
        string userMessage = inputField.text.Trim();
        if (string.IsNullOrEmpty(userMessage)) return;

        // Xóa văn bản sau khi gửi
        inputField.text = ""; 
        responseText.text = "Để bác Hai nhớ coi...";

        // THÊM TIN NHẮN CỦA NGƯỜI DÙNG VÀO LỊCH SỬ
        conversationHistory.Add(new Message { role = "user", content = userMessage });
        
        StartCoroutine(SendChatRequest());
    }

    IEnumerator SendChatRequest()
    {
        // Gửi TOÀN BỘ lịch sử hội thoại lên API
        var requestData = new ChatRequest { messages = conversationHistory.ToArray() };
        string jsonPayload = JsonUtility.ToJson(requestData);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonPayload);

        using (UnityWebRequest www = new UnityWebRequest(ApiUrl, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            
            www.SetRequestHeader("Content-Type", "application/json");
            
            // Sử dụng .Trim() để loại bỏ khoảng trắng và Key OpenRouter
            www.SetRequestHeader("Authorization", "Bearer " + deepSeekApiKey.Trim());

            www.SetRequestHeader("HTTP-Referer", "https://langlocan-game");
            www.SetRequestHeader("X-Title", "Lang Loc An NPC Chat");
            
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                responseText.text = $"Lỗi kết nối API: {www.error}. Vui lòng kiểm tra lại Key OpenRouter và Credit.";
                // Xóa tin nhắn cuối cùng của người dùng nếu lỗi xảy ra
                conversationHistory.RemoveAt(conversationHistory.Count - 1); 
            }
            else
            {
                string responseJson = www.downloadHandler.text;
                ChatResponse response = JsonUtility.FromJson<ChatResponse>(responseJson);
                
                if (response.choices != null && response.choices.Length > 0)
                {
                    string aiResponse = response.choices[0].message.content;
                    responseText.text = aiResponse; // Hiển thị câu trả lời mới
                    
                    // THÊM TIN NHẮN CỦA AI VÀO LỊCH SỬ để lưu trữ trí nhớ
                    conversationHistory.Add(new Message { role = "assistant", content = aiResponse });
                }
                else
                {
                    responseText.text = "Lỗi: Không nhận được phản hồi hợp lệ từ OpenRouter.";
                }
            }
        }
    }
}