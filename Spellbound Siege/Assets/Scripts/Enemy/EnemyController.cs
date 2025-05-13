using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 3f;
    public float health = 100f;
    public float speedMultiplier = 1f;
    public GameObject deathEffectPrefab;

    private List<Transform> waypoints;
    private int waypointIndex = 0;

    public enum EnemyType { Normal, Elite, Boss }
    public EnemyType enemyType = EnemyType.Normal;
    public int rewardGold = 10;

    [Header("Health Bar")]
    public GameObject healthBarPrefab;
    private SimpleHealthBar healthBar;

    private void Start()
    {
        switch (enemyType)
        {
            case EnemyType.Normal: rewardGold = 10; break;
            case EnemyType.Elite: rewardGold = 25; break;
            case EnemyType.Boss: rewardGold = 100; break;
        }
    }

    public void Initialize(List<Transform> waypoints)
    {
        this.waypoints = waypoints;
        transform.position = waypoints[0].position;
        waypointIndex = 0;

        health = 100f;
        speedMultiplier = 1f;

        // 체력바 인스턴스화 및 설정
        if (healthBar == null && healthBarPrefab != null)
        {
            GameObject go = Instantiate(healthBarPrefab);
            healthBar = go.GetComponent<SimpleHealthBar>();
            healthBar.target = this.transform;
        }

        if (healthBar != null)
        {
            healthBar.SetHealth(health, 100f);
            healthBar.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (waypoints == null || waypointIndex >= waypoints.Count) return;

        Transform targetPoint = waypoints[waypointIndex];
        Vector3 direction = (targetPoint.position - transform.position).normalized;
        float adjustedSpeed = speed * speedMultiplier;
        transform.position += direction * adjustedSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            waypointIndex++;
            if (waypointIndex >= waypoints.Count)
            {
                RoundManager rm = FindFirstObjectByType<RoundManager>();
                if (rm != null)
                {
                    rm.DecreaseLife();
                }

                OnDeath(false);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;

        if (healthBar != null)
        {
            healthBar.SetHealth(health, 100f);
        }

        if (health <= 0)
        {
            OnDeath(true);
        }
    }

    private void OnDeath(bool killedByPlayer)
    {
        if (killedByPlayer && GoldManager.instance != null)
        {
            GoldManager.instance.AddGold(rewardGold);
        }

        RoundManager rm = FindFirstObjectByType<RoundManager>();
        if (rm != null)
        {
            rm.NotifyEnemyKilled();
        }

        if (DeathEffectPool.instance != null)
        {
            GameObject fx = DeathEffectPool.instance.GetEffect();
            fx.transform.position = transform.position;
            fx.SetActive(true);

            DeathEffectPool.instance.PlayAndRelease(fx, 1f);
        }

        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        // 풀링 대비 초기화
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(true);
            healthBar.SetHealth(100f, 100f);
        }
    }
}
