namespace Pegasus
{
    internal sealed record ConfiguracaoBanco(
        string TipoServidor,
        string Driver,
        string Servidor,
        uint Porta,
        string Database,
        string Usuario,
        string Senha,
        uint Timeout,
        string StringConexao);

    internal sealed record ConfiguracaoConexao(int Codigo, string Nome, ConfiguracaoBanco BancoA, ConfiguracaoBanco BancoB)
    {
        public override string ToString() => Nome;
    }
}
