using UnityEngine;
using UnityEngine.UI;

public class SimpleHealthBar : MonoBehaviour
{
    public Image fillImage;
    public Transform target;
    public Vector3 offset = new Vector3(0, 2f, 0);
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.LookAt(transform.position + cam.transform.forward);
        }

        float ratio = Mathf.Clamp01(currentHealth / maxHealth);
        fillImage.fillAmount = ratio;
    }

    public void SetHealth(float current, float max)
    {
        currentHealth = current;
        maxHealth = max;
    }
}
