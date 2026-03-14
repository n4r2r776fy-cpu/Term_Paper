using System.Text.Json;
using MyBlazorApp.Models;
using MyBlazorApp.Tests.TestInfrastructure;

namespace MyBlazorApp.Tests;

public class DataServiceTests
{
    [Fact]
    public void Constructor_CreatesAppDataDirectory()
    {
        using var context = new TestDataServiceContext();

        Assert.True(Directory.Exists(context.AppDataPath));
    }

    [Fact]
    public void Constructor_CreatesUsersFile()
    {
        using var context = new TestDataServiceContext();

        Assert.True(File.Exists(context.UsersFilePath));
    }

    [Fact]
    public void Constructor_CreatesAppointmentsFile()
    {
        using var context = new TestDataServiceContext();

        Assert.True(File.Exists(context.AppointmentsFilePath));
    }

    [Fact]
    public void GetUsers_ReturnsSeededDemoUser()
    {
        using var context = new TestDataServiceContext();

        var users = context.DataService.GetUsers();

        Assert.Contains(users, user => user.Email == "demo@clinic.local" && user.Name == "Demo User");
    }

    [Fact]
    public void GetAppointments_ReturnsSeededAppointments()
    {
        using var context = new TestDataServiceContext();

        var appointments = context.DataService.GetAppointments();

        Assert.Equal(3, appointments.Count);
    }

    [Fact]
    public void GetDoctors_ReturnsThreeDoctors()
    {
        using var context = new TestDataServiceContext();

        var doctors = context.DataService.GetDoctors();

        Assert.Equal(3, doctors.Count);
    }

    [Theory]
    [InlineData("Erica Lim", "Терапевт")]
    [InlineData("Kevin Zane", "Стоматолог")]
    [InlineData("Mina Park", "Дерматолог")]
    public void GetDoctors_ReturnsExpectedDoctorEntries(string name, string specialty)
    {
        using var context = new TestDataServiceContext();

        var doctors = context.DataService.GetDoctors();

        Assert.Contains(doctors, doctor => doctor.Name == name && doctor.Specialty == specialty && !string.IsNullOrWhiteSpace(doctor.AvatarPath));
    }

    [Fact]
    public void GetAppointments_ReturnsItemsSortedByDateThenTimeThenName()
    {
        using var context = new TestDataServiceContext();

        context.DataService.SaveAppointment(new Appointment
        {
            Name = "Aaron",
            Email = "aaron@example.com",
            Phone = "+380000000001",
            Service = "Терапевтична консультація",
            DoctorName = "Erica Lim",
            Date = DateTime.Today,
            Time = "08:15"
        });

        context.DataService.SaveAppointment(new Appointment
        {
            Name = "Bella",
            Email = "bella@example.com",
            Phone = "+380000000002",
            Service = "Терапевтична консультація",
            DoctorName = "Erica Lim",
            Date = DateTime.Today,
            Time = "08:15"
        });

        var sameSlot = context.DataService.GetAppointments()
            .Where(appointment => appointment.Date.Date == DateTime.Today && appointment.Time == "08:15")
            .Select(appointment => appointment.Name)
            .ToList();

        Assert.Equal(new[] { "Aaron", "Bella" }, sameSlot);
    }

    [Fact]
    public void SaveAppointment_AssignsNextId()
    {
        using var context = new TestDataServiceContext();
        var appointment = CreateAppointment();

        context.DataService.SaveAppointment(appointment);

        Assert.Equal(4, appointment.Id);
    }

    [Fact]
    public void SaveAppointment_PersistsAppointment()
    {
        using var context = new TestDataServiceContext();
        var appointment = CreateAppointment(name: "Saved Patient");

        context.DataService.SaveAppointment(appointment);

        Assert.Contains(context.DataService.GetAppointments(), item => item.Name == "Saved Patient");
    }

    [Fact]
    public void SaveAppointment_SetsCreatedAtToRecentTimestamp()
    {
        using var context = new TestDataServiceContext();
        var before = DateTime.Now.AddSeconds(-5);
        var appointment = CreateAppointment();

        context.DataService.SaveAppointment(appointment);

        Assert.True(appointment.CreatedAt >= before);
    }

