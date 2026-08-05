using UnityEngine;

// 이 컴포넌트가 없으면 작동 못하도록 설정 - 오브젝트가 제대로 작동하기 위해.
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]

// 이 스크립트 자체의 클래스
public class PlayerController : MonoBehaviour
{
    // 플레이어의 움직임을 조정하는 인스펙터
    [Header("움직임")]
    //private이지만 인스펙터에서 조절할 수 있도록 SerializeField사용
    [SerializeField] private float moveSpeed = 5f;

    // 앞,뒤,옆 방향을 const상수로 지정하여 변경불가능(고정)하게
    private const int DIR_DOWN = 0;
    private const int DIR_UP = 1;
    private const int DIR_SIDE = 2;

    // 스크립트가 붙은 오브젝트의 컴포넌트를 사용하기 위해 컴포넌트별 변수 선언
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Vector2 moveInput;
    private int lastFacingDir = DIR_DOWN;
    private bool lastFacingRight = true;

    // 애니메이터 파라미터를 ID로 변환해
    private static readonly int FacingDirHash = Animator.StringToHash("FacingDir");
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private float baseMoveSpeed;
    private Vector2 lastFacingVector =  Vector2.down;

    // 오브젝트가 작동하기 전에 컴포넌트들을 불러오는 Awake함수
    public void Awake()
    {    
        // 기본 이동속도를 저장하여 스탯 적용 오류를 방
        baseMoveSpeed = moveSpeed;

        // 해당 오브젝트의 컴포넌트를 변수에 저장
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // rigidbody에서 freezeRotation을 활성화 하고 중력을 없앰 (2D게임)
        rb.freezeRotation = true;
        rb.gravityScale = 0f;
    }

    // 매 프레임 호출(계속해서 검사해야하는 것들을 위한것)하는 Update함수
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

    // 사용자 입력에 따라 오브젝트가 바라보고 있는 방향을 저장 ( 이동을 멈춰도 바라보고 있던 방향을 바라보게 하기 위해서 )
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

    // 오브젝트가 바라보는 방향을 애니메이터가 인식할 수 있는 값으로 바꾸는 함수
    private void ApplyDirection(Vector2 dir)
    {    
        // 모름
        if (dir.sqrMagnitude < 0.0001f) {
        return;
        }

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

    // 오브젝트가 바라보는 방향대로 애니메이터가 작동하기 위한 함수
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

    // 오브젝트에 아이템 스탯을 적용하는 함수
    public void ApplyItemStats(
    float bonusMoveSpeed
    )
    {    
        // 일단 오브젝트의 이동속도 스탯만 이 스크립트에서 구현
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

    // 오브젝트 현재 이동속도 스탯을 나타내는 함수 ( 스탯창에 필요 )
    public float GetCurrentMoveSpeed()
    {
        return moveSpeed;
    }

    // 오브젝트의 방향을 다른 스크립트( ex/PlayerGun.cs ) 에 전달하기 위한 함수
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
