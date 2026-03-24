using Xunit;
using AppointmentSystem.Services;
using AppointmentSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSystem.Tests;

public class BookingServiceTests
{
    private BookingService GetService()
    {
        // Створюємо тимчасову базу в пам'яті для кожного тесту
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        var context = new ApplicationDbContext(options);
        return new BookingService(context);
    }

    [Fact]
    public void ValidateBooking_PastDate_ReturnsError()
    {
        // Arrange (Підготовка): беремо час, який уже минув
        var service = GetService();
        var pastDate = DateTime.Now.AddDays(-1);

        // Act (Дія)
        var result = service.ValidateBooking(1, pastDate);

        // Assert (Перевірка)
        Assert.Equal("Час уже минув.", result);
    }

    [Fact]
    public void ValidateBooking_ValidTime_ReturnsNull()
    {
        // Arrange: беремо завтра на 10:00 (робочий час)
        var service = GetService();
        var validDate = DateTime.Now.AddDays(1).Date.AddHours(10);

        // Act
        var result = service.ValidateBooking(1, validDate);

        // Assert (null означає, що помилок немає)
        Assert.Equal("OK", result);
    }
}