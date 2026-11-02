using JWTTokenBaseAuthentication;
using JWTTokenBaseAuthentication.Services;
using JWTTokenBaseAuthentication.Services.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. DATABASE & DEPENDENCY INJECTION
// ============================================================

// Register EF Core DbContext with SQL Server.
// Users (with their roles and password hashes) are stored here.
// During login we check the credentials against this database.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

// UserService  -> handles login/register and generates the JWT token
// ProductService -> business logic for products (protected by [Authorize])
builder.Services.AddScoped<IUserInterface, UserService>();
builder.Services.AddScoped<IProductInterface, ProductService>();


// ============================================================
// 2. JWT AUTHENTICATION CONFIGURATION
// ============================================================

// HOW JWT AUTHENTICATION WORKS (Big Picture):
//
//  Step 1: Client sends username + password to the Login API.
//  Step 2: Server verifies credentials against the database.
//  Step 3: If valid, server GENERATES a JWT token and signs it
//          with a SecretKey. The token contains claims
//          (UserId, Email, Role, Expiry time, etc.).
//  Step 4: Client stores the token (localStorage / memory in Angular).
//  Step 5: For every protected API call, client sends the token in
//          the request header:
//              Authorization: Bearer <token>
//  Step 6: Server VALIDATES the token (signature, issuer, audience,
//          expiry). If valid, the user is authenticated and the
//          claims (like Role) are available to the controller.
//  Step 7: If invalid/expired/missing -> server returns 401 Unauthorized.
//          If valid but role not allowed -> server returns 403 Forbidden.
//
// A JWT token has 3 parts separated by dots:
//      HEADER.PAYLOAD.SIGNATURE
//      - Header    : algorithm used (e.g. HS256) and token type
//      - Payload   : claims (user data, role, expiry)
//      - Signature : hash of header + payload using the SecretKey
//                    (this prevents anyone from tampering with the token)

// Read the "JWT" section from appsettings.json.
// It contains: Issuer, Audience, SecretKey (and optionally expiry minutes).
var jwtSettings = builder.Configuration.GetSection("JWT");

// Header format sent by client -> Authorization: Bearer sjdgjkdfskjgf
builder.Services.AddAuthentication(options =>
{
    // DefaultAuthenticateScheme: which scheme is used to READ and validate
    // the incoming token on every request.
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

    // DefaultChallengeScheme: which scheme decides what happens when the
    // user is NOT authenticated (returns 401 Unauthorized).
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(x =>
{
    // These parameters tell ASP.NET Core HOW to validate every incoming token.
    x.TokenValidationParameters = new TokenValidationParameters
    {
        // Check that the token was issued by a trusted server (the "iss" claim).
        ValidateIssuer = true,

        // Check that the token was meant for this API/application (the "aud" claim).
        ValidateAudience = true,

        // Check that the token has not expired (the "exp" claim).
        ValidateLifetime = true,

        // Check that the signature is valid, i.e. the token was NOT modified
        // and was signed with our secret key.
        ValidateIssuerSigningKey = true,

        // The expected values. They must match what was used while
        // generating the token in UserService.
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],

        // SYMMETRIC KEY AUTHENTICATION:
        // The SAME secret key is used to
        //   1) sign the token when we generate it (login)
        //   2) verify the token when we validate it (every request)
        // Keep this key secret (use User Secrets / Environment Variables /
        // Azure Key Vault in production, never commit it to Git).
        // The key should be long enough (at least 32 characters for HS256).
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["SecretKey"])
        ),

        // By default .NET allows 5 minutes of extra time after expiry.
        // Zero means the token expires exactly at its expiry time.
        ClockSkew = TimeSpan.Zero
    };
});


// ============================================================
// 3. ROLE-BASED AUTHORIZATION
// ============================================================

// AUTHENTICATION vs AUTHORIZATION:
//   Authentication = "Who are you?"        (validate the token)
//   Authorization  = "What can you access?" (check the role/policy)
//
// The Role claim stored inside the JWT token is used here.
// Policy "AdminOnly" allows users whose role is Admin OR Employee.
// Usage in controller:  [Authorize(Policy = "AdminOnly")]
// Or directly by role:  [Authorize(Roles = "Admin")]
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin", "Employee"));
});

builder.Services.AddControllers();


// ============================================================
// 4. OPENAPI + SCALAR (API DOCUMENTATION UI)
// ============================================================

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "Demo API",
            Version = "v1"
        };

        // Define the JWT Bearer security scheme so the Scalar UI shows an
        // "Authorize" option where you can paste the token.
        // Scalar then automatically adds the header to every request:
        //      Authorization: Bearer <token>
        var securityScheme = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Enter JWT Bearer token: Bearer {token}",
            In = ParameterLocation.Header,   // token is sent in the HTTP header
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT"
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = securityScheme;

        // Apply the Bearer scheme globally, so all endpoints show the lock icon
        // and use the token when testing from Scalar.
        var requirement = new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
        };

        document.Security ??= new List<OpenApiSecurityRequirement>();
        document.Security.Add(requirement);

        return Task.CompletedTask;
    });
});


// ============================================================
// 5. CORS (FOR ANGULAR FRONTEND)
// ============================================================

// Angular runs on a different origin (localhost:4200) than this API,
// so the browser blocks calls unless CORS is allowed.
// AllowAnyHeader is required so the "Authorization" header
// (which carries the JWT token) can be sent from Angular.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();


// ============================================================
// 6. HTTP REQUEST PIPELINE (MIDDLEWARE)
// ============================================================

// Enable API documentation only in Development environment.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.MapGet("/swagger", () => Results.Redirect("/scalar/v1"));
    app.MapGet("/scalar", () => Results.Redirect("/scalar/v1"));
    app.MapGet("/scaler", () => Results.Redirect("/scalar/v1"));
}

app.UseHttpsRedirection();   // Redirect HTTP -> HTTPS (token is safer over HTTPS)

app.UseCors("AllowAngular"); // CORS must come before authentication

// MIDDLEWARE ORDER IS VERY IMPORTANT:
//   UseAuthentication() MUST come BEFORE UseAuthorization()
//
// UseAuthentication():
//   - Reads the "Authorization: Bearer <token>" header
//   - Validates the token using TokenValidationParameters above
//   - If valid, builds the User (ClaimsPrincipal) with all claims
//     so User.Identity.IsAuthenticated = true
app.UseAuthentication();

// UseAuthorization():
//   - Checks [Authorize], [Authorize(Roles = "...")] or policies
//   - Compares the role claim from the token with the allowed roles
//   - Returns 401 if not authenticated, 403 if role not allowed
app.UseAuthorization();

// Maps controller routes (e.g. /api/Product) to the pipeline.
app.MapControllers();

app.Run();