using AppointmentSystem.Data;

namespace AppointmentSystem.Services;

public class BookingService
{
    // Впроваджуємо контекст БД прямо в сервіс
    private readonly ApplicationDbContext _db;

    public BookingService(ApplicationDbContext db)
    {
        _db = db;
    }

    public string ValidateBooking(int doctorId, DateTime time)
    {
        if (doctorId == 0) return "Оберіть майстра!";
        if (time < DateTime.Now) return "Час уже минув.";
        if (time.Hour < 9 || time.Hour >= 19) return "Робочий час: 09:00 - 19:00.";
        
        // Перевірка на зайнятість
        bool isBusy = _db.Appointments.Any(a => a.DoctorId == doctorId && a.AppointmentDate == time);
        if (isBusy) return "Цей час уже заброньовано.";

        return "OK"; // Все добре
    }
}