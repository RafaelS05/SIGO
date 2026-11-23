namespace SIGO.Api.Estados.Model
{
    public class Estado
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public bool CerradoFlag { get; set; }
    }
}
