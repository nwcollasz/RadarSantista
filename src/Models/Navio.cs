namespace RadarSantista.src.Models
{
    public class Navio
    {
        public string Nome { get; set; } = string.Empty;
        public string Imo { get; set; } = string.Empty;
        public string Terminal { get; set; } = string.Empty;
        public string Carga { get; set; } = string.Empty;
        public string Evento { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? DataPrevisao { get; set; }
        public string Descarga { get; set; } = "0";
        public string Embarque { get; set; } = "0";
        public DateTime DataRegistro { get; set; } = DateTime.Now;
    }
}