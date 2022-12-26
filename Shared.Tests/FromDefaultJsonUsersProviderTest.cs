using FluentAssertions;
using Shared.Services;

namespace Shared.Tests;

public class FromDefaultJsonUsersProviderTest
{
    [Theory]
    [ClassData(typeof(FromDefaultJsonUsersProviderTestData))]
    public void FromDefaultJsonUsersProviderTest_CorrectParse(string content, List<User> users)
    {
        var file = new TempJsonUserFileProvider(content);
        var userProvider = new FromDefaultJsonUsersProvider<User>(file.FilePath);
        userProvider.Get().Should()
            .HaveCount(users.Count)
            .And.Equal(users);
    }
}