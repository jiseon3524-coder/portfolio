using UnityEngine;

public enum MonsterType
{
    Rat,
    Cat,
    Ghost
}

[RequireComponent(typeof(Rigidbody2D))]
public class MonsterAI : MonoBehaviour
{
    [SerializeField] private MonsterType monsterType;

    [Header("Common Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float changeDirectionTime = 2f;
    [SerializeField] private float wallEscapeTime = 0.25f;

    [Header("Cat")]
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float chaseRange = 8f;
    [SerializeField] private float catRetreatSpeed = 7f;
    [SerializeField] private float catRetreatDuration = 0.35f;
    [SerializeField] private float catRecoveryDuration = 0.45f;

    [Header("Ghost")]
    [SerializeField] private float ghostDetectionRange = 12f;
    [SerializeField] private float ghostAttackRange = 8f;
    [SerializeField] private float ghostAttackCooldown = 2f;
    [SerializeField] private GameObject ghostProjectilePrefab;

    [Header("Ghost Projectile Spawn")]
    [SerializeField]
    private Vector2 ghostProjectileOffset =
        new Vector2(0f, -0.3f);

    private Rigidbody2D rb;
    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private Vector2 moveDirection;
    private Vector2 escapeDirection;

    private float directionTimer;
    private float escapeTimer;
    private float ghostAttackTimer;

    private Vector2 catRetreatDirection;

    private float catRetreatTimer;
    private float catRecoveryTimer;

    private static readonly int IsMovingHash =
        Animator.StringToHash("IsMoving");

    private static readonly int AttackTriggerHash =
        Animator.StringToHash("Attack");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    private void Start()
    {
        FindPlayer();
        PickRandomDirection();

        ghostAttackTimer = ghostAttackCooldown;
    }

    private void Update()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }

        if (monsterType == MonsterType.Ghost)
        {
            UpdateGhostAttack();
        }
    }

    private void FixedUpdate()
    {
        if (player == null)
        {
            FindPlayer();
        }

        Vector2 direction;
        float currentSpeed;

        // 벽에 걸린 상태라면 벽 탈출을 가장 먼저 처리
        if (escapeTimer > 0f)
        {
            escapeTimer -=
                Time.fixedDeltaTime;

            direction =
                escapeDirection;

            currentSpeed =
                moveSpeed;
        }
        // 고양이가 공격한 직후 뒤로 물러나는 상태
        else if (
            monsterType == MonsterType.Cat &&
            catRetreatTimer > 0f
        )
        {
            catRetreatTimer -=
                Time.fixedDeltaTime;

            direction =
                catRetreatDirection;

            currentSpeed =
                catRetreatSpeed;
        }
        // 후퇴 후 다시 달려들기 전 잠깐 대기
        else if (
            monsterType == MonsterType.Cat &&
            catRecoveryTimer > 0f
        )
        {
            catRecoveryTimer -=
                Time.fixedDeltaTime;

            direction =
                Vector2.zero;

            currentSpeed =
                0f;
        }
        else
        {
            GetMovement(
                out direction,
                out currentSpeed
            );
        }

        Move(
            direction,
            currentSpeed
        );

        UpdateVisual(
            direction
        );
    }

    private void GetMovement(
        out Vector2 direction,
        out float currentSpeed
    )
    {
        switch (monsterType)
        {
            case MonsterType.Cat:
                GetCatMovement(
                    out direction,
                    out currentSpeed
                );
                break;

            case MonsterType.Ghost:
                GetGhostMovement(
                    out direction,
                    out currentSpeed
                );
                break;

            default:
                GetRandomMovement(
                    out direction,
                    out currentSpeed
                );
                break;
        }
    }

    // =========================================================
    // 쥐 랜덤 이동
    // =========================================================

    private void GetRandomMovement(
        out Vector2 direction,
        out float currentSpeed
    )
    {
        UpdateRandomDirection();

        direction = moveDirection;
        currentSpeed = moveSpeed;
    }

    // =========================================================
    // 고양이
    // =========================================================

    private void GetCatMovement(
        out Vector2 direction,
        out float currentSpeed
    )
    {
        if (IsPlayerInRange(chaseRange))
        {
            direction =
                GetDirectionToPlayer();

            currentSpeed = chaseSpeed;
            return;
        }

        GetRandomMovement(
            out direction,
            out currentSpeed
        );
    }
    public void BeginCatRetreat(
    Vector2 playerPosition
)
    {
        if (monsterType !=
            MonsterType.Cat)
        {
            return;
        }

        Vector2 retreatDirection =
            rb.position
            - playerPosition;

        if (retreatDirection.sqrMagnitude
            <= 0.0001f)
        {
            retreatDirection =
                -moveDirection;
        }

        if (retreatDirection.sqrMagnitude
            <= 0.0001f)
        {
            retreatDirection =
                Vector2.down;
        }

        catRetreatDirection =
            retreatDirection.normalized;

        catRetreatTimer =
            Mathf.Max(
                0f,
                catRetreatDuration
            );

        catRecoveryTimer =
            catRetreatTimer
            + Mathf.Max(
                0f,
                catRecoveryDuration
            );

        // 기존 추적 방향을 초기화해서
        // 공격 직후 다시 플레이어 쪽으로 밀고 들어오는 것을 방지
        moveDirection =
            catRetreatDirection;

        Debug.Log(
            $"{gameObject.name}: 공격 후 후퇴"
        );
    }

