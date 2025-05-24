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
    public float zoomHeightOffset = 2f;  // �ణ ������ ������ ����
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

        // ������ ���� ���� ���� (��: ������ �ٶ󺸴� ������ �ݴ������� �̵�)
        Vector3 unitForward = target.forward; // ����

        // �Ÿ� ���� + �ణ ���� �ø���
        Vector3 offset = unitForward * zoomDistance; // ���� �Ÿ�
        offset.y += 1.5f; // ������ �ణ ���� �ø�

        Vector3 targetPos = target.position + offset;
        Quaternion targetRot = Quaternion.LookRotation(target.position - targetPos); // ��Ȯ�� ���� ���� �ٶ�

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
