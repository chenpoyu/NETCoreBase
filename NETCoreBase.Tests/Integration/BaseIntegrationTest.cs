using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NETCoreBase.API;
using Xunit;

namespace NETCoreBase.Tests.Integration
{
    [Collection("Integration")]
    public abstract class BaseIntegrationTest : IClassFixture<SqlServerFixture>
    {
        protected readonly HttpClient Client;
        protected readonly SqlServerFixture Fixture;

        protected static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        protected BaseIntegrationTest(SqlServerFixture fixture)
        {
            Fixture = fixture;

            var factory = new WebApplicationFactory<NETCoreBase.API.Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Testing");
                    builder.ConfigureAppConfiguration((ctx, config) =>
                    {
                        // Override the connection string with the test container's
                        config.AddInMemoryCollection(new[]
                        {
                            new System.Collections.Generic.KeyValuePair<string, string?>(
                                "ConnectionStrings:DefaultConnection",
                                GetTestConnectionString(fixture.ConnectionString))
                        });
                    });
                    builder.ConfigureServices(services =>
                    {
                        // Disable HTTPS requirement for tests
                    });
                });

            Client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                HandleCookies = false,
            });
        }

        private static string GetTestConnectionString(string containerConnStr)
        {
            // Append the NETCoreBase database name to the container connection string
            var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(containerConnStr)
            {
                InitialCatalog = "NETCoreBase",
                TrustServerCertificate = true,
            };
            return builder.ConnectionString;
        }

        protected async Task<T?> PostAsync<T>(string url, object body, string? bearerToken = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = new StringContent(
                JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            if (bearerToken != null)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await Client.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }

        protected static StringContent Json(object value) =>
            new(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
    }

    [CollectionDefinition("Integration")]
    public class IntegrationCollection : ICollectionFixture<SqlServerFixture> { }
}
