namespace BlScraper.DependencyInjection.Tests.Mocks;

internal static class PublicSImpleDataFactory
{
    /// <summary>
    /// Get ordered data, '0' to <see cref="lenght"/>, initializes in '1'
    /// <exception cref="IndexOutOfRangeException"></exception>
    public static IEnumerable<PublicSimpleData> GetData(int length)
    {
        if (length < 1)
            throw new IndexOutOfRangeException($"{nameof(length)} should be more than '0'.");

        var list = new List<PublicSimpleData>();
        for (int i = 1; i <= length; i++)
        {
            list.Add(new PublicSimpleData());
        }

        return list;
    }
}