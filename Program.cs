using System.Security.Claims;
using System.Text;
using JwtAspNetBlazor;
using JwtAspNetBlazor.Extensions;
using JwtAspNetBlazor.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<TokenService>();

// Autenticação e autorização tem que ser usado nessa ordem para funcionar corretamente
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.PrivateKey)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization(x =>
{
    x.AddPolicy("admin", p => p.RequireRole("admin"));
});

var app = builder.Build();

// Autenticação e autorização tem que ser usado nessa ordem para funcionar corretamente
app.UseAuthentication(); // tem que ser usado antes das rotas
app.UseAuthorization();


app.MapGet("/Login", (TokenService service) =>

{

    var user = new User(
        Id: 1,
        Name: "Guilherme Henrique",
        Email: "Teste@teste.com",
        Image: "https://teste.com",
        Password: "xyz",
        Roles: new[] { "Student, Premium" });

    return service.Create(user);

}


);

app.MapGet("/restrito", (ClaimsPrincipal user) => new

{
    id = user.Id(),
    name = user.Name(),
    email = user.Email(),
    givenName = user.GivenName(),
    image = user.Image()

}).RequireAuthorization();
app.MapGet("/admin", () => "você tem acesso!").RequireAuthorization("admin");

app.Run();
