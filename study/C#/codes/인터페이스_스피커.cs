using System;

// 인터페이스
// 소리 나는 스피커
// AI도움 : 인터페이스 메서드와 자식 클래스 메서드의 매개변수 차이 문제점 확인

interface Ispeaker
{
    public void Sound()
    {
    }

    public void ID()
    {

    }
}

public class Aspeaker : Ispeaker
{
    static void Main(string[] args)
    {
        Aspeaker aspeaker = new Aspeaker();
        Bspeaker bspeaker = new Bspeaker();
        aspeaker.Sound();
        aspeaker.ID();
        bspeaker.Sound();
        bspeaker.ID();
    }

    public void Sound()
    {
        Console.WriteLine("A스피커 소리");
    }

    public void ID() 
    {
        int id = 1;
    }
}

public class Bspeaker : Ispeaker
{
    public void Sound()
    {
        Console.WriteLine("B스피커 소리");
    }

    public void ID()
    {
        int id = 2;
    }
}
