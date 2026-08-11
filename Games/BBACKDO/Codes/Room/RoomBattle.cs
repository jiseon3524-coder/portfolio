using UnityEngine;

public class RoomBattle : MonoBehaviour
{

    // 살아있는 몬스터 수, 방 클리어했는지
    private int aliveMonsterCount;
    private bool isCleared;

    // 그걸 다른 스크립트에서 읽을 수 있게
    public bool IsCleared => isCleared;
    public bool IsBattleActive => aliveMonsterCount > 0;

    // 몬스터들의 컴포넌트 가져오고 살아있는 몬스터 수 세기, 클리어 조건
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

    // 몬스터가 죽었을 때 몇마리 남았는지 세는 함수
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
