using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffectPool : MonoBehaviour
{
    public static DeathEffectPool instance;

    public GameObject effectPrefab;
    public int initialSize = 10;

    private Queue<GameObject> pool = new();

    private void Awake()
    {
        instance = this;

        for (int i = 0; i < initialSize; i++)
        {
            GameObject fx = Instantiate(effectPrefab);
            fx.SetActive(false);
            pool.Enqueue(fx);
        }
    }

    public GameObject GetEffect()
    {
        if (pool.Count > 0)
        {
            return pool.Dequeue();
        }

        // 부족할 경우 추가 생성
        GameObject fx = Instantiate(effectPrefab);
        fx.SetActive(false);
        return fx;
    }

    public void ReturnEffect(GameObject fx)
    {
        fx.SetActive(false);
        pool.Enqueue(fx);
    }
    public void PlayAndRelease(GameObject fx, float delay)
    {
        StartCoroutine(ReturnEffectAfterDelay(fx, delay));
    }

    private IEnumerator ReturnEffectAfterDelay(GameObject fx, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnEffect(fx);
    }
}
