using UnityEngine;

public class RoomBattle : MonoBehaviour
{
    private int aliveMonsterCount;
    private bool isCleared;

    public bool IsCleared => isCleared;
    public bool IsBattleActive => aliveMonsterCount > 0;

    private void Awake()
    {
        Monster[] monsters = GetComponentsInChildren<Monster>(true);

        aliveMonsterCount = monsters.Length;
        isCleared = aliveMonsterCount == 0;

        Debug.Log(
            gameObject.name
            + " 몬스터 수: "
            + aliveMonsterCount
        );
    }

    public void OnMonsterDead()
    {
        if (isCleared)
        {
            return;
        }

        aliveMonsterCount--;
        aliveMonsterCount = Mathf.Max(aliveMonsterCount, 0);

        Debug.Log(
            "몬스터 사망. 남은 몬스터 수: "
            + aliveMonsterCount
        );

        if (aliveMonsterCount == 0)
        {
            isCleared = true;

            Debug.Log(
                gameObject.name
                + " 방 클리어! 다른 방으로 이동 가능"
            );
        }
    }
}
