using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class DoorMove : MonoBehaviour
{
    // RoomManager 스크립트 연결
    [SerializeField] private RoomManager roomManager;

    [Header("현재 방")]
    [SerializeField] private Room currentRoom;

    [Header("이동할 방")]
    [SerializeField] private Room targetRoom;
    [SerializeField] private Transform targetSpawnPoint;

    // 타일맵으로 만든 문 오브젝트 연결
    [Header("문 타일맵 (2x2 고정)")]
    [SerializeField] private Tilemap doorTilemap;
    [SerializeField] private BoxCollider2D doorCollider;
    [SerializeField] private float openDelay = 0.3f;

    // 문 열리는 애니메이션 구현
    [Header("열린 문 타일 (닫힌 타일과 같은 순서: 왼아래, 오른아래, 왼위, 오른위)")]
    [SerializeField] private TileBase[] openTiles = new TileBase[4];

    // 보스방 문
    [Header("보스방 진입 경고")]
    [SerializeField] private bool useBossWarning;
    [SerializeField] private BossDoorWarningUI bossWarningUI;

    [SerializeField] private string bossWarningMessage = "살기가 느껴진다…";

    // Tilemap에서 문의 cell의 상대좌표 저장
    private static readonly Vector3Int[] cellOffsets =
    {
        new Vector3Int(0, 0, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(1, 1, 0)
    };

    // ? 모름
    private readonly List<Vector3Int> doorCells =
        new List<Vector3Int>();

    // 열리기 전 기본 문의 타일 저
    private readonly TileBase[] originalTiles =
        new TileBase[4];

    // 플레이어가 들어가 있는지랑 움직이고 있는지
    private bool playerInside;
    private bool isMoving;

    // PlayerGun 스크립트 가져오기
    private PlayerGun playerGun;

    // 문의 Colider 컴포넌트 가져오고 문 cell의 좌표 기억 ( 문에 닿아야만 이동하는 방식이므로 )
    private void Awake()
    {
        if (doorCollider == null)
        {
            doorCollider =
                GetComponent<BoxCollider2D>();
        }

        CalculateDoorCellsAndRememberOriginal();
    }

    // 문 cell의 위치를 계산하고 열리기 전 기본 cell을 저장.
    private void CalculateDoorCellsAndRememberOriginal()
    {
        doorCells.Clear();

        if (doorTilemap == null ||
            doorCollider == null)
        {
            Debug.LogWarning(
                gameObject.name +
                "의 Door Tilemap 또는 Door Collider가 연결되지 않았습니다."
            );

            return;
        }

        Vector3Int baseCell =
            doorTilemap.WorldToCell(
                doorCollider.bounds.min
            );

        for (int i = 0;
             i < cellOffsets.Length;
             i++)
        {
            Vector3Int cell =
                baseCell + cellOffsets[i];

            doorCells.Add(cell);

            originalTiles[i] =
                doorTilemap.GetTile(cell);
        }
    }

    // 문 타일?... 이런 함수가 왤케 많은지 모름
    private void SetDoorTiles(
        TileBase[] tiles
    )
    {
        if (doorTilemap == null ||
            tiles == null ||
            tiles.Length < 4)
        {
            return;
        }

        for (int i = 0;
             i < doorCells.Count;
             i++)
        {
            doorTilemap.SetTile(
                doorCells[i],
                tiles[i]
            );
        }
    }

    // 일시정지 상태인지, 플레이어가 안에 있는지, 스페이스바 입력이 들어왔는지 매 프레임 갱신
    private void Update()
    {
        if (PauseMenu.IsPaused)
        {
            return;
        }

        if (!playerInside ||
            isMoving)
        {
            return;
        }

        if (!Input.GetKeyDown(
            KeyCode.Space
        ))
        {
            return;
        }

        TryMoveRoom();
    }

    // 현재 방에서 몬스터가 없어야만 다른 방으로 이동할 수 있도록
    private void TryMoveRoom()
    {
        if (currentRoom == null)
        {
            Debug.LogWarning(
                gameObject.name +
                "의 Current Room이 연결되지 않았습니다."
            );

            return;
        }

        if (!currentRoom.IsCleared)
        {
            Debug.Log(
                "방 안의 몬스터를 모두 처치해야 이동할 수 있습니다."
            );

            return;
        }

        if (roomManager == null ||
            targetRoom == null ||
            targetSpawnPoint == null)
        {
            Debug.LogWarning(
                gameObject.name +
                "의 문 이동 설정이 비어 있습니다."
            );

            return;
        }

        // 문이 열리는 동안 잠깐 멈춰있는 Coroutine
        StartCoroutine(
            MoveRoomRoutine()
        );
    }

    // 문에서 이동할 때 잠깐 열리는 애니메이션이 나온 후에 이동되는 코루틴 함수
    private IEnumerator MoveRoomRoutine()
    {
        isMoving = true;

        if (playerGun != null)
        {
            playerGun.SetCanShoot(false);
        }

        // 문을 열린 모습으로 변경
        SetDoorTiles(openTiles);

        // 문 열릴 때 사운드
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayDoorOpen();
        }

        // 보스방 문 들어갈 때 경고 문구가 사라질 때 까지 기다리는 코루틴
        if (useBossWarning)
        {
            if (bossWarningUI != null)
            {
                yield return bossWarningUI.ShowWarning(
                    bossWarningMessage
                );
            }
            else
            {
                Debug.LogWarning(
                    gameObject.name +
                    "의 Boss Warning UI가 연결되지 않았습니다."
                );

                yield return new WaitForSeconds(
                    openDelay
                );
            }
        }
        else
        {
            // 일반 문은 기존 지연 후 이동
            yield return new WaitForSeconds(
                openDelay
            );
        }

        // RoomManger스크립트에 이동할 방과 스폰포인트를 전달
        roomManager.MoveRoom(
            targetRoom,
            targetSpawnPoint
        );

        // 이동할 때 각 방의 BGM을 재생
        if (BGMPlayer.Instance != null)
        {
            BGMPlayer.Instance.PlayForRoom(
                targetRoom
            );
        }

        playerInside = false;
        isMoving = false;

        // 이전 문의 타일을 닫힌 모습으로 복구
        SetDoorTiles(originalTiles);

        // 이동이 끝나면 총을 다시 쏠 수 있게
        if (playerGun != null)
        {
            playerGun.SetCanShoot(true);
            playerGun = null;
        }
    }

    // Player태그가 붙어 있어야만 문의 Trigger가 작동하여 영역에 들어왔음을 인지
    private void OnTriggerEnter2D(
        Collider2D other
    )
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInside = true;

        playerGun =
            other.GetComponentInChildren<PlayerGun>();

        // 플레이어가 문의 Trigger영역에 있으면 총 발사 못함
        if (playerGun != null)
        {
            playerGun.SetCanShoot(false);
        }
    }

    // 나갈 때도 위에와 마찬가지
    private void OnTriggerExit2D(
        Collider2D other
    )
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInside = false;

        if (!isMoving &&
            playerGun != null)
        {
            playerGun.SetCanShoot(true);
        }

        playerGun = null;

        if (!isMoving)
        {
            SetDoorTiles(originalTiles);
        }
    }
}
