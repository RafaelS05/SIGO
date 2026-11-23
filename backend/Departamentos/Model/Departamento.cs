namespace SIGO.Api.Departamentos.Model
{
    public class Departamento
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Sigla { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;
    }
}
