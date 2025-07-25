using Congratulator.ApiTests;
using Congratulator.Contracts.Users;
using Congratulator.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;

public class UsersControllerIntegrationTests : IClassFixture<TestWebAppFactory<Congratulator.API.Program>>, IAsyncLifetime
{
    private readonly TestWebAppFactory<Congratulator.API.Program> _factory;
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public UsersControllerIntegrationTests(TestWebAppFactory<Congratulator.API.Program> factory, ITestOutputHelper output)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost:5135")
        });
        _output = output;
    }

    [Fact]
    public async Task CreateUser_ReturnsCreatedUser_WhenDataIsValid()
    {
        // Arrange
        var request = new CreateUserRequest { FirstName = "Alice", LastName = "Smith", Login = "asmith", Password = "Pass123" };
        var formData = ToFormData(request);

        _output.WriteLine($"[TEST] Sending POST to: /api/Users/create");

        // Act
        var response = await _client.PostAsync("/api/Users/create", formData);
        _output.WriteLine($"[TEST] Server response: {await response.Content.ReadAsStringAsync()}");

        // Assert
        response.EnsureSuccessStatusCode();
        var user = await response.Content.ReadFromJsonAsync<UserDTO>();
        Assert.NotNull(user);
        Assert.Equal("asmith", user.Login);
        Assert.Equal("Alice", user.FirstName);
    }

    [Fact]
    public async Task CreateUser_ReturnsConflict_WhenUserAlreadyExists()
    {
        // Arrange
        var request = new CreateUserRequest { FirstName = "Bob", LastName = "Brown", Login = "bbrown", Password = "Pass!23" };
        var formData = ToFormData(request);

        // First create
        await _client.PostAsync("/api/Users/create", formData);

        // Act
        var second = await _client.PostAsync("/api/Users/create", formData);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    private MultipartFormDataContent ToFormData(CreateUserRequest request)
    {
        var form = new MultipartFormDataContent
        {
            { new StringContent(request.FirstName), nameof(request.FirstName) },
            { new StringContent(request.LastName), nameof(request.LastName) },
            { new StringContent(request.Login), nameof(request.Login) },
            { new StringContent(request.Password), nameof(request.Password) }
        };
        if (!string.IsNullOrEmpty(request.TelegramId))
            form.Add(new StringContent(request.TelegramId), nameof(request.TelegramId));
        return form;
    }

    public async Task InitializeAsync()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
