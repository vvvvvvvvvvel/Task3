using FluentAssertions;
using Shared.Exceptions;
using Shared.Services;

namespace Shared.Tests;

public class DefaultEmissionStrategyTest
{
    [Theory]
    [InlineData(1, new[] {1}, new long[] {1})]
    [InlineData(3, new[] {1, 1, 1}, new long[] {1, 1, 1})]
    [InlineData(3, new[] {3, 3, 3}, new long[] {1, 1, 1})]
    [InlineData(10, new[] {4, 2, 4}, new long[] {4, 2, 4})]
    [InlineData(10, new[] {2, 1, 3}, new long[] {3, 2, 5})]
    [InlineData(10, new[] {8, 1, 1}, new long[] {8, 1, 1})]
    [InlineData(9, new[] {1000, 1000, 1001}, new long[] {3, 3, 3})]
    [InlineData(9, new[] {1001, 1000, 1001}, new long[] {3, 3, 3})]
    [InlineData(9, new[] {1000, 1501, 1001}, new long[] {2, 4, 3})]
    [InlineData(9, new[] {1000, 1001, 1001}, new long[] {3, 3, 3})]
    [InlineData(10, new[] {5000, 1000, 800}, new long[] {7, 2, 1})]
    public void CalcEmissionTest_FairlyEmission(int amount, int[] rating, long[] expectedEmission)
    {
        var defEmissionStrategy = new DefaultEmissionStrategy<User>();
        var users = Enumerable.Range(0, rating.Length).Select(x => new User(string.Empty, rating[x], 0));
        var result = defEmissionStrategy.CalcEmission(users.ToArray(), amount);
        result.Should()
            .HaveCount(expectedEmission.Length)
            .And.Equal(expectedEmission)
            .And.Match(emission => emission.Sum() == amount);
    }

    [Theory]
    [InlineData(1, new[] {0}, new long[] {1})]
    [InlineData(10, new[] {8, 1, 0}, new long[] {8, 1, 1})]
    [InlineData(3, new[] {1, 0, 0}, new long[] {1, 1, 1})]
    [InlineData(3, new[] {0, 1, 0}, new long[] {1, 1, 1})]
    [InlineData(3, new[] {0, 0, 1}, new long[] {1, 1, 1})]
    [InlineData(5, new[] {0, 0, 1}, new long[] {1, 1, 3})]
    [InlineData(10, new[] {0, 0, 1}, new long[] {1, 1, 8})]
    [InlineData(3, new[] {0, 0, 0}, new long[] {1, 1, 1})]
    public void CalcEmissionTest_GetMinimum(int amount, int[] rating, long[] expectedEmission)
    {
        var defEmissionStrategy = new DefaultEmissionStrategy<User>();
        var users = Enumerable.Range(0, rating.Length).Select(x => new User(string.Empty, rating[x], 0));
        var result = defEmissionStrategy.CalcEmission(users.ToArray(), amount);
        result.Should()
            .HaveCount(expectedEmission.Length)
            .And.Equal(expectedEmission)
            .And.Match(emission => emission.Sum() == amount);
    }

    [Theory]
    [InlineData(4, new[] {1, 1, 1}, new long[] {1, 1, 2})]
    [InlineData(5, new[] {1, 1, 1}, new long[] {1, 2, 2})]
    [InlineData(4, new[] {0, 0, 0}, new long[] {1, 1, 2})]
    [InlineData(5, new[] {0, 0, 0}, new long[] {1, 2, 2})]
    [InlineData(10, new[] {3, 3, 3}, new long[] {3, 3, 4})]
    [InlineData(10, new[] {0, 0, 0}, new long[] {3, 3, 4})]
    [InlineData(8, new[] {0, 0, 0}, new long[] {3, 3, 2})]
    public void CalcEmissionTest_FullDistributedEmission(int amount, int[] rating, long[] expectedEmission)
    {
        var defEmissionStrategy = new DefaultEmissionStrategy<User>();
        var users = Enumerable.Range(0, rating.Length).Select(x => new User(string.Empty, rating[x], 0));
        var result = defEmissionStrategy.CalcEmission(users.ToArray(), amount);
        result.Should()
            .HaveCount(expectedEmission.Length)
            .And.Match(emission => emission.OrderBy(r => r).SequenceEqual(expectedEmission.OrderBy(r => r)))
            .And.Match(emission => emission.Sum() == amount);
    }

    [Theory]
    [InlineData(2, new[] {1, 1, 1})]
    [InlineData(0, new[] {1})]
    [InlineData(-1, new[] {1})]
    public void CalcEmissionTest_AmountLessThanUsers(int amount, int[] rating)
    {
        var defEmissionStrategy = new DefaultEmissionStrategy<User>();
        var users = Enumerable.Range(0, rating.Length).Select(x => new User(string.Empty, rating[x], 0));
        Action calcEmission = () => defEmissionStrategy.CalcEmission(users.ToArray(), amount);
        calcEmission.Should().Throw<AmountLessThanNumbersOfUsersException>();
    }
}