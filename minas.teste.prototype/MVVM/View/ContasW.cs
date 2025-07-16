using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using minas.teste.prototype.MVVM.Model.Concrete;

namespace minas.teste.prototype.MVVM.View
{
    public partial class ContasW : Form
    {
        // --- Mock Data Storage ---
        // In a real application, these would be replaced by service calls to a database.
        private List<Usuario> _usuarios = new List<Usuario>();
        private List<Empresa> _empresas = new List<Empresa>();
        private List<Cliente> _clientes = new List<Cliente>();
        private List<Imagem> _imagens = new List<Imagem>();

        private int _nextUsuarioId = 1;
        private int _nextEmpresaId = 1;
        private int _nextClienteId = 1;
        private int _nextImagemId = 1;

        // Represents the currently logged-in user.
        // For simplicity, we'll assume an admin is logged in.
        private Usuario _currentUser;
        private const string SUPER_ADMIN_PASSWORD = "admin_password"; // Password for sensitive operations

        // Temporary storage for image data before saving
        private byte[] _empresaCurrentImageBytes = null;
        private byte[] _clienteCurrentImageBytes = null;

        public ContasW()
        {
            InitializeComponent();
            InitializeMockAdminUser(); // Initialize a mock admin user
            SetupDataGridViews();
            LoadInitialData();
            WireUpEventHandlers();
        }

