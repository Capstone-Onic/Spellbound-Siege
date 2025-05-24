using UnityEngine;
using System.Collections;

public class CameraZoomController : MonoBehaviour
{
    public static CameraZoomController Instance;

    private Camera mainCam;
    private Vector3 defaultPosition;
    private Quaternion defaultRotation;

    private bool isZoomed = false;

    public float zoomDuration = 0.5f;
    public float zoomDistance = 3f;
    public float zoomHeightOffset = 2f;  // 약간 위에서 보도록 설정
    public bool IsZoomed() => isZoomed;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        mainCam = Camera.main;
        defaultPosition = mainCam.transform.position;
        defaultRotation = mainCam.transform.rotation;
    }

    public void ZoomToUnit(Transform target)
    {
        if (isZoomed)
        {
            ResetCamera();
            return;
        }

        isZoomed = true;

        // 유닛의 정면 방향 기준 (예: 유닛이 바라보는 방향의 반대편으로 이동)
        Vector3 unitForward = target.forward; // 유닛

        // 거리 조절 + 약간 위로 올리기
        Vector3 offset = unitForward * zoomDistance; // 앞쪽 거리
        offset.y += 1.5f; // 시점을 약간 위로 올림

        Vector3 targetPos = target.position + offset;
        Quaternion targetRot = Quaternion.LookRotation(target.position - targetPos); // 정확히 유닛 방향 바라봄

        StopAllCoroutines();
        StartCoroutine(ZoomTo(targetPos, targetRot));
    }

    public void ResetCamera()
    {
        isZoomed = false;
        StopAllCoroutines();
        StartCoroutine(ZoomTo(defaultPosition, defaultRotation));
    }

    private IEnumerator ZoomTo(Vector3 targetPos, Quaternion targetRot)
    {
        Vector3 startPos = mainCam.transform.position;
        Quaternion startRot = mainCam.transform.rotation;
        float elapsed = 0f;

        while (elapsed < zoomDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / zoomDuration;

            mainCam.transform.position = Vector3.Lerp(startPos, targetPos, t);
            mainCam.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        mainCam.transform.position = targetPos;
        mainCam.transform.rotation = targetRot;
    }
}
