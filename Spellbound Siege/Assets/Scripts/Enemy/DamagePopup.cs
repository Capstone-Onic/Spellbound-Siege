using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public TextMeshProUGUI damageText; // 프리팹 내부에서 연결
    public float floatSpeed = 1f;       // 위로 뜨는 속도
    public float duration = 1f;         // 몇 초 후 사라짐
    public Vector3 floatDirection = Vector3.up;

    private float timer;

    public void Setup(float damageAmount)
    {
        if (damageText != null)
            damageText.text = damageAmount.ToString("0");
        timer = duration;
    }

    void Update()
    {
        transform.position += floatDirection * floatSpeed * Time.deltaTime;
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
