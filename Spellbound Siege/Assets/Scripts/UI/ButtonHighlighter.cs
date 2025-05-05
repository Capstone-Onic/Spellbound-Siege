using UnityEngine;
using UnityEngine.UI;

public class ButtonHighlighter : MonoBehaviour
{
    [SerializeField] private Outline outline;

    public void SetHighlighted(bool isOn)
    {
        if (outline != null)
            outline.enabled = isOn;
    }
}

