using UnityEngine;

// 몬스터 종류 3개로 나누기
public enum MonsterType
{
    Rat,
    Cat,
    Ghost
}

// Rigidbody2D 컴포넌트가 없다면 가져오기
[RequireComponent(typeof(Rigidbody2D))]
public class MonsterAI : MonoBehaviour
{
    // 몬스터 종류 - 종류에 따라 인스펙터가 달라짐
    [SerializeField] private MonsterType monsterType;

    // 이동속도 등
    [Header("Common Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float changeDirectionTime = 2f;
    [SerializeField] private float wallEscapeTime = 0.25f;

    // 몬스터가 고양이일 경우 스탯
    [Header("Cat")]
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float chaseRange = 8f;
    [SerializeField] private float catRetreatSpeed = 7f;
    [SerializeField] private float catRetreatDuration = 0.35f;
    [SerializeField] private float catRecoveryDuration = 0.45f;

    // 몬스터가 귀신일 경우 스탯과 투사체 프리팹
    [Header("Ghost")]
    [SerializeField] private float ghostDetectionRange = 12f;
    [SerializeField] private float ghostAttackRange = 8f;
    [SerializeField] private float ghostAttackCooldown = 2f;
    [SerializeField] private GameObject ghostProjectilePrefab;

    // 귀신의 투사체 스폰 좌표 지정
    [Header("Ghost Projectile Spawn")]
    [SerializeField] private Vector2 ghostProjectileOffset = new Vector2(0f, -0.3f);

    // 컴포넌트 변수 선언
    private Rigidbody2D rb;
    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // 움직임 방향 변수
    private Vector2 moveDirection;
    private Vector2 escapeDirection;

    // 타이머 변수
    private float directionTimer;
    private float escapeTimer;
    private float ghostAttackTimer;

    // 고양이 몬스터일 때 후퇴하는 방향 변수
    private Vector2 catRetreatDirection;

    // 고양이 몬스터의 타이머
    private float catRetreatTimer;
    private float catRecoveryTimer;

    // 애니메이션 트리거의 문자열을 Hash로 바꿔서 저장
    private static readonly int IsMovingHash =
        Animator.StringToHash("IsMoving");

    private static readonly int AttackTriggerHash =
        Animator.StringToHash("Attack");

    // 오브젝트들이 로드 되기 전에 컴포넌트를 변수에 저장하고 Rigidbody 설정
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    // 게임이 시작되면 몬스터 종류에 따라 플레이어를 감지하고, 무작위 방향으로 이동
    private void Start()
    {
        FindPlayer();
        PickRandomDirection();

        ghostAttackTimer = ghostAttackCooldown;
    }

    // 몬스터 종류에 따라 매 프레임 마다 플레이어가 어디있는지, 귀신 몬스터인지 확인
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

    // 몬스터들의 물리처리 함수
    private void FixedUpdate()
    {    
        // 플레이어가 없으면 플레이어 찾는 함수 호출
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

        // 이동할 때 방향과 속도 갱신하기 위해 함수 호출
        Move(
            direction,
            currentSpeed
        );

        UpdateVisual(
            direction
        );
    }

    // 어떤 몬스터들이 언제 어떻게 움직일지에 대한 함수
    private void GetMovement(
        out Vector2 direction,
        out float currentSpeed
    )
    {
        // 몬스터 타입에 따른 경우 ( 몬스터마다 스탯이 달라서 나눔 )
        switch (monsterType)
        {    
            // 고양이일 경우 고양이 스탯
            case MonsterType.Cat:
                GetCatMovement(
                    out direction,
                    out currentSpeed
                );
                break;

            // 귀신일 경우 귀신 스탯
            case MonsterType.Ghost:
                GetGhostMovement(
                    out direction,
                    out currentSpeed
                );
                break;

            // 기본적으로 랜덤적인 움직임 스탯
            default:
                GetRandomMovement(
                    out direction,
                    out currentSpeed
                );
                break;
        }
    }

// [ 몬스터 : 1스테이지 골목쥐 ]

    // 랜덤한 이동 ( 몬스터가 쥐 일때 )
    private void GetRandomMovement(
        out Vector2 direction,
        out float currentSpeed
    )
    {
        UpdateRandomDirection();

        direction = moveDirection;
        currentSpeed = moveSpeed;
    }

// [ 몬스터 : 1스테이지 길고양이 ]

    // 고양이 움직임 함수
    private void GetCatMovement(
        out Vector2 direction,
        out float currentSpeed
    )
    {
        // 플레이어가 영역에 들어오면 쫓아가기
        if (IsPlayerInRange(chaseRange))
        {
            direction =
                GetDirectionToPlayer();

            currentSpeed = chaseSpeed;
            return;
        }

        // 플레이어가 영역 밖이면 랜덤하게 돌아다니기
        GetRandomMovement(
            out direction,
            out currentSpeed
        );
    }

    // 고양이가 후퇴하는 패턴 함수
    public void BeginCatRetreat(
    Vector2 playerPosition
)
    {
        // 몬스터가 고양이가 아니면 함수에서 나가기
        if (monsterType !=
            MonsterType.Cat)
        {
            return;
        }

        // 플레이어 위치와 반대 위치 방향
        Vector2 retreatDirection =
            rb.position
            - playerPosition;

        // 반대 위치 방향이 어딘지 계산
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

        // 벡터의 방향만 남기게 정규화
        catRetreatDirection =
            retreatDirection.normalized;

        // 고양이가 공격 후 후퇴하는 시간 조정
        catRetreatTimer =
            Mathf.Max(
                0f,
                catRetreatDuration
            );

        // 다시 공격을 재게하는 시간 조정
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

// [ 몬스터 : 1스테이지 하수도귀신 ]

    // 귀신 움직임 함수
    private void GetGhostMovement(
        out Vector2 direction,
        out float currentSpeed
    )
    {    
        // 플레이어가 영역에 들어오면 추적
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

    // 플레이어가 있는 방향으로 가는 함수
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

    // 귀신 공격 함수
    private void UpdateGhostAttack()
    {
        // 귀신 공격 쿨타임 계산
        ghostAttackTimer -= Time.deltaTime;

        // 플레이어가 영역 밖이면 함수에서 나오기
        if (!IsPlayerInRange(ghostAttackRange))
        {
            return;
        }

        // 아직 쿨타임이면 함수에서 나오기
        if (ghostAttackTimer > 0f)
        {
            return;
        }

        // 귀신이 투사체를 던지는 공격 함수 호출
        ShootGhostProjectile();

        ghostAttackTimer =
            ghostAttackCooldown;
    }

    // 귀신이 투사체 던지는 공격 함수
    private void ShootGhostProjectile()
    {
        // 귀신 투사체 프리팹이 연결되지 않으면 오류 콘솔 로그 출력
        if (ghostProjectilePrefab == null)
        {
            Debug.LogWarning(
                $"{gameObject.name}: " +
                "Ghost Projectile Prefab이 연결되지 않았습니다."
            );

            return;
        }

        // 플레이어가 연결 안되어 있으면 함수에서 나가기
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

        // 투사체 발사 방향
        Vector2 shootDirection =
            GetDirectionToPlayer();

        // 투사체 스폰 위치
        Vector3 spawnPosition =
            transform.position
            + (Vector3)ghostProjectileOffset;

        // 투사체 프리팹을 씬에 생성
        GameObject projectileObject =
            Instantiate(
                ghostProjectilePrefab,
                spawnPosition,
                Quaternion.identity
            );

        // 투사체의 GhostProjectile 컴포넌트 가져오기
        GhostProjectile projectile =
            projectileObject.GetComponent<GhostProjectile>();

        // 투사체 연결 안되어 있으면 오류 콘솔로그 출력
        if (projectile == null)
        {
            Debug.LogError(
                $"{ghostProjectilePrefab.name}에 " +
                "GhostProjectile 컴포넌트가 없습니다."
            );

            // 충돌이나 트리거 일어나면 투사체 파괴
            Destroy(projectileObject);
            return;
        }

        // 투사체 발사 방향 초기화
        projectile.Initialize(
            shootDirection
        );
    }

    // [ 몬스터들 공통 ]

    // 계속 랜덤 방향으로 이동하도록
    private void UpdateRandomDirection()
    {
        directionTimer -=
            Time.fixedDeltaTime;

        if (directionTimer <= 0f)
        {
            PickRandomDirection();
        }
    }

    // 랜덤적으로 방향을 결정
    private void PickRandomDirection()
    {
        moveDirection =
            Random.insideUnitCircle.normalized;

        directionTimer =
            changeDirectionTime;
    }

    // 방향과 속도에 따라 움직이기
    private void Move(
        Vector2 direction,
        float currentSpeed
    )
    {
        // 방향과 속도에 따른 움직임 계산
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

    // 플레이어가 몬스터 인식 영역에 있는지 여부
    private bool IsPlayerInRange(
        float range
    )
    {
        if (player == null)
        {
            return false;
        }

        // 몬스터에 대한 플레이어 위치가 어딘지 계산
        Vector2 difference =
            (Vector2)player.position
            - rb.position;

        return difference.sqrMagnitude
               <= range * range;
    }

    // 플레이어가 어디에 있는지 없는지 찾는 함수
    private void FindPlayer()
    {
        // Tag가 Player인 오브젝트 찾기
        GameObject playerObject =
            GameObject.FindGameObjectWithTag(
                "Player"
            );

        // Player의 transform 컴포넌트 없으면 가져오기
        if (playerObject != null)
        {
            player =
                playerObject.transform;
        }
    }

    // 움직임, 바라보는 방향에 따른 애니메이션 적용
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

   // 충돌이 일어났을 때
    private void OnCollisionEnter2D(
        Collision2D collision
    )
    {
        // 벽에서 탈출하는 함수
        EscapeFromWall(
            collision
        );
    }

    // 충돌이 일어나고 있는 중이여도
    private void OnCollisionStay2D(
        Collision2D collision
    )
    {
        // 벽에서 탈출
        EscapeFromWall(
            collision
        );
    }

    // 벽에 낑기지 않기 위해 벽에서 탈출하는 함수
    private void EscapeFromWall(
        Collision2D collision
    )
    {
        // 벽에 Collision이 없으면 함수에서 나가기
        if (!IsWall(collision.collider) ||
            collision.contactCount == 0)
        {
            return;
        }

        // 벽의 방향 체크..? 모름
        Vector2 wallNormal =
            collision.GetContact(0).normal;

        Vector2 sideDirection =
            new Vector2(
                -wallNormal.y,
                wallNormal.x
            );

        // 이건 또 뭐지
        if (Random.value < 0.5f)
        {
            sideDirection =
                -sideDirection;
        }

        // 벽에서 탈출하는 방향과 시간 계산
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

    // 벽인지 아닌지 검사하는 함수
    private bool IsWall(
        Collider2D targetCollider
    )
    {
        Transform current =
            targetCollider.transform;

        // 태그가 Wall 이여야 벽으로 인식
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
