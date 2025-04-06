using UnityEngine;

public class ClickableUnit : MonoBehaviour
{
    private void OnMouseDown()
    {
        Destroy(gameObject); // 유닛 오브젝트 제거
    }
}
