using Billing.Exceptions;
using Billing.Models;


namespace Billing.Services;

public class DefaultEmissionStrategy : IEmissionStrategy
{
    private static int GetMinFractionalIndex(IReadOnlyList<double> numbers, int skipIndex = -1)
    {
        var currentMinFractional = 1d;
        var currentMinFractionalIndex = -1;
        for (var i = 0; i < numbers.Count; i++)
        {
            var fractionalPart = numbers[i] - (int) numbers[i];
            if (fractionalPart == 0 || skipIndex == i || !(fractionalPart < currentMinFractional))
            {
                continue;
            }

            currentMinFractional = fractionalPart;
            currentMinFractionalIndex = i;
        }

        return currentMinFractionalIndex;
    }

    private static int GetMaxFractionalIndex(IReadOnlyList<double> numbers, int skipIndex = -1)
    {
        var currentMaxFractional = 0d;
        var currentMaxFractionalIndex = -1;
        for (var i = 0; i < numbers.Count; i++)
        {
            var fractionalPart = numbers[i] - (int) numbers[i];
            if (skipIndex == i || !(fractionalPart > currentMaxFractional))
            {
                continue;
            }

            currentMaxFractional = fractionalPart;
            currentMaxFractionalIndex = i;
        }

        return currentMaxFractionalIndex;
    }

    private static bool TryCorrectionDouble(ref double number, double epsilon = 0.001)
    {
        if (!(Math.Abs(number - Math.Round(number)) < epsilon)) return false;
        number = Math.Round(number);
        return true;
    }

    public long[] CalcEmission(User[] users, long amount)
    {
        if (amount < users.Length)
        {
            throw new AmountLessThanNumbersOfUsersException();
        }

        double totalRating = users.Sum(u => u.Rating);
        var reward = users.Select(u => u.Rating / totalRating * amount).ToArray();
        var count = 0;
        while (count < reward.Length && ((int) reward[count] == 0 || (count = GetMaxFractionalIndex(reward)) != -1))
        {
            var minFractionalIndex = GetMinFractionalIndex(reward, count);
            if (minFractionalIndex == -1)
            {
                reward[count] += 1;
                reward[^1] -= 1;
                count++;
            }
            else
            {
                reward[count] += reward[minFractionalIndex] - (int) reward[minFractionalIndex];
                reward[minFractionalIndex] = (int) reward[minFractionalIndex];
                var isInt = TryCorrectionDouble(ref reward[count]);
                if (isInt)
                {
                    count++;
                }
            }
        }

        return reward.Select(Convert.ToInt64).ToArray();
    }
}