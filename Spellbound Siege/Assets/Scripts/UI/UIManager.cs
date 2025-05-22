using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public GameObject currentUnitUI;

    public bool IsUIOpen { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void OpenUI(GameObject newUI)
    {
        CloseUI();
        currentUnitUI = newUI;
    }

    public void CloseUI()
    {
        if (currentUnitUI != null)
        {
            Destroy(currentUnitUI);
            currentUnitUI = null;
        }

        IsUIOpen = false;

        // ī�޶� ���� ��ġ�� ����
        CameraFocus camFocus = Camera.main.GetComponent<CameraFocus>();
        if (camFocus != null)
            camFocus.ResetFocus();

        ClickableUnit.currentlyFocusedUnit = null;
    }
}