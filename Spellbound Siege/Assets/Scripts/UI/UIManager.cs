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

    public void CloseUI(bool resetCamera = false)
    {
        if (currentUnitUI != null)
            Destroy(currentUnitUI);

        currentUnitUI = null;
        IsUIOpen = false;
        ClickableUnit.currentlyFocusedUnit = null;

        if (resetCamera)
            CameraZoomController.Instance?.ResetCamera();
    }
}