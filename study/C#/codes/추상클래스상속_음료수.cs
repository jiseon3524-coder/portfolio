using System;

// 추상 클래스 상속
// 카페 음료수
// AI사용 : 추상클래스문법, 클래스 내에 함수 한 번에 호출하는 방법

public abstract class Juice
{   
    static void Main(string[] args)
    {
        GrapeJuice grapeJuice = new GrapeJuice();
        OrangeJuice orangeJuice = new OrangeJuice();
        grapeJuice.Test();
        orangeJuice.Test();
    }

    public double sugar;
    public string fruit;

    public abstract void taste();
    public void sweet()
    {
        Console.WriteLine($"이 음료는 {sugar}%의 설탕이 함유되어 있다.");
    }
}

public class GrapeJuice : Juice
{
    public override void taste()
    {
        Console.WriteLine($"이 음료는 {fruit}맛이다.");
    }

    public void Test()
    {
        sugar = 0.8;
        fruit = "포도";
        sweet();
        taste();
    }
}

public class OrangeJuice : Juice
{
    public override void taste()
    {
        Console.WriteLine($"이 음료는 {fruit}맛이다");
    }

    public void Test()
    {
        sugar = 0.95;
        fruit = "오렌지";
        sweet();
        taste();
    }
}



