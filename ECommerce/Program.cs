using ECommerce;
using ECommerce.APIBehavior;
using ECommerce.Configuration;
using ECommerce.Filters;
using ECommerce.Models;
using ECommerce.Security;
using ECommerce.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);




// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ExceptionFilters));
    options.Filters.Add(typeof(ParseBadRequest));
}).ConfigureApiBehaviorOptions(BadRequestBehavior.Parse);
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("https://localhost:44351", "http://127.0.0.1:5173")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
        });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("EcommerceConnection"));
});
builder.Services.AddTransient<IProductRepository, ProductRepository>();
builder.Services.AddTransient<IOrderRepository, OrderRepository>();
builder.Services.AddCors(option =>
{
    option.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
        .WithExposedHeaders(new string[] { "TotalNumberOfRecords" });

    });
});
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{

    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.SignIn.RequireConfirmedEmail = true;
}).AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();
builder.Services.AddAuthentication(auth =>
{
    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    googleOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var key = Encoding.UTF8.GetBytes(builder.Configuration["AuthSettings:Key"]);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        RequireExpirationTime = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuerSigningKey = true
    };
});
builder.Services.AddTransient<ECommerce.Services.IAuthorizationService, AuthorizationService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IMailService, SendGridMailService>();
builder.Services.AddTransient<IRoleService, RoleManagerService>();
builder.Services.AddTransient<IReviewService, ReviewService>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ViewItemsPolicy",
        policy => policy.RequireRole("Adminstration"));


    options.AddPolicy("testhandler", policy =>
    policy.Requirements.Add(new UserCanOnlyAccessAndEditHisDataRequirement()));
});
builder.Services.AddScoped<IAuthorizationHandler, UserCanAccessOnlyHisDataHandler>();




var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles")),
    RequestPath = "/StaticFiles",
    EnableDefaultFiles = true
});
app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

app.UseAuthentication( );

app.UseAuthorization();

app.MapControllers();
 
app.Run();
