using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("방 설정")]
    [SerializeField] private Transform cameraPoint;
    [SerializeField] private RoomBattle roomBattle;

    [Header("스테이지 정보")]
    [SerializeField] private int stageIndex;

    [Header("지도 설정")]
    [SerializeField] private Vector2Int mapPosition;
    [SerializeField] private bool isBossRoom;
    [SerializeField] private Room[] connectedRooms;

    private MonsterAI[] monsterAIs;
    private MonsterAttack[] monsterAttacks;
    private BossBase[] bosses;
    private Rigidbody2D[] monsterRigidbodies;

    private bool isDiscovered;
    private bool isVisited;

    public Transform CameraPoint => cameraPoint;
    public int StageIndex => stageIndex;

    public Vector2Int MapPosition => mapPosition;
    public bool IsBossRoom => isBossRoom;
    public bool IsDiscovered => isDiscovered;
    public bool IsVisited => isVisited;

    public bool IsCleared =>
        roomBattle == null || roomBattle.IsCleared;

    private void Awake()
    {
        FindRoomMonsters();

        SetMonstersActive(false);
    }

    public void Visit()
    {
        isDiscovered = true;
        isVisited = true;
    }

    public void DiscoverConnectedRooms()
    {
        if (connectedRooms == null)
        {
            return;
        }

        foreach (Room connectedRoom in connectedRooms)
        {
            if (connectedRoom == null)
            {
                continue;
            }

            connectedRoom.Discover();
        }
    }

    private void Discover()
    {
        isDiscovered = true;
    }

    private void FindRoomMonsters()
    {
        monsterAIs =
            GetComponentsInChildren<MonsterAI>(true);

        monsterAttacks =
            GetComponentsInChildren<MonsterAttack>(true);

        bosses =
            GetComponentsInChildren<BossBase>(true);

        Monster[] monsters =
            GetComponentsInChildren<Monster>(true);

        monsterRigidbodies =
            new Rigidbody2D[monsters.Length];

        for (int i = 0; i < monsters.Length; i++)
        {
            monsterRigidbodies[i] =
                monsters[i].GetComponent<Rigidbody2D>();
        }
    }

    public void SetMonstersActive(bool active)
    {
        if (monsterAIs == null ||
            monsterAttacks == null ||
            bosses == null ||
            monsterRigidbodies == null)
        {
            FindRoomMonsters();
        }

        SetMonsterAIActive(active);
        SetMonsterAttackActive(active);
        SetBossActive(active);

        if (!active)
        {
            StopMonsterMovement();
        }

        Debug.Log(
            gameObject.name
            + " 몬스터 "
            + (active ? "활성화" : "비활성화")
        );
    }

    private void SetMonsterAIActive(bool active)
    {
        if (monsterAIs == null)
        {
            return;
        }

        foreach (MonsterAI monsterAI in monsterAIs)
        {
            if (monsterAI == null)
            {
                continue;
            }

            monsterAI.enabled = active;
        }
    }

    private void SetMonsterAttackActive(bool active)
    {
        if (monsterAttacks == null)
        {
            return;
        }

        foreach (MonsterAttack monsterAttack in monsterAttacks)
        {
            if (monsterAttack == null)
            {
                continue;
            }

            monsterAttack.enabled = active;
        }
    }

    private void SetBossActive(bool active)
    {
        if (bosses == null)
        {
            return;
        }

        foreach (BossBase boss in bosses)
        {
            if (boss == null)
            {
                continue;
            }

            boss.enabled = active;
        }
    }

    private void StopMonsterMovement()
    {
        if (monsterRigidbodies == null)
        {
            return;
        }

        foreach (Rigidbody2D monsterRigidbody
                 in monsterRigidbodies)
        {
            if (monsterRigidbody == null)
            {
                continue;
            }

            monsterRigidbody.linearVelocity =
                Vector2.zero;

            monsterRigidbody.angularVelocity =
                0f;
        }
    }

    [ContextMenu("몬스터 목록 다시 찾기")]
    private void RefreshMonsterList()
    {
        FindRoomMonsters();

        Debug.Log(
            gameObject.name
            + "의 몬스터 목록을 다시 찾았습니다."
        );
    }
}
