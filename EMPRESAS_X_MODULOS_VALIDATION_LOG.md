# Validação - Módulos com EMPRESAS_X_MODULOS

## Status: ✅ COMPLETO

### Compilação
✅ **Resultado**: Compilação bem-sucedida sem erros ou avisos

### Mudanças Implementadas

#### 1. Novo Método: `PossuiModuloAsync`
**Assinatura:**
```csharp
private async Task<bool> PossuiModuloAsync(DbTarget target, string siglaModulo, string empCodigo = "")
```

**Comportamento:**
- Consulta a tabela `EMPRESAS_X_MODULOS`
- Filtra por `mem_Modulo` (sempre, com parâmetro SQL)
- Filtra por `emp_Codigo` se fornecido e diferente de "00"
- Retorna `true` se existir pelo menos um registro
- Em caso de erro, escreve log e retorna `false`
- Usa `MySqlConnector` com queries parametrizadas (sem concatenação de strings)

**Localização:** Logo após `MPrc_CarregarMetadadosKeyTabelasPorBanco` no Form1.cs

#### 2. Refactor: `MPrc_PreencherGridModulosAsync`
**Assinatura:**
```csharp
private async Task MPrc_PreencherGridModulosAsync(DbTarget target, IReadOnlyCollection<WMTbl_Tabela> tabelas)
```

**Mudanças:**
- Agora é `async` e recebe `DbTarget target` como parâmetro
- Valida cada sigla encontrada em `KEY_TABELAS.ktb_Modulo`
- **Regra ALL**: Sempre adicionada como "Geral (ALL)" sem validação
- **Outras siglas**: Apenas adicionadas se `await PossuiModuloAsync(target, sigla)` retornar `true`
- Mantém a mesma lógica de ordenação alfabética por descrição
- Tag continua armazenando a sigla original para filtragem de tabelas

**Lógica:**
```
Para cada sigla unique encontrada em ktb_Modulo:
  ├─ Se sigla == "ALL"
  │  └─ Adicionar como "Geral (ALL)" (sem validação)
  └─ Senão
	 └─ Validar com PossuiModuloAsync
		└─ Se existe em EMPRESAS_X_MODULOS
		   └─ Adicionar como ModuloItem
```

#### 3. Chamadas Atualizadas

**Em `MPrc_CarregarTabelasBancoAsync` (linha ~1239):**
```csharp
// Antes:
MPrc_PreencherGridModulos(tabelasFiltradas);

// Depois:
await MPrc_PreencherGridModulosAsync(targetA, tabelasFiltradas);
```

**Em `MPrc_CarregarTabelasRestoreAsync` (linha ~1316):**
```csharp
// Antes:
MPrc_PreencherGridModulos(WMTbl_Tabelas);

// Depois:
await MPrc_PreencherGridModulosAsync(targetA, WMTbl_Tabelas);
```

### Efeitos Esperados

#### Antes da Mudança:
- Módulo GLASS aparecia na lista mesmo usando carregamento do VB6
- Todos os módulos em `KEY_TABELAS.ktb_Modulo` eram exibidos automaticamente
- Sem validação por `EMPRESAS_X_MODULOS`

#### Depois da Mudança:
- Apenas módulos liberados em `EMPRESAS_X_MODULOS` aparecem na lista
- GLASS e outros módulos não-liberados **não aparecem**
- ALL sempre aparece como "Geral (ALL)"
- Descrições amigáveis continua funcionando corretamente
- Filtragem de tabelas por sigla **continua funcionando** (não afetada)

### Exemplo de Comportamento

**Banco de dados:**
```
KEY_TABELAS:
  | ktb_Modulo      |
  |-----------------|
  | ALL CAD         |
  | ALL FAT ORC     |
  | GLASS           |
  | CTB CPG CRP     |

EMPRESAS_X_MODULOS:
  | mem_Modulo |
  |------------|
  | ALL        |
  | CAD        |
  | FAT        |
  | ORC        |
  | CTB        |
  | CPG        |
  | CRP        |
  (GLASS não está aqui)
```

**Resultado na UI:**

✅ Módulos exibidos:
```
Cadastros          (CAD)
Compras            (CPR - não, CPR é "Compras" em ModuloHelper)
Contas a Pagar     (CPG)
Contas a Receber   (CRP)
Contabilidade      (CTB)
Faturamento        (FAT)
Geral (ALL)        (ALL)
Orçamentos         (ORC)
```

❌ Módulos **não exibidos**:
```
KeyGlass (GLASS)   - não está em EMPRESAS_X_MODULOS
```

### Testes Recomendados

1. **Carregar tabelas** com o banco de dados configurado
2. **Verificar lista de módulos** - deve mostrar apenas os liberados
3. **Teste GLASS**:
   - Verificar que GLASS não aparece na lista (se não está em EMPRESAS_X_MODULOS)
   - Se estiver em EMPRESAS_X_MODULOS, deve aparecer e estar selecionável
4. **Seleção de tabelas**:
   - Marcar módulos deve continuar filtrando tabelas corretamente
   - Exemplo: marcar "Cadastros" deve mostrar tabelas com CAD em ktb_Modulo
5. **Teste de erro**:
   - Se EMPRESAS_X_MODULOS não existir ou houver erro de conexão, métodos retornam `false` (log de aviso)
   - Módulos aparecerão apenas se validação retornar `true` ou tratamento de erro permitir

### Compatibilidade com VB6

A implementação replica a lógica VB6 de `GFKEY_PossuiModulo`:
- ✅ Consulta `EMPRESAS_X_MODULOS`
- ✅ Filtra por `mem_Modulo = @SiglaModulo`
- ✅ Filtra opcionalmente por `emp_Codigo` se diferente de "00"
- ✅ Retorna `true` com `COUNT(*) > 0`
- ✅ Usa parâmetros SQL (seguro, sem concatenação)
- ✅ Captura erros e retorna `false`

### Funções Não Alteradas

As seguintes funcionalidades **permanecem intactas**:
- Backup/Restore
- BulkLoader
- Filtros de período
- Conexões com banco A/B
- Grids de tabelas
- Filtragem de tabelas por sigla (mantém internamente a sigla original)
- Todas as demais funcionalidades

### Log de Modificações

**Removido:**
- Nada (apenas refactor)

**Adicionado:**
- `PossuiModuloAsync()` - nova função private
- Parameter `DbTarget target` em `MPrc_PreencherGridModulosAsync`

**Modificado:**
- `MPrc_PreencherGridModulosAsync()` - agora async com validação
- `MPrc_CarregarTabelasBancoAsync()` - chamada await adicionada
- `MPrc_CarregarTabelasRestoreAsync()` - chamada await adicionada

**Estrutura:**
- Mantém `ModuloItem` com Sigla + Descricao
- Mantém `ModuloHelper` com DescModulo/SiglaModulo/SepararModulos
- Mantém Tag = Sigla em DataGridViewRow
