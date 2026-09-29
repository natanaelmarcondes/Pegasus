# Integração de Módulos - C# vs VB6

## Resumo das Alterações

O projeto Pegasus foi ajustado para reproduzir exatamente as funções `DLLKEY_DescModulo` e `DLLKEY_SiglaModulo` do programa VB6 original, sem dependência da DLL.

## Arquivos Modificados

### 1. **ModuloHelper.cs** (NOVO)
   - Classe estática com mapeamento completo Sigla ↔ Descrição
   - **Métodos públicos:**
	 - `DescModulo(string sigla)` - Converte sigla para descrição amigável
	 - `SiglaModulo(string descricao)` - Converte descrição para sigla
	 - `SepararModulos(string modulosString)` - Separa string "ALL CAD FAT" em lista de siglas individuais
   - **Classe ModuloItem:**
	 - Propriedades: `Sigla` (string), `Descricao` (string)
	 - Utilizada para armazenar tanto a sigla quanto a descrição

### 2. **Form1.cs** (MODIFICADO)

#### Remoções:
   - Removido `Dictionary<string, string> DescricoesModulos` (obsoleto)
   - Removido `Dictionary<string, string> SiglasPorDescricaoModulo` (obsoleto)
   - Removido método `DLLKEY_SiglaModulo(string valor)` (substituído por ModuloHelper)
   - Removido método `DLLKEY_DescModulo(string valor)` (substituído por ModuloHelper)

#### Alterações em `MPrc_PreencherGridModulos`:
   - Usa `ModuloHelper.SepararModulos()` para quebrar strings de módulos
   - Cria classe `ModuloItem` para cada sigla única
   - Armazena sigla em `DataGridViewRow.Tag`
   - Exibe apenas `Descricao` amigável no grid
   - Ordena alfabeticamente por descrição

#### Alterações em `MFcn_TabelaAtendeFiltroModulo`:
   - Usa `ModuloHelper.SepararModulos()` para quebrar ktb_Modulo
   - Compara siglas individuais (não substring matching)
   - Suporta corretamente a semântica de ALL

## Mapeamento Sigla → Descrição

```
AGE   → Agenda
AJU   → Ajuda
BAR   → Código de Barras
CAD   → Cadastros
CAX   → Caixa
CCR   → Contas Correntes
CFE   → Cupom Fiscal Eletrônico
CFG   → Configurações
CHQ   → Cheques
CMD   → Controle Materiais Diversos
CMM   → Controle Modificações
CPG   → Contas a Pagar
CPR   → Compras
CPRC  → Compras Consumo
CRM   → Controle de Recebimento de Materiais
CRP   → Contas a Receber
CST   → Custos
CTB   → Contabilidade
CTE   → Conhecimento de Transporte
DEP   → Deposito Fechado
EBI   → Exportação de dados para BI
ECH   → Emissão de Cheques
ECO   → E-Commerce
ECP   → Entrada de Compras
ESF   → Escrita Fiscal
EST   → Estoques
ETT   → Estatísticas
FAC   → Factoring
FAT   → Faturamento
FCX   → Fluxo de Caixa
FOL   → Folha de Pagamento
GAR   → Controle Devolução e Garantia
GDF   → Gestão de Documentos Fiscais
GER   → Módulo Gerencial
GLASS → KeyGlass
INT   → Integrações
INV   → Inventário
ODF   → Ordem de Fabricação
ORC   → Orçamentos
PAI   → Painel Gráfico
PDW   → Pedidos WEB
SAC   → SAC
SER   → Serasa
TBP   → Tabela de Preços
TER   → Terceirização
TLP   → Teleprocessamento
TMK   → CRM
TRE   → Treinamentos
UTI   → Utilitários
WTR   → Integração Webtray
PAT   → Patrimônio Web
ALL   → Geral (ALL)
```

## Comportamento Esperado

### Carregamento de Módulos
1. Lê `KEY_TABELAS.ktb_Modulo` (ex: "ALL CAD" ou "FAT ORC")
2. Separa as siglas por espaço
3. Remove siglas vazias e duplicatas
4. Converte cada sigla para descrição amigável via `ModuloHelper.DescModulo()`
5. Ordena alfabeticamente por descrição
6. Exibe somente descrição no grid (ex: "Cadastros", "Faturamento")
7. Armazena sigla original em `Row.Tag` para filtragem

### Seleção de Tabelas
1. Ao marcar um módulo (ex: "Cadastros"), lê a sigla do `Row.Tag` (ex: "CAD")
2. Para cada tabela, separa `ktb_Modulo` e verifica pertencimento:
   - Se tabela tem `ktb_Modulo = "ALL CAD"` e usuário marcou "CAD", tabela é selecionada
   - Se tabela tem `ktb_Modulo = "ALL CAD"` e usuário marcou "Geral (ALL)", tabela é selecionada
   - Comparação respeitando maiúsculas/minúsculas normalizadas

### Regra ALL
- `ALL` representa especificamente `Geral (ALL)`
- Tabelas com `ALL` na sigla aparecem para ambos os módulos (ALL e o outro)
- Exemplo: `ktb_Modulo = "ALL CAD"` → pertence a `Geral (ALL)` e `Cadastros`

## Testes Recomendados

1. Carregar a aplicação com um banco de dados existente
2. Verificar se a quantidade de módulos no grid corresponde ao VB6
3. Verificar se apenas descrições amigáveis são exibidas (sem siglas)
4. Verificar seleção de tabelas ao marcar/desmarcar módulos
5. Validar comportamento com `ALL` (se aplicável no banco de dados)

## Status da Compilação

✅ Compilação bem-sucedida após todas as alterações
✅ Nenhum erro ou aviso reportado
✅ Projeto pronto para testes em runtime
