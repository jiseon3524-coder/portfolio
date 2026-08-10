using System;
using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // 플레이어의 체력과 무적시간
    [Header("체력 설정")]
    [SerializeField] private int maxHp = 100;
    [SerializeField] private float invincibleTime = 1f;

    // 플레이어 스탯이 참조할 스크립트
    [Header("필수 연결")]
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerGun playerGun;

    private Rigidbody2D playerRigidbody;
    private DamageFlash damageFlash;

    // 기본 체력과 무적시간과 스탯에 따라 바뀐 체력과 무적시간
    private int baseMaxHp;
    private float baseInvincibleTime;

    private int currentHp;
    private bool isInvincible;
    private bool isDead;

    // 다른 스크립트에서 읽을 수 있도록
    public int CurrentHp => currentHp;
    public int MaxHp => maxHp;
    public bool IsDead => isDead;

    // 체력 변화<현재HP, 최대HP> 이벤트
    public event Action<int, int> OnHealthChanged;

    // 게임 시작 시 플레이어 스탯을 초기화하고 컴포넌트 가져오는 함수
    private void Awake()
    {    

        // 시작 HP와 무적시간 가져오기
        baseMaxHp = maxHp;
        baseInvincibleTime = invincibleTime;

        // 시작HP를 현재HP로 저장
        currentHp = maxHp;

        // 컴포넌트 변수에 저장하기
        playerRigidbody =
            GetComponent<Rigidbody2D>();

        damageFlash =
            GetComponent<DamageFlash>();
    }

    // 시작 체력이 UI에도 적용되기 위해 알리는 함수
    private void Start()
    {
        NotifyHealthChanged();
    }

    // 플레이어 스탯에 장착한 아이템 스탯을 적용
    public void ApplyItemStats(
        int bonusMaxHp,
        float bonusInvincibleTime
    )
    {
        maxHp =
            Mathf.Max(
                1,
                baseMaxHp + bonusMaxHp
            );

        invincibleTime =
            Mathf.Max(
                0f,
                baseInvincibleTime
                + bonusInvincibleTime
            );

        currentHp =
            Mathf.Clamp(
                currentHp,
                0,
                maxHp
            );

        NotifyHealthChanged();

        Debug.Log(
            $"플레이어 아이템 스탯 적용 - " +
            $"최대 체력: {maxHp}, " +
            $"무적 시간: {invincibleTime:0.##}"
        );
    }

    //  플레이어가 데미지를 입어서 HP가 깎이는 함수
    public void TakeDamage(
        int damage
    )
    {    
        // 죽었거나 무적상태거나 피해량이 0이하면 종료
        if (isDead ||
            isInvincible ||
            damage <= 0)
        {
            return;
        }

        currentHp =
            Mathf.Max(
                0,
                currentHp - damage
            );
        
        // 체력 변화를 알리는 함수를 호출하여 플레이어의 체력이 변화했는지 UI에 전달
        NotifyHealthChanged();

        Debug.Log(
            "플레이어 피격. 남은 체력: " +
            currentHp
        );

        // 플레이어가 데미지를 입었을때 DamageFlash.cs를 통해 효과
        if (damageFlash != null)
        {
            damageFlash.Play();
        }

        // 현재HP가 0이하가 되면 죽음
        if (currentHp <= 0)
        {
            Die();
            return;
        }

        // 데미지를 입을 때 사운드
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance
                .PlayPlayerHit();
        }

        // 무적시간 코루틴 함수를 호출하여 0.1초(예시)마다 계속 데미지를 받거나 하는 상황이 없게
        StartCoroutine(
            InvincibleRoutine()
        );
    }

    // 플레이어가 죽었을 경우( HP가 0이하일 경우 )의 함수
    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        Debug.Log("플레이어 사망");

        // 플레이어 죽을 때 사운드
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance
                .PlayPlayerDeath();
        }

        // 플레이어 관련된 스크립트가 있으면 비활성화
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        if (playerGun != null)
        {
            playerGun.enabled = false;
        }

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity =
                Vector2.zero;
        }

        // 현재 스테이지를 저장하는 함수를 GameProgress.cs에서 가져오기
        GameProgress.SaveCurrentStage();

        // 죽었으면 아이템 초기화
        if (ItemManager.Instance != null)
        {
            ItemManager.Instance
                .ResetRunItems();
        }

        // 게임오버창 보여주기
        if (gameOverUI != null)
        {
            gameOverUI.ShowGameOver();
        }
        else
        {
            Debug.LogWarning(
                "PlayerStats에 GameOverUI가 연결되지 않았습니다."
            );
        }
    }

    // 플레이어 무적시간을 구현하는 코루틴 함수
    private IEnumerator InvincibleRoutine()
    {
        isInvincible = true;

        Debug.Log("플레이어 무적 시작");

        yield return new WaitForSeconds(
            invincibleTime
        );

        if (!isDead)
        {
            isInvincible = false;

            Debug.Log("플레이어 무적 끝");
        }
    }

    // 플레이어 체력 회복 함수
    public bool Heal(
        int healAmount
    )
    {

        // 죽었거나 체력이 꽉차있으면 회복 안됨
        if (isDead ||
            healAmount <= 0 ||
            currentHp >= maxHp)
        {
            return false;
        }

        // 체력 회복시 기존 체력에 얼만큼 더해졌는지 계산하는 방식
        int previousHp =
            currentHp;

        currentHp +=
            healAmount;

        currentHp =
            Mathf.Min(
                currentHp,
                maxHp
            );

        int actualHealAmount =
            currentHp - previousHp;

        // 체력이 바뀜을 UI변화를 위해 알리는 함수 호출
        NotifyHealthChanged();

        Debug.Log(
            $"플레이어 체력 회복: +{actualHealAmount} / " +
            $"현재 HP: {currentHp} / {maxHp}"
        );

        return actualHealAmount > 0;
    }

    // 체력이 변화한걸 UI변화를 위해 알리는 함수
    private void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke(
            currentHp,
            maxHp
        );
    }
}
