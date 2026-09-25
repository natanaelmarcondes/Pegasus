using System.Globalization;
using MySqlConnector;

namespace Pegasus
{
    internal sealed class FormConfiguracaoInicial : Form
    {
        private readonly TextBox _txtHostA = new() { Width = 220 , Text = "192.168.1.14" };
        private readonly TextBox _txtPortA = new() { Width = 80, Text = "3306" };
        private readonly TextBox _txtDatabaseA = new() { Width = 220 , Text = "gdrwa" };
        private readonly TextBox _txtUserA = new() { Width = 220 , Text = "root" };
        private readonly TextBox _txtPasswordA = new() { Width = 220, UseSystemPasswordChar = true };

        private readonly TextBox _txtHostB = new() { Width = 220 , Text = "192.168.1.14" };
        private readonly TextBox _txtPortB = new() { Width = 80, Text = "3306" };
        private readonly TextBox _txtDatabaseB = new() { Width = 220 , Text = "gdrwb" };
        private readonly TextBox _txtUserB = new() { Width = 220 ,Text = "root" };
        private readonly TextBox _txtPasswordB = new() { Width = 220, UseSystemPasswordChar = true };

        public BancoConfiguracao ConfigBancoA { get; private set; } = null!;
        public BancoConfiguracao ConfigBancoB { get; private set; } = null!;

        public FormConfiguracaoInicial()
        {
            Text = "Configuração inicial - Bancos A e B";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(640, 310);

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12),
                ColumnCount = 2,
                RowCount = 2,
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            root.Controls.Add(CriarPainelBanco("Banco A", _txtHostA, _txtPortA, _txtDatabaseA, _txtUserA, _txtPasswordA), 0, 0);
            root.Controls.Add(CriarPainelBanco("Banco B", _txtHostB, _txtPortB, _txtDatabaseB, _txtUserB, _txtPasswordB), 1, 0);

            var panelButtons = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Fill,
                AutoSize = true,
                Padding = new Padding(0, 8, 0, 0),
            };

            var btnOk = new Button { Text = "Confirmar", Width = 110, DialogResult = DialogResult.None };
            var btnCancel = new Button { Text = "Cancelar", Width = 110, DialogResult = DialogResult.Cancel };
            var btnTestar = new Button { Text = "Testar conexões", Width = 130, DialogResult = DialogResult.None };
            btnOk.Click += (_, _) => MPrc_Confirmar();
            btnTestar.Click += async (_, _) => await MPrc_TestarConexoesAsync();

            panelButtons.Controls.Add(btnOk);
            panelButtons.Controls.Add(btnCancel);
            panelButtons.Controls.Add(btnTestar);

            root.SetColumnSpan(panelButtons, 2);
            root.Controls.Add(panelButtons, 0, 1);

            Controls.Add(root);
            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }

        private static GroupBox CriarPainelBanco(string titulo, TextBox txtHost, TextBox txtPort, TextBox txtDb, TextBox txtUser, TextBox txtPass)
        {
            var panel = new GroupBox { Text = titulo, Dock = DockStyle.Fill };
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                ColumnCount = 2,
                RowCount = 5,
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            layout.Controls.Add(new Label { Text = "Host", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
            layout.Controls.Add(txtHost, 1, 0);

            layout.Controls.Add(new Label { Text = "Porta", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
            layout.Controls.Add(txtPort, 1, 1);

            layout.Controls.Add(new Label { Text = "Database", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 2);
            layout.Controls.Add(txtDb, 1, 2);

            layout.Controls.Add(new Label { Text = "Usuário", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 3);
            layout.Controls.Add(txtUser, 1, 3);

            layout.Controls.Add(new Label { Text = "Senha", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 4);
            layout.Controls.Add(txtPass, 1, 4);

            panel.Controls.Add(layout);
            return panel;
        }

        private void MPrc_Confirmar()
        {
            if (!MFcn_TentarCriarConfiguracao(_txtHostA.Text, _txtPortA.Text, _txtDatabaseA.Text, _txtUserA.Text, _txtPasswordA.Text, "A", out var configA))
            {
                return;
            }

            if (!MFcn_TentarCriarConfiguracao(_txtHostB.Text, _txtPortB.Text, _txtDatabaseB.Text, _txtUserB.Text, _txtPasswordB.Text, "B", out var configB))
            {
                return;
            }

            ConfigBancoA = configA!;
            ConfigBancoB = configB!;
            DialogResult = DialogResult.OK;
            Close();
        }

        private async Task MPrc_TestarConexoesAsync()
        {
            if (!MFcn_TentarCriarConfiguracao(_txtHostA.Text, _txtPortA.Text, _txtDatabaseA.Text, _txtUserA.Text, _txtPasswordA.Text, "A", out var configA))
            {
                return;
            }

            if (!MFcn_TentarCriarConfiguracao(_txtHostB.Text, _txtPortB.Text, _txtDatabaseB.Text, _txtUserB.Text, _txtPasswordB.Text, "B", out var configB))
            {
                return;
            }

            UseWaitCursor = true;
            try
            {
                await MFcn_TestarConexaoBancoAsync(configA!, "A");
                await MFcn_TestarConexaoBancoAsync(configB!, "B");
                MessageBox.Show("Conexões A e B testadas com sucesso.", "Teste de conexão", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Falha no teste de conexão: {ex.Message}", "Teste de conexão", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private static async Task MFcn_TestarConexaoBancoAsync(BancoConfiguracao config, string alias)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = config.Host,
                Port = config.Port,
                Database = config.Database,
                UserID = config.User,
                Password = config.Password,
                ConnectionTimeout = 10,
                DefaultCommandTimeout = 10,
            };

            await using var conn = new MySqlConnection(builder.ConnectionString);
            await conn.OpenAsync();
            await using var cmd = new MySqlCommand("SELECT 1;", conn);
            _ = await cmd.ExecuteScalarAsync();
            _ = alias;
        }

        private bool MFcn_TentarCriarConfiguracao(string host, string portaTexto, string database, string user, string password, string alias, out BancoConfiguracao? config)
        {
            config = null;

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(database) || string.IsNullOrWhiteSpace(user))
            {
                MessageBox.Show($"Preencha host, database e usuário do banco {alias}.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!uint.TryParse(portaTexto.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var porta))
            {
                MessageBox.Show($"Porta inválida no banco {alias}.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            config = new BancoConfiguracao(host.Trim(), porta, database.Trim(), user.Trim(), password);
            return true;
        }
    }
}
