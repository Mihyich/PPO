using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MetroGidDesktop.Services.Interfaces;

namespace MetroGidDesktop.Services.Concrete;

public class AuthState : IAuthState
{
    private int? _id;
    private string? _token;
    private string? _login;
    private string? _role;

    public int? id
    {
        get => _id;
        set => SetField(ref _id, value);
    }

    public string? Token
    {
        get => _token;
        set => SetField(ref _token, value);
    }

    public string? Login
    {
        get => _login;
        set => SetField(ref _login, value);
    }

    public string? Role
    {
        get => _role;
        set => SetField(ref _role, value);
    }

    public bool IsAuthenticated() => !string.IsNullOrEmpty(Token);

    public void Clear()
    {
        id = null;
        Token = null;
        Login = null;
        Role = null;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}