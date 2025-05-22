using System.Collections;
using UnityEngine;

public class CameraFocus : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Camera cam;

    void Awake()
    {
        cam = Camera.main;

        // 원래 위치 및 회전값 저장 (초기값은 당신의 원래 상태 기준)
        originalPosition = new Vector3(51f, 21f, -2f);
        originalRotation = Quaternion.Euler(70f, 0f, 0f); // 카메라 원래 각도
    }

    /// <summary>
    /// 유닛 클릭 시 → 위치 이동 + 회전 전환
    /// </summary>
    public void FocusOn(Vector3 unitPosition)
    {
        StopAllCoroutines();

        // 시점을 유닛 뒤 + 아래에서 보이도록 조정
        Vector3 offset = cam.transform.forward * -6f + cam.transform.up * -2.5f;
        Vector3 focusPosition = unitPosition + offset;

        // 회전도 같이 → 유닛을 더 가까이, 수평에 가깝게 보기
        Quaternion focusRotation = Quaternion.Euler(40f, 0f, 0f);

        StartCoroutine(MoveAndRotateCamera(focusPosition, focusRotation));
    }

    /// <summary>
    /// UI 닫힐 때 → 원래 위치 + 원래 회전으로 복귀
    /// </summary>
    public void ResetFocus()
    {
        StopAllCoroutines();
        StartCoroutine(MoveAndRotateCamera(originalPosition, originalRotation));
    }

    private IEnumerator MoveAndRotateCamera(Vector3 destination, Quaternion targetRotation)
    {
        while (Vector3.Distance(cam.transform.position, destination) > 0.05f ||
               Quaternion.Angle(cam.transform.rotation, targetRotation) > 0.5f)
        {
            cam.transform.position = Vector3.Lerp(cam.transform.position, destination, Time.unscaledDeltaTime * moveSpeed);
            cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, targetRotation, Time.unscaledDeltaTime * rotationSpeed);
            yield return null;
        }

        cam.transform.position = destination;
        cam.transform.rotation = targetRotation;
    }
}
