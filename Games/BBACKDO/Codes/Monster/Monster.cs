using System.Collections;
using UnityEngine;

public enum MonsterRank
{
    Normal,
    Boss
}

[RequireComponent(typeof(Animator))]
public class Monster : MonoBehaviour
{
    [Header("몬스터 기본 설정")]
    [SerializeField] private string monsterName = "몬스터";
    [SerializeField] private MonsterRank monsterRank = MonsterRank.Normal;
    [SerializeField] private int maxHp = 30;
    [SerializeField] private float deathAnimDuration = 0.5f;

    [Header("일반 몬스터 드롭")]
    [SerializeField, Range(0f, 1f)]
    private float normalDropChance = 0.15f;

    [Header("보스 드롭")]
    [SerializeField, Range(2, 3)]
    private int bossDropCount = 2;

    [Header("공통 드롭 프리팹")]
    [SerializeField] private GameObject itemPickupPrefab;

    private int currentHp;

    private RoomBattle roomBattle;
    private Animator animator;
    private MonsterAI monsterAI;
    private MonsterAttack monsterAttack;
    private BossBase boss;
    private DamageFlash damageFlash;

    private bool isDead;

    public int CurrentHp => currentHp;
    public int MaxHp => maxHp;
    public bool IsDead => isDead;

    public float HealthRate
    {
        get
        {
            if (maxHp <= 0)
            {
                return 0f;
            }

            return (float)currentHp / maxHp;
        }
    }

    private static readonly int HitTriggerHash =
        Animator.StringToHash("Hit");

    private static readonly int DeathTriggerHash =
        Animator.StringToHash("Death");

    private void Awake()
    {
        currentHp = maxHp;

        roomBattle =
            GetComponentInParent<RoomBattle>();

        animator =
            GetComponent<Animator>();

        monsterAI =
            GetComponent<MonsterAI>();

        monsterAttack =
            GetComponent<MonsterAttack>();

        boss =
            GetComponent<BossBase>();

        damageFlash =
            GetComponent<DamageFlash>();
    }

    public void TakeDamage(
        int damage
    )
    {
        if (isDead ||
            damage <= 0)
        {
            return;
        }

        currentHp =
            Mathf.Max(
                0,
                currentHp - damage
            );

        Debug.Log(
            monsterName +
            " 피격. 남은 체력: " +
            currentHp
        );

        // 실제 피해가 적용됐을 때 피격 색상 표시
        if (damageFlash != null)
        {
            damageFlash.Play();
        }

        if (currentHp <= 0)
        {
            Die();
            return;
        }

        if (SFXManager.Instance != null)
        {
            SFXManager.Instance
                .PlayMonsterHit();
        }

        if (animator != null)
        {
            animator.SetTrigger(
                HitTriggerHash
            );
        }
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        Debug.Log(
            $"{monsterName} 사망"
        );

        if (SFXManager.Instance != null)
        {
            SFXManager.Instance
                .PlayMonsterDeath();
        }

        DisableMonsterBehaviour();

        if (animator != null)
        {
            animator.SetTrigger(
                DeathTriggerHash
            );
        }

        TryDropItem();

        if (roomBattle != null)
        {
            roomBattle.OnMonsterDead();
        }

        StartCoroutine(
            DestroyAfterAnimation()
        );
    }

    private void DisableMonsterBehaviour()
    {
        if (monsterAI != null)
        {
            monsterAI.enabled = false;
        }

        if (monsterAttack != null)
        {
            monsterAttack.enabled = false;
        }

        if (boss != null)
        {
            boss.enabled = false;
        }

        Rigidbody2D rb =
            GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity =
                Vector2.zero;
        }

        Collider2D[] colliders =
            GetComponentsInChildren<Collider2D>();

        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }
    }

    private void TryDropItem()
    {
        if (ItemManager.Instance == null)
        {
            Debug.LogWarning(
                "ItemManager가 없어 아이템 드롭을 건너뜁니다."
            );

            return;
        }

        if (itemPickupPrefab == null)
        {
            Debug.LogWarning(
                $"{monsterName}: ItemPickup 프리팹이 없습니다."
            );

            return;
        }

        Room room =
            GetComponentInParent<Room>();

        int stage =
            room != null
                ? room.StageIndex
                : 0;

        switch (monsterRank)
        {
            case MonsterRank.Normal:

                ItemManager.Instance
                    .DropNormalMonsterItem(
                        stage,
                        normalDropChance,
                        transform.position,
                        itemPickupPrefab
                    );

                break;

            case MonsterRank.Boss:

                ItemManager.Instance
                    .DropBossItems(
                        stage,
                        bossDropCount,
                        transform.position,
                        itemPickupPrefab
                    );

                break;
        }
    }

    private IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(
            deathAnimDuration
        );

        Destroy(gameObject);
    }
}
