using System.ComponentModel;

namespace MetroGidDesktop.Services.Interfaces;

public interface IAuthState : INotifyPropertyChanged
{
    int? id { get; set; }
    string? Token { get; set; }
    string? Login { get; set; }
    string? Role { get; set; }

    bool IsAuthenticated();
    void Clear();
};