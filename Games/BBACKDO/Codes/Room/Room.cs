using UnityEngine;

public class Room : MonoBehaviour
{

    // 각 방마다 있어야 하는 카메라포인트와 룸배틀 컴포넌트
    [Header("방 설정")]
    [SerializeField] private Transform cameraPoint;
    [SerializeField] private RoomBattle roomBattle;

    // 현재 스테이지의 번호
    [Header("스테이지 정보")]
    [SerializeField] private int stageIndex;

    // UI패널에 있는 지도를 위한 연결
    [Header("지도 설정")]
    [SerializeField] private Vector2Int mapPosition;
    [SerializeField] private bool isBossRoom;
    [SerializeField] private Room[] connectedRooms;

    // ?오잉
    private MonsterAI[] monsterAIs;
    private MonsterAttack[] monsterAttacks;
    private BossBase[] bosses;
    private Rigidbody2D[] monsterRigidbodies;

    // 발견된 방인지 방문한 방인지 저장 ( 지도를 위해 존재 )
    private bool isDiscovered;
    private bool isVisited;

    // 각 방의 카메라포인트와 스테이지번호를 다른 스크립트에서 읽을 수 있도록
    public Transform CameraPoint => cameraPoint;
    public int StageIndex => stageIndex;

    // 지도를 위해 현재위치, 보스방 등을 다른 스크립트에서 읽을 수 있도록
    public Vector2Int MapPosition => mapPosition;
    public bool IsBossRoom => isBossRoom;
    public bool IsDiscovered => isDiscovered;
    public bool IsVisited => isVisited;

    // 룸배틀이 진행중이 아니면 배틀이 끝난 상태가 되게
    public bool IsCleared =>
        roomBattle == null || roomBattle.IsCleared;

    // 게임 시작 시 각 방에서 몬스터 존재여부를 확인하고 플레이어가 가지 않은 방에선 몬스터가 활성화 안됨
    private void Awake()
    {
        FindRoomMonsters();

        SetMonstersActive(false);
    }

    // 방문한 방은 발견과 방문 상태가 되도록
    public void Visit()
    {
        isDiscovered = true;
        isVisited = true;
    }

    // 연결된 방을 찾는 함수....? 모름
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

    // 발견한 방을 발견된 상태로 변경하는 함수
    private void Discover()
    {
        isDiscovered = true;
    }

    // 몬스터 스크립트들을 통해 방에 몬스터가 있는지 찾는 함수
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

        // 몬스터 갯수만큼 머....?머지
        for (int i = 0; i < monsters.Length; i++)
        {
            monsterRigidbodies[i] =
                monsters[i].GetComponent<Rigidbody2D>();
        }
    }

    // 플레이어가 방에 들어왔을 때를 위해서 몬스터를 활성화 시키는 함수
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

        // 활성화 되지 않을땐 몬스터의 움직임을 멈추는 함수 호출
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

    // 몬스터AI 스크립트를 활성화 ( 특정 조건에서만 활성화 하기 위해 )
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

    // 몬스터가 플레이어를 공격할 수 있는 상태로 활성화 ( 아마 쿨타임 때문에..? )
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

    // 보스몬스터 활성화 ( 보스방에 들어갔을때만 )
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

    // 몬스터의 움직임을 멈추기 ( 들어가지 않은 방에서는 몬스터가 움직이지 않도록 )
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

    // 이건뭐지
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
