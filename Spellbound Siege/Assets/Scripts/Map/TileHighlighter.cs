using UnityEngine;
using cakeslice;

public class TileHighlighter : MonoBehaviour
{
    private Outline outline;

    void Awake()
    {
        outline = GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = true;
            outline.color = 0; // OutlineEffect에서 정의한 색상 slot (0: 빨강, 1: 초록, 2: 파랑)
        }
    }

    public void SetHighlighted(bool isOn)
    {
        if (outline != null)
        {
            outline.enabled = isOn;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (outline != null)
            {
                Debug.Log("강제 하이라이트 ON");
                outline.enabled = true;
            }
        }
    }
}