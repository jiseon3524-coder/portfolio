using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackCooldown = 1f;

    private float lastAttackTime;
    private Animator animator;
    private MonsterAI monsterAI;

    private static readonly int AttackTriggerHash =
        Animator.StringToHash("Attack");

    private void Awake()
    {
        animator =
            GetComponent<Animator>();

        monsterAI =
            GetComponent<MonsterAI>();

        if (monsterAI == null)
        {
            monsterAI =
                GetComponentInParent<MonsterAI>();
        }
    }

    private void OnCollisionStay2D(
        Collision2D collision
    )
    {
        TryAttack(
            collision.collider
        );
    }

    private void OnTriggerStay2D(
        Collider2D other
    )
    {
        TryAttack(other);
    }

    private void TryAttack(
        Collider2D other
    )
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (Time.time <
            lastAttackTime + attackCooldown)
        {
            return;
        }

        PlayerStats playerStats =
            other.GetComponent<PlayerStats>();

        if (playerStats == null)
        {
            playerStats =
                other.GetComponentInParent<PlayerStats>();
        }

        if (playerStats == null)
        {
            return;
        }

        lastAttackTime =
            Time.time;

        playerStats.TakeDamage(
            damage
        );

        if (animator != null &&
            animator.enabled)
        {
            animator.SetTrigger(
                AttackTriggerHash
            );
        }

        // Cat인 경우 공격 직후 플레이어 반대 방향으로 후퇴
        if (monsterAI != null)
        {
            monsterAI.BeginCatRetreat(
                other.transform.position
            );
        }

        Debug.Log(
            gameObject.name
            + "가 플레이어에게 "
            + damage
            + "의 피해를 입혔습니다."
        );
    }
}
