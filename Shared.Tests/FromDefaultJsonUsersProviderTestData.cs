using System.Collections;

namespace Shared.Tests;

public class TempJsonUserFileProvider : IDisposable
{
    public readonly string FilePath;

    public TempJsonUserFileProvider(string content)
    {
        var file = Guid.NewGuid().ToString();
        FilePath = Path.Combine(Path.GetTempPath(), file);
        File.WriteAllText(FilePath, content);
    }

    public void Dispose()
    {
        File.Delete(FilePath);
    }
}

public class FromDefaultJsonUsersProviderTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            @"[{""name"":""boris"",""rating"":5000},{""name"":""maria"",""rating"":1000},{""name"":""oleg"",""rating"":800}]",
            new List<User>
            {
                new("boris", 5000, 0),
                new("maria", 1000, 0),
                new("oleg", 800, 0)
            }
        };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}