using UnityEngine;

public class ClickableUnit : MonoBehaviour
{
    private void OnMouseDown()
    {
        Destroy(gameObject); // ���� ������Ʈ ����
    }
}
