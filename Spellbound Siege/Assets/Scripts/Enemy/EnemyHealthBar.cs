using UnityEngine;
using UnityEngine.UIElements;

public class EnemyHealthBar : MonoBehaviour
{
    public EnemyController enemy; // 적 스크립트 (체력을 확인하기 위함)
    private VisualElement healthBarFill; // 체력바의 채워진 부분
    private VisualElement root; // UI Document의 루트
    private UIDocument uiDocument;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;

        // UI Document를 가져오고 VisualElement를 설정
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        // UXML 파일 안에서 이름으로 VisualElement 찾기
        healthBarFill = root.Q<VisualElement>("HealthBarFill");
    }

    private void Update()
    {
        if (enemy == null || healthBarFill == null) return;

        // 적의 체력 상태를 반영해서 너비 조정 (0~1 사이 값)
        float healthPercentage = Mathf.Clamp01(enemy.health / 100f);
        healthBarFill.style.width = new Length(healthPercentage * 100, LengthUnit.Percent);
    }

    private void LateUpdate()
    {
        if (enemy == null) return;

        // 체력바 위치를 적 머리 위로 조정
        float height = enemy.GetComponent<Renderer>().bounds.size.y;
        transform.position = enemy.transform.position + new Vector3(0, height + 0.3f, 0);

        // UI가 카메라를 바라보도록
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                         mainCamera.transform.rotation * Vector3.up);
    }
}