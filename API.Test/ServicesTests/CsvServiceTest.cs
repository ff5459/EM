using API.Models;
using API.Services;

namespace API.Test.ServicesTests;

public class CsvServiceTest
{
    private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "data.csv");
    
    [Fact]
    public async Task WriteToStream_Return5Elements()
    {
        await using var dataStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read);
        await using var data5Stream = new MemoryStream();

        var orders = await CsvService.ParseAsync<Order>(dataStream, ',');
        await CsvService.WriteToStreamAsync(data5Stream, orders.Take(5));
        data5Stream.Position = 0;
        orders = await CsvService.ParseAsync<Order>(data5Stream, ',');

        Assert.NotEmpty(orders);
        Assert.Equal(5, orders.Count);
    }

    [Fact]
    public async Task ParseAsync_ReturnNotNullData()
    {
        await using var fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read);

        var orders = await CsvService.ParseAsync<Order>(fs, ',');

        Assert.NotEmpty(orders);
        Assert.All(orders, order => Assert.NotNull(order.Region));
    }
}