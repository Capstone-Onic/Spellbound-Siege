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
            outline.color = 0; // OutlineEffect���� ������ ���� slot (0: ����, 1: �ʷ�, 2: �Ķ�)
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
                Debug.Log("���� ���̶���Ʈ ON");
                outline.enabled = true;
            }
        }
    }
}