using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using WebApi;
using System.Text.Json;
using System.Text;
using WebApi.Models.Accounts;

namespace WebApiTests
{
    public class JwtMiddlewareTest
    {
        // cannot check as database is not loaded
        // or token needs to be created artificially as it can expire
        [Obsolete]
        public async Task ValidToken()
        {
            using var host = await Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .UseTestServer()
                        .UseUrls("http://localhost:4000");
                }).StartAsync();

            /*

        using var host = await new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {

                webBuilder.UseStartup<Startup>()
                    .UseTestServer()
                    .UseUrls("http://localhost:4000");
                    /*
                webBuilder
                    .UseTestServer()
                    .ConfigureServices(static services =>
                    {
                        // services.Configure<AppSettings>(webBuilder.Configure<AppSettings>())
                        services.AddDbContext<DataContext>();
                        services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.IgnoreNullValues = true);
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();

                        app.UseMiddleware<JwtMiddleware>();
                    });*/
            // })
            // .Build()
            // .Run();
            // .StartAsync();

            // first login, get token and then check context.Items["Account"]
            // cannot check as database is not loaded
            var client = host.GetTestClient();
            var strCon = new StringContent(JsonSerializer.Serialize(new { password = "string", email = "user@example.com" }), Encoding.UTF8, "application/json");
            strCon.Headers.Add("X-Forwarded-For", "0.0.0.0");
            var response = await client.PostAsync("/Accounts/authenticate", strCon);

            Assert.True(response.IsSuccessStatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var resp = JsonSerializer.Deserialize<AuthenticateResponse>(responseBody);
            Assert.NotNull(resp.JwtToken);

            // response = await client.GetAsync("Accounts");
            // Assert.True(response.IsSuccessStatusCode);

            var server = host.GetTestServer();
            // server.BaseAddress = new Uri("https://example.com/A/Path/");

            var context = await server.SendAsync(c =>
            {
                /* c.Request.Method = HttpMethods.Post;
                c.Request.Path = "/and/file.txt";
                c.Request.QueryString = new QueryString("?and=query");*/

                // c.Request.Headers.Authorization = "Bearer someTokenHere";
                // c.Request.Headers.Authorization = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjIiLCJuYmYiOjE3MjA1NTI3ODgsImV4cCI6MTcyMDU1MzY4OCwiaWF0IjoxNzIwNTUyNzg4fQ.cu3ttEiDn5sshXCeJzL10iu1vLE43CEVI-_FfDdWcnc";
                c.Request.Headers.Authorization = "Bearer " + resp.JwtToken;
            });

            Assert.NotNull(context.Items["Account"]);
       }

        [Fact]
        public async Task InvalidToken()
        {
            using var host = await Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .UseTestServer()
                        .UseUrls("http://localhost:4000");
                }).StartAsync();

            var server = host.GetTestServer();
            // server.BaseAddress = new Uri("https://example.com/A/Path/");

            var context = await server.SendAsync(c =>
            {
                /* c.Request.Method = HttpMethods.Post;
                c.Request.Path = "/and/file.txt";
                c.Request.QueryString = new QueryString("?and=query");*/

                c.Request.Headers.Authorization = "Bearer someTokenHere";
            });

            // check nothing is attached
            Assert.False(context.Items.ContainsKey("Account"));
       }
    }
}
