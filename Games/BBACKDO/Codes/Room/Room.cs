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

    // 몬스터들의 컴포넌트 ( 몬스터가 여러 마리이기 때문에 배열 사용 )
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

    // 룸배틀이 없거나 룸배틀이 끝난 상태면 IsCleared상태 ( 전투끝난상태 )
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

    // 지도에 현재 방에 연결된 방들을 발견된 방으로 표시
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

        // 플레이어가 들어가지 않은 방의 몬스터들의 rigidbody를 비활성화 ( 몬스터는 여러 마리이기 때문에 반복문 사용 )
        for (int i = 0; i < monsters.Length; i++)
        {
            monsterRigidbodies[i] =
                monsters[i].GetComponent<Rigidbody2D>();
        }
    }

    // 플레이어가 어떤 방에 있는지에 따른 몬스터를 활성화/비활성
    public void SetMonstersActive(bool active)
    {
        if (monsterAIs == null ||
            monsterAttacks == null ||
            bosses == null ||
            monsterRigidbodies == null)
        {
            FindRoomMonsters();
        }

        // 몬스터AI 스크립트를 활성화 ( 플레이어 방에 있는 몬스터들만 활성화 하기 위해 )
        foreach (MonsterAI monsterAI in monsterAIs)
        {
            if (monsterAI == null)
            {
                continue;
            }

            monsterAI.enabled = active;
        }

        // 몬스터가 플레이어를 공격할 수 있는 상태를 활성화/비활성 ( MonsterAttack.cs
        foreach (MonsterAttack monsterAttack in monsterAttacks)
        {
            if (monsterAttack == null)
            {
                continue;
            }

            monsterAttack.enabled = active;
        }

        // 보스몬스터 활성화/비활성화
        foreach (BossBase boss in bosses)
        {
            if (boss == null)
            {
                continue;
            }

            boss.enabled = active;
        }

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

    // Unity Inspector에서 몬스터 목록 조회
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
