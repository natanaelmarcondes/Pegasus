using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Pegasus
{
    internal static class AppInfo
    {
        // Retorna apenas a parte numérica da versão (ex: 1.2.3.4)
        public static string Versao
        {
            get
            {
                var raw = Application.ProductVersion ?? string.Empty;
                if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
                var m = Regex.Match(raw, @"\d+(?:\.\d+)+");
                return m.Success ? m.Value : raw;
            }
        }

        public static void AppendVersao(Form form)
        {
            if (form is null) return;
            var v = Versao;
            if (string.IsNullOrWhiteSpace(v)) return;
            var marker = $"v{v}";
            if (form.Text != null && form.Text.Contains(marker, StringComparison.OrdinalIgnoreCase)) return;
            form.Text = $"{form.Text} - v{v}";
        }
    }
}
