using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MetroGid.Controllers.Utility.DTO.Auth;
using MetroGidDesktop.Services.Interfaces;

namespace MetroGidDesktop.Services.Concrete;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthState _authState;

    public AuthService(HttpClient httpClient, IAuthState authState)
    {
        _httpClient = httpClient;
        _authState = authState;
    }

    public async Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto), "Данные регистрации не могут быть пустыми.");

        if (string.IsNullOrWhiteSpace(dto.Login))
            throw new ArgumentException("Логин не может быть пустым.", nameof(dto.Login));

        if (string.IsNullOrWhiteSpace(dto.Password))
            throw new ArgumentException("Пароль не может быть пустым.", nameof(dto.Password));

        if (string.IsNullOrWhiteSpace(dto.Mail))
            throw new ArgumentException("Email не может быть пустым.", nameof(dto.Mail));

        var response = await _httpClient.PostAsJsonAsync("api/auth/register", dto);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<AuthResponseDTO>();

            if (result == null)
                throw new InvalidOperationException("Сервер вернул пустой ответ при регистрации.");

            return result;
        }

        throw new InvalidOperationException("Неизвестная ошибка при регистрации.");
    }
    
    public async Task<AuthResponseDTO> LogInAsync(LoginRequestDTO dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto), "Данные входа не могут быть пустыми.");

        if (string.IsNullOrWhiteSpace(dto.Login))
            throw new ArgumentException("Логин не может быть пустым.", nameof(dto.Login));

        if (string.IsNullOrWhiteSpace(dto.Password))
            throw new ArgumentException("Пароль не может быть пустым.", nameof(dto.Password));

        var response = await _httpClient.PostAsJsonAsync("api/auth/login", dto);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<AuthResponseDTO>();

            if (result == null)
                throw new InvalidOperationException("Сервер вернул пустой ответ при регистрации.");

            return result;
        }

        throw new InvalidOperationException("Неизвестная ошибка при выходе.");
    }

    public async Task<bool> LogOutAsync()
    {
        var response = await _httpClient.PostAsync("api/auth/logout", null);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<AuthResponseDTO>();

            if (result == null)
                throw new InvalidOperationException("Сервер вернул пустой ответ при выходе.");

            return true;
        }

        return false;
    }
    
    public async Task<bool> UnregisterAsync(UnregisterRequestDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto), "Данные удаления аккаунта не могут быть пустыми.");

        if (string.IsNullOrWhiteSpace(dto.Password))
            throw new ArgumentException("Пароль не может быть пустым.", nameof(dto.Password));

        var response = await _httpClient.SendAsync(
            new HttpRequestMessage(HttpMethod.Delete, "api/auth/unregister")
            {
                Content = JsonContent.Create(dto)
            }
        );

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<AuthResponseDTO>();

            if (result == null)
                throw new InvalidOperationException("Сервер вернул пустой ответ при удалении аккаунта.");

            return true;
        }

        return false;
    }
}