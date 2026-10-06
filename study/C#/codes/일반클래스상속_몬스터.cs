using System;

// 일반클래스 상속
// 주제 : 부모몬스터, 자식몬스터

// AI사용 : main선언 위치, 메서드 안에서 함수호출 등이 문제점임을 알아냄

public class ParentMonster
{
    static void Main(string[] args)
    {
        ParentMonster parentMonster = new ParentMonster();
        SonMonster sonMonster = new SonMonster();

        sonMonster.Test();
    }

    public int damage { get; }
    public int HP;
    public void Attack()
    {
        HP = -damage;
        Console.WriteLine("공격 함수 상속 테스트");
    }

    public void Die()
    {
        HP = 0;
    }

}

public class SonMonster : ParentMonster
{
    public void Test()
    {
        Attack();

        if (HP <= 0)
        {
            Die();
        }

        Console.WriteLine("자식클래스 호출 테스트");
    }
}
