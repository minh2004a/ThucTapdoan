using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public enum PopupType { PlayerDamage, EnemyDamage }
    public static DamagePopup prefab;
    public static Transform parent;
    [SerializeField] private TextMeshPro textMeshPro;

    float lifeTime = 0.8f;
    float timer;

    Vector3 move;
    float speed = 1.2f;
    Color startColor;
    Vector3 startScale;

    public static Color playerColor = new Color(1f, 0.75f, 0f);  // vàng
    public static Color enemyColor = new Color(1f, 0f, 0f);    // đỏ

    private void Awake()
    {
        if (textMeshPro == null)
        {
            textMeshPro = GetComponent<TextMeshPro>();
        }
        startScale = transform.localScale;
        startColor = textMeshPro.color;
    }

    public static void Init(DamagePopup damagePopup)
    {
        prefab = damagePopup;
        if (parent == null)
        {
            GameObject go = new GameObject("DamagePopup");
            parent = go.transform;
        }
    }

    // Tạo Popup
    public static void Create(Vector3 worldPos, int damage, PopupType type)
    {
        if (prefab == null) return;

        var popup = Instantiate(prefab, worldPos, Quaternion.identity, parent);
        popup.Setup(damage, type);
    }

    
    public void Setup(int damage, PopupType type)
    {
        textMeshPro.text = damage.ToString();

        timer = lifeTime;
        transform.localScale = startScale;

        if (type == PopupType.PlayerDamage)
        {
            startColor = playerColor;
        }
        else
        {
            startColor = enemyColor;
        }

        textMeshPro.color = startColor;

        move = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(1.2f, 1.6f), 0f);
    }

    private void Update()
    {
        transform.position += move * speed * Time.deltaTime;
        move.y -= 1f * Time.deltaTime;

        float progress = 1 - (timer / lifeTime);

        if (progress < 0.25f)
        {
            float bounce = Mathf.Lerp(1.6f, 1f, progress / 0.25f);
            transform.localScale = startScale * bounce;
        }
        else
        {
            transform.localScale = startScale;
        }

        // Fade out
        float fade = Mathf.Lerp(1f, 0.7f, progress);
        Color c = textMeshPro.color;
        c.a = fade;
        textMeshPro.color = c;

        timer -= Time.deltaTime;
        if (timer <= 0f) Destroy(gameObject);
    }
}
