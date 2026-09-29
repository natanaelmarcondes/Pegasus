namespace Pegasus
{
    /// <summary>
    /// Classe auxiliar para mapeamento entre siglas e descrições de módulos.
    /// Reproduz as funcionalidades das funções VB6: DLLKEY_DescModulo e DLLKEY_SiglaModulo.
    /// </summary>
    public static class ModuloHelper
    {
        private static readonly Dictionary<string, string> SiglaParaDescricao = new(StringComparer.OrdinalIgnoreCase)
        {
            ["AGE"] = "Agenda",
            ["AJU"] = "Ajuda",
            ["BAR"] = "Código de Barras",
            ["CAD"] = "Cadastros",
            ["CAX"] = "Caixa",
            ["CCR"] = "Contas Correntes",
            ["CFE"] = "Cupom Fiscal Eletrônico",
            ["CFG"] = "Configurações",
            ["CHQ"] = "Cheques",
            ["CMD"] = "Controle Materiais Diversos",
            ["CMM"] = "Controle Modificações",
            ["CPG"] = "Contas a Pagar",
            ["CPR"] = "Compras",
            ["CPRC"] = "Compras Consumo",
            ["CRM"] = "Controle de Recebimento de Materiais",
            ["CRP"] = "Contas a Receber",
            ["CST"] = "Custos",
            ["CTB"] = "Contabilidade",
            ["CTE"] = "Conhecimento de Transporte",
            ["DEP"] = "Deposito Fechado",
            ["EBI"] = "Exportação de dados para BI",
            ["ECH"] = "Emissão de Cheques",
            ["ECO"] = "E-Commerce",
            ["ECP"] = "Entrada de Compras",
            ["ESF"] = "Escrita Fiscal",
            ["EST"] = "Estoques",
            ["ETT"] = "Estatísticas",
            ["FAC"] = "Factoring",
            ["FAT"] = "Faturamento",
            ["FCX"] = "Fluxo de Caixa",
            ["FOL"] = "Folha de Pagamento",
            ["GAR"] = "Controle Devolução e Garantia",
            ["GDF"] = "Gestão de Documentos Fiscais",
            ["GER"] = "Módulo Gerencial",
            ["GLASS"] = "KeyGlass",
            ["INT"] = "Integrações",
            ["INV"] = "Inventário",
            ["ODF"] = "Ordem de Fabricação",
            ["ORC"] = "Orçamentos",
            ["PAI"] = "Painel Gráfico",
            ["PDW"] = "Pedidos WEB",
            ["SAC"] = "SAC",
            ["SER"] = "Serasa",
            ["TBP"] = "Tabela de Preços",
            ["TER"] = "Terceirização",
            ["TLP"] = "Teleprocessamento",
            ["TMK"] = "CRM",
            ["TRE"] = "Treinamentos",
            ["UTI"] = "Utilitários",
            ["WTR"] = "Integração Webtray",
            ["PAT"] = "Patrimônio Web",
            ["ALL"] = "Geral (ALL)",
        };

        private static readonly Dictionary<string, string> DescricaoParaSigla = SiglaParaDescricao
            .ToDictionary(par => par.Value, par => par.Key, StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Converte uma sigla para sua descrição amigável.
        /// Se a sigla não for encontrada, retorna a sigla normalizada (maiúsculas).
        /// </summary>
        /// <param name="sigla">A sigla do módulo (ex: "CAD", "FAT", "ALL").</param>
        /// <returns>A descrição do módulo ou a sigla normalizada se não encontrada.</returns>
        public static string DescModulo(string? sigla)
        {
            if (string.IsNullOrWhiteSpace(sigla))
            {
                return string.Empty;
            }

            var siglaTrimmed = sigla.Trim();
            if (SiglaParaDescricao.TryGetValue(siglaTrimmed, out var descricao))
            {
                return descricao;
            }

            return siglaTrimmed.ToUpperInvariant();
        }

        /// <summary>
        /// Converte uma descrição para sua sigla correspondente.
        /// Se a descrição não for encontrada, retorna a descrição normalizada (maiúsculas).
        /// </summary>
        /// <param name="descricao">A descrição do módulo (ex: "Cadastros", "Faturamento").</param>
        /// <returns>A sigla do módulo ou a descrição normalizada se não encontrada.</returns>
        public static string SiglaModulo(string? descricao)
        {
            if (string.IsNullOrWhiteSpace(descricao))
            {
                return string.Empty;
            }

            var descricaoTrimmed = descricao.Trim();
            if (DescricaoParaSigla.TryGetValue(descricaoTrimmed, out var sigla))
            {
                return sigla;
            }

            return descricaoTrimmed.ToUpperInvariant();
        }

        /// <summary>
        /// Separa uma string de módulos em siglas individuais, elimina duplicatas e retorna uma lista normalizada.
        /// A string de entrada pode conter siglas separadas por espaço, ex: "ALL CAD FAT".
        /// </summary>
        /// <param name="modulosString">A string contendo as siglas separadas por espaço.</param>
        /// <returns>Lista de siglas únicas, normalizada (maiúsculas).</returns>
        public static List<string> SepararModulos(string? modulosString)
        {
            if (string.IsNullOrWhiteSpace(modulosString))
            {
                return new List<string>();
            }

            return modulosString
                .Split([' '], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(sigla => sigla.Trim().ToUpperInvariant())
                .Where(sigla => !string.IsNullOrWhiteSpace(sigla))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }

    /// <summary>
    /// Modelo para representar um módulo no grid de seleção.
    /// </summary>
    public class ModuloItem
    {
        public string Sigla { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;

        public override string ToString() => Descricao;
    }
}
