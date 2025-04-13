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

        // �ִϸ��̼� ȿ��
        var ui = FindObjectOfType<GoldUIController>();
        if (ui != null)
        {
            LeanTween.cancel(ui.goldText.gameObject); // �ߺ� ����
            ui.goldText.transform.localScale = Vector3.one * 1.1f;
            LeanTween.scale(ui.goldText.gameObject, Vector3.one, 0.25f).setEaseOutBack();
        }
    }
}
