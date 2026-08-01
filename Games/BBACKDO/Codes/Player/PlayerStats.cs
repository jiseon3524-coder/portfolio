using System;
using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("체력 설정")]
    [SerializeField] private int maxHp = 100;
    [SerializeField] private float invincibleTime = 1f;

    [Header("게임오버")]
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerGun playerGun;

    private Rigidbody2D playerRigidbody;
    private DamageFlash damageFlash;

    private int baseMaxHp;
    private float baseInvincibleTime;

    private int currentHp;
    private bool isInvincible;
    private bool isDead;

    public int CurrentHp => currentHp;
    public int MaxHp => maxHp;
    public bool IsDead => isDead;

    public event Action<int, int> OnHealthChanged;

    private void Awake()
    {
        baseMaxHp = maxHp;
        baseInvincibleTime = invincibleTime;

        currentHp = maxHp;

        playerRigidbody =
            GetComponent<Rigidbody2D>();

        damageFlash =
            GetComponent<DamageFlash>();
    }

    private void Start()
    {
        NotifyHealthChanged();
    }

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

    public void TakeDamage(
        int damage
    )
    {
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

        NotifyHealthChanged();

        Debug.Log(
            "플레이어 피격. 남은 체력: " +
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
                .PlayPlayerHit();
        }

        StartCoroutine(
            InvincibleRoutine()
        );
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        Debug.Log("플레이어 사망");

        if (SFXManager.Instance != null)
        {
            SFXManager.Instance
                .PlayPlayerDeath();
        }

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

        GameProgress.SaveCurrentStage();

        if (ItemManager.Instance != null)
        {
            ItemManager.Instance
                .ResetRunItems();
        }

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

    public bool Heal(
        int healAmount
    )
    {
        if (isDead ||
            healAmount <= 0 ||
            currentHp >= maxHp)
        {
            return false;
        }

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

        NotifyHealthChanged();

        Debug.Log(
            $"플레이어 체력 회복: +{actualHealAmount} / " +
            $"현재 HP: {currentHp} / {maxHp}"
        );

        return actualHealAmount > 0;
    }

    private void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke(
            currentHp,
            maxHp
        );
    }
}
