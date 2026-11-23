using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SIGO.Api.Canales.Model;
using SIGO.Api.Departamentos.Model;
using SIGO.Api.Estados.Model;
using SIGO.Api.Prioridades.Model;
using SIGO.Api.Roles.Model;
using SIGO.Api.TiposOficios.Model;
using SIGO.Api.Usuarios.Model;

namespace SIGO.Api
{
    /// <summary>
    /// Carga inicial de catálogos y usuarios demo (documento SIGO, sección Configuración).
    /// Idempotente: solo siembra si la tabla está vacía. Además asigna la contraseña inicial
    /// (Auth:PasswordInicial) a todo usuario que aún no tenga credencial.
    /// </summary>
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context, IPasswordHasher<Usuario> hasher, IConfiguration configuration)
        {
            await context.Database.EnsureCreatedAsync();
            await ActualizarEsquemaAsync(context);

            if (!await context.Estados.AnyAsync())
            {
                context.Estados.AddRange(
                    new Estado { Nombre = "Recibido", CerradoFlag = false },
                    new Estado { Nombre = "Asignado", CerradoFlag = false },
                    new Estado { Nombre = "En proceso", CerradoFlag = false },
                    new Estado { Nombre = "Pendiente de respuesta", CerradoFlag = false },
                    new Estado { Nombre = "Enviado a respuesta", CerradoFlag = false },
                    new Estado { Nombre = "Cerrado", CerradoFlag = true },
                    new Estado { Nombre = "Rechazado/Devolución", CerradoFlag = true });
            }

            if (!await context.Prioridades.AnyAsync())
            {
                context.Prioridades.AddRange(
                    new Prioridad { Nombre = "Urgente", Nivel = 1, SlaHoras = 8 },
                    new Prioridad { Nombre = "Alta", Nivel = 2, SlaHoras = 24 },
                    new Prioridad { Nombre = "Media", Nivel = 3, SlaHoras = 72 },
                    new Prioridad { Nombre = "Baja", Nivel = 4, SlaHoras = 168 });
            }

            if (!await context.Canales.AnyAsync())
            {
                context.Canales.AddRange(
                    new Canal { Nombre = "Correo" },
                    new Canal { Nombre = "Físico" },
                    new Canal { Nombre = "Plataforma" });
            }

            if (!await context.TiposOficios.AnyAsync())
            {
                context.TiposOficios.AddRange(
                    new TipoOficio { Nombre = "Solicitud", RequiereRespuestaFlag = true },
                    new TipoOficio { Nombre = "Informativo", RequiereRespuestaFlag = false },
                    new TipoOficio { Nombre = "Circular", RequiereRespuestaFlag = false },
                    new TipoOficio { Nombre = "Respuesta", RequiereRespuestaFlag = false },
                    new TipoOficio { Nombre = "Requerimiento legal", RequiereRespuestaFlag = true });
            }

            if (!await context.Roles.AnyAsync())
            {
                context.Roles.AddRange(
                    new Rol { Nombre = "Administrador" },
                    new Rol { Nombre = "MesaEntrada" },
                    new Rol { Nombre = "Jefatura" },
                    new Rol { Nombre = "Analista" },
                    new Rol { Nombre = "Auditor" });
            }

            if (!await context.Departamentos.AnyAsync())
            {
                context.Departamentos.AddRange(
                    new Departamento { Nombre = "Alcaldía", Sigla = "ALC", Activo = true },
                    new Departamento { Nombre = "Recursos Humanos", Sigla = "REHU", Activo = true },
                    new Departamento { Nombre = "Tecnologías de Información", Sigla = "TI", Activo = true },
                    new Departamento { Nombre = "Asesoría Legal", Sigla = "LEG", Activo = true },
                    new Departamento { Nombre = "Mesa de Entrada", Sigla = "MDE", Activo = true });
            }

            await context.SaveChangesAsync();

            if (!await context.Usuarios.AnyAsync())
            {
                var deptoPorSigla = await context.Departamentos.ToDictionaryAsync(d => d.Sigla, d => d.Id);
                var rolPorNombre = await context.Roles.ToDictionaryAsync(r => r.Nombre, r => r.Id);

                context.Usuarios.AddRange(
                    new Usuario
                    {
                        Nombre = "Rafael Solano", Correo = "rafaeldemo@gmail.com",
                        IdDepartamento = deptoPorSigla["TI"], Cargo = "Administrador del sistema", OrigenAd = false,
                        Roles = { new UsuarioRol { IdRol = rolPorNombre["Administrador"] } },
                    },
                    new Usuario
                    {
                        Nombre = "María Jiménez", Correo = "maria.jimenez@demo.local",
                        IdDepartamento = deptoPorSigla["MDE"], Cargo = "Mesa de entrada", OrigenAd = false,
                        Roles = { new UsuarioRol { IdRol = rolPorNombre["MesaEntrada"] } },
                    },
                    new Usuario
                    {
                        Nombre = "Carlos Rodríguez", Correo = "carlos.rodriguez@demo.local",
                        IdDepartamento = deptoPorSigla["REHU"], Cargo = "Jefe de departamento", OrigenAd = false,
                        Roles = { new UsuarioRol { IdRol = rolPorNombre["Jefatura"] } },
                    },
                    new Usuario
                    {
                        Nombre = "Ana Vargas", Correo = "ana.vargas@demo.local",
                        IdDepartamento = deptoPorSigla["LEG"], Cargo = "Analista legal", OrigenAd = false,
                        Roles = { new UsuarioRol { IdRol = rolPorNombre["Analista"] } },
                    },
                    new Usuario
                    {
                        Nombre = "Luis Mora", Correo = "luis.mora@demo.local",
                        IdDepartamento = deptoPorSigla["ALC"], Cargo = "Auditor interno", OrigenAd = false,
                        Roles = { new UsuarioRol { IdRol = rolPorNombre["Auditor"] } },
                    });

                await context.SaveChangesAsync();
            }

            // Credencial inicial para cuentas sin contraseña (usuarios demo o BD creada antes del módulo Auth).
            var passwordInicial = configuration["Auth:PasswordInicial"];
            if (!string.IsNullOrWhiteSpace(passwordInicial))
            {
                var sinCredencial = await context.Usuarios.Where(u => u.PasswordHash == null).ToListAsync();
                foreach (var usuario in sinCredencial)
                    usuario.PasswordHash = hasher.HashPassword(usuario, passwordInicial);

                if (sinCredencial.Count > 0)
                    await context.SaveChangesAsync();
            }
        }

        // EnsureCreated no altera tablas existentes: se agregan las columnas del módulo Auth
        // si la BD se creó con la versión anterior del modelo (idempotente).
        private static async Task ActualizarEsquemaAsync(AppDbContext context)
        {
            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Usuarios', 'PasswordHash') IS NULL ALTER TABLE Usuarios ADD PasswordHash nvarchar(max) NULL;");
            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Usuarios', 'UltimoAcceso') IS NULL ALTER TABLE Usuarios ADD UltimoAcceso datetime2 NULL;");
        }
    }
}
