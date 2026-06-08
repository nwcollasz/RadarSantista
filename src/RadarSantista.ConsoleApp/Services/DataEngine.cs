using RadarSantista.ConsoleApp.Models;

namespace RadarSantista.ConsoleApp.Services
{
    public static class DataEngine
    {
        public static List<Navio> Consolidar(List<Navio> atracados, List<Navio> programados)
        {
            var dicionario = new Dictionary<string, Navio>();

            foreach (var p in programados)
            {
                dicionario[p.Nome] = p;
            }

            foreach (var a in atracados)
            {
                if (dicionario.ContainsKey(a.Nome))
                {
                    dicionario[a.Nome].Status = "BERÇO (JÁ ATRACOU)";
                    dicionario[a.Nome].Carga = a.Carga;
                    dicionario[a.Nome].Descarga = a.Descarga;
                    dicionario[a.Nome].Embarque = a.Embarque;
                    dicionario[a.Nome].Terminal = a.Terminal; 
                }
                else
                {
                    dicionario[a.Nome] = a;
                }
            }

            return dicionario.Values.OrderBy(n => n.DataPrevisao ?? DateTime.MaxValue).ToList();
        }
    }
}