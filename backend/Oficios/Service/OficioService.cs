using System.Security.Cryptography;
using SIGO.Api.Departamentos.Repository;
using SIGO.Api.Estados.Repository;
using SIGO.Api.Oficios.DTO;
using SIGO.Api.Oficios.Model;
using SIGO.Api.Oficios.Repository;
using SIGO.Api.Prioridades.Repository;
using SIGO.Api.Usuarios.Repository;

namespace SIGO.Api.Oficios.Service
{
    public class OficioService : IOficioService
    {
        private static readonly string[] ExtensionesPermitidas = { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx" };

        private readonly IOficioRepository _repository;
        private readonly IEstadoRepository _estadoRepository;
        private readonly IPrioridadRepository _prioridadRepository;
        private readonly IDepartamentoRepository _departamentoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly AppDbContext _context; // the EF Core unit of work — commit lives here, not in the repos.
        private readonly IWebHostEnvironment _environment;

        public OficioService(
            IOficioRepository repository,
            IEstadoRepository estadoRepository,
            IPrioridadRepository prioridadRepository,
            IDepartamentoRepository departamentoRepository,
            IUsuarioRepository usuarioRepository,
            AppDbContext context,
            IWebHostEnvironment environment)
        {
            _repository = repository;
            _estadoRepository = estadoRepository;
            _prioridadRepository = prioridadRepository;
            _departamentoRepository = departamentoRepository;
            _usuarioRepository = usuarioRepository;
            _context = context;
            _environment = environment;
        }

        public async Task<List<OficioResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetAllAsync(cancellationToken);
            return items.Select(ToResponse).ToList();
        }

        public async Task<List<OficioResponseDto>> GetFilteredAsync(
            int? estadoId,
            int? prioridadId,
            int? departamentoId,
            int? responsableId,
            string? busqueda,
            DateTime? desde,
            DateTime? hasta,
            bool soloAtrasados,
            CancellationToken cancellationToken = default)
        {
            var items = await _repository.GetFilteredAsync(
                estadoId, prioridadId, departamentoId, responsableId,
                busqueda, desde, hasta, soloAtrasados, cancellationToken);
            return items.Select(ToResponse).ToList();
        }

        public async Task<OficioResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var oficio = await _repository.GetByIdAsync(id, cancellationToken);
            return oficio is null ? null : ToResponse(oficio);
        }

        public async Task<OficioResponseDto> CreateAsync(OficioCreateDto dto, CancellationToken cancellationToken = default)
        {
            var prioridad = await _prioridadRepository.GetByIdAsync(dto.IdPrioridad, cancellationToken)
                ?? throw new InvalidOperationException($"Prioridad {dto.IdPrioridad} no existe.");
            var destino = await _departamentoRepository.GetByIdAsync(dto.IdDepartamentoDestino, cancellationToken)
                ?? throw new InvalidOperationException($"Departamento {dto.IdDepartamentoDestino} no existe.");
            var estadoInicial = await GetEstadoPorNombreAsync("Recibido", cancellationToken);

            // Numeración: prefijo + sigla del área destino + año + consecutivo anual.
            var anio = dto.FechaIngreso.Year;
            var consecutivo = await _repository.GetMaxConsecutivoAsync(anio, cancellationToken) + 1;

            var oficio = new Oficio
            {
                Numero = $"SIGO-{destino.Sigla}-{anio}-{consecutivo:D4}",
                Anio = anio,
                Consecutivo = consecutivo,
                Direccion = dto.Direccion,
                IdTipoOficio = dto.IdTipoOficio,
                IdDepartamentoOrigen = dto.IdDepartamentoOrigen,
                IdDepartamentoDestino = dto.IdDepartamentoDestino,
                Remitente = dto.Remitente,
                Destinatario = dto.Destinatario,
                Asunto = dto.Asunto,
                Descripcion = dto.Descripcion,
                IdPrioridad = dto.IdPrioridad,
                IdEstadoActual = estadoInicial.Id,
                IdCanal = dto.IdCanal,
                FechaIngreso = dto.FechaIngreso,
                FechaVencimientoSla = dto.FechaIngreso.AddHours(prioridad.SlaHoras),
                CreadoPor = dto.CreadoPor,
                CreadoEn = DateTime.Now,
                Bitacora =
                {
                    new OficioBitacora
                    {
                        IdEstadoDestino = estadoInicial.Id,
                        Comentario = "Oficio registrado.",
                        IdUsuario = dto.CreadoPor,
                        FechaEvento = DateTime.Now,
                    },
                },
            };

            await _repository.AddAsync(oficio, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken); // one commit: oficio + bitácora inicial

            var created = await _repository.GetByIdAsync(oficio.Id, cancellationToken);
            return ToResponse(created!);
        }

