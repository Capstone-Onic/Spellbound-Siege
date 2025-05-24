using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class CanvasCameraAssigner : MonoBehaviour
{
    void Awake()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas.renderMode == RenderMode.WorldSpace && canvas.worldCamera == null)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                canvas.worldCamera = cam;
            }
        }
    }
}
