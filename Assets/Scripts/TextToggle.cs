using UnityEngine;
using TMPro;
using System.Collections;

public class TextToggle : MonoBehaviour
{
    public float fadeSpeed = 1.0f;
    private TextMeshPro myText;

    void Awake()
    {
        myText = GetComponent<TextMeshPro>();
    }

    void Start()
    {
        SetAlpha(0f);
    }

    public void ShowText()
    {
        if (!gameObject.activeInHierarchy) return;
        StopAllCoroutines();
        StartCoroutine(FadeTo(1f));
    }

    public void HideText()
    {
        if (!gameObject.activeInHierarchy) return;
        StopAllCoroutines();
        StartCoroutine(FadeTo(0f));
    }

    public void RemoveText()
    {
        gameObject.SetActive(false);
    }
    public void DestroyText()
{
    Destroy(gameObject);
}


    private void SetAlpha(float alpha)
    {
        if (myText != null)
        {
            Color c = myText.color;
            c.a = alpha;
            myText.color = c;
        }
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        if (myText == null) yield break;

        float startAlpha = myText.color.a;
        float time = 0;

        while (time < fadeSpeed)
        {
            time += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeSpeed);
            SetAlpha(newAlpha);
            yield return null;
        }

        SetAlpha(targetAlpha);
    }
}