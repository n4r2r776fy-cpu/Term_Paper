using MyBlazorApp.Models;

namespace MyBlazorApp.Services
{
    public class AuthService
    {
        public User? CurrentUser { get; private set; }

        public bool IsLoggedIn => CurrentUser != null;

        public void Login(User user)
        {
            CurrentUser = user;
        }

        public void Logout()
        {
            CurrentUser = null;
        }
    }
}