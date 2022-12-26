using Shared.Exceptions;
using Shared.Interfaces.Models;
using Shared.Interfaces.Services;

namespace Shared.Services;

public class DefaultEmissionStrategy<TUsers> : IEmissionStrategy<TUsers> where TUsers : IUser

{
    public long[] CalcEmission(TUsers[] users, long amount)
    {
        if (amount < users.Length) throw new AmountLessThanNumbersOfUsersException();
        double totalRating = users.Sum(u => u.Rating);
        var reward =
            totalRating == 0
                ? users.Select(_ => (double) amount / users.Length).ToArray()
                : users.Select(u => u.Rating / totalRating * amount).ToArray();

        var easyCalcReward = reward.Select(Convert.ToInt64).ToArray();
        if (easyCalcReward.Sum() == amount && !easyCalcReward.Contains(0)) return easyCalcReward;

        int count;
        while ((count = GetMaxFractionalIndex(reward)) != -1 ||
               (count = Array.IndexOf(reward, 0)) != -1)
        {
            var minFractionalIndex = GetMinFractionalIndex(reward, count);
            if (minFractionalIndex == -1)
            {
                reward[count] = (int) (reward[count] + 1);
                var minFindIndex = Array.FindIndex(reward, x => x > 1);
                reward[minFindIndex] -= 1;
            }
            else
            {
                reward[count] += reward[minFractionalIndex] - (int) reward[minFractionalIndex];
                reward[minFractionalIndex] = (int) reward[minFractionalIndex];
                TryCorrectionDouble(ref reward[count]);
            }
        }

        return reward.Select(x => (long) x).ToArray();
    }

    private static int GetMinFractionalIndex(double[] numbers, int skipIndex = -1)
    {
        var minFractional = 1d;
        var minFractionalIndex = -1;
        for (var i = 0; i < numbers.Length; i++)
        {
            var fractionalPart = numbers[i] - (int) numbers[i];
            if (!(fractionalPart < minFractional) || fractionalPart == 0 || skipIndex == i) continue;

            minFractional = fractionalPart;
            minFractionalIndex = i;
        }

        return minFractionalIndex;
    }

    private static int GetMaxFractionalIndex(double[] numbers, int skipIndex = -1)
    {
        var maxFractional = 0d;
        var maxFractionalIndex = -1;
        for (var i = 0; i < numbers.Length; i++)
        {
            var fractionalPart = numbers[i] - (int) numbers[i];
            if (!(fractionalPart > maxFractional) || fractionalPart == 0 || skipIndex == i) continue;

            maxFractional = fractionalPart;
            maxFractionalIndex = i;
        }

        return maxFractionalIndex;
    }

    private static bool TryCorrectionDouble(ref double number, double epsilon = 0.001)
    {
        if (!(Math.Abs(number - Math.Round(number)) < epsilon)) return false;
        number = Math.Round(number);
        return true;
    }
}