using System.Text.RegularExpressions;
using System.Net;

namespace RadarSantista.src.Utils
{
    public static class TextHelper
    {
        public static string Limpar(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return string.Empty;
            texto = WebUtility.HtmlDecode(texto);
            return Regex.Replace(texto, @"\s+", " ").Trim().ToUpper();
        }
    }
}