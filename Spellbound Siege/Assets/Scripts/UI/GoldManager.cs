using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public static GoldManager instance;
    public int currentGold = 0;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        Debug.Log($"��� ȹ��: +{amount} �� ���� ���: {currentGold}");

        var ui = FindFirstObjectByType<GoldUIController>();
        if (ui != null)
        {
            ui.UpdateGold(currentGold); //  ���� �ؽ�Ʈ ���� �߰�

            LeanTween.cancel(ui.goldText.gameObject);
            ui.goldText.transform.localScale = Vector3.one * 1.1f;
            LeanTween.scale(ui.goldText.gameObject, Vector3.one, 0.25f).setEaseOutBack();
        }
    }

    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            Debug.Log($"��� ���: -{amount} �� ���� ���: {currentGold}");

            var ui = FindFirstObjectByType<GoldUIController>();
            if (ui != null)
            {
                ui.UpdateGold(currentGold); //  ���� �ؽ�Ʈ ���� �߰�

                LeanTween.cancel(ui.goldText.gameObject);
                ui.goldText.transform.localScale = Vector3.one * 0.9f;
                LeanTween.scale(ui.goldText.gameObject, Vector3.one, 0.25f).setEaseOutBack();
            }

            return true;
        }
        else
        {
            Debug.Log("��� �������� ���� ��ġ �Ұ�");
            return false;
        }
    }
}
