using UnityEngine;
using UnityEngine.UI;

public class PlayerGun : MonoBehaviour
{    

    // 이 스크립트가 필수로 참조해야 하는 오브젝트 또는 스크립트
    [Header("필수 연결")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject boomBulletPrefab;
    [SerializeField] private PlayerController playerController;

    // PlayerGun 오브젝트에서 생성되는 총알의 UI
    [Header("총알 HUD")]
    [SerializeField] private Image bulletIcon;
    [SerializeField] private Sprite normalBulletIcon;
    [SerializeField] private Sprite boomBulletIcon;

    // 총 공격 기본 스탯
    [Header("기본 공격 설정")]
    [SerializeField] private int baseDamage = 10;
    [SerializeField] private float baseFireCooldown = 0.2f;
    [SerializeField] private float baseProjectileSpeed = 10f;
    [SerializeField] private float baseAttackRange = 8f;
    [SerializeField] private float baseProjectileSize = 1f;
    [SerializeField] private int baseProjectileCount = 1;

    // 플레이어 기준 총알이 생성되는 위치
    [Header("총알 생성 위치")]
    [SerializeField] private float firePointDistance = 0.4f;

    // 총알이 여러 개인 경우 발사되는 범위
    [Header("다중 발사")]
    [SerializeField] private float spreadAngle = 12f;

    // PlayerGun의 현재 스탯, 총알 등을 변수에 저장
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

    // 다른 스크립에서 현재 스탯을 읽을 수 있도록
    public int CurrentDamage => currentDamage;
    public float CurrentFireCooldown => currentFireCooldown;
    public float CurrentProjectileSpeed => currentProjectileSpeed;
    public float CurrentAttackRange => currentAttackRange;
    public float CurrentProjectileSize => currentProjectileSize;
    public int CurrentProjectileCount => currentProjectileCount;
    public bool IsBoomShot => isBoomShot;

    // 오브젝트가 실행되기 전 세팅
    private void Awake()
    {    
        // PlayerController 스크립트가 없으면 가져오는 안전코드
        if (playerController == null)
        {
            playerController =
                GetComponent<PlayerController>();
        }

        // 기본 총알 아이콘이 따로 연결되지 않았다면 현재 BulletIcon의 이미지를 기본 아이콘으로 사용
        if (normalBulletIcon == null &&
            bulletIcon != null)
        {
            normalBulletIcon =
                bulletIcon.sprite;
        }
        
        // 스탯 초기화 함수 호출
        ResetToBaseStats();
    }

    // 일시정지( ESC창 ) 상태에서 멈추기
    private void Update()
    {
        if (PauseMenu.IsPaused)
        {
            return;
        }

        HandleShoot();
    }

    // 게임 시작(재시작) 시에 기본 스탯으로 초기화
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

        // 총알 종류 갱신
        RefreshBulletType();
    }

    // 아이템에 따라 스탯 변화를 적용 ( 기존스탯에 더해진 추가스탯을 계산하는 방식 )
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

        // 총알 종류 갱신
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

    // 총알 종류를 갱신하는 함수 ( 총알 종류가 여러 개인 게임이기에 )
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

        // 총알 종류에 따른 아이콘도 갱신
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

    // 총을 쏠수 있는 조건인지 아닌지를 변수에 저장
    public void SetCanShoot(
        bool value
    )
    {
        canShoot = value;
    }

    // 총알 발사 조건을 다루는 함수
    private void HandleShoot()
    {
        if (!canShoot ||
            playerController == null)
        {
            return;
        }

        // 마우스 왼쪽 버튼 누르면 발사
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

        // 총알 발사 방식 함수를 호출
        Fire();
    }

    // 총알이 얼마나 어떻게 나갈지를 다루는 함수
    private void Fire()
    {
        if (currentBulletPrefab == null ||
            playerController == null)
        {
            return;
        }

        // 총알 갯수, 발사 방향을 정함
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

        // 총알 발사시 효과음 적용
        if (spawnedBullet &&
            SFXManager.Instance != null)
        {
            SFXManager.Instance
                .PlayGunShot();
        }
    }

    // 총알 프리팹이 생성될때를 다루는 함수 즉, 총알이 진짜 생성되게 하는 함수
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

        // 발사되는 총알의 스탯을 전
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
