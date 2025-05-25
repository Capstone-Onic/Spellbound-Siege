using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public TextMeshProUGUI damageText; // ������ ���ο��� ����
    public float floatSpeed = 1f;       // ���� �ߴ� �ӵ�
    public float duration = 1f;         // �� �� �� �����
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
