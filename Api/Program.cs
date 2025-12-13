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
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

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

// Registro das dependências
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
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVetorRepository, VetorRepository>();
builder.Services.AddScoped<IPartnerRepository, PartnerRepository>();
builder.Services.AddScoped<IBusinessTypeRepository, BusinessTypeRepository>();
builder.Services.AddScoped<IBusinessRepository, BusinessRepository>();
builder.Services.AddScoped<ICommissionRepository, CommissionRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
