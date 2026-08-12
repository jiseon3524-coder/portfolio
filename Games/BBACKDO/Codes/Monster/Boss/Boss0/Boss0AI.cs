using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Monster))]
public class Boss0AI : BossBase
{
    private enum BossState
    {
        Chase,
        Attack,
        Dead
    }

    [Header("이동")]
    [SerializeField] private float moveSpeed = 3.2f;

    [Header("공격")]
    [SerializeField] private Boss0Hit attackHitbox;
    [SerializeField] private int damage = 20;

    [Tooltip("공격을 시작할 수 있는 플레이어와의 거리")]
    [SerializeField] private float attackRange = 3f;

    [Tooltip("공격과 공격 사이의 대기 시간")]
    [SerializeField] private float attackCooldown = 1.8f;

    [Tooltip("공격 전에 멈춰 있는 시간")]
    [SerializeField] private float prepareTime = 0.35f;

    [Tooltip("실제로 공격 판정이 활성화되는 시간")]
    [SerializeField] private float activeTime = 0.2f;

    [Tooltip("공격 후 다시 움직이기까지의 시간")]
    [SerializeField] private float recoveryTime = 0.4f;

    [Tooltip("방에 들어온 후 첫 공격까지 기다리는 시간")]
    [SerializeField] private float firstAttackDelay = 0.8f;

    [Header("공격 범위 위치")]
    [Tooltip("보스 중심에서 공격 범위가 떨어지는 거리")]
    [SerializeField] private float hitboxDistance = 1.3f;

    [Header("선택 사항")]
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private Monster monster;
    private Transform player;
    private SpriteRenderer spriteRenderer;

    private BossState state =
        BossState.Chase;

    private float attackTimer;
    private Vector2 lastAttackDirection =
        Vector2.down;

    private Coroutine attackCoroutine;

    private static readonly int IsMovingHash =
        Animator.StringToHash("IsMoving");

    private static readonly int AttackTriggerHash =
        Animator.StringToHash("Attack");

