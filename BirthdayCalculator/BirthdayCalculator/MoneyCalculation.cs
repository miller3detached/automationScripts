using System;
using System.Collections.Generic;
using System.Linq; 
using System.Text;
using System.Threading.Tasks;


namespace BirthdayCalculator
{
    public static class MoneyCalculation
    {
        public static int workers;
        public static int birthdayWorkers;

        public const int MoneyForOneBirthdayWorker = 100;

        private static int totalNeededMoney;

        public static double moneyFromWorkerForOneBirthday;
        public static double moneyFromBirthdayWorkerForActualBirthdayWorkers;
        public static double moneyFromWorker;


        public static void PrepareDataForCalculation()
        {
            totalNeededMoney = birthdayWorkers * MoneyForOneBirthdayWorker;
            moneyFromWorkerForOneBirthday = Math.Round((double)MoneyForOneBirthdayWorker / (workers - 1), 1);
        }

        public static double GetMoneyCountFromWorkers()
        {
            double allMoneyFromBirthdayWorkers = Math.Round(moneyFromBirthdayWorkerForActualBirthdayWorkers * birthdayWorkers, 1);
            moneyFromWorker = Math.Round(((double)totalNeededMoney - allMoneyFromBirthdayWorkers) / (workers - birthdayWorkers), 1);
            return moneyFromWorker + 0.1; //дописать нормальное округление чисел
        }

        public static double GetMoneyCountFromBirthdayWorkers()
        {
            moneyFromBirthdayWorkerForActualBirthdayWorkers = Math.Round(moneyFromWorkerForOneBirthday * (birthdayWorkers - 1), 1);
            return moneyFromBirthdayWorkerForActualBirthdayWorkers + 0.1;
        }

    }
}
