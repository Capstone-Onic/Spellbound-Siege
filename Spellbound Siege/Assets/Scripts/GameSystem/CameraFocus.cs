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

        // ���� ��ġ �� ȸ���� ���� (�ʱⰪ�� ����� ���� ���� ����)
        originalPosition = new Vector3(51f, 21f, -2f);
        originalRotation = Quaternion.Euler(70f, 0f, 0f); // ī�޶� ���� ����
    }

    /// <summary>
    /// ���� Ŭ�� �� �� ��ġ �̵� + ȸ�� ��ȯ
    /// </summary>
    public void FocusOn(Vector3 unitPosition)
    {
        StopAllCoroutines();

        // ������ ���� �� + �Ʒ����� ���̵��� ����
        Vector3 offset = cam.transform.forward * -6f + cam.transform.up * -2.5f;
        Vector3 focusPosition = unitPosition + offset;

        // ȸ���� ���� �� ������ �� ������, ���� ������ ����
        Quaternion focusRotation = Quaternion.Euler(40f, 0f, 0f);

        StartCoroutine(MoveAndRotateCamera(focusPosition, focusRotation));
    }

    /// <summary>
    /// UI ���� �� �� ���� ��ġ + ���� ȸ������ ����
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
