using UnityEngine;
using UnityEngine.UI;

public class PlayerGun : MonoBehaviour
{
    [Header("필수 연결")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject boomBulletPrefab;
    [SerializeField] private PlayerController playerController;

    [Header("총알 HUD")]
    [SerializeField] private Image bulletIcon;
    [SerializeField] private Sprite normalBulletIcon;
    [SerializeField] private Sprite boomBulletIcon;

    [Header("기본 공격 설정")]
    [SerializeField] private int baseDamage = 10;
    [SerializeField] private float baseFireCooldown = 0.2f;
    [SerializeField] private float baseProjectileSpeed = 10f;
    [SerializeField] private float baseAttackRange = 8f;
    [SerializeField] private float baseProjectileSize = 1f;
    [SerializeField] private int baseProjectileCount = 1;

    [Header("FirePoint 위치")]
    [SerializeField] private float firePointDistance = 0.4f;

    [Header("다중 발사")]
    [SerializeField] private float spreadAngle = 12f;

    private GameObject currentBulletPrefab;

    private float lastFireTime;
    private bool canShoot = true;
    private bool isBoomShot;

    private int currentDamage;
    private float currentFireCooldown;
    private float currentProjectileSpeed;
    private float currentAttackRange;
    private float currentProjectileSize;
    private int currentProjectileCount;

    public int CurrentDamage => currentDamage;
    public float CurrentFireCooldown => currentFireCooldown;
    public float CurrentProjectileSpeed => currentProjectileSpeed;
    public float CurrentAttackRange => currentAttackRange;
    public float CurrentProjectileSize => currentProjectileSize;
    public int CurrentProjectileCount => currentProjectileCount;
    public bool IsBoomShot => isBoomShot;

    private void Awake()
    {
        if (playerController == null)
        {
            playerController =
                GetComponent<PlayerController>();
        }

        // 기본 총알 아이콘이 따로 연결되지 않았다면
        // 현재 BulletIcon의 이미지를 기본 아이콘으로 사용한다.
        if (normalBulletIcon == null &&
            bulletIcon != null)
        {
            normalBulletIcon =
                bulletIcon.sprite;
        }

        ResetToBaseStats();
    }

    private void Update()
    {
        if (PauseMenu.IsPaused)
        {
            return;
        }

        HandleShoot();
    }

    private void ResetToBaseStats()
    {
        currentDamage =
            baseDamage;

        currentFireCooldown =
            baseFireCooldown;

        currentProjectileSpeed =
            baseProjectileSpeed;

        currentAttackRange =
            baseAttackRange;

        currentProjectileSize =
            baseProjectileSize;

        currentProjectileCount =
            baseProjectileCount;

        isBoomShot = false;

        RefreshBulletType();
    }

    public void ApplyItemStats(
        ItemStatTotal itemStats
    )
    {
        currentDamage =
            Mathf.Max(
                0,
                baseDamage
                + itemStats.attackDamage
            );

        float attackSpeedMultiplier =
            Mathf.Max(
                0.1f,
                1f + itemStats.attackSpeed
            );

        currentFireCooldown =
            baseFireCooldown
            / attackSpeedMultiplier;

        currentProjectileSpeed =
            Mathf.Max(
                0.1f,
                baseProjectileSpeed
                + itemStats.projectileSpeed
            );

        currentAttackRange =
            Mathf.Max(
                0.1f,
                baseAttackRange
                + itemStats.attackRange
            );

        currentProjectileSize =
            Mathf.Max(
                0.1f,
                baseProjectileSize
                + itemStats.projectileSize
            );

        currentProjectileCount =
            Mathf.Max(
                1,
                baseProjectileCount
                + itemStats.projectileCount
            );

        // Item_3042 장착 여부
        isBoomShot =
            itemStats.boomShot;

        RefreshBulletType();

        Debug.Log(
            $"총 아이템 스탯 적용 - " +
            $"공격력: {currentDamage}, " +
            $"쿨타임: {currentFireCooldown:0.###}, " +
            $"탄속: {currentProjectileSpeed:0.##}, " +
            $"사거리: {currentAttackRange:0.##}, " +
            $"크기: {currentProjectileSize:0.##}, " +
            $"발사체 수: {currentProjectileCount}, " +
            $"폭발탄: {isBoomShot}"
        );
    }

    private void RefreshBulletType()
    {
        bool useBoomBullet =
            isBoomShot &&
            boomBulletPrefab != null;

        currentBulletPrefab =
            useBoomBullet
                ? boomBulletPrefab
                : bulletPrefab;

        if (isBoomShot &&
            boomBulletPrefab == null)
        {
            Debug.LogWarning(
                "PlayerGun의 Boom Bullet Prefab이 연결되지 않았습니다."
            );
        }

        if (bulletIcon == null)
        {
            return;
        }

        Sprite targetIcon =
            useBoomBullet
                ? boomBulletIcon
                : normalBulletIcon;

        if (targetIcon != null)
        {
            bulletIcon.sprite =
                targetIcon;
        }

        bulletIcon.preserveAspect = true;
    }

    public void SetCanShoot(
        bool value
    )
    {
        canShoot = value;
    }

    private void HandleShoot()
    {
        if (!canShoot ||
            playerController == null)
        {
            return;
        }

        bool isFiring =
            Input.GetMouseButton(0);

        if (!isFiring)
        {
            return;
        }

        if (Time.time <
            lastFireTime
            + currentFireCooldown)
        {
            return;
        }

        lastFireTime =
            Time.time;

        Fire();
    }

    private void Fire()
    {
        if (currentBulletPrefab == null ||
            playerController == null)
        {
            return;
        }

        Vector2 facingDirection =
            playerController
                .GetFacingDirection();

        int projectileCount =
            Mathf.Max(
                1,
                currentProjectileCount
            );

        bool spawnedBullet = false;

        if (projectileCount == 1)
        {
            spawnedBullet =
                SpawnBullet(
                    facingDirection
                );
        }
        else
        {
            float totalAngle =
                spreadAngle
                * (projectileCount - 1);

            float startAngle =
                -totalAngle * 0.5f;

            for (int i = 0;
                 i < projectileCount;
                 i++)
            {
                float angle =
                    startAngle
                    + spreadAngle * i;

                Vector2 projectileDirection =
                    Quaternion.Euler(
                        0f,
                        0f,
                        angle
                    )
                    * facingDirection;

                if (SpawnBullet(
                    projectileDirection
                ))
                {
                    spawnedBullet = true;
                }
            }
        }

        if (spawnedBullet &&
            SFXManager.Instance != null)
        {
            SFXManager.Instance
                .PlayGunShot();
        }
    }

    private bool SpawnBullet(
        Vector2 direction
    )
    {
        if (currentBulletPrefab == null)
        {
            return false;
        }

        Vector2 normalizedDirection =
            direction.normalized;

        Vector3 spawnPosition =
            transform.position
            + (Vector3)(
                normalizedDirection
                * firePointDistance
            );

        GameObject bulletObject =
            Instantiate(
                currentBulletPrefab,
                spawnPosition,
                Quaternion.identity
            );

        Bullet bullet =
            bulletObject.GetComponent<Bullet>();

        if (bullet == null)
        {
            Debug.LogError(
                currentBulletPrefab.name +
                " 프리팹에 Bullet 컴포넌트가 없습니다."
            );

            Destroy(bulletObject);
            return false;
        }

        bullet.Init(
            normalizedDirection,
            currentDamage,
            currentProjectileSpeed,
            currentAttackRange,
            currentProjectileSize
        );

        return true;
    }
}
