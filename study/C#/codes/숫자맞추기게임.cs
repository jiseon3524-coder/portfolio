// 숫자 맞추기 게임

using System;

public class numbergame
{

    static void Main()
    {
        int a = 10;
        Console.WriteLine("숫자 맞추기 게임!\n1부터 10까지 숫자 중에서 맞춰보세요");

        while (true)
        {
            int num = int.Parse(Console.ReadLine());
            if (num < 1)
            {
                Console.WriteLine("범위는 1부터 10까지의 숫자입니다.");
                continue;
            }

            if (num == a)
            {
                Console.WriteLine($"성공! 정답은 {a}였습니다!");
                break;
            }

            else if (num < a && num > 0)
            {
                Console.WriteLine("입력하신 값 보다 더 큰 숫자입니다!");
            }

            else if (num > a )
            {
                Console.WriteLine("입력하신 값 보다 더 작은 숫자입니다!");
            }


        }
    }
}
