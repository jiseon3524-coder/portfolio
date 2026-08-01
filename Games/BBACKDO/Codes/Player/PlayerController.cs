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

    //오브젝트가 계속 땅으로 떨어지는걸 막는 rigidbody2D 컴포넌트 타입의 변수 선언 - 변수를 굳이..?
    private Rigidbody2D rb;

    //오브젝트가 카메라에 보일 수 있도록 하는 SpriteRenderer 컴포넌트 타입의 변수 선언 - 굳이 변수?
    private SpriteRenderer spriteRenderer;

    //오브젝트의 움직임을 구현할 수 있는 Animator 컴포넌트 타입의 변수 선언 - 변수를 따로 선언한 이유가 뭘까..?
    private Animator animator;

    //2D이기 때문에 2차원 벡터 타입의 플레이어의 입력값 변수를 선언 - 플레이어의 입력에 따라 움직여야 하기 때문에 필요
    private Vector2 moveInput;

    //마지막으로 오브젝트가 마주한 방향 변수를 아래방향으로 설정 - 왠지는 모르겟
    private int lastFacingDir = DIR_DOWN;
    //마지막으로 오브젝트가 제대로 된 방향으로 갔을 경우 변수를 true로 선언 - 아마 오브젝트가 올바르게 작동하지 못했을때에 대비한게아닐지..?
    private bool lastFacingRight = true;

    // [지피티가 써준 주석]사격 중일 때만 값이 들어있고, 아니면 null (이동 방향보다 우선순위 높음) - 이건 대체 뭐임..? 2차원벡터 타입의 머 총쏘는 방향 변수?..
    private Vector2? shootDirectionOverride;

    //정적이고 바꿀 수 없는 FacingDirHash변수를 선언해 애니메이터와 연결..? - 진심 모르겟어
    private static readonly int FacingDirHash = Animator.StringToHash("FacingDir");
    //이것도 마찬가지. 걍 변수의 의미를 모르겟어
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private float baseMoveSpeed;

    private Vector2 lastFacingVector =  Vector2.down;

    //유니티 픽스드파이프펑션 이거 공부를 안해서 심각하다.. start가 아닌 awake를 통해 다른 스크립트 참조할 수 잇엇고 시작할때 한 번 호출되는 함수이다.
    //start를 쓰면 게임플레이 중에는 활성화 되지 않지만, awake를 쓰면 활성화된 프리팹? 오브젝트에는 스크립트가 다시 호출된다고 알고있음
    public void Awake()
    {
        baseMoveSpeed = moveSpeed;
        //여기서 rigidbody타입의 변수로 컴포넌트 정보? 가져오기
        rb = GetComponent<Rigidbody2D>();
        // 마찬가지
        spriteRenderer = GetComponent<SpriteRenderer>();
        // 마찬가지
        animator = GetComponent<Animator>();

        //rigidbody 컴포넌트에서 z회전을 막기 위해(2d게임이니까) freezeRotation 가능하도록 - 근데 변수를 써야햇나 굳이
        rb.freezeRotation = true;
        //rigidbody 컴포넌트에서 중력크기를 0으로 설정. 이것도 2d게임이므로 중력이 필요 없기 때문에 설정
        rb.gravityScale = 0f;
    }

    //매 프레임 호출(계속해서 검사해야하는 것들을 위한것)하는 Update함수
    private void Update()
    {
        if (PauseMenu.IsPaused)
        {
            return;
        }

        //사용자 입력에서 오른쪽 왼쪽 즉 x축 입력을 1, -1로 방향만 받아서, 거기에 속도를 곱해주는것
        float x = Input.GetAxisRaw("Horizontal");

        //사용자 입력에서 위 아래 즉 y축 입력을 1, -1로 방향만 받아서, 거기에 속도를 곱해주는것
        float y = Input.GetAxisRaw("Vertical");

        // 여기서 1,-1로 방향만 받기 위해 벡터의 정규화를 함 - 근데 new는 뭐고 .은 뭐지..?
        moveInput = new Vector2(x, y).normalized;

        //오브젝트의 방향과 애니메이션을 계속 새롭게 업데이트? 함 - 이미 Update함수안에 잇는데 왜 이렇게 한거지?
        UpdateFacingDirection();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
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
