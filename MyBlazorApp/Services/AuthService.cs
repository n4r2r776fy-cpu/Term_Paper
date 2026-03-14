namespace MyBlazorApp.Services
{
    using MyBlazorApp.Models;
    using System;

    public class AuthService
    {
        private readonly DataService _dataService;
        private readonly IAuthStateStore _authStateStore;

        public User? CurrentUser { get; private set; }

        public bool IsLoggedIn => CurrentUser != null;

        public bool IsInitialized { get; private set; }

        public event Action? OnChange;

        public AuthService(DataService dataService, IAuthStateStore authStateStore)
        {
            _dataService = dataService;
            _authStateStore = authStateStore;
        }

        public async Task InitializeAsync()
        {
            if (IsInitialized)
            {
                return;
            }

            var email = await _authStateStore.GetUserEmailAsync();
            if (!string.IsNullOrWhiteSpace(email))
            {
                CurrentUser = _dataService.GetUserByEmail(email);
                if (CurrentUser is null)
                {
                    await _authStateStore.ClearUserEmailAsync();
                }
            }

            IsInitialized = true;
            NotifyStateChanged();
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            var user = _dataService.GetUserByEmail(email);
            if (user is null || user.Password != password)
            {
                return false;
            }

            CurrentUser = user;
            IsInitialized = true;
            await _authStateStore.SetUserEmailAsync(user.Email);
            NotifyStateChanged();
            return true;
        }

        public async Task LogoutAsync()
        {
            CurrentUser = null;
            IsInitialized = true;
            await _authStateStore.ClearUserEmailAsync();
            NotifyStateChanged();
        }

        public Task<bool> RegisterAsync(User newUser) => Task.FromResult(_dataService.AddUser(newUser));

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}