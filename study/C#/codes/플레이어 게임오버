// 플레이어가 죽어서 죽은 화면이 나오는 코드
// 인스턴스, 프로퍼티, 이벤트 사용

// AI도움 : 코드 전체의 오류 총 3회 검토. 틀린 부분 확인 후 내가 직접 고친걸 다시 검토 => 반복
// AI에게 문법적 오류 수정 도움을 전반적으로 받음

using System;

// 플레이어 클래스
public class Player
{
    static void Main()
    {
        // 객체 생성 및 이벤트 함수 연결
        Player player = new Player();

        // DieMessage이벤트에 ShowPlayerDie함수 등록 - 이벤트에 함수를 등록해야 그 함수가 작동한다는 도움을 AI에게 받음
        player.DieMessage += player.ShowPlayerDie;

        // PlayerDie함수 호출
        player.PlayerDie();
    }

    // 플레이어가 죽었다는 의미로 임의로 HP 0으로 설정
    private int currentHP = 0;

    // CurrentHP를 외부 클래스에서 읽을 수 있는 프로퍼티 - private가 아닌 public으로 적어야 외부 클래스에서 읽을 수 있음을 AI에게 도움받음
    public int CurrentHP
    {
        get
        {
            return currentHP;
        }
    }

    public void ShowPlayerDie()
    {
        Console.WriteLine("게임 오버");
    }

    // DieMessage를 전할 수 있는 이벤트
    public event Action DieMessage;

    
    // 플레이어가 죽는 함수
    private void PlayerDie()
    {
        if (CurrentHP <= 0 )
        {
            GameOver();
        }
    }

    // 게임오버 된 경우 DieMessage 호출
    private void GameOver()
    {
        // DieMessage가 Null이 아니면 이벤트 함수 호출
        DieMessage?.Invoke();
    }
}