        public async Task<OficioResponseDto?> UpdateAsync(int id, OficioUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var oficio = await _repository.GetByIdAsync(id, cancellationToken);
            if (oficio is null)
                return null;

            oficio.IdTipoOficio = dto.IdTipoOficio;
            oficio.IdDepartamentoOrigen = dto.IdDepartamentoOrigen;
            oficio.IdDepartamentoDestino = dto.IdDepartamentoDestino;
            oficio.Remitente = dto.Remitente;
            oficio.Destinatario = dto.Destinatario;
            oficio.Asunto = dto.Asunto;
            oficio.Descripcion = dto.Descripcion;
            oficio.IdPrioridad = dto.IdPrioridad;
            oficio.IdCanal = dto.IdCanal;

            DetachGraph(oficio);
            _repository.Update(oficio);
            await _context.SaveChangesAsync(cancellationToken);

            var updated = await _repository.GetByIdAsync(id, cancellationToken);
            return ToResponse(updated!);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var oficio = await _repository.GetByIdAsync(id, cancellationToken);
            if (oficio is null)
                return false;

            DetachGraph(oficio, clearOwned: false); // owned rows cascade with the root
            _repository.Remove(oficio);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<OficioResponseDto?> CambiarEstadoAsync(int id, OficioCambioEstadoDto dto, CancellationToken cancellationToken = default)
        {
            var oficio = await _repository.GetByIdAsync(id, cancellationToken);
            if (oficio is null)
                return null;

            var estadoDestino = await _estadoRepository.GetByIdAsync(dto.IdEstadoDestino, cancellationToken)
                ?? throw new InvalidOperationException($"Estado {dto.IdEstadoDestino} no existe.");

            var estadoOrigen = oficio.IdEstadoActual;
            oficio.IdEstadoActual = estadoDestino.Id;
            oficio.FechaCierre = estadoDestino.CerradoFlag ? DateTime.Now : null;

            await _repository.AddBitacoraAsync(new OficioBitacora
            {
                IdOficio = id,
                IdEstadoOrigen = estadoOrigen,
                IdEstadoDestino = estadoDestino.Id,
                Comentario = dto.Comentario,
                IdUsuario = dto.IdUsuario,
                FechaEvento = DateTime.Now,
            }, cancellationToken);

            DetachGraph(oficio);
            _repository.Update(oficio);
            await _context.SaveChangesAsync(cancellationToken); // one commit: estado + bitácora

            var updated = await _repository.GetByIdAsync(id, cancellationToken);
            return ToResponse(updated!);
        }

        public async Task<OficioResponseDto?> AsignarAsync(int id, OficioAsignacionDto dto, CancellationToken cancellationToken = default)
        {
            var oficio = await _repository.GetByIdAsync(id, cancellationToken);
            if (oficio is null)
                return null;

            var usuario = await _usuarioRepository.GetByIdAsync(dto.IdUsuario, cancellationToken)
                ?? throw new InvalidOperationException($"Usuario {dto.IdUsuario} no existe.");

            // Reasignación con historial: el responsable anterior queda inactivo, nunca se borra.
            foreach (var anterior in oficio.Responsables.Where(r => r.Activo))
            {
                anterior.Activo = false;
                anterior.Usuario = null;
                anterior.Asignador = null;
                _repository.UpdateResponsable(anterior);
            }

            await _repository.AddResponsableAsync(new OficioResponsable
            {
                IdOficio = id,
                IdUsuario = dto.IdUsuario,
                Activo = true,
                AsignadoEn = DateTime.Now,
                AsignadoPor = dto.AsignadoPor,
            }, cancellationToken);

            // Al asignar, un oficio "Recibido" pasa a "Asignado".
            var estadoOrigen = oficio.IdEstadoActual;
            var cambioEstado = false;
            if (string.Equals(oficio.EstadoActual?.Nombre, "Recibido", StringComparison.OrdinalIgnoreCase))
            {
                var asignado = await GetEstadoPorNombreAsync("Asignado", cancellationToken);
                oficio.IdEstadoActual = asignado.Id;
                cambioEstado = true;
            }

            await _repository.AddBitacoraAsync(new OficioBitacora
            {
                IdOficio = id,
                IdEstadoOrigen = cambioEstado ? estadoOrigen : null,
                IdEstadoDestino = cambioEstado ? oficio.IdEstadoActual : null,
                Comentario = string.IsNullOrWhiteSpace(dto.Comentario)
                    ? $"Asignado a {usuario.Nombre}."
                    : $"Asignado a {usuario.Nombre}. {dto.Comentario}",
                IdUsuario = dto.AsignadoPor,
                FechaEvento = DateTime.Now,
            }, cancellationToken);

            DetachGraph(oficio);
            _repository.Update(oficio);
            await _context.SaveChangesAsync(cancellationToken); // one commit: responsables + estado + bitácora

            var updated = await _repository.GetByIdAsync(id, cancellationToken);
            return ToResponse(updated!);
        }

        public async Task<OficioResponseDto?> ComentarAsync(int id, OficioComentarioDto dto, CancellationToken cancellationToken = default)
        {
            var oficio = await _repository.GetByIdAsync(id, cancellationToken);
            if (oficio is null)
                return null;

            await _repository.AddBitacoraAsync(new OficioBitacora
            {
                IdOficio = id,
                Comentario = dto.Comentario,
                IdUsuario = dto.IdUsuario,
                FechaEvento = DateTime.Now,
            }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            var updated = await _repository.GetByIdAsync(id, cancellationToken);
            return ToResponse(updated!);
        }

        public async Task<OficioResponseDto?> AgregarAdjuntoAsync(int id, IFormFile archivo, int subidoPor, CancellationToken cancellationToken = default)
        {
            var oficio = await _repository.GetByIdAsync(id, cancellationToken);
            if (oficio is null)
                return null;

            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            if (!ExtensionesPermitidas.Contains(extension))
                throw new InvalidOperationException($"Extensión no permitida: {extension}. Use PDF/JPG/PNG/DOC.");

            var carpeta = Path.Combine(_environment.ContentRootPath, "App_Data", "adjuntos", id.ToString());
            Directory.CreateDirectory(carpeta);
            var nombreFisico = $"{Guid.NewGuid():N}{extension}";
            var rutaFisica = Path.Combine(carpeta, nombreFisico);

            string hash;
            await using (var destino = File.Create(rutaFisica))
            {
                await archivo.CopyToAsync(destino, cancellationToken);
            }
            await using (var lectura = File.OpenRead(rutaFisica))
            {
                hash = Convert.ToHexString(await SHA256.HashDataAsync(lectura, cancellationToken));
            }

            var version = oficio.Adjuntos.Count(a => a.NombreArchivo == archivo.FileName) + 1;

            await _repository.AddAdjuntoAsync(new OficioAdjunto
            {
                IdOficio = id,
                NombreArchivo = archivo.FileName,
                Ruta = Path.Combine("App_Data", "adjuntos", id.ToString(), nombreFisico),
                Hash = hash,
                PesoBytes = archivo.Length,
                Version = version,
                SubidoPor = subidoPor,
                SubidoEn = DateTime.Now,
            }, cancellationToken);

            await _repository.AddBitacoraAsync(new OficioBitacora
            {
                IdOficio = id,
                Comentario = $"Adjunto agregado: {archivo.FileName} (v{version}).",
                IdUsuario = subidoPor,
                FechaEvento = DateTime.Now,
            }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken); // one commit: adjunto + bitácora

            var updated = await _repository.GetByIdAsync(id, cancellationToken);
            return ToResponse(updated!);
        }

        public async Task<OficioAdjuntoDescargaDto?> GetAdjuntoDescargaAsync(int id, int idAdjunto, CancellationToken cancellationToken = default)
        {
            var adjunto = await _repository.GetAdjuntoByIdAsync(id, idAdjunto, cancellationToken);
            if (adjunto is null)
                return null;

            return new OficioAdjuntoDescargaDto
            {
                RutaAbsoluta = Path.Combine(_environment.ContentRootPath, adjunto.Ruta),
                NombreArchivo = adjunto.NombreArchivo,
                ContentType = Path.GetExtension(adjunto.NombreArchivo).ToLowerInvariant() switch
                {
                    ".pdf" => "application/pdf",
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".doc" => "application/msword",
                    ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    _ => "application/octet-stream",
                },
            };
        }

        private async Task<Estados.Model.Estado> GetEstadoPorNombreAsync(string nombre, CancellationToken cancellationToken)
        {
            var estados = await _estadoRepository.GetAllAsync(cancellationToken);
            return estados.FirstOrDefault(e => string.Equals(e.Nombre, nombre, StringComparison.OrdinalIgnoreCase))
                ?? estados.OrderBy(e => e.Id).First(e => !e.CerradoFlag);
        }

        // The aggregate comes back detached with navs populated (AsNoTracking + Include).
        // Before re-attaching the root via Update()/Remove(), strip the graph so EF only
        // touches the root row — owned rows are staged explicitly through the repository.
        private static void DetachGraph(Oficio oficio, bool clearOwned = true)
        {
            oficio.TipoOficio = null;
            oficio.DepartamentoOrigen = null;
            oficio.DepartamentoDestino = null;
            oficio.Prioridad = null;
            oficio.EstadoActual = null;
            oficio.Canal = null;
            oficio.Creador = null;
            if (clearOwned)
            {
                oficio.Responsables = new List<OficioResponsable>();
                oficio.Bitacora = new List<OficioBitacora>();
                oficio.Adjuntos = new List<OficioAdjunto>();
            }
        }

        private static OficioResponseDto ToResponse(Oficio oficio)
        {
            var responsableActivo = oficio.Responsables.FirstOrDefault(r => r.Activo);
            return new OficioResponseDto
            {
                Id = oficio.Id,
                Numero = oficio.Numero,
                Anio = oficio.Anio,
                Direccion = oficio.Direccion,
                IdTipoOficio = oficio.IdTipoOficio,
                TipoOficioNombre = oficio.TipoOficio?.Nombre ?? string.Empty,
                IdDepartamentoOrigen = oficio.IdDepartamentoOrigen,
                DepartamentoOrigenNombre = oficio.DepartamentoOrigen?.Nombre ?? string.Empty,
                IdDepartamentoDestino = oficio.IdDepartamentoDestino,
                DepartamentoDestinoNombre = oficio.DepartamentoDestino?.Nombre ?? string.Empty,
                Remitente = oficio.Remitente,
                Destinatario = oficio.Destinatario,
                Asunto = oficio.Asunto,
                Descripcion = oficio.Descripcion,
                IdPrioridad = oficio.IdPrioridad,
                PrioridadNombre = oficio.Prioridad?.Nombre ?? string.Empty,
                IdEstadoActual = oficio.IdEstadoActual,
                EstadoNombre = oficio.EstadoActual?.Nombre ?? string.Empty,
                EstadoCerrado = oficio.EstadoActual?.CerradoFlag ?? false,
                IdCanal = oficio.IdCanal,
                CanalNombre = oficio.Canal?.Nombre ?? string.Empty,
                FechaIngreso = oficio.FechaIngreso,
                FechaVencimientoSla = oficio.FechaVencimientoSla,
                FechaCierre = oficio.FechaCierre,
                Atrasado = oficio.FechaCierre is null
                    && !(oficio.EstadoActual?.CerradoFlag ?? false)
                    && oficio.FechaVencimientoSla < DateTime.Now,
                CreadoPor = oficio.CreadoPor,
                CreadoPorNombre = oficio.Creador?.Nombre ?? string.Empty,
                CreadoEn = oficio.CreadoEn,
                ResponsableActual = responsableActivo?.Usuario?.Nombre ?? string.Empty,
                IdResponsableActual = responsableActivo?.IdUsuario,
                Responsables = oficio.Responsables
                    .OrderByDescending(r => r.AsignadoEn)
                    .Select(r => new OficioResponsableResponseDto
                    {
                        Id = r.Id,
                        IdUsuario = r.IdUsuario,
                        UsuarioNombre = r.Usuario?.Nombre ?? string.Empty,
                        Activo = r.Activo,
                        AsignadoEn = r.AsignadoEn,
                        AsignadoPorNombre = r.Asignador?.Nombre ?? string.Empty,
                    })
                    .ToList(),
                Bitacora = oficio.Bitacora
                    .OrderByDescending(b => b.FechaEvento)
                    .Select(b => new OficioBitacoraResponseDto
                    {
                        Id = b.Id,
                        EstadoOrigenNombre = b.EstadoOrigen?.Nombre ?? string.Empty,
                        EstadoDestinoNombre = b.EstadoDestino?.Nombre ?? string.Empty,
                        Comentario = b.Comentario,
                        UsuarioNombre = b.Usuario?.Nombre ?? string.Empty,
                        FechaEvento = b.FechaEvento,
                    })
                    .ToList(),
                Adjuntos = oficio.Adjuntos
                    .OrderByDescending(a => a.SubidoEn)
                    .Select(a => new OficioAdjuntoResponseDto
                    {
                        Id = a.Id,
                        NombreArchivo = a.NombreArchivo,
                        PesoBytes = a.PesoBytes,
                        Version = a.Version,
                        SubidoPorNombre = a.Subidor?.Nombre ?? string.Empty,
                        SubidoEn = a.SubidoEn,
                    })
                    .ToList(),
            };
        }
    }
}