    [Fact]
    public void SaveAppointment_SetsUpcomingStatusWhenStatusIsEmpty()
    {
        using var context = new TestDataServiceContext();
        var appointment = CreateAppointment();
        appointment.Status = string.Empty;

        context.DataService.SaveAppointment(appointment);

        Assert.Equal(AppointmentStatuses.Upcoming, appointment.Status);
    }

    [Fact]
    public void SaveAppointment_PreservesExplicitStatus()
    {
        using var context = new TestDataServiceContext();
        var appointment = CreateAppointment();
        appointment.Status = AppointmentStatuses.Completed;

        context.DataService.SaveAppointment(appointment);

        Assert.Equal(AppointmentStatuses.Completed, appointment.Status);
    }

    [Theory]
    [InlineData("Терапевтична консультація", 600)]
    [InlineData("Стоматологічний огляд", 850)]
    [InlineData("Дерматологічна консультація", 700)]
    [InlineData("Невідома послуга", 500)]
    public void SaveAppointment_AssignsCalculatedPriceForZeroPrice(string service, decimal expectedPrice)
    {
        using var context = new TestDataServiceContext();
        var appointment = CreateAppointment(service: service, price: 0);

        context.DataService.SaveAppointment(appointment);

        Assert.Equal(expectedPrice, appointment.Price);
    }

    [Fact]
    public void SaveAppointment_PreservesPositivePrice()
    {
        using var context = new TestDataServiceContext();
        var appointment = CreateAppointment(price: 1234m);

        context.DataService.SaveAppointment(appointment);

        Assert.Equal(1234m, appointment.Price);
    }

    [Fact]
    public void UpdateAppointment_UpdatesExistingRecord()
    {
        using var context = new TestDataServiceContext();
        var appointment = context.DataService.GetAppointments().First();
        appointment.Name = "Updated Name";
        appointment.Time = "16:20";

        context.DataService.UpdateAppointment(appointment);

        var updated = context.DataService.GetAppointments().First(item => item.Id == appointment.Id);
        Assert.Equal("Updated Name", updated.Name);
        Assert.Equal("16:20", updated.Time);
    }

    [Fact]
    public void UpdateAppointment_RecalculatesPriceWhenCurrentPriceIsZero()
    {
        using var context = new TestDataServiceContext();
        var appointment = context.DataService.GetAppointments().First();
        appointment.Service = "Стоматологічний огляд";
        appointment.Price = 0;

        context.DataService.UpdateAppointment(appointment);

        var updated = context.DataService.GetAppointments().First(item => item.Id == appointment.Id);
        Assert.Equal(850m, updated.Price);
    }

    [Fact]
    public void UpdateAppointment_DoesNothingForMissingId()
    {
        using var context = new TestDataServiceContext();
        var countBefore = context.DataService.GetAppointments().Count;

        context.DataService.UpdateAppointment(CreateAppointment());

        Assert.Equal(countBefore, context.DataService.GetAppointments().Count);
    }

    [Fact]
    public void UpdateAppointmentStatus_ChangesStatus()
    {
        using var context = new TestDataServiceContext();
        var appointment = context.DataService.GetAppointments().First();

        context.DataService.UpdateAppointmentStatus(appointment.Id, AppointmentStatuses.Completed);

        Assert.Equal(AppointmentStatuses.Completed, context.DataService.GetAppointments().First(item => item.Id == appointment.Id).Status);
    }

    [Fact]
    public void UpdateAppointmentStatus_DoesNothingForMissingId()
    {
        using var context = new TestDataServiceContext();
        var statusesBefore = context.DataService.GetAppointments().Select(item => item.Status).ToList();

        context.DataService.UpdateAppointmentStatus(999, AppointmentStatuses.Cancelled);

        Assert.Equal(statusesBefore, context.DataService.GetAppointments().Select(item => item.Status).ToList());
    }

    [Fact]
    public void TogglePaid_SetsUnpaidAppointmentToPaid()
    {
        using var context = new TestDataServiceContext();
        var appointment = context.DataService.GetAppointments().First(item => !item.Paid);

        context.DataService.TogglePaid(appointment.Id);

        Assert.True(context.DataService.GetAppointments().First(item => item.Id == appointment.Id).Paid);
    }

