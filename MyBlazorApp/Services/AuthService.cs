namespace MyBlazorApp.Services
{
    using MyBlazorApp.Models;
    using System;
    using System.Linq;

    public class AuthService
    {
        private readonly DataService _dataService;
        public User? CurrentUser { get; private set; }

        public bool IsLoggedIn => CurrentUser != null;

        public event Action? OnChange;

        public AuthService(DataService dataService)
        {
            _dataService = dataService;
        }

        public bool Login(string email, string password)
        {
            var user = _dataService.GetUserByEmail(email);
            // УВАГА: У реальному додатку ніколи не зберігайте паролі у відкритому вигляді!
            // Використовуйте хешування (наприклад, BCrypt.Net).
            if (user != null && user.Password == password)
            {
                CurrentUser = user;
                NotifyStateChanged();
                return true;
            }
            return false;
        }

        public void Logout()
        {
            CurrentUser = null;
            NotifyStateChanged();
        }

        public bool Register(User newUser) => _dataService.AddUser(newUser);

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}