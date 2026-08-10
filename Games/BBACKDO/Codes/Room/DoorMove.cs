using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class DoorMove : MonoBehaviour
{
    [SerializeField] private RoomManager roomManager;

    [Header("현재 방")]
    [SerializeField] private Room currentRoom;

    [Header("이동할 방")]
    [SerializeField] private Room targetRoom;
    [SerializeField] private Transform targetSpawnPoint;

    [Header("문 타일맵 (2x2 고정)")]
    [SerializeField] private Tilemap doorTilemap;
    [SerializeField] private BoxCollider2D doorCollider;
    [SerializeField] private float openDelay = 0.3f;

    [Header("열린 문 타일 (닫힌 타일과 같은 순서: 왼아래, 오른아래, 왼위, 오른위)")]
    [SerializeField] private TileBase[] openTiles = new TileBase[4];

    [Header("보스방 진입 경고")]
    [SerializeField] private bool useBossWarning;
    [SerializeField] private BossDoorWarningUI bossWarningUI;

    [SerializeField]
    private string bossWarningMessage =
        "살기가 느껴진다…";
    private static readonly Vector3Int[] cellOffsets =
    {
        new Vector3Int(0, 0, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(1, 1, 0)
    };

    private readonly List<Vector3Int> doorCells =
        new List<Vector3Int>();

    private readonly TileBase[] originalTiles =
        new TileBase[4];

    private bool playerInside;
    private bool isMoving;

    private PlayerGun playerGun;

    private void Awake()
    {
        if (doorCollider == null)
        {
            doorCollider =
                GetComponent<BoxCollider2D>();
        }

        CalculateDoorCellsAndRememberOriginal();
    }

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

        StartCoroutine(
            MoveRoomRoutine()
        );
    }

    private IEnumerator MoveRoomRoutine()
    {
        isMoving = true;

        if (playerGun != null)
        {
            playerGun.SetCanShoot(false);
        }

        // 문을 열린 모습으로 변경
        SetDoorTiles(openTiles);

        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayDoorOpen();
        }

        if (useBossWarning)
        {
            if (bossWarningUI != null)
            {
                // 경고 문구가 완전히 사라질 때까지 기다린다.
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

        roomManager.MoveRoom(
            targetRoom,
            targetSpawnPoint
        );

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

        if (playerGun != null)
        {
            playerGun.SetCanShoot(true);
            playerGun = null;
        }
    }

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

        if (playerGun != null)
        {
            playerGun.SetCanShoot(false);
        }
    }

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
