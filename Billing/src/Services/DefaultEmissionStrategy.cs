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
        while (count < reward.Length && ((count = GetMaxFractionalIndex(reward)) != -1) || (count = Array.IndexOf(reward, 0)) !=-1)
        {
            var minFractionalIndex = GetMinFractionalIndex(reward, count);
            if (minFractionalIndex == -1)
            {
                reward[count] += 1;
                for (var i = reward.Length - 1; i > -1; i--)
                {
                    if (reward[i] > 1)
                    {
                        reward[i] -= 1;
                        break;
                    }
                }
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
                if (reward[minFractionalIndex] == 0 && minFractionalIndex < count)
                {
                    count = minFractionalIndex;
                }
            }
        }

        return reward.Select(Convert.ToInt64).ToArray();
    }
}