    // =========================================================
    // 귀신 이동
    // =========================================================

    private void GetGhostMovement(
        out Vector2 direction,
        out float currentSpeed
    )
    {
        if (IsPlayerInRange(ghostDetectionRange))
        {
            // 플레이어가 가까우면 계속 추적
            direction = GetDirectionToPlayer();

            // 랜덤 이동과 같은 속도 사용
            currentSpeed = moveSpeed;
            return;
        }

        // 플레이어가 멀면 랜덤 이동
        GetRandomMovement(
            out direction,
            out currentSpeed
        );
    }

    private Vector2 GetDirectionToPlayer()
    {
        if (player == null)
        {
            return Vector2.zero;
        }

        return (
            (Vector2)player.position
            - rb.position
        ).normalized;
    }

    // =========================================================
    // 귀신 공격
    // =========================================================

    private void UpdateGhostAttack()
    {
        ghostAttackTimer -= Time.deltaTime;

        if (!IsPlayerInRange(ghostAttackRange))
        {
            return;
        }

        if (ghostAttackTimer > 0f)
        {
            return;
        }

        ShootGhostProjectile();

        ghostAttackTimer =
            ghostAttackCooldown;
    }

    private void ShootGhostProjectile()
    {
        if (ghostProjectilePrefab == null)
        {
            Debug.LogWarning(
                $"{gameObject.name}: " +
                "Ghost Projectile Prefab이 연결되지 않았습니다."
            );

            return;
        }

        if (player == null)
        {
            return;
        }

        // 공격 애니메이션 트리거
        if (animator != null)
        {
            animator.SetTrigger(
                AttackTriggerHash
            );
        }

        Vector2 shootDirection =
            GetDirectionToPlayer();

        Vector3 spawnPosition =
            transform.position
            + (Vector3)ghostProjectileOffset;

        GameObject projectileObject =
            Instantiate(
                ghostProjectilePrefab,
                spawnPosition,
                Quaternion.identity
            );

        GhostProjectile projectile =
            projectileObject.GetComponent<GhostProjectile>();

        if (projectile == null)
        {
            Debug.LogError(
                $"{ghostProjectilePrefab.name}에 " +
                "GhostProjectile 컴포넌트가 없습니다."
            );

            Destroy(projectileObject);
            return;
        }

        projectile.Initialize(
            shootDirection
        );
    }

    // =========================================================
    // 공통 이동
    // =========================================================

    private void UpdateRandomDirection()
    {
        directionTimer -=
            Time.fixedDeltaTime;

        if (directionTimer <= 0f)
        {
            PickRandomDirection();
        }
    }

    private void PickRandomDirection()
    {
        moveDirection =
            Random.insideUnitCircle.normalized;

        directionTimer =
            changeDirectionTime;
    }

    private void Move(
        Vector2 direction,
        float currentSpeed
    )
    {
        if (direction.sqrMagnitude <= 0.001f)
        {
            return;
        }

        Vector2 nextPosition =
            rb.position
            + direction.normalized
            * currentSpeed
            * Time.fixedDeltaTime;

        rb.MovePosition(
            nextPosition
        );
    }

    private bool IsPlayerInRange(
        float range
    )
    {
        if (player == null)
        {
            return false;
        }

        Vector2 difference =
            (Vector2)player.position
            - rb.position;

        return difference.sqrMagnitude
               <= range * range;
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

    private void UpdateVisual(
        Vector2 direction
    )
    {
        bool isMoving =
            direction.sqrMagnitude
            > 0.001f;

        if (animator != null &&
            animator.enabled)
        {
            animator.SetBool(
                IsMovingHash,
                isMoving
            );
        }

        if (spriteRenderer != null &&
            Mathf.Abs(direction.x) > 0.01f)
        {
            spriteRenderer.flipX =
                direction.x < 0f;
        }
    }

    // =========================================================
    // 벽 탈출
    // =========================================================

    private void OnCollisionEnter2D(
        Collision2D collision
    )
    {
        EscapeFromWall(
            collision
        );
    }

    private void OnCollisionStay2D(
        Collision2D collision
    )
    {
        EscapeFromWall(
            collision
        );
    }

    private void EscapeFromWall(
        Collision2D collision
    )
    {
        if (!IsWall(collision.collider) ||
            collision.contactCount == 0)
        {
            return;
        }

        Vector2 wallNormal =
            collision.GetContact(0).normal;

        Vector2 sideDirection =
            new Vector2(
                -wallNormal.y,
                wallNormal.x
            );

        if (Random.value < 0.5f)
        {
            sideDirection =
                -sideDirection;
        }

        escapeDirection =
            (
                wallNormal
                + sideDirection * 0.5f
            ).normalized;

        moveDirection =
            escapeDirection;

        escapeTimer =
            wallEscapeTime;

        directionTimer =
            changeDirectionTime;
    }

    private bool IsWall(
        Collider2D targetCollider
    )
    {
        Transform current =
            targetCollider.transform;

        while (current != null)
        {
            if (current.CompareTag("Wall"))
            {
                return true;
            }

            current = current.parent;
        }

        return false;
    }
}
