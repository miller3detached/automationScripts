using System;
using BirthdayCalculator;

namespace Calculation
{
    public class Task
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите число всех рабоников");
            MoneyCalculation.workers = int.Parse(Console.ReadLine());

            Console.WriteLine("Введите количество именинников");
            MoneyCalculation.birthdayWorkers = int.Parse(Console.ReadLine());

            MoneyCalculation.PrepareDataForCalculation();
            
            Console.WriteLine($"Именинникам нужно будет заплатить {MoneyCalculation.GetMoneyCountFromBirthdayWorkers()} рублей");
            Console.WriteLine($"Работникам, у которых нет дня рождения, нужно будет заплатить {MoneyCalculation.GetMoneyCountFromWorkers()} рублей");
            Thread.Sleep(15000);
        }

    }
}




