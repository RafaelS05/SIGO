using Microsoft.EntityFrameworkCore;
using SIGO.Api;
using SIGO.Api.Canales.Repository;
using SIGO.Api.Canales.Service;
using SIGO.Api.Departamentos.Repository;
using SIGO.Api.Departamentos.Service;
using SIGO.Api.Estados.Repository;
using SIGO.Api.Estados.Service;
using SIGO.Api.Oficios.Repository;
using SIGO.Api.Oficios.Service;
using SIGO.Api.Prioridades.Repository;
using SIGO.Api.Prioridades.Service;
using SIGO.Api.Roles.Repository;
using SIGO.Api.Roles.Service;
using SIGO.Api.TiposOficios.Repository;
using SIGO.Api.TiposOficios.Service;
using SIGO.Api.Usuarios.Repository;
using SIGO.Api.Usuarios.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Sigo")));

builder.Services.AddCors(options => options.AddPolicy("frontend", policy =>
    policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();
builder.Services.AddScoped<IDepartamentoService, DepartamentoService>();
builder.Services.AddScoped<IRolRepository, RolRepository>();
builder.Services.AddScoped<IRolService, RolService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IPrioridadRepository, PrioridadRepository>();
builder.Services.AddScoped<IPrioridadService, PrioridadService>();
builder.Services.AddScoped<IEstadoRepository, EstadoRepository>();
builder.Services.AddScoped<IEstadoService, EstadoService>();
builder.Services.AddScoped<ITipoOficioRepository, TipoOficioRepository>();
builder.Services.AddScoped<ITipoOficioService, TipoOficioService>();
builder.Services.AddScoped<ICanalRepository, CanalRepository>();
builder.Services.AddScoped<ICanalService, CanalService>();
builder.Services.AddScoped<IOficioRepository, OficioRepository>();
builder.Services.AddScoped<IOficioService, OficioService>();

var app = builder.Build();

// Traduce errores esperables a respuestas HTTP claras: reglas de negocio → 400,
// restricciones FK (catálogo referenciado por oficios) → 409.
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (InvalidOperationException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new { title = ex.Message });
    }
    catch (DbUpdateException)
    {
        context.Response.StatusCode = StatusCodes.Status409Conflict;
        await context.Response.WriteAsJsonAsync(new { title = "No se puede completar: el registro está referenciado por otros datos." });
    }
});

// Crea la BD (LocalDB) y siembra catálogos en el arranque. Para producción,
// sustituir por migraciones: dotnet ef migrations add Inicial && dotnet ef database update
using (var scope = app.Services.CreateScope())
{
    await DbSeeder.SeedAsync(scope.ServiceProvider.GetRequiredService<AppDbContext>());
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("frontend");

app.MapControllers();

app.Run();
