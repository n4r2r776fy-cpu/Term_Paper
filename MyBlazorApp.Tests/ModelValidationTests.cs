using System.ComponentModel.DataAnnotations;
using MyBlazorApp.Models;

namespace MyBlazorApp.Tests;

public class ModelValidationTests
{
    [Fact]
    public void ValidUser_PassesValidation()
    {
        var user = new User
        {
            Name = "Valid User",
            Email = "valid@example.com",
            Password = "secret1"
        };

        var result = Validate(user);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", "Ім'я є обов'язковим")]
    [InlineData(null, "Ім'я є обов'язковим")]
    public void User_RequiresName(string? name, string expectedError)
    {
        var user = new User
        {
            Name = name!,
            Email = "valid@example.com",
            Password = "secret1"
        };

        var result = Validate(user);

        Assert.Contains(result.Errors, error => error.ErrorMessage == expectedError);
    }

    [Theory]
    [InlineData("", "Email є обов'язковим")]
    [InlineData("invalid-email", "Неправильний формат Email")]
    public void User_ValidatesEmail(string email, string expectedError)
    {
        var user = new User
        {
            Name = "User",
            Email = email,
            Password = "secret1"
        };

        var result = Validate(user);

        Assert.Contains(result.Errors, error => error.ErrorMessage == expectedError);
    }

    [Theory]
    [InlineData("", "Пароль є обов'язковим")]
    [InlineData("12345", "Пароль має містити принаймні 6 символів")]
    public void User_ValidatesPassword(string password, string expectedError)
    {
        var user = new User
        {
            Name = "User",
            Email = "valid@example.com",
            Password = password
        };

        var result = Validate(user);

        Assert.Contains(result.Errors, error => error.ErrorMessage == expectedError);
    }

    [Fact]
    public void ValidAppointment_PassesValidation()
    {
        var appointment = CreateValidAppointment();

        var result = Validate(appointment);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", "Ім'я є обов'язковим")]
    [InlineData(null, "Ім'я є обов'язковим")]
    public void Appointment_RequiresName(string? name, string expectedError)
    {
        var appointment = CreateValidAppointment();
        appointment.Name = name!;

        var result = Validate(appointment);

        Assert.Contains(result.Errors, error => error.ErrorMessage == expectedError);
    }

    [Theory]
    [InlineData("", "Email є обов'язковим")]
    [InlineData(null, "Email є обов'язковим")]
    public void Appointment_RequiresEmail(string? email, string expectedError)
    {
        var appointment = CreateValidAppointment();
        appointment.Email = email!;

        var result = Validate(appointment);

        Assert.Contains(result.Errors, error => error.ErrorMessage == expectedError);
    }

    [Theory]
    [InlineData("", "Телефон є обов'язковим")]
    [InlineData(null, "Телефон є обов'язковим")]
    public void Appointment_RequiresPhone(string? phone, string expectedError)
    {
        var appointment = CreateValidAppointment();
        appointment.Phone = phone!;

        var result = Validate(appointment);

        Assert.Contains(result.Errors, error => error.ErrorMessage == expectedError);
    }

    [Theory]
    [InlineData(null, "Будь ласка, виберіть послугу")]
    [InlineData("", "Будь ласка, виберіть послугу")]
    public void Appointment_RequiresService(string? service, string expectedError)
    {
        var appointment = CreateValidAppointment();
        appointment.Service = service;

        var result = Validate(appointment);

        Assert.Contains(result.Errors, error => error.ErrorMessage == expectedError);
    }

    [Theory]
    [InlineData("", "Будь ласка, виберіть лікаря")]
    [InlineData(null, "Будь ласка, виберіть лікаря")]
    public void Appointment_RequiresDoctor(string? doctorName, string expectedError)
    {
        var appointment = CreateValidAppointment();
        appointment.DoctorName = doctorName!;

        var result = Validate(appointment);

        Assert.Contains(result.Errors, error => error.ErrorMessage == expectedError);
    }

    [Theory]
    [InlineData("", "Час є обов'язковим")]
    [InlineData(null, "Час є обов'язковим")]
    public void Appointment_RequiresTime(string? time, string expectedError)
    {
        var appointment = CreateValidAppointment();
        appointment.Time = time!;

        var result = Validate(appointment);

        Assert.Contains(result.Errors, error => error.ErrorMessage == expectedError);
    }

    [Fact]
    public void Appointment_RejectsPastDate()
    {
        var appointment = CreateValidAppointment();
        appointment.Date = DateTime.Today.AddDays(-1);

        var result = Validate(appointment);

        Assert.Contains(result.Errors, error => error.ErrorMessage == "Дата запису не може бути в минулому");
    }

    [Theory]
    [InlineData(AppointmentStatuses.Upcoming)]
    [InlineData(AppointmentStatuses.Completed)]
    [InlineData(AppointmentStatuses.Missed)]
    [InlineData(AppointmentStatuses.Cancelled)]
    public void AppointmentStatuses_DefineExpectedValues(string status)
    {
        var knownStatuses = new[]
        {
            AppointmentStatuses.Upcoming,
            AppointmentStatuses.Completed,
            AppointmentStatuses.Missed,
            AppointmentStatuses.Cancelled
        };

        Assert.Contains(status, knownStatuses);
    }

    private static (bool IsValid, List<ValidationResult> Errors) Validate(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);
        var isValid = Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return (isValid, results);
    }

    private static Appointment CreateValidAppointment() => new()
    {
        Name = "Patient",
        Email = "patient@example.com",
        Phone = "+380931112233",
        Service = "Терапевтична консультація",
        DoctorName = "Erica Lim",
        Date = DateTime.Today.AddDays(1),
        Time = "10:30"
    };
}