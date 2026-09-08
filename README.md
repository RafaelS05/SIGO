# SIGO — Sistema de Gestión de Oficios

Control integral de oficios entrantes y salientes por departamento, con trazabilidad,
SLA, adjuntos, reportes y permisos por rol/área. Proyecto de Trabajo Comunal Universitario.

## Stack

| Capa | Tecnología |
|---|---|
| Backend | ASP.NET Core Web API (.NET 10) + EF Core 10 |
| Base de datos | SQL Server (LocalDB en desarrollo) |
| Frontend | React 18 + TypeScript + Vite + React Router |

## Arquitectura del backend (vertical slice / screaming architecture)

El código se organiza **por concepto de dominio, no por capa técnica**. Cada feature es
una carpeta de primer nivel que contiene su stack completo:

```
backend/
├── Oficios/                  # slice principal
│   ├── Model/                # Oficio + OficioResponsable + OficioBitacora + OficioAdjunto
│   ├── DTO/                  # contratos de entrada/salida (con validación por atributos)
│   ├── Repository/           # IOficioRepository / OficioRepository (AsNoTracking, sin SaveChanges)
│   ├── Service/              # IOficioService / OficioService (dueño del commit — Unit of Work)
│   └── Controllers/          # OficiosController
├── Departamentos/ Usuarios/ Roles/ Prioridades/ Estados/ TiposOficios/ Canales/
│   └── (misma estructura de 5 subcarpetas)
├── AppDbContext.cs           # unidad de trabajo EF Core compartida
├── DbSeeder.cs               # catálogos y usuarios demo
└── Program.cs                # DI (scoped), CORS, EnsureCreated + seed
```

Reglas aplicadas:

- Repositorios devuelven **entidades**, nunca DTOs ni `IQueryable`; siempre `AsNoTracking()`.
- Los repositorios **no** llaman `SaveChangesAsync`: el servicio es dueño de la transacción,
  de modo que una operación que toca varios repositorios (p. ej. asignar = responsables +
  estado + bitácora) se confirma con un único commit.
- Todos los campos de DTOs de entrada llevan atributos de validación (`[Required]`,
  `[MaxLength]`, `[Range]`…) — `[ApiController]` devuelve 400 automáticamente.

## Funcionalidad

- **Registro de oficios** entrantes/salientes con numeración automática
  (`SIGO-{SIGLA}-{AÑO}-{CONSECUTIVO}`) y cálculo de vencimiento SLA según prioridad.
- **Asignación y seguimiento**: estados (Recibido → Asignado → En proceso → … → Cerrado),
  reasignaciones con historial (el responsable anterior queda inactivo, nunca se borra).
- **Bitácora**: todo cambio de estado, asignación, comentario y adjunto queda registrado
  con usuario, fecha y motivo.
- **Gestión documental**: adjuntos PDF/JPG/PNG/DOC con hash SHA-256, versionado por nombre
  y descarga vía API (los binarios viven en `backend/App_Data/adjuntos/`).
- **Bandejas**: Mi bandeja, Departamento, Atrasados (SLA vencido) y Todos (admin/auditor),
  con búsqueda por número/asunto/remitente, filtros por estado/prioridad y rangos rápidos.
- **Reportes**: volumen por mes y por departamento, incumplimiento SLA por prioridad,
  carga de trabajo por responsable, lead time promedio.
- **Catálogos** (rol Administrador): departamentos, prioridades/SLA, estados, tipos de
  oficio, canales y usuarios con roles.

## Cómo ejecutar

Requisitos: .NET 10 SDK, Node 20+, SQL Server LocalDB (incluido con Visual Studio).

```bash
# Terminal 1 — API (crea y siembra la BD automáticamente)
cd backend
dotnet run          # http://localhost:5099

# Terminal 2 — Frontend
cd frontend
npm install
npm run dev         # http://localhost:5173 (proxy /api → 5099)
```

## Autenticación y RBAC

- **Login** público en `/login` (correo + contraseña). El API emite un JWT (`POST api/Auth/login`)
  con los roles como claims; el frontend lo guarda en `localStorage` y lo envía en `Authorization: Bearer`.
- **Registro de usuarios** en `/admin/registro-usuario`: URL restringida al rol Administrador
  (`POST api/Auth/registro`). Ahí se asignan datos, credenciales y roles.
- Contraseñas con `PasswordHasher` (PBKDF2). El usuario cambia la suya en `/perfil`;
  el Administrador puede restablecerla desde Configuración → Usuarios.
- **Matriz RBAC** en `backend/Auth/Permisos.cs` (policies) y replicada en
  `frontend/src/auth/permisos.ts` (navegación y acciones visibles). Todo endpoint exige token
  salvo el login; el autor de cada acción sobre oficios se toma del token, nunca del cuerpo.

| Permiso | Administrador | Mesa de entrada | Jefatura | Analista | Auditor |
|---|:-:|:-:|:-:|:-:|:-:|
| Administrar usuarios / catálogos | ✔ | | | | |
| Ver todos los oficios (sin filtro de depto.) | ✔ | | | | ✔ |
| Crear / editar oficios | ✔ | ✔ | ✔ | | |
| Asignar responsables | ✔ | | ✔ | | |
| Tramitar (estado, comentarios, adjuntos) | ✔ | ✔ | ✔ | ✔ | |
| Eliminar oficios | ✔ | | | | |
| Reportes | ✔ | | ✔ | | ✔ |

Quien no tiene "Ver todos" queda acotado a los oficios de su departamento o donde figura como responsable.

Usuarios demo (contraseña inicial `Sigo2025!`, configurable en `Auth:PasswordInicial`):

| Rol | Correo |
|---|---|
| Administrador | rafaelsolanosilv@gmail.com |
| Mesa de entrada | maria.jimenez@demo.local |
| Jefatura | carlos.rodriguez@demo.local |
| Analista | ana.vargas@demo.local |
| Auditor | luis.mora@demo.local |

Cambiar `Jwt:Key` en `appsettings.json` antes de desplegar (mínimo 32 caracteres).

## Pendientes para producción (según anteproyecto)

- Autenticación corporativa Azure AD (OpenID Connect / MSAL) como proveedor adicional al login local.
- Migraciones EF (`dotnet ef migrations add Inicial`) en lugar de `EnsureCreated`.
- Notificaciones por correo/Teams (48/24/8 h antes del vencimiento) — SMTP o Power Automate.
- Reportería Power BI sobre el esquema estrella propuesto.
- Storage de adjuntos en file share/Azure Blob con URLs firmadas.