    private void Awake()
    {
        rb =
            GetComponent<Rigidbody2D>();

        monster =
            GetComponent<Monster>();

        spriteRenderer =
            GetComponent<SpriteRenderer>();

        if (animator == null)
        {
            animator =
                GetComponent<Animator>();
        }

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    private void Start()
    {
        FindPlayer();

        attackTimer =
            firstAttackDelay;

        if (attackHitbox != null)
        {
            attackHitbox.EndAttack();
        }
    }

    private void OnEnable()
    {
        state =
            BossState.Chase;

        attackTimer =
            firstAttackDelay;
    }

    private void Update()
    {
        if (monster == null ||
            monster.IsDead)
        {
            SetDeadState();
            return;
        }

        if (player == null)
        {
            FindPlayer();
            return;
        }

        if (state != BossState.Chase)
        {
            return;
        }

        attackTimer -=
            Time.deltaTime;

        if (attackTimer > 0f)
        {
            return;
        }

        if (!IsPlayerInAttackRange())
        {
            return;
        }

        attackCoroutine =
            StartCoroutine(
                AttackRoutine()
            );
    }

    private void FixedUpdate()
    {
        if (state != BossState.Chase ||
            player == null ||
            monster == null ||
            monster.IsDead)
        {
            StopMovement();
            return;
        }

        ChasePlayer();
    }

    private void ChasePlayer()
    {
        Vector2 direction =
            GetDirectionToPlayer();

        rb.linearVelocity =
            direction * moveSpeed;

        UpdateFacingDirection(
            direction
        );

        SetMovingAnimation(
            true
        );
    }

    private IEnumerator AttackRoutine()
    {
        state =
            BossState.Attack;

        StopMovement();

        SetMovingAnimation(
            false
        );

        /*
         * 공격 시작 순간의 플레이어 방향을 저장한다.
         * 이후 플레이어가 움직여도 공격 방향은 바뀌지 않는다.
         */
        lastAttackDirection =
            GetDirectionToPlayer();

        UpdateFacingDirection(
            lastAttackDirection
        );

        UpdateAttackHitboxPosition(
            lastAttackDirection
        );

        yield return new WaitForSeconds(
            prepareTime
        );

        if (monster == null ||
            monster.IsDead)
        {
            EndAttackImmediately();
            yield break;
        }

        PlayAttackAnimation();

        if (attackHitbox != null)
        {
            attackHitbox.BeginAttack(
                damage
            );
        }

        yield return new WaitForSeconds(
            activeTime
        );

        if (attackHitbox != null)
        {
            attackHitbox.EndAttack();
        }

        yield return new WaitForSeconds(
            recoveryTime
        );

        attackTimer =
            attackCooldown;

        state =
            BossState.Chase;

        attackCoroutine =
            null;
    }

    private void UpdateAttackHitboxPosition(
        Vector2 direction
    )
    {
        if (attackHitbox == null)
        {
            return;
        }

        Vector2 normalizedDirection =
            direction.sqrMagnitude > 0.001f
                ? direction.normalized
                : Vector2.down;

        Transform hitboxTransform =
            attackHitbox.transform;

        /*
         * Boss0Hit이 Boss0의 자식이므로 localPosition을 사용한다.
         * Boss0의 Scale이 크더라도 인스펙터의 Hitbox Distance를
         * 보면서 조절하면 된다.
         */
        hitboxTransform.localPosition =
            normalizedDirection
            * hitboxDistance;

        float angle =
            Mathf.Atan2(
                normalizedDirection.y,
                normalizedDirection.x
            )
            * Mathf.Rad2Deg;

        hitboxTransform.localRotation =
            Quaternion.Euler(
                0f,
                0f,
                angle
            );
    }

    private void PlayAttackAnimation()
    {
        if (animator == null ||
            !animator.enabled)
        {
            return;
        }

        animator.SetTrigger(
            AttackTriggerHash
        );
    }

    private void SetMovingAnimation(
        bool isMoving
    )
    {
        if (animator == null ||
            !animator.enabled)
        {
            return;
        }

        animator.SetBool(
            IsMovingHash,
            isMoving
        );
    }

    private bool IsPlayerInAttackRange()
    {
        if (player == null)
        {
            return false;
        }

        Vector2 difference =
            (Vector2)player.position
            - rb.position;

        return difference.sqrMagnitude
               <= attackRange
               * attackRange;
    }

    private Vector2 GetDirectionToPlayer()
    {
        if (player == null)
        {
            return Vector2.down;
        }

        Vector2 direction =
            (Vector2)player.position
            - rb.position;

        if (direction.sqrMagnitude <=
            0.001f)
        {
            return lastAttackDirection;
        }

        return direction.normalized;
    }

    private void UpdateFacingDirection(
        Vector2 direction
    )
    {
        if (spriteRenderer == null ||
            Mathf.Abs(direction.x) <= 0.01f)
        {
            return;
        }

        spriteRenderer.flipX =
            direction.x < 0f;
    }

    private void FindPlayer()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag(
                "Player"
            );

        if (playerObject != null)
        {
            player =
                playerObject.transform;
        }
    }

    private void StopMovement()
    {
        if (rb == null)
        {
            return;
        }

        rb.linearVelocity =
            Vector2.zero;

        rb.angularVelocity =
            0f;
    }

    private void SetDeadState()
    {
        if (state == BossState.Dead)
        {
            return;
        }

        state =
            BossState.Dead;

        EndAttackImmediately();

        SetMovingAnimation(
            false
        );
    }

    private void EndAttackImmediately()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(
                attackCoroutine
            );

            attackCoroutine =
                null;
        }

        if (attackHitbox != null)
        {
            attackHitbox.EndAttack();
        }

        StopMovement();
    }

    private void OnDisable()
    {
        EndAttackImmediately();

        state =
            BossState.Chase;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            attackRange
        );
    }
}
