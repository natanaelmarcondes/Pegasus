namespace Pegasus
{
    internal static class ConfiguracaoRuntime
    {
        public static string DiretorioConfigIni { get; set; } = string.Empty;
        public static string TipoSistema { get; set; } = "IND";

        public static void AplicarArgumentos(string[] args)
        {
            if (args is null || args.Length == 0)
            {
                return;
            }

            foreach (var argumento in args)
            {
                if (!argumento.StartsWith("/INI:", StringComparison.OrdinalIgnoreCase))
                {
                    if (argumento.StartsWith("/TIPSIS:", StringComparison.OrdinalIgnoreCase))
                    {
                        var tipoSis = argumento[8..].Trim().Trim('"');
                        if (!string.IsNullOrWhiteSpace(tipoSis))
                        {
                            TipoSistema = tipoSis;
                        }
                    }

                    continue;
                }

                var diretorio = argumento[5..].Trim().Trim('"');
                DiretorioConfigIni = diretorio;
                break;
            }
        }
    }
}