        private void InitializeMockAdminUser()
        {
            _currentUser = new Usuario
            {
                ID = 0, // Or some other special ID for system/logged-in user
                Name = "AdminLogado",
                IsAdmin = true,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
        }


        private void SetupDataGridViews()
        {
            // Common settings
            Action<DataGridView> configureDgv = dgv =>
            {
                dgv.AutoGenerateColumns = false;
                dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgv.MultiSelect = false;
                dgv.AllowUserToAddRows = false;
                dgv.AllowUserToDeleteRows = false;
                dgv.ReadOnly = true;
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            };

            // Users DGV
            configureDgv(dgvUsers);
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "ID", DataPropertyName = "ID", HeaderText = "ID", Width = 20 });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", DataPropertyName = "Name", HeaderText = "Nome", FillWeight = 40 });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "EmpresaName", DataPropertyName = "EmpresaName", HeaderText = "Empresa", FillWeight = 30 });
            dgvUsers.Columns.Add(new DataGridViewCheckBoxColumn { Name = "IsAdmin", DataPropertyName = "IsAdmin", HeaderText = "Admin?", Width = 70 });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "CreateTime", DataPropertyName = "CreateTime", HeaderText = "Criação", DefaultCellStyle = new DataGridViewCellStyle { Format = "g" }, FillWeight = 30 });


            // Empresas DGV
            configureDgv(dgvEmpresas);
            dgvEmpresas.Columns.Add(new DataGridViewTextBoxColumn { Name = "ID", DataPropertyName = "ID", HeaderText = "ID", Width = 20 });
            dgvEmpresas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", DataPropertyName = "Name", HeaderText = "Nome", FillWeight = 30 });
            dgvEmpresas.Columns.Add(new DataGridViewTextBoxColumn { Name = "CNPJ", DataPropertyName = "CNPJ", HeaderText = "CNPJ", FillWeight = 25 });
            dgvEmpresas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Telefone", DataPropertyName = "Telefone", HeaderText = "Telefone", FillWeight = 20 });
            dgvEmpresas.Columns.Add(new DataGridViewTextBoxColumn { Name = "Email", DataPropertyName = "Email", HeaderText = "Email", FillWeight = 25 });


            // Clientes DGV
            configureDgv(dgvClientes);
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { Name = "ID", DataPropertyName = "ID", HeaderText = "ID", Width = 20 });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", DataPropertyName = "Name", HeaderText = "Nome", FillWeight = 25 });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { Name = "CPF", DataPropertyName = "CPF", HeaderText = "CPF", FillWeight = 20 });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { Name = "CNPJ", DataPropertyName = "CNPJ", HeaderText = "CNPJ", FillWeight = 20 });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { Name = "Telefone", DataPropertyName = "Telefone", HeaderText = "Telefone", FillWeight = 20 });
            dgvClientes.Columns.Add(new DataGridViewTextBoxColumn { Name = "EmpresaName", DataPropertyName = "EmpresaName", HeaderText = "Empresa Vinculada", FillWeight = 15 });
        }


        private void WireUpEventHandlers()
        {
            // Tab selection changes to refresh ComboBoxes if necessary
            this.tabControlMain.SelectedIndexChanged += (s, e) => {
                if (tabControlMain.SelectedTab == tabPageUsuarios || tabControlMain.SelectedTab == tabPageClientes)
                {
                    LoadEmpresasToComboBoxes();
                }
            };

            // User Tab
            this.btnUserNew.Click += (s, e) => ClearUserFields();
            this.btnUserSave.Click += BtnUserSave_Click;
            this.btnUserDelete.Click += BtnUserDelete_Click;
            this.dgvUsers.SelectionChanged += (s, e) => LoadSelectedUserToForm();
            this.chkUserIsAdmin.CheckedChanged += (s, e) => UpdateUserAccessLevelLabel();


            // Empresa Tab
            this.btnEmpresaNew.Click += (s, e) => ClearEmpresaFields();
            this.btnEmpresaSave.Click += BtnEmpresaSave_Click;
            this.btnEmpresaDelete.Click += BtnEmpresaDelete_Click;
            this.dgvEmpresas.SelectionChanged += (s, e) => LoadSelectedEmpresaToForm();
            this.btnEmpresaLoadImage.Click += BtnEmpresaLoadImage_Click;
            this.btnEmpresaClearImage.Click += (s, e) => { picEmpresaImage.Image = null; _empresaCurrentImageBytes = null; };

            // Cliente Tab
            this.btnClienteNew.Click += (s, e) => ClearClienteFields();
            this.btnClienteSave.Click += BtnClienteSave_Click;
            this.btnClienteDelete.Click += BtnClienteDelete_Click;
            this.dgvClientes.SelectionChanged += (s, e) => LoadSelectedClienteToForm();
            this.btnClienteLoadImage.Click += BtnClienteLoadImage_Click;
            this.btnClienteClearImage.Click += (s, e) => { picClienteImage.Image = null; _clienteCurrentImageBytes = null; };
            

            // Common
            this.btnSair.Click += (s, e) => this.Close();
        }

        private void LoadInitialData()
        {
            LoadEmpresas();
            LoadUsuarios();
            LoadClientes();
            LoadEmpresasToComboBoxes(); // Populate ComboBoxes after loading empresas
            InitializeMockDeletedClientes();
        }

        private void LoadEmpresasToComboBoxes()
        {
            var empresaDisplay = _empresas
                .Where(emp => emp.TerminateTime == null)
                .Select(emp => new { emp.ID, emp.Name }).ToList();

            // Preserve selected values if possible
            object selectedUserEmpresa = cmbUserEmpresa.SelectedValue;
            object selectedClienteEmpresa = cmbClienteEmpresa.SelectedValue;

            cmbUserEmpresa.DataSource = null;
            cmbUserEmpresa.DisplayMember = "Name";
            cmbUserEmpresa.ValueMember = "ID";
            cmbUserEmpresa.DataSource = new List<object>(empresaDisplay); // Create a new list for the DataSource
            cmbUserEmpresa.SelectedItem = null; // Default to no selection or handle re-selection
            if (selectedUserEmpresa != null) cmbUserEmpresa.SelectedValue = selectedUserEmpresa;


            cmbClienteEmpresa.DataSource = null;
            cmbClienteEmpresa.DisplayMember = "Name";
            cmbClienteEmpresa.ValueMember = "ID";
            cmbClienteEmpresa.DataSource = new List<object>(empresaDisplay); // Create a new list
            cmbClienteEmpresa.SelectedItem = null;
            if (selectedClienteEmpresa != null) cmbClienteEmpresa.SelectedValue = selectedClienteEmpresa;

        }

        #region User Management

        private void LoadUsuarios()
        {
            // In a real app, fetch from a service
            // For display, add EmpresaName
            var userView = _usuarios.Where(u => u.TerminateTime == null)
                .Select(u => new
                {
                    u.ID,
                    u.Name,
                    EmpresaName = u.EmpresaID.HasValue ? _empresas.FirstOrDefault(emp => emp.ID == u.EmpresaID.Value)?.Name : "N/A",
                    u.IsAdmin,
                    u.CreateTime
                }).ToList();
            dgvUsers.DataSource = null;
            dgvUsers.DataSource = userView;
        }

        private void ClearUserFields()
        {
            txtUserName.Clear();
            cmbUserEmpresa.SelectedItem = null;
            chkUserIsAdmin.Checked = false;
            lblUserAccessLevel.Text = "Usuário";
            dgvUsers.ClearSelection();
            txtUserName.Tag = null; // Used to store selected user's ID
        }

        private void LoadSelectedUserToForm()
        {
            if (dgvUsers.CurrentRow != null && dgvUsers.CurrentRow.DataBoundItem != null)
            {
                // Retrieving ID from the anonymous type used for the DataGridView
                var selectedRow = dgvUsers.CurrentRow;
                int userId = (int)selectedRow.Cells["ID"].Value;


                Usuario user = _usuarios.FirstOrDefault(u => u.ID == userId);
                if (user != null)
                {
                    txtUserName.Tag = user.ID; // Store ID
                    txtUserName.Text = user.Name;
                    chkUserIsAdmin.Checked = user.IsAdmin;
                    UpdateUserAccessLevelLabel();
                    if (user.EmpresaID.HasValue)
                    {
                        cmbUserEmpresa.SelectedValue = user.EmpresaID.Value;
                    }
                    else
                    {
                        cmbUserEmpresa.SelectedItem = null;
                    }
                }
            }
        }
        private void UpdateUserAccessLevelLabel()
        {
            lblUserAccessLevel.Text = chkUserIsAdmin.Checked ? "Administrador" : "Usuário";
        }


        private void BtnUserSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUserName.Text))
            {
                MessageBox.Show("O nome do usuário é obrigatório.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int? empresaId = (cmbUserEmpresa.SelectedValue != null) ? (int?)cmbUserEmpresa.SelectedValue : null;
            bool isNewUser = (txtUserName.Tag == null);
            Usuario user;

            if (isNewUser)
            {
                user = new Usuario { ID = _nextUsuarioId++ };
            }
            else
            {
                user = _usuarios.FirstOrDefault(u => u.ID == (int)txtUserName.Tag);
                if (user == null)
                {
                    MessageBox.Show("Usuário não encontrado para atualização.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Permission change check
                if (user.IsAdmin != chkUserIsAdmin.Checked)
                {
                    if (!_currentUser.IsAdmin)
                    {
                        MessageBox.Show("Você não tem permissão para alterar o status de administrador.", "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        chkUserIsAdmin.Checked = user.IsAdmin; // Revert
                        return;
                    }
                    // Prompt for current admin's password
                    string password = PromptForPassword("Para alterar a permissão de administrador, por favor, insira sua senha de administrador:");
                    if (password != SUPER_ADMIN_PASSWORD)
                    {
                        MessageBox.Show("Senha incorreta. A alteração de permissão foi cancelada.\nSe necessário, entre em contato com 'minas teste' para resetar sua senha.", "Falha na Autenticação", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        chkUserIsAdmin.Checked = user.IsAdmin; // Revert change
                        return;
                    }
                }
            }

            user.Name = txtUserName.Text.Trim();
            user.EmpresaID = empresaId;
            user.IsAdmin = chkUserIsAdmin.Checked;
            user.UpdateTime = DateTime.Now;

            if (isNewUser)
            {
                user.CreateTime = DateTime.Now;
                _usuarios.Add(user);
            }

            LoadUsuarios();
            ClearUserFields();
            MessageBox.Show("Usuário salvo com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void BtnUserDelete_Click(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow == null || txtUserName.Tag == null)
            {
                MessageBox.Show("Selecione um usuário para excluir.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_currentUser.IsAdmin)
            {
                MessageBox.Show("Você não tem permissão para excluir usuários.", "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            // Prompt for current admin's password
            string password = PromptForPassword("Para excluir este usuário, por favor, insira sua senha de administrador:");
            if (password != SUPER_ADMIN_PASSWORD)
            {
                MessageBox.Show("Senha incorreta. A exclusão foi cancelada.\nSe necessário, entre em contato com 'minas teste' para resetar sua senha.", "Falha na Autenticação", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            int userId = (int)txtUserName.Tag;
            Usuario user = _usuarios.FirstOrDefault(u => u.ID == userId);

            if (user != null)
            {
                if (MessageBox.Show($"Tem certeza que deseja excluir o usuário '{user.Name}'?", "Confirmar Exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Soft delete:
                    user.TerminateTime = DateTime.Now;
                    // Or hard delete: _usuarios.Remove(user);
                    LoadUsuarios();
                    ClearUserFields();
                    MessageBox.Show("Usuário excluído com sucesso.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        #endregion

        #region Empresa Management
        private void LoadEmpresas()
        {
            dgvEmpresas.DataSource = null; // Unbind first
            dgvEmpresas.DataSource = _empresas.Where(e => e.TerminateTime == null).ToList(); // Rebind
            LoadEmpresasToComboBoxes();
        }

        private void ClearEmpresaFields()
        {
            txtEmpresaName.Clear();
            txtEmpresaCNPJ.Clear();
            txtEmpresaEndereco.Clear();
            txtEmpresaEmail.Clear();
            txtEmpresaTelefone.Clear();
            picEmpresaImage.Image = null;
            _empresaCurrentImageBytes = null;
            dgvEmpresas.ClearSelection();
            txtEmpresaName.Tag = null; // Used to store selected ID
        }

        private void LoadSelectedEmpresaToForm()
        {
            if (dgvEmpresas.CurrentRow != null && dgvEmpresas.CurrentRow.DataBoundItem is Empresa selectedEmpresa)
            {
                txtEmpresaName.Tag = selectedEmpresa.ID;
                txtEmpresaName.Text = selectedEmpresa.Name;
                txtEmpresaCNPJ.Text = selectedEmpresa.CNPJ;
                txtEmpresaEndereco.Text = selectedEmpresa.Endereco;
                txtEmpresaEmail.Text = selectedEmpresa.Email;
                txtEmpresaTelefone.Text = selectedEmpresa.Telefone;

                // Load image
                _empresaCurrentImageBytes = null;
                picEmpresaImage.Image = null;
                Imagem img = _imagens.FirstOrDefault(i => i.EmpresaID == selectedEmpresa.ID && i.TerminateTime == null);
                if (img != null && img.Dado != null)
                {
                    using (MemoryStream ms = new MemoryStream(img.Dado))
                    {
                        picEmpresaImage.Image = Image.FromStream(ms);
                        _empresaCurrentImageBytes = img.Dado; // Keep a copy for potential re-save without changes
                    }
                }
            }
        }

        private void BtnEmpresaSave_Click(object sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtEmpresaName.Text) ||
                string.IsNullOrWhiteSpace(txtEmpresaCNPJ.Text) ||
                string.IsNullOrWhiteSpace(txtEmpresaEndereco.Text) ||
                string.IsNullOrWhiteSpace(txtEmpresaTelefone.Text))
            {
                MessageBox.Show("Campos obrigatórios para Empresa: Nome, CNPJ, Endereço e Telefone.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // CNPJ uniqueness check (simplified for mock data)
            if (txtEmpresaName.Tag == null && _empresas.Any(emp => emp.CNPJ == txtEmpresaCNPJ.Text.Trim() && emp.TerminateTime == null))
            {
                MessageBox.Show("Já existe uma empresa cadastrada com este CNPJ.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (txtEmpresaName.Tag != null && _empresas.Any(emp => emp.CNPJ == txtEmpresaCNPJ.Text.Trim() && emp.ID != (int)txtEmpresaName.Tag && emp.TerminateTime == null))
            {
                MessageBox.Show("Já existe outra empresa cadastrada com este CNPJ.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            bool isNew = (txtEmpresaName.Tag == null);
            Empresa empresa;

            if (isNew)
            {
                empresa = new Empresa { ID = _nextEmpresaId++ };
            }
            else
            {
                empresa = _empresas.FirstOrDefault(emp => emp.ID == (int)txtEmpresaName.Tag);
                if (empresa == null)
                {
                    MessageBox.Show("Empresa não encontrada para atualização.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            empresa.Name = txtEmpresaName.Text.Trim();
            empresa.CNPJ = txtEmpresaCNPJ.Text.Trim();
            empresa.Endereco = txtEmpresaEndereco.Text.Trim();
            empresa.Email = txtEmpresaEmail.Text.Trim();
            empresa.Telefone = txtEmpresaTelefone.Text.Trim();
            empresa.UpdateTime = DateTime.Now;

            if (isNew)
            {
                empresa.CreateTime = DateTime.Now;
                _empresas.Add(empresa);
            }

            // Handle Image
            if (_empresaCurrentImageBytes != null)
            {
                Imagem img = _imagens.FirstOrDefault(i => i.EmpresaID == empresa.ID && i.TerminateTime == null);
                if (img == null) // New image for this empresa
                {
                    img = new Imagem
                    {
                        ID = _nextImagemId++,
                        EmpresaID = empresa.ID,
                        Dado = _empresaCurrentImageBytes,
                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now
                    };
                    _imagens.Add(img);
                }
                else // Update existing image
                {
                    img.Dado = _empresaCurrentImageBytes;
                    img.UpdateTime = DateTime.Now;
                }
            }
            else
            { // If image was cleared or never set for an existing company
                Imagem existingImg = _imagens.FirstOrDefault(i => i.EmpresaID == empresa.ID && i.TerminateTime == null);
                if (existingImg != null)
                {
                    // Soft delete image or remove association
                    existingImg.TerminateTime = DateTime.Now;
                }
            }


            LoadEmpresas();
            ClearEmpresaFields();
            MessageBox.Show("Empresa salva com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnEmpresaDelete_Click(object sender, EventArgs e)
        {
            if (dgvEmpresas.CurrentRow == null || txtEmpresaName.Tag == null)
            {
                MessageBox.Show("Selecione uma empresa para excluir.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int empresaId = (int)txtEmpresaName.Tag;

            // Check if any user or client is associated with this company
            if (_usuarios.Any(u => u.EmpresaID == empresaId && u.TerminateTime == null) ||
                _clientes.Any(c => c.EmpresaID == empresaId && c.TerminateTime == null))
            {
                MessageBox.Show("Esta empresa não pode ser excluída pois está associada a usuários ou clientes ativos.", "Impossível Excluir", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }


            Empresa empresa = _empresas.FirstOrDefault(emp => emp.ID == empresaId);
            if (empresa != null)
            {
                if (MessageBox.Show($"Tem certeza que deseja excluir a empresa '{empresa.Name}'?", "Confirmar Exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    empresa.TerminateTime = DateTime.Now; // Soft delete
                    // Also soft-delete associated images
                    _imagens.Where(img => img.EmpresaID == empresa.ID).ToList().ForEach(img => img.TerminateTime = DateTime.Now);

                    LoadEmpresas();
                    ClearEmpresaFields();
                    MessageBox.Show("Empresa excluída com sucesso.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnEmpresaLoadImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _empresaCurrentImageBytes = File.ReadAllBytes(openFileDialog.FileName);
                        using (MemoryStream ms = new MemoryStream(_empresaCurrentImageBytes))
                        {
                            picEmpresaImage.Image = Image.FromStream(ms);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao carregar imagem: " + ex.Message, "Erro de Imagem", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        _empresaCurrentImageBytes = null;
                        picEmpresaImage.Image = null;
                    }
                }
            }
        }
        #endregion

        #region Cliente Management
        private void LoadClientes()
        {
            var clienteView = _clientes.Where(c => c.TerminateTime == null)
                .Select(c => new
                {
                    c.ID,
                    c.Name,
                    c.CPF,
                    c.CNPJ,
                    c.Telefone,
                    EmpresaName = c.EmpresaID.HasValue ? _empresas.FirstOrDefault(emp => emp.ID == c.EmpresaID.Value)?.Name : "N/A"
                }).ToList();
            dgvClientes.DataSource = null;
            dgvClientes.DataSource = clienteView;
        }

        private void ClearClienteFields()
        {
            txtClienteName.Clear();
            txtClienteCNPJ.Clear();
            txtClienteCPF.Clear();
            txtClienteEndereco.Clear();
            txtClienteEmail.Clear();
            txtClienteTelefone.Clear();
            cmbClienteEmpresa.SelectedItem = null;
            picClienteImage.Image = null;
            _clienteCurrentImageBytes = null;
            dgvClientes.ClearSelection();
            txtClienteName.Tag = null; // Used to store selected ID
        }

        private void LoadSelectedClienteToForm()
        {
            if (dgvClientes.CurrentRow != null && dgvClientes.CurrentRow.DataBoundItem != null)
            {
                var selectedRow = dgvClientes.CurrentRow;
                int clienteId = (int)selectedRow.Cells["ID"].Value;

                Cliente cliente = _clientes.FirstOrDefault(c => c.ID == clienteId);
                if (cliente != null)
                {
                    txtClienteName.Tag = cliente.ID;
                    txtClienteName.Text = cliente.Name;
                    txtClienteCNPJ.Text = cliente.CNPJ;
                    txtClienteCPF.Text = cliente.CPF;
                    txtClienteEndereco.Text = cliente.Endereco;
                    txtClienteEmail.Text = cliente.Email;
                    txtClienteTelefone.Text = cliente.Telefone;
                    if (cliente.EmpresaID.HasValue)
                    {
                        cmbClienteEmpresa.SelectedValue = cliente.EmpresaID.Value;
                    }
                    else
                    {
                        cmbClienteEmpresa.SelectedItem = null;
                    }

                    // Load image
                    _clienteCurrentImageBytes = null;
                    picClienteImage.Image = null;
                    Imagem img = _imagens.FirstOrDefault(i => i.ClienteID == cliente.ID && i.TerminateTime == null);
                    if (img != null && img.Dado != null)
                    {
                        using (MemoryStream ms = new MemoryStream(img.Dado))
                        {
                            picClienteImage.Image = Image.FromStream(ms);
                            _clienteCurrentImageBytes = img.Dado;
                        }
                    }
                }
            }
        }

        private void BtnClienteSave_Click(object sender, EventArgs e)
        {
            // Validation - Name, CNPJ, CPF, Endereco, Telefone are mandatory for Cliente
            if (string.IsNullOrWhiteSpace(txtClienteName.Text) ||
                string.IsNullOrWhiteSpace(txtClienteCNPJ.Text) || // As per prompt, both CNPJ and CPF are mandatory
                string.IsNullOrWhiteSpace(txtClienteCPF.Text) ||
                string.IsNullOrWhiteSpace(txtClienteEndereco.Text) ||
                string.IsNullOrWhiteSpace(txtClienteTelefone.Text))
            {
                MessageBox.Show("Campos obrigatórios para Cliente: Nome, CNPJ, CPF, Endereço e Telefone.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool isNew = (txtClienteName.Tag == null);
            Cliente cliente;

            if (isNew)
            {
                cliente = new Cliente { ID = _nextClienteId++ };
            }
            else
            {
                cliente = _clientes.FirstOrDefault(c => c.ID == (int)txtClienteName.Tag);
                if (cliente == null)
                {
                    MessageBox.Show("Cliente não encontrado para atualização.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            cliente.Name = txtClienteName.Text.Trim();
            cliente.CNPJ = txtClienteCNPJ.Text.Trim();
            cliente.CPF = txtClienteCPF.Text.Trim();
            cliente.Endereco = txtClienteEndereco.Text.Trim();
            cliente.Email = txtClienteEmail.Text.Trim();
            cliente.Telefone = txtClienteTelefone.Text.Trim();
            cliente.EmpresaID = (cmbClienteEmpresa.SelectedValue != null) ? (int?)cmbClienteEmpresa.SelectedValue : null;
            cliente.UpdateTime = DateTime.Now;

            if (isNew)
            {
                cliente.CreateTime = DateTime.Now;
                _clientes.Add(cliente);
            }

            // Handle Image
            if (_clienteCurrentImageBytes != null)
            {
                Imagem img = _imagens.FirstOrDefault(i => i.ClienteID == cliente.ID && i.TerminateTime == null);
                if (img == null)
                {
                    img = new Imagem
                    {
                        ID = _nextImagemId++,
                        ClienteID = cliente.ID,
                        Dado = _clienteCurrentImageBytes,
                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now
                    };
                    _imagens.Add(img);
                }
                else
                {
                    img.Dado = _clienteCurrentImageBytes;
                    img.UpdateTime = DateTime.Now;
                }
            }
            else
            {
                Imagem existingImg = _imagens.FirstOrDefault(i => i.ClienteID == cliente.ID && i.TerminateTime == null);
                if (existingImg != null)
                {
                    existingImg.TerminateTime = DateTime.Now;
                }
            }


            LoadClientes();
            ClearClienteFields();
            MessageBox.Show("Cliente salvo com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnClienteDelete_Click(object sender, EventArgs e)
        {
            if (dgvClientes.CurrentRow == null || txtClienteName.Tag == null)
            {
                MessageBox.Show("Selecione um cliente para excluir.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int clienteId = (int)txtClienteName.Tag;
            Cliente cliente = _clientes.FirstOrDefault(c => c.ID == clienteId);

            if (cliente != null)
            {
                if (MessageBox.Show($"Tem certeza que deseja excluir o cliente '{cliente.Name}'?", "Confirmar Exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cliente.TerminateTime = DateTime.Now; // Soft delete
                    _imagens.Where(img => img.ClienteID == cliente.ID).ToList().ForEach(img => img.TerminateTime = DateTime.Now);

                    LoadClientes();
                    ClearClienteFields();
                    MessageBox.Show("Cliente excluído com sucesso.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnClienteLoadImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _clienteCurrentImageBytes = File.ReadAllBytes(openFileDialog.FileName);
                        using (MemoryStream ms = new MemoryStream(_clienteCurrentImageBytes))
                        {
                            picClienteImage.Image = Image.FromStream(ms);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro ao carregar imagem: " + ex.Message, "Erro de Imagem", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        _clienteCurrentImageBytes = null;
                        picClienteImage.Image = null;
                    }
                }
            }
        }

        #endregion

        #region Cliente Recovery Management

        /// <summary>
        /// Recupera clientes que foram excluídos (soft delete) e permite restaurá-los
        /// </summary>
        private void ShowClienteRecoveryDialog()
        {
            // Buscar clientes excluídos (com TerminateTime != null)
            var clientesExcluidos = _clientes.Where(c => c.TerminateTime != null).ToList();

            if (!clientesExcluidos.Any())
            {
                MessageBox.Show("Não há clientes excluídos para recuperar.", "Informação",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Criar formulário de recuperação
            Form recoveryForm = new Form()
            {
                Width = 800,
                Height = 600,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Recuperar Clientes Excluídos",
                StartPosition = FormStartPosition.CenterScreen,
                MaximizeBox = false,
                MinimizeBox = false
            };

            // DataGridView para exibir clientes excluídos
            DataGridView dgvRecovery = new DataGridView()
            {
                Left = 20,
                Top = 20,
                Width = 740,
                Height = 400,
                AutoGenerateColumns = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Configurar colunas do DataGridView
            dgvRecovery.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ID",
                DataPropertyName = "ID",
                HeaderText = "ID",
                Width = 50
            });
            dgvRecovery.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Name",
                DataPropertyName = "Name",
                HeaderText = "Nome",
                FillWeight = 25
            });
            dgvRecovery.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CPF",
                DataPropertyName = "CPF",
                HeaderText = "CPF",
                FillWeight = 15
            });
            dgvRecovery.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CNPJ",
                DataPropertyName = "CNPJ",
                HeaderText = "CNPJ",
                FillWeight = 15
            });
            dgvRecovery.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Telefone",
                DataPropertyName = "Telefone",
                HeaderText = "Telefone",
                FillWeight = 15
            });
            dgvRecovery.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "EmpresaName",
                DataPropertyName = "EmpresaName",
                HeaderText = "Empresa",
                FillWeight = 15
            });
            dgvRecovery.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TerminateTime",
                DataPropertyName = "TerminateTime",
                HeaderText = "Data Exclusão",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "g" },
                FillWeight = 15
            });

            // Preparar dados para exibição
            var clientesView = clientesExcluidos.Select(c => new
            {
                c.ID,
                c.Name,
                c.CPF,
                c.CNPJ,
                c.Telefone,
                EmpresaName = c.EmpresaID.HasValue ?
                    _empresas.FirstOrDefault(emp => emp.ID == c.EmpresaID.Value)?.Name ?? "Empresa Excluída" :
                    "N/A",
                c.TerminateTime
            }).ToList();

            dgvRecovery.DataSource = clientesView;

            // Botões
            Button btnRecover = new Button()
            {
                Text = "Recuperar Cliente",
                Left = 20,
                Top = 440,
                Width = 150,
                Height = 30
            };

            Button btnDeletePermanent = new Button()
            {
                Text = "Excluir Permanentemente",
                Left = 190,
                Top = 440,
                Width = 180,
                Height = 30,
                BackColor = Color.FromArgb(220, 53, 69), // Vermelho
                ForeColor = Color.White
            };

            Button btnClose = new Button()
            {
                Text = "Fechar",
                Left = 600,
                Top = 440,
                Width = 100,
                Height = 30,
                DialogResult = DialogResult.Cancel
            };

            // Labels informativos
            Label lblInfo = new Label()
            {
                Text = "Selecione um cliente da lista para recuperar ou excluir permanentemente.",
                Left = 20,
                Top = 480,
                Width = 740,
                Height = 20,
                ForeColor = Color.Gray
            };

            Label lblCount = new Label()
            {
                Text = $"Total de clientes excluídos: {clientesExcluidos.Count}",
                Left = 20,
                Top = 500,
                Width = 300,
                Height = 20,
                Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold)
            };

            // Eventos dos botões
            btnRecover.Click += (s, e) =>
            {
                if (dgvRecovery.CurrentRow == null)
                {
                    MessageBox.Show("Selecione um cliente para recuperar.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int clienteId = (int)dgvRecovery.CurrentRow.Cells["ID"].Value;
                Cliente cliente = _clientes.FirstOrDefault(c => c.ID == clienteId);

                if (cliente != null)
                {
                    string clienteName = cliente.Name;
                    var result = MessageBox.Show($"Tem certeza que deseja recuperar o cliente '{clienteName}'?",
                        "Confirmar Recuperação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // Recuperar cliente
                        cliente.TerminateTime = null;
                        cliente.UpdateTime = DateTime.Now;

                        // Recuperar imagens associadas (se existirem)
                        var imagensAssociadas = _imagens.Where(img => img.ClienteID == clienteId && img.TerminateTime != null).ToList();
                        foreach (var img in imagensAssociadas)
                        {
                            img.TerminateTime = null;
                            img.UpdateTime = DateTime.Now;
                        }

                        MessageBox.Show($"Cliente '{clienteName}' recuperado com sucesso!", "Sucesso",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Atualizar a lista de clientes ativos
                        LoadClientes();

                        // Fechar o formulário de recuperação
                        recoveryForm.Close();
                    }
                }
            };

            btnDeletePermanent.Click += (s, e) =>
            {
                if (dgvRecovery.CurrentRow == null)
                {
                    MessageBox.Show("Selecione um cliente para excluir permanentemente.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int clienteId = (int)dgvRecovery.CurrentRow.Cells["ID"].Value;
                Cliente cliente = _clientes.FirstOrDefault(c => c.ID == clienteId);

                if (cliente != null)
                {
                    string clienteName = cliente.Name;
                    var result = MessageBox.Show($"ATENÇÃO: Esta ação não pode ser desfeita!\n\n" +
                        $"Tem certeza que deseja excluir permanentemente o cliente '{clienteName}'?",
                        "Confirmar Exclusão Permanente", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        // Excluir permanentemente
                        _clientes.Remove(cliente);

                        // Excluir imagens associadas permanentemente
                        var imagensAssociadas = _imagens.Where(img => img.ClienteID == clienteId).ToList();
                        foreach (var img in imagensAssociadas)
                        {
                            _imagens.Remove(img);
                        }

                        MessageBox.Show($"Cliente '{clienteName}' excluído permanentemente!", "Excluído",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Atualizar a exibição
                        clientesView = _clientes.Where(c => c.TerminateTime != null).Select(c => new
                        {
                            c.ID,
                            c.Name,
                            c.CPF,
                            c.CNPJ,
                            c.Telefone,
                            EmpresaName = c.EmpresaID.HasValue ?
                                _empresas.FirstOrDefault(emp => emp.ID == c.EmpresaID.Value)?.Name ?? "Empresa Excluída" :
                                "N/A",
                            c.TerminateTime
                        }).ToList();

                        dgvRecovery.DataSource = null;
                        dgvRecovery.DataSource = clientesView;
                        lblCount.Text = $"Total de clientes excluídos: {clientesView.Count}";

                        // Se não há mais clientes excluídos, fechar o formulário
                        if (!clientesView.Any())
                        {
                            MessageBox.Show("Não há mais clientes excluídos.", "Informação",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            recoveryForm.Close();
                        }
                    }
                }
            };

            btnClose.Click += (s, e) => recoveryForm.Close();

            // Adicionar controles ao formulário
            recoveryForm.Controls.Add(dgvRecovery);
            recoveryForm.Controls.Add(btnRecover);
            recoveryForm.Controls.Add(btnDeletePermanent);
            recoveryForm.Controls.Add(btnClose);
            recoveryForm.Controls.Add(lblInfo);
            recoveryForm.Controls.Add(lblCount);

            // Configurar botão padrão
            recoveryForm.AcceptButton = btnRecover;
            recoveryForm.CancelButton = btnClose;

            // Exibir formulário
            recoveryForm.ShowDialog();
        }

        /// <summary>
        /// Método para criar dados mockados de clientes excluídos para testes
        /// </summary>
        private void CreateMockDeletedClientes()
        {
            // Criar alguns clientes mockados já excluídos para teste
            var mockDeletedClientes = new List<Cliente>
    {
        new Cliente
        {
            ID = 9001,
            Name = "João Silva Santos",
            CPF = "123.456.789-10",
            CNPJ = "12.345.678/0001-90",
            Endereco = "Rua das Flores, 123",
            Email = "joao.silva@email.com",
            Telefone = "(31) 99999-1234",
            EmpresaID = _empresas.FirstOrDefault()?.ID,
            CreateTime = DateTime.Now.AddDays(-30),
            UpdateTime = DateTime.Now.AddDays(-5),
            TerminateTime = DateTime.Now.AddDays(-5) // Excluído há 5 dias
        },
        new Cliente
        {
            ID = 9002,
            Name = "Maria Oliveira Costa",
            CPF = "987.654.321-00",
            CNPJ = "98.765.432/0001-10",
            Endereco = "Av. Principal, 456",
            Email = "maria.oliveira@email.com",
            Telefone = "(31) 88888-5678",
            EmpresaID = _empresas.Skip(1).FirstOrDefault()?.ID,
            CreateTime = DateTime.Now.AddDays(-20),
            UpdateTime = DateTime.Now.AddDays(-3),
            TerminateTime = DateTime.Now.AddDays(-3) // Excluído há 3 dias
        },
        new Cliente
        {
            ID = 9003,
            Name = "Carlos Pereira Ltda",
            CPF = "456.789.123-45",
            CNPJ = "45.678.912/0001-34",
            Endereco = "Rua do Comércio, 789",
            Email = "carlos.pereira@empresa.com",
            Telefone = "(31) 77777-9012",
            EmpresaID = null, // Sem empresa vinculada
            CreateTime = DateTime.Now.AddDays(-15),
            UpdateTime = DateTime.Now.AddDays(-1),
            TerminateTime = DateTime.Now.AddDays(-1) // Excluído há 1 dia
        }
    };

            // Adicionar os clientes mockados à lista principal
            _clientes.AddRange(mockDeletedClientes);

            // Atualizar o próximo ID para evitar conflitos
            _nextClienteId = Math.Max(_nextClienteId, mockDeletedClientes.Max(c => c.ID) + 1);
        }

        /// <summary>
        /// Método público para ser chamado de fora da classe (ex: de um botão no formulário)
        /// </summary>
        public void ShowClienteRecovery()
        {
            ShowClienteRecoveryDialog();
        }

        /// <summary>
        /// Método público para criar dados de teste (chamado uma vez para popular dados mockados)
        /// </summary>
        public void InitializeMockDeletedClientes()
        {
            CreateMockDeletedClientes();
        }

        #endregion

        #region Helper Methods
        private string PromptForPassword(string promptMessage)
        {
            // This is a very basic password prompt. Consider creating a dedicated dialog form for better UI/UX.
            Form prompt = new Form()
            {
                Width = 400,
                Height = 180,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Autenticação Necessária",
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 20, Top = 20, Width = 360, Text = promptMessage };
            TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 350, PasswordChar = '*' };
            Button confirmation = new Button() { Text = "Ok", Left = 270, Width = 100, Top = 80, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }

        private void BtnSair_Click(object sender, EventArgs e)
        {
            this.Hide(); // Hide the current MainForm

            // --- Code to show MenuApp ---
            // This assumes 'Menuapp' is a class representing your menu form,
            // and it has a public static 'Instance' property that returns the
            // active (or creates/retrieves a new) instance of Menuapp.

            // You will need to ensure the Menuapp class and its Instance property are correctly defined
            // and accessible from MainForm.

            System.Windows.Forms.Form menuAppInstanceToShow = null;

            try
            {
                // Attempt to get the instance of Menuapp using the provided pattern
                var menuAppSingleton = Menuapp.Instance; // This line comes from your request.

                if (menuAppSingleton is System.Windows.Forms.Form formInstance)
                {
                    menuAppInstanceToShow = formInstance;
                }
                else if (menuAppSingleton != null)
                {
                    // If Menuapp.Instance returns something but it's not a Form, it's an issue.
                    MessageBox.Show("Menuapp.Instance did not return a valid Form object.",
                                    "Error de Configuração", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close(); // Or Application.Exit(); to close the entire application
                    return;
                }
                // If menuAppSingleton is null, it will be handled by the 'else' block below.
            }
            catch (Exception ex)
            {
                // Catch potential errors if Menuapp.Instance itself throws an exception
                MessageBox.Show($"Erro ao tentar acessar o Menu Principal (Menuapp):\n{ex.Message}\n\nA aplicação será fechada.",
                                "Erro Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close(); // Or Application.Exit();
                return;
            }

            if (menuAppInstanceToShow != null)
            {
                if (menuAppInstanceToShow.IsDisposed)
                {
                    // Handle the case where the Menuapp instance is disposed.
                    // Option 1: Show an error and close the current form (or the application).
                    MessageBox.Show("A instância do Menu Principal foi descartada e não pode ser reaberta.\n" +
                                    "A aplicação será fechada.",
                                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close(); // Or Application.Exit();

                    // Option 2 (Advanced): If your Menuapp.Instance logic can recreate the form,
                    // you might call Menuapp.Instance again. However, this depends on your specific
                    // implementation of the Singleton pattern for Menuapp.
                    // For example:
                    // menuAppInstanceToShow = Menuapp.Instance; // Try getting it again
                    // if (menuAppInstanceToShow != null && !menuAppInstanceToShow.IsDisposed) {
                    //     menuAppInstanceToShow.Show();
                    //     menuAppInstanceToShow.BringToFront();
                    // } else { ... error handling ... }
                }
                else
                {
                    // If the instance is valid and not disposed, show it and bring it to front.
                    menuAppInstanceToShow.Show();
                    menuAppInstanceToShow.BringToFront();
                }
            }
            else
            {
                // Menuapp.Instance returned null, meaning the menu form couldn't be retrieved.
                MessageBox.Show("Instância do Menu Principal (Menuapp) não encontrada.\n" +
                                "Verifique a configuração da aplicação.\n\nA aplicação será fechada.",
                                "Erro Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close(); // Or Application.Exit();
            }
        }

        #endregion

        // In MainForm.Designer.cs, you would add these controls using the designer:
        // TabControl: tabControlMain
        // TabPages: tabPageUsuarios, tabPageEmpresas, tabPageClientes
        // For Usuários: txtUserName, cmbUserEmpresa, chkUserIsAdmin, lblUserAccessLevel, btnUserNew, btnUserSave, btnUserDelete, dgvUsers
        // For Empresas: txtEmpresaName, txtEmpresaCNPJ, txtEmpresaEndereco, txtEmpresaEmail, txtEmpresaTelefone, picEmpresaImage, btnEmpresaLoadImage, btnEmpresaClearImage, btnEmpresaNew, btnEmpresaSave, btnEmpresaDelete, dgvEmpresas
        // For Clientes: txtClienteName, txtClienteCNPJ, txtClienteCPF, txtClienteEndereco, txtClienteEmail, txtClienteTelefone, cmbClienteEmpresa, picClienteImage, btnClienteLoadImage, btnClienteClearImage, btnClienteNew, btnClienteSave, btnClienteDelete, dgvClientes
        // Common: btnSair
        // Make sure control names in this MainForm.cs match what you create in the designer.

        // Example of how you'd initialize components in MainForm.Designer.cs (partial)
        // This is generated by Visual Studio. You normally don't write this manually.
        /*
        private void InitializeComponent()
        {
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPageUsuarios = new System.Windows.Forms.TabPage();
            // ... (all other controls for tabPageUsuarios) ...
            this.dgvUsers = new System.Windows.Forms.DataGridView();
            this.tabPageEmpresas = new System.Windows.Forms.TabPage();
            // ... (all other controls for tabPageEmpresas) ...
            this.dgvEmpresas = new System.Windows.Forms.DataGridView();
            this.tabPageClientes = new System.Windows.Forms.TabPage();
            // ... (all other controls for tabPageClientes) ...
            this.dgvClientes = new System.Windows.Forms.DataGridView();
            this.btnSair = new System.Windows.Forms.Button();
            // ...SuspendLayout(); ... AddRange ... Set properties ...ResumeLayout(false);
            this.ClientSize = new System.Drawing.Size(1024, 768);
        }
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageUsuarios;
        private System.Windows.Forms.DataGridView dgvUsers;
        // ... (declare all other private member variables for controls) ...
        private System.Windows.Forms.Button btnSair;
        */
    }

}
