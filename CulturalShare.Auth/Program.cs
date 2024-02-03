using CulturalShare.Auth.API.Configuration.Base;
using CulturalShare.Auth.API.DependencyInjection;
using CulturalShare.Auth.Services;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
builder.InstallServices(typeof(IServiceInstaller).Assembly);
var app = builder.Build();

// Seed database.
//using (var scope = app.Services.CreateScope())
//{
//    try
//    {
//        var db = scope.ServiceProvider.GetRequiredService<AuthDBContext>();
//        for (int i = 0; i < 1; i++)
//        {
//            db.Users.Add(new UserEntity()
//            {
//                Email = "User@gmail.com",
//                FirstName = "FirstName",
//                LastName = "LastName",
//                PasswordHash = new byte[] {1,2,34,3,4,5,6,},
//                PasswordSalt = new byte[] {1,2,3,4,5,6,},
//            });

//            db.SaveChanges();
//        }
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine(ex.Message);
//        throw ex;
//    }
//}

app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<AuthenticationService>();

app.MapHealthChecks("/_health", new HealthCheckOptions()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
