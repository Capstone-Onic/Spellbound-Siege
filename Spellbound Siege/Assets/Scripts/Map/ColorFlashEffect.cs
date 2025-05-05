using UnityEngine;

public class ColorFlashEffect : MonoBehaviour
{
    private Renderer tileRenderer;
    private Color originalColor;

    void Awake()
    {
        tileRenderer = GetComponent<Renderer>();
        if (tileRenderer != null)
        {
            originalColor = tileRenderer.material.color;
        }
    }

    public void FlashColor(Color color, float duration = 1f)
    {
        if (tileRenderer == null) return;

        StopAllCoroutines();
        StartCoroutine(FlashRoutine(color, duration));
    }

    private System.Collections.IEnumerator FlashRoutine(Color color, float duration)
    {
        tileRenderer.material.color = color;
        yield return new WaitForSeconds(duration);
        tileRenderer.material.color = originalColor;
    }
}