    [Fact]
    public void TogglePaid_SetsPaidAppointmentToUnpaid()
    {
        using var context = new TestDataServiceContext();
        var appointment = context.DataService.GetAppointments().First(item => item.Paid);

        context.DataService.TogglePaid(appointment.Id);

        Assert.False(context.DataService.GetAppointments().First(item => item.Id == appointment.Id).Paid);
    }

    [Fact]
    public void TogglePaid_DoesNothingForMissingId()
    {
        using var context = new TestDataServiceContext();
        var paidStates = context.DataService.GetAppointments().Select(item => item.Paid).ToList();

        context.DataService.TogglePaid(999);

        Assert.Equal(paidStates, context.DataService.GetAppointments().Select(item => item.Paid).ToList());
    }

    [Fact]
    public void DeleteAppointment_RemovesExistingRecord()
    {
        using var context = new TestDataServiceContext();
        var appointment = context.DataService.GetAppointments().First();

        context.DataService.DeleteAppointment(appointment.Id);

        Assert.DoesNotContain(context.DataService.GetAppointments(), item => item.Id == appointment.Id);
    }

    [Fact]
    public void DeleteAppointment_DoesNothingForMissingId()
    {
        using var context = new TestDataServiceContext();
        var countBefore = context.DataService.GetAppointments().Count;

        context.DataService.DeleteAppointment(999);

        Assert.Equal(countBefore, context.DataService.GetAppointments().Count);
    }

    [Fact]
    public void GetUsers_ReturnsUsersSortedByName()
    {
        using var context = new TestDataServiceContext();
        context.DataService.AddUser(new User { Name = "Zed", Email = "zed@example.com", Password = "secret1" });
        context.DataService.AddUser(new User { Name = "Anna", Email = "anna@example.com", Password = "secret2" });

        var names = context.DataService.GetUsers().Select(user => user.Name).ToList();

        Assert.Equal(names.OrderBy(name => name).ToList(), names);
    }

    [Fact]
    public void GetUserByEmail_IsCaseInsensitive()
    {
        using var context = new TestDataServiceContext();

        var user = context.DataService.GetUserByEmail("DEMO@CLINIC.LOCAL");

        Assert.NotNull(user);
        Assert.Equal("Demo User", user!.Name);
    }

    [Fact]
    public void GetUserByEmail_ReturnsNullForUnknownEmail()
    {
        using var context = new TestDataServiceContext();

        var user = context.DataService.GetUserByEmail("missing@example.com");

        Assert.Null(user);
    }

    [Fact]
    public void AddUser_AssignsNextId()
    {
        using var context = new TestDataServiceContext();
        var user = new User { Name = "New User", Email = "new@example.com", Password = "secret1" };

        var added = context.DataService.AddUser(user);

        Assert.True(added);
        Assert.Equal(2, user.Id);
    }

    [Fact]
    public void AddUser_PersistsUser()
    {
        using var context = new TestDataServiceContext();

        context.DataService.AddUser(new User { Name = "Stored User", Email = "stored@example.com", Password = "secret1" });

        Assert.Contains(context.DataService.GetUsers(), user => user.Email == "stored@example.com");
    }

    [Fact]
    public void AddUser_ReturnsFalseForDuplicateEmailIgnoringCase()
    {
        using var context = new TestDataServiceContext();

        var added = context.DataService.AddUser(new User { Name = "Duplicate", Email = "DEMO@CLINIC.LOCAL", Password = "secret1" });

        Assert.False(added);
    }

    [Fact]
    public void SeedFiles_ContainValidJson()
    {
        using var context = new TestDataServiceContext();

        var usersJson = File.ReadAllText(context.UsersFilePath);
        var appointmentsJson = File.ReadAllText(context.AppointmentsFilePath);

        Assert.NotNull(JsonSerializer.Deserialize<List<User>>(usersJson));
        Assert.NotNull(JsonSerializer.Deserialize<List<Appointment>>(appointmentsJson));
    }

    private static Appointment CreateAppointment(
        string name = "Test Patient",
        string service = "Терапевтична консультація",
        decimal price = 0m) =>
        new()
        {
            Name = name,
            Email = "patient@example.com",
            Phone = "+380999999999",
            Service = service,
            DoctorName = "Erica Lim",
            Date = DateTime.Today.AddDays(2),
            Time = "10:00",
            Price = price
        };
}