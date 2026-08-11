using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Header("필수 연결")]
    [SerializeField] private Transform player;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Room startRoom;

    // 지도 UI 스크립트
    [Header("지도 UI")]
    [SerializeField] private MapUI mapUI;

    // 현재 방, 다른 스크립트에서 읽을 수 있도록
    private Room currentRoom;
    public Room CurrentRoom => currentRoom;

    // 게임 시작 시, 컴포넌트 연결이 안 되어 있을 때를 위한 안전 코드
    private void Start()
    {
        if (player == null)
        {
            Debug.LogError(
                "RoomManager의 Player가 연결되지 않았습니다."
            );

            return;
        }

        if (cameraController == null)
        {
            Debug.LogError(
                "RoomManager의 Camera Controller가 연결되지 않았습니다."
            );

            return;
        }

        if (startRoom == null)
        {
            Debug.LogError(
                "RoomManager의 Start Room이 연결되지 않았습니다."
            );

            return;
        }

        if (mapUI == null)
        {
            Debug.LogWarning(
                "RoomManager의 Map UI가 연결되지 않았습니다."
            );
        }

        // 시작방에서 시작하도록
        EnterRoom(startRoom);
    }

    // 연결된 타겟 룸으로 이동하는 함수 + 컴포넌트 연결 안전코드
    public void MoveRoom(
        Room targetRoom,
        Transform spawnPoint
    )
    
    {
        if (targetRoom == null)
        {
            Debug.LogWarning(
                "이동할 Target Room이 연결되지 않았습니다."
            );

            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning(
                "이동할 Spawn Point가 연결되지 않았습니다."
            );

            return;
        }

        // 플레이어가 각 방의 스폰포인트에서 시작하도록
        player.position = spawnPoint.position;
        // 타겟룸으로 들어갈 때의 검사 함수
        EnterRoom(targetRoom);
    }

    // 다른 룸으로 이동할 때 필요한 조정 ( 카메라전환, 몬스터활성화, 맵UI처리 )
    private void EnterRoom(Room room)
    {
        if (room == null)
        {
            return;
        }

        currentRoom = room;

        cameraController.MoveToRoom(currentRoom);

        currentRoom.SetMonstersActive(true);

        currentRoom.Visit();
        currentRoom.DiscoverConnectedRooms();

        if (mapUI != null)
        {
            mapUI.Refresh(currentRoom);
        }

        Debug.Log(
            currentRoom.gameObject.name
            + " 입장"
        );
    }
}
