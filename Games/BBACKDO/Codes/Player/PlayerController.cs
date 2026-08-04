using UnityEngine;

//이 컴포넌트가 없으면 작동못하도록 설정 - 오브젝트가 제대로 작동하기 위해.
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]

//이 스크립트 자체의 클래스
public class PlayerController : MonoBehaviour
{
    //플레이어의 움직임을 조정하는 인스펙터
    [Header("움직임")]
    //private이지만 인스펙터에서 조절할 수 있도록 SerializeField사용
    [SerializeField] private float moveSpeed = 5f;

    //앞,뒤,옆 방향을 const상수로 지정하여 변경불가능(고정)하게
    private const int DIR_DOWN = 0;
    private const int DIR_UP = 1;
    private const int DIR_SIDE = 2;

    //스크립트가 붙은 오브젝트의 컴포넌트를 사용하기 위해 컴포넌트별 변수 선언
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Vector2 moveInput;
    private int lastFacingDir = DIR_DOWN;
    private bool lastFacingRight = true;

    // [지피티가 써준 주석]사격 중일 때만 값이 들어있고, 아니면 null (이동 방향보다 우선순위 높음) - 이건 대체 뭐임..? 2차원벡터 타입의 머 총쏘는 방향 변수?..
    private Vector2? shootDirectionOverride;

    // 움직임 방향과 속도를 정적변수로 선언
    private static readonly int FacingDirHash = Animator.StringToHash("FacingDir");
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private float baseMoveSpeed;
    private Vector2 lastFacingVector =  Vector2.down;

    // 오브젝트가 작동하기 전에 컴포넌트들을 불러오는 Awake함수
    public void Awake()
    {    
        // 해당 오브젝트의 컴포넌트를 변수에 저장
        baseMoveSpeed = moveSpeed;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // rigidbody에서 freezeRotation을 활성화 하고 중력을 없앰 (2D게임)
        rb.freezeRotation = true;
        rb.gravityScale = 0f;
    }

    //매 프레임 호출(계속해서 검사해야하는 것들을 위한것)하는 Update함수
    private void Update()
    {    
        // 일시정지창
        if (PauseMenu.IsPaused)
        {
            return;
        }

        // 사용자 입력에 따른 오브젝트의 x축, y축 이동을 변수의 저장
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        // 그 변수들을 움직임 입력 변수에 저장
        moveInput = new Vector2(x, y).normalized;

        // 오브젝트의 방향과 애니메이터 함수를 호출
        UpdateFacingDirection();
        UpdateAnimator();
    }

    // 물리현상 구현하는 FixedUpdate함수
    private void FixedUpdate()
    {
        // 오브젝트가 어떤 위치에서 어떤 속도로 어디로 이동할건지 구현
        Vector2 nextPosition = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(nextPosition);
    }

    /// <summary>총 스크립트가 발사 중일 때 매 프레임 호출. dir = 캐릭터→마우스 방향</summary>
    public void SetShootDirection(Vector2 dir)
    {
        shootDirectionOverride = dir;
    }

    /// <summary>발사 멈추면 총 스크립트가 호출</summary>
    public void ClearShootDirection()
    {
        shootDirectionOverride = null;
    }

    private void UpdateFacingDirection()
    {
        if (moveInput.sqrMagnitude < 0.01f)
        {
            return;
        }

        lastFacingVector =
            moveInput.normalized;

        ApplyDirection(
            lastFacingVector
        );
    }

    private void ApplyDirection(Vector2 dir)
    {
        if (dir.sqrMagnitude < 0.0001f) return;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            lastFacingDir = DIR_SIDE;
            lastFacingRight = dir.x > 0f;
        }
        else
        {
            lastFacingDir = dir.y > 0f ? DIR_UP : DIR_DOWN;
        }
    }

    private void UpdateAnimator()
    {
        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        animator.SetBool(IsMovingHash, isMoving);
        animator.SetInteger(FacingDirHash, lastFacingDir);

        if (lastFacingDir == DIR_SIDE)
        {
            spriteRenderer.flipX = lastFacingRight;
        }
    }
    public void ApplyItemStats(
    float bonusMoveSpeed
)
    {
        moveSpeed =
            Mathf.Max(
                0f,
                baseMoveSpeed
                + bonusMoveSpeed
            );

        Debug.Log(
            $"플레이어 이동 속도 적용: {moveSpeed:0.##}"
        );
    }

    public float GetCurrentMoveSpeed()
    {
        return moveSpeed;
    }
    public Vector2 GetFacingDirection()
    {
        if (lastFacingVector.sqrMagnitude
            < 0.001f)
        {
            return Vector2.down;
        }

        return lastFacingVector.normalized;
    }
}
