using Microsoft.AspNetCore.Authorization;

namespace SIGO.Api.Auth
{
    /// <summary>Nombres de rol tal como se siembran en la tabla Roles (claim "role" del JWT).</summary>
    public static class Roles
    {
        public const string Administrador = "Administrador";
        public const string MesaEntrada = "MesaEntrada";
        public const string Jefatura = "Jefatura";
        public const string Analista = "Analista";
        public const string Auditor = "Auditor";

        public static readonly string[] Todos = { Administrador, MesaEntrada, Jefatura, Analista, Auditor };
    }

    /// <summary>
    /// Matriz RBAC de SIGO: cada permiso (policy) declara qué roles lo tienen.
    /// El frontend replica esta tabla en <c>src/auth/permisos.ts</c> para ocultar
    /// navegación y acciones; la autoridad real es siempre esta clase.
    /// </summary>
    public static class Permisos
    {
        // Administración
        public const string AdministrarUsuarios = "AdministrarUsuarios";
        public const string AdministrarCatalogos = "AdministrarCatalogos";

        // Oficios
        public const string VerTodosLosOficios = "VerTodosLosOficios";   // bandeja "Todos" sin filtro de departamento
        public const string CrearOficios = "CrearOficios";
        public const string EditarOficios = "EditarOficios";
        public const string EliminarOficios = "EliminarOficios";
        public const string AsignarOficios = "AsignarOficios";
        public const string TramitarOficios = "TramitarOficios";         // cambiar estado, comentar, adjuntar

        // Reportes
        public const string VerReportes = "VerReportes";

        public static readonly IReadOnlyDictionary<string, string[]> Matriz = new Dictionary<string, string[]>
        {
            [AdministrarUsuarios] = new[] { Roles.Administrador },
            [AdministrarCatalogos] = new[] { Roles.Administrador },
            [VerTodosLosOficios] = new[] { Roles.Administrador, Roles.Auditor },
            [CrearOficios] = new[] { Roles.Administrador, Roles.MesaEntrada, Roles.Jefatura },
            [EditarOficios] = new[] { Roles.Administrador, Roles.MesaEntrada, Roles.Jefatura },
            [EliminarOficios] = new[] { Roles.Administrador },
            [AsignarOficios] = new[] { Roles.Administrador, Roles.Jefatura },
            [TramitarOficios] = new[] { Roles.Administrador, Roles.MesaEntrada, Roles.Jefatura, Roles.Analista },
            [VerReportes] = new[] { Roles.Administrador, Roles.Jefatura, Roles.Auditor },
        };

        /// <summary>Registra una policy por permiso y exige autenticación por defecto en todo endpoint.</summary>
        public static IServiceCollection AddSigoAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                foreach (var (permiso, roles) in Matriz)
                    options.AddPolicy(permiso, policy => policy.RequireRole(roles));

                // Deny-by-default: todo controlador exige token salvo que declare [AllowAnonymous].
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            return services;
        }
    }
}
