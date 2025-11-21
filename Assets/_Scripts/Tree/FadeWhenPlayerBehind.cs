using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FadeWhenPlayerBehind : MonoBehaviour
{
    [System.Serializable]
    public struct FadeSprite {
        public SpriteRenderer renderer;
        [Range(0f,1f)] public float fadeMultiplier; 
        // 1 = mờ tối đa theo fadedAlpha
        // 0.5 = mờ một nửa
    }

    [Range(0f,1f)] public float fadedAlpha = 0.25f;
    public float fadeDuration = 0.15f;
    public string playerTag = "Player";

    public FadeSprite[] sprites;

    int insideCount;
    Coroutine co;

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Awake()
    {
        // Nếu chưa set trong Inspector → tự lấy tất cả SpriteRenderer cha
        if (sprites == null || sprites.Length == 0)
        {
            var rs = GetComponentsInParent<SpriteRenderer>(true);
            sprites = new FadeSprite[rs.Length];
            for (int i = 0; i < rs.Length; i++)
            {
                sprites[i].renderer = rs[i];
                sprites[i].fadeMultiplier = 1f; // mặc định mờ như nhau
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        insideCount++;
        StartFade(fadedAlpha);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        insideCount = Mathf.Max(0, insideCount - 1);
        if (insideCount == 0) StartFade(1f);
    }

    void OnDisable()
    {
        if (co != null) StopCoroutine(co);
        SetAlpha(1f);
    }

    void StartFade(float target)
    {
        if (!isActiveAndEnabled || !gameObject.activeInHierarchy)
        {
            SetAlpha(target);
            return;
        }

        if (co != null) StopCoroutine(co);
        co = StartCoroutine(FadeTo(target));
    }

    IEnumerator FadeTo(float target)
    {
        float t = 0f, dur = Mathf.Max(0.0001f, fadeDuration);

        float[] startAlpha = new float[sprites.Length];
        float[] targetAlpha = new float[sprites.Length];

        for (int i = 0; i < sprites.Length; i++)
        {
            startAlpha[i] = sprites[i].renderer.color.a;

            // Mỗi sprite có hệ số mờ riêng
            targetAlpha[i] = Mathf.Lerp(1f, target, sprites[i].fadeMultiplier);
        }

        while (t < dur)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / dur);

            for (int i = 0; i < sprites.Length; i++)
            {
                var c = sprites[i].renderer.color;
                c.a = Mathf.Lerp(startAlpha[i], targetAlpha[i], k);
                sprites[i].renderer.color = c;
            }

            yield return null;
        }

        SetAlpha(target);
        co = null;
    }

    void SetAlpha(float target)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            var c = sprites[i].renderer.color;

            // Áp dụng fade multiplier
            c.a = Mathf.Lerp(1f, target, sprites[i].fadeMultiplier);
            sprites[i].renderer.color = c;
        }
    }
}
