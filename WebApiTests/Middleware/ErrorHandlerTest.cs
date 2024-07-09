using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.TestHost;
using WebApi.Middleware;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using WebApi.Services;
using WebApi.Helpers;
using System.Net;

namespace WebApiTests
{
    public class ErrorHandlerTest
    {
        [Fact(DisplayName = "ErrorHandler Middleware tests")]
        public async Task MiddlewareTest_ReturnsNotFoundForRequest()
        {
            using var host = await new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseTestServer()
                        .ConfigureServices(static services =>
                        {
                            services.AddDbContext<DataContext>();
                            services.AddCors();
                            services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.IgnoreNullValues = true);
                            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

                            // configure strongly typed settings object
                            // services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
                            // services.Configure<AppSettings>()

                            // services.AddMyServices();
                            services.AddScoped<IAccountService, AccountService>();
                            services.AddScoped<IEmailService, EmailService>();
                        })
                        .Configure(app =>
                        {
                            // migrate database changes on startup (includes initial db creation)
                            // context.Database.Migrate();

                            // generated swagger json and swagger ui middleware
                            // app.UseSwagger();
                            // app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.json", "ASP.NET Core Sign-up and Verification API"));

                            app.UseRouting();

                            // global cors policy
                            app.UseCors(x => x
                                .SetIsOriginAllowed(origin => true)
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials());

                            app.UseMiddleware<ErrorHandlerMiddleware>();

                            // custom jwt auth middleware
                            // app.UseMiddleware<JwtMiddleware>();

                            app.UseEndpoints(x => {
                                x.MapControllers();

                                x.MapGet("/appexception", () => {
                                    throw new AppException("App Excpn");
                                });

                                x.MapGet("/keynotfoundexception", () => {
                                    throw new KeyNotFoundException("Key Not Found Excpn");
                                });

                                x.MapGet("/anyotherexception", () => {
                                    throw new Exception("Any other Excpn");
                                });
                            });
                        });
                })
                .StartAsync();

            var client = host.GetTestClient();

            var response = await client.GetAsync("/appexception");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.Equal("{\"message\":\"App Excpn\"}", responseBody);

            response = await client.GetAsync("/keynotfoundexception");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            responseBody = await response.Content.ReadAsStringAsync();
            Assert.Equal("{\"message\":\"Key Not Found Excpn\"}", responseBody);

            response = await client.GetAsync("/anyotherexception");
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            responseBody = await response.Content.ReadAsStringAsync();
            Assert.Equal("{\"message\":\"Any other Excpn\"}", responseBody);
        }
    }
}
