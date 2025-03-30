using UnityEngine;
using UnityEngine.UIElements;

public class EnemyHealthBar : MonoBehaviour
{
    public EnemyController enemy; // �� ��ũ��Ʈ (ü���� Ȯ���ϱ� ����)
    private VisualElement healthBarFill; // ü�¹��� ä���� �κ�
    private VisualElement root; // UI Document�� ��Ʈ
    private UIDocument uiDocument;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;

        // UI Document�� �������� VisualElement�� ����
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        // UXML ���� �ȿ��� �̸����� VisualElement ã��
        healthBarFill = root.Q<VisualElement>("HealthBarFill");
    }

    private void Update()
    {
        if (enemy == null || healthBarFill == null) return;

        // ���� ü�� ���¸� �ݿ��ؼ� �ʺ� ���� (0~1 ���� ��)
        float healthPercentage = Mathf.Clamp01(enemy.health / 100f);
        healthBarFill.style.width = new Length(healthPercentage * 100, LengthUnit.Percent);
    }

    private void LateUpdate()
    {
        if (enemy == null) return;

        // ü�¹� ��ġ�� �� �Ӹ� ���� ����
        float height = enemy.GetComponent<Renderer>().bounds.size.y;
        transform.position = enemy.transform.position + new Vector3(0, height + 0.3f, 0);

        // UI�� ī�޶� �ٶ󺸵���
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                         mainCamera.transform.rotation * Vector3.up);
    }
}