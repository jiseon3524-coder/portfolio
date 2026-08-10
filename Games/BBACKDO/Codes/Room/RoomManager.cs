using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Header("필수 연결")]
    [SerializeField] private Transform player;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Room startRoom;

    [Header("지도 UI")]
    [SerializeField] private MapUI mapUI;

    private Room currentRoom;

    public Room CurrentRoom => currentRoom;

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

        EnterRoom(startRoom);
    }

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

        if (currentRoom != null)
        {
            currentRoom.SetMonstersActive(false);
        }

        player.position = spawnPoint.position;

        EnterRoom(targetRoom);
    }

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
