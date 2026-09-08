using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SIGO.Api.Auth.DTO;
using SIGO.Api.Usuarios.DTO;
using SIGO.Api.Usuarios.Model;
using SIGO.Api.Usuarios.Repository;

namespace SIGO.Api.Auth.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarios;
        private readonly AppDbContext _context; // unit of work — el servicio confirma los cambios
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<Usuario> _hasher;

        public AuthService(
            IUsuarioRepository usuarios,
            AppDbContext context,
            IConfiguration configuration,
            IPasswordHasher<Usuario> hasher)
        {
            _usuarios = usuarios;
            _context = context;
            _configuration = configuration;
            _hasher = hasher;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto, CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarios.GetByCorreoAsync(dto.Correo, cancellationToken);

            // Misma respuesta para "no existe", "sin credencial", "inactivo" y "contraseña incorrecta":
            // no se revela cuál de ellas falló.
            if (usuario is null || !usuario.Activo || string.IsNullOrEmpty(usuario.PasswordHash))
                return null;

            var verificacion = _hasher.VerifyHashedPassword(usuario, usuario.PasswordHash, dto.Password);
            if (verificacion == PasswordVerificationResult.Failed)
                return null;

            if (verificacion == PasswordVerificationResult.SuccessRehashNeeded)
                usuario.PasswordHash = _hasher.HashPassword(usuario, dto.Password);

            usuario.UltimoAcceso = DateTime.Now;
            ReattachRoot(usuario);
            _usuarios.Update(usuario);
            await _context.SaveChangesAsync(cancellationToken);

            var actualizado = (await _usuarios.GetByIdAsync(usuario.Id, cancellationToken))!;
            var (token, expira) = EmitirToken(actualizado);

            return new LoginResponseDto
            {
                Token = token,
                ExpiraEn = expira,
                Usuario = ToResponse(actualizado),
            };
        }

        public async Task<UsuarioResponseDto?> GetPerfilAsync(int idUsuario, CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarios.GetByIdAsync(idUsuario, cancellationToken);
            return usuario is null || !usuario.Activo ? null : ToResponse(usuario);
        }

        public async Task<UsuarioResponseDto> RegistrarAsync(RegistroUsuarioDto dto, CancellationToken cancellationToken = default)
        {
            var correo = dto.Correo.Trim().ToLowerInvariant();
            if (await _usuarios.GetByCorreoAsync(correo, cancellationToken) is not null)
                throw new InvalidOperationException($"Ya existe un usuario con el correo {correo}.");

            var usuario = new Usuario
            {
                Nombre = dto.Nombre.Trim(),
                Correo = correo,
                IdDepartamento = dto.IdDepartamento,
                Cargo = dto.Cargo.Trim(),
                Activo = dto.Activo,
                OrigenAd = false,
                Roles = dto.RolIds.Distinct().Select(rolId => new UsuarioRol { IdRol = rolId }).ToList(),
            };
            usuario.PasswordHash = _hasher.HashPassword(usuario, dto.Password);

            await _usuarios.AddAsync(usuario, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken); // un commit: usuario + roles

            var creado = await _usuarios.GetByIdAsync(usuario.Id, cancellationToken);
            return ToResponse(creado!);
        }

        public async Task<bool> CambiarPasswordAsync(int idUsuario, CambioPasswordDto dto, CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarios.GetByIdAsync(idUsuario, cancellationToken);
            if (usuario is null || string.IsNullOrEmpty(usuario.PasswordHash))
                return false;

            if (_hasher.VerifyHashedPassword(usuario, usuario.PasswordHash, dto.PasswordActual) == PasswordVerificationResult.Failed)
                return false;

            usuario.PasswordHash = _hasher.HashPassword(usuario, dto.PasswordNueva);
            ReattachRoot(usuario);
            _usuarios.Update(usuario);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> RestablecerPasswordAsync(int idUsuario, RestablecerPasswordDto dto, CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarios.GetByIdAsync(idUsuario, cancellationToken);
            if (usuario is null)
                return false;

            usuario.PasswordHash = _hasher.HashPassword(usuario, dto.PasswordNueva);
            ReattachRoot(usuario);
            _usuarios.Update(usuario);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        // ---- JWT ----

        private (string token, DateTime expira) EmitirToken(Usuario usuario)
        {
            var seccion = _configuration.GetSection("Jwt");
            var clave = seccion["Key"] ?? throw new InvalidOperationException("Falta Jwt:Key en la configuración.");
            var minutos = int.TryParse(seccion["ExpiraMinutos"], out var m) ? m : 480;
            var expira = DateTime.UtcNow.AddMinutes(minutos);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new(ClaimTypes.Name, usuario.Nombre),
                new(ClaimTypes.Email, usuario.Correo),
                new(ClaimsPrincipalExtensions.ClaimDepartamento, usuario.IdDepartamento.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            };
            claims.AddRange(usuario.Roles
                .Where(r => r.Rol is not null)
                .Select(r => new Claim(ClaimTypes.Role, r.Rol!.Nombre)));

            var credenciales = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave)),
                SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: seccion["Issuer"],
                audience: seccion["Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expira,
                signingCredentials: credenciales);

            return (new JwtSecurityTokenHandler().WriteToken(jwt), expira);
        }

        // La entidad llega desconectada con navegaciones cargadas; se limpian para que Update()
        // solo marque la fila raíz (misma regla que UsuarioService).
        private static void ReattachRoot(Usuario usuario)
        {
            usuario.Roles = new List<UsuarioRol>();
            usuario.Departamento = null;
        }

        private static UsuarioResponseDto ToResponse(Usuario usuario) => new()
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Correo = usuario.Correo,
            IdDepartamento = usuario.IdDepartamento,
            DepartamentoNombre = usuario.Departamento?.Nombre ?? string.Empty,
            Cargo = usuario.Cargo,
            Activo = usuario.Activo,
            OrigenAd = usuario.OrigenAd,
            TieneCredencial = !string.IsNullOrEmpty(usuario.PasswordHash),
            UltimoAcceso = usuario.UltimoAcceso,
            Roles = usuario.Roles
                .Select(ur => new UsuarioRolResponseDto { IdRol = ur.IdRol, Nombre = ur.Rol?.Nombre ?? string.Empty })
                .ToList(),
        };
    }
}
