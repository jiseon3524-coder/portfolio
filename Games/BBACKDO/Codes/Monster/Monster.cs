using System.Collections;
using UnityEngine;

// 몬스터 랭크 나눔
public enum MonsterRank
{
    Normal,
    Boss
}

// Animator 컴포넌트가 없으면 불러오기
[RequireComponent(typeof(Animator))]

public class Monster : MonoBehaviour
{
    [Header("몬스터 기본 설정")]
    [SerializeField] private string monsterName = "몬스터";
    [SerializeField] private MonsterRank monsterRank = MonsterRank.Normal;
    [SerializeField] private int maxHp = 30;
    [SerializeField] private float deathAnimDuration = 0.5f;

    // 몬스터별 아이템 드롭 확률
    [Header("일반 몬스터 드롭")]
    [SerializeField, Range(0f, 1f)] private float normalDropChance = 0.15f;

    [Header("보스 드롭")]
    [SerializeField, Range(2, 3)]
    private int bossDropCount = 2;

    // 몬스터들이 떨구는 아이템 공통 프리팹
    [Header("공통 드롭 프리팹")]
    [SerializeField] private GameObject itemPickupPrefab;

    // 컴포넌트와 스탯을 변수에 저장
    private int currentHp;
    private RoomBattle roomBattle;
    private Animator animator;
    private MonsterAI monsterAI;
    private MonsterAttack monsterAttack;
    private BossBase boss;
    private DamageFlash damageFlash;

    private bool isDead;

    // 다른 스크립트에서 HP와 죽음 여부를 읽을 수 있게
    public int CurrentHp => currentHp;
    public int MaxHp => maxHp;
    public bool IsDead => isDead;

    // 몬스터들의 체력 비율 ( 현재 체력이 몇 퍼센트 남았는지 등 )
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

    // 애니메이터 트리거를 해쉬 처리 - 애니메이터에서 정수값으로 트리거를 사용하기 위해
    private static readonly int HitTriggerHash =
        Animator.StringToHash("Hit");

    private static readonly int DeathTriggerHash =
        Animator.StringToHash("Death");

    // 게임 시작 시 현재 체력을 최대로 만들고 컴포넌트들 가져오기.
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

    // 몬스터가 피해를 입을 때의 함수
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

        // 체력 0이하 되면 죽음
        if (currentHp <= 0)
        {
            Die();
            return;
        }

        // 몬스터 피격 시 효과음
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance
                .PlayMonsterHit();
        }

        // 피격 당할 때의 애니메이션
        if (animator != null)
        {
            animator.SetTrigger(
                HitTriggerHash
            );
        }
    }

    // 몬스터가 죽을 때 함수
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

        // 몬스터 죽을 때 효과음
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance
                .PlayMonsterDeath();
        }

        // 죽은 몬스터 행동 멈추기
        DisableMonsterBehaviour();

        // 죽는 애니메이션
        if (animator != null)
        {
            animator.SetTrigger(
                DeathTriggerHash
            );
        }

        // 죽으면서 아이템 드롭 ( 확률에 따라 다름 )
        TryDropItem();

        // 룸배틀 컴포넌트가 붙어있으면 룸배틀에게 몬스터가 죽었다고 전달하기
        if (roomBattle != null)
        {
            roomBattle.OnMonsterDead();
        }

        // 죽었을 때 애니메이션 나올때의 코루틴 적용
        StartCoroutine(
            DestroyAfterAnimation()
        );
    }

    // 몬스터를 비활성화 해서 멈추는 함수
    private void DisableMonsterBehaviour()
    {
        // 몬스터 스크립트들 비활성화
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

        // Rigidbody 컴포넌트 변수에 저장하기
        Rigidbody2D rb =
            GetComponent<Rigidbody2D>();

        // Rigidbody 속도를 0으로 해놓기
        if (rb != null)
        {
            rb.linearVelocity =
                Vector2.zero;
        }

        // 몬스터 오브젝트의 자식 오브젝트들의 Collider까지 변수에 저장
        Collider2D[] colliders =
            GetComponentsInChildren<Collider2D>();

        // Collider 비활성화
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }
    }

    // 아이템 떨구는 함수
    private void TryDropItem()
    {
        // ItemManger 컴포넌트가 없을 경우 콘솔 로그 출력
        if (ItemManager.Instance == null)
        {
            Debug.LogWarning(
                "ItemManager가 없어 아이템 드롭을 건너뜁니다."
            );

            return;
        }

        // 아이템 기본프리팹이 연결되어 있지 않아도 콘솔 로그 출력
        if (itemPickupPrefab == null)
        {
            Debug.LogWarning(
                $"{monsterName}: ItemPickup 프리팹이 없습니다."
            );

            return;
        }

        // 해당 오브젝트의 부모 오브젝트에서 제일 먼저 Room 컴포넌트가 발견될 때 가져오기 
        Room room =
            GetComponentInParent<Room>();

        // 저장한 변수로 스테이지 번호 저장
        int stage =
            room != null
                ? room.StageIndex
                : 0;

        // 몬스터 랭크에 따른 경우 분리
        switch (monsterRank)
        {
            // 일반 몬스터 경우
            case MonsterRank.Normal:

                ItemManager.Instance
                    .DropNormalMonsterItem(
                        stage,
                        normalDropChance,
                        transform.position,
                        itemPickupPrefab
                    );

                break;

            // 보스 몬스터 경우
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

    // 몬스터 죽었을 때 죽는 애니메이션이 나온 이후 몬스터가 파괴되기 위한 코루틴 함수
    private IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(
            deathAnimDuration
        );

        // 몇 초 기다리고 몬스터 오브젝트 파괴
        Destroy(gameObject);
    }
}
