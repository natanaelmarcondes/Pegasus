using System;

namespace Pegasus
{
    internal static class ConfiguracaoRuntime
    {
        public static string DiretorioConfigIni { get; private set; } = string.Empty;
        public static string TipoSistema { get; set; } = "IND";
        public static ParametrosLinhaComando Parametros { get; } = new ParametrosLinhaComando();

        public static void AplicarArgumentos(string[] args)
        {
            if (args is null || args.Length == 0)
            {
                return;
            }

            foreach (var argumento in args)
            {
                if (string.IsNullOrWhiteSpace(argumento))
                {
                    continue;
                }

                if (argumento.StartsWith("/INI:", StringComparison.OrdinalIgnoreCase))
                {
                    var diretorio = argumento[5..].Trim().Trim('"');
                    if (!string.IsNullOrWhiteSpace(diretorio))
                    {
                        var expandido = Environment.ExpandEnvironmentVariables(diretorio);
                        Parametros.DiretorioIni = expandido;
                        DiretorioConfigIni = expandido;
                    }

                    continue;
                }

                if (argumento.StartsWith("/INIDEF:", StringComparison.OrdinalIgnoreCase))
                {
                    var diretorio = argumento[8..].Trim().Trim('"');
                    if (!string.IsNullOrWhiteSpace(diretorio))
                    {
                        Parametros.DiretorioIniDefault = Environment.ExpandEnvironmentVariables(diretorio);
                    }

                    continue;
                }

                if (argumento.StartsWith("/CNX:", StringComparison.OrdinalIgnoreCase))
                {
                    var nome = argumento[5..].Trim().Trim('"');
                    if (!string.IsNullOrWhiteSpace(nome))
                    {
                        Parametros.NomeConexao = nome.ToUpperInvariant();
                    }

                    continue;
                }

                if (argumento.StartsWith("/TIPSIS:", StringComparison.OrdinalIgnoreCase))
                {
                    var tipoSis = argumento[8..].Trim().Trim('"');
                    if (!string.IsNullOrWhiteSpace(tipoSis))
                    {
                        TipoSistema = tipoSis;
                    }

                    continue;
                }
            }

            // Se nenhum /INI: foi informado, mas existe /INIDEF:, usa como fallback para DiretorioConfigIni
            if (string.IsNullOrWhiteSpace(DiretorioConfigIni) && !string.IsNullOrWhiteSpace(Parametros.DiretorioIniDefault))
            {
                DiretorioConfigIni = Parametros.DiretorioIniDefault;
            }
        }
    }
}
