using Application.Interfaces.IUseCases;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.UseCases.AuthenticateUser;
using Application.UseCases.RefreshToken;
using Application.UseCases.Logout;
using Application.UseCases.CreateUser;
using Application.UseCases.UpdateUser;
using Application.UseCases.ChangePassword;
using Application.UseCases.ActivateDeactivateUser;
using Application.UseCases.ListUsers;
using Application.UseCases.GetUserById;
using Application.UseCases.CreateVetor;
using Application.UseCases.UpdateVetor;
using Application.UseCases.DeactivateVetor;
using Application.UseCases.ListVetores;
using Application.UseCases.GetVetorById;
using Application.UseCases.CreatePartner;
using Application.UseCases.UpdatePartner;
using Application.UseCases.ActivateDeactivatePartner;
using Application.UseCases.ListPartners;
using Application.UseCases.GetPartnerById;
using Application.UseCases.GetPartnerTree;
using Application.UseCases.CreateBusinessType;
using Application.UseCases.UpdateBusinessType;
using Application.UseCases.DeactivateBusinessType;
using Application.UseCases.ListBusinessTypes;
using Application.UseCases.GetBusinessTypeById;
using Application.UseCases.CreateBusiness;
using Application.UseCases.UpdateBusiness;
using Application.UseCases.CancelBusiness;
using Application.UseCases.ListBusinesses;
using Application.UseCases.GetBusinessById;
using Application.UseCases.ListPayments;
using Application.UseCases.ProcessPayment;
using Application.UseCases.GetBusinessPayments;
using Application.UseCases.PartnersReport;
using Application.UseCases.FinancialReport;
using Application.UseCases.BusinessReport;
using Application.UseCases.LogAudit;
using Application.UseCases.AuditLogQuery;
using Infraestructure.Repositories;
using Infraestructure.Services;
using Infraestructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.File(
        path: "logs/partnermesh-.log",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Iniciando PartnerMesh API");

// Adicionar Serilog
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Configuração do Entity Framework Core com SQL Server
builder.Services.AddDbContext<PartnerMeshDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração do Swagger com JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "PartnerMesh API", 
        Version = "v1",
        Description = "API do Sistema de Rede de Credenciamento / Vetores"
    });
    
    // Configuração para JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Exemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// Configuração JWT
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "SuperSecretKeyForDevelopmentOnly123!@#$%^&*()";
var key = Encoding.ASCII.GetBytes(jwtSecret);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Configuração CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173", "http://localhost:4000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Registro das dependências

#region UseCases

builder.Services.AddScoped<IAuthenticateUserUseCase, AuthenticateUserUseCase>();
builder.Services.AddScoped<IRefreshTokenUseCase, RefreshTokenUseCase>();
builder.Services.AddScoped<ILogoutUseCase, LogoutUseCase>();
builder.Services.AddScoped<ICreateUserUseCase, CreateUserUseCase>();
builder.Services.AddScoped<IUpdateUserUseCase, UpdateUserUseCase>();
builder.Services.AddScoped<IChangePasswordUseCase, ChangePasswordUseCase>();
builder.Services.AddScoped<IActivateDeactivateUserUseCase, ActivateDeactivateUserUseCase>();
builder.Services.AddScoped<IListUsersUseCase, ListUsersUseCase>();
builder.Services.AddScoped<IGetUserByIdUseCase, GetUserByIdUseCase>();
builder.Services.AddScoped<ICreateVetorUseCase, CreateVetorUseCase>();
builder.Services.AddScoped<IUpdateVetorUseCase, UpdateVetorUseCase>();
builder.Services.AddScoped<IDeactivateVetorUseCase, DeactivateVetorUseCase>();
builder.Services.AddScoped<IListVetoresUseCase, ListVetoresUseCase>();
builder.Services.AddScoped<IGetVetorByIdUseCase, GetVetorByIdUseCase>();
builder.Services.AddScoped<ICreatePartnerUseCase, CreatePartnerUseCase>();
builder.Services.AddScoped<IUpdatePartnerUseCase, UpdatePartnerUseCase>();
builder.Services.AddScoped<IActivateDeactivatePartnerUseCase, ActivateDeactivatePartnerUseCase>();
builder.Services.AddScoped<IListPartnersUseCase, ListPartnersUseCase>();
builder.Services.AddScoped<IGetPartnerByIdUseCase, GetPartnerByIdUseCase>();
builder.Services.AddScoped<IGetPartnerTreeUseCase, GetPartnerTreeUseCase>();
builder.Services.AddScoped<ICreateBusinessTypeUseCase, CreateBusinessTypeUseCase>();
builder.Services.AddScoped<IUpdateBusinessTypeUseCase, UpdateBusinessTypeUseCase>();
builder.Services.AddScoped<IDeactivateBusinessTypeUseCase, DeactivateBusinessTypeUseCase>();
builder.Services.AddScoped<IListBusinessTypesUseCase, ListBusinessTypesUseCase>();
builder.Services.AddScoped<IGetBusinessTypeByIdUseCase, GetBusinessTypeByIdUseCase>();
builder.Services.AddScoped<ICreateBusinessUseCase, CreateBusinessUseCase>();
builder.Services.AddScoped<IUpdateBusinessUseCase, UpdateBusinessUseCase>();
builder.Services.AddScoped<ICancelBusinessUseCase, CancelBusinessUseCase>();
builder.Services.AddScoped<IListBusinessesUseCase, ListBusinessesUseCase>();
builder.Services.AddScoped<IGetBusinessByIdUseCase, GetBusinessByIdUseCase>();
builder.Services.AddScoped<IListPaymentsUseCase, ListPaymentsUseCase>();
builder.Services.AddScoped<IProcessPaymentUseCase, ProcessPaymentUseCase>();
builder.Services.AddScoped<IGetBusinessPaymentsUseCase, GetBusinessPaymentsUseCase>();
builder.Services.AddScoped<IPartnersReportUseCase, PartnersReportUseCase>();
builder.Services.AddScoped<IFinancialReportUseCase, FinancialReportUseCase>();
builder.Services.AddScoped<IBusinessReportUseCase, BusinessReportUseCase>();
builder.Services.AddScoped<ILogAuditUseCase, LogAuditUseCase>();
builder.Services.AddScoped<IAuditLogQueryUseCase, AuditLogQueryUseCase>();

#endregion

#region Repositories

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVetorRepository, VetorRepository>();
builder.Services.AddScoped<IPartnerRepository, PartnerRepository>();
builder.Services.AddScoped<IBusinessTypeRepository, BusinessTypeRepository>();
builder.Services.AddScoped<IBusinessRepository, BusinessRepository>();
builder.Services.AddScoped<ICommissionRepository, CommissionRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

#endregion

#region Services

builder.Services.AddScoped<ITokenService, TokenService>();

#endregion

var app = builder.Build();

// Executar seeding do banco de dados
using (var scope = app.Services.CreateScope())
{
    await DatabaseSeeder.SeedAsync(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PartnerMesh API v1");
        c.RoutePrefix = string.Empty; // Para acessar Swagger na raiz
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "A aplicação falhou ao iniciar");
}
finally
{
    Log.CloseAndFlush();
}
