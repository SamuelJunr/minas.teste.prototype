namespace minas.teste.prototype.MVVM.View
{
    
        partial class ContasW
        {
            /// <summary>
            /// Required designer variable.
            /// </summary>
            private System.ComponentModel.IContainer components = null;

            /// <summary>
            /// Clean up any resources being used.
            /// </summary>
            /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
            protected override void Dispose(bool disposing)
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                base.Dispose(disposing);
            }

            #region Windows Form Designer generated code

            /// <summary>
            /// Required method for Designer support - do not modify
            /// the contents of this method with the code editor.
            /// </summary>
            private void InitializeComponent()
            {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ContasW));
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPageUsuarios = new System.Windows.Forms.TabPage();
            this.grpUserAccessLevel = new System.Windows.Forms.GroupBox();
            this.lblUserAccessLevel = new System.Windows.Forms.Label();
            this.btnUserClearFields = new System.Windows.Forms.Button();
            this.dgvUsers = new System.Windows.Forms.DataGridView();
            this.btnUserDelete = new System.Windows.Forms.Button();
            this.btnUserSave = new System.Windows.Forms.Button();
            this.btnUserNew = new System.Windows.Forms.Button();
            this.grpUserDetails = new System.Windows.Forms.GroupBox();
            this.chkUserIsAdmin = new System.Windows.Forms.CheckBox();
            this.cmbUserEmpresa = new System.Windows.Forms.ComboBox();
            this.lblUserEmpresa = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.lblUserName = new System.Windows.Forms.Label();
            this.tabPageEmpresas = new System.Windows.Forms.TabPage();
            this.btnEmpresaClearFields = new System.Windows.Forms.Button();
            this.dgvEmpresas = new System.Windows.Forms.DataGridView();
            this.btnEmpresaDelete = new System.Windows.Forms.Button();
            this.btnEmpresaSave = new System.Windows.Forms.Button();
            this.btnEmpresaNew = new System.Windows.Forms.Button();
            this.grpEmpresaImage = new System.Windows.Forms.GroupBox();
            this.btnEmpresaClearImage = new System.Windows.Forms.Button();
            this.btnEmpresaLoadImage = new System.Windows.Forms.Button();
            this.picEmpresaImage = new System.Windows.Forms.PictureBox();
            this.grpEmpresaDetails = new System.Windows.Forms.GroupBox();
            this.txtEmpresaTelefone = new System.Windows.Forms.TextBox();
            this.lblEmpresaTelefone = new System.Windows.Forms.Label();
            this.txtEmpresaEmail = new System.Windows.Forms.TextBox();
            this.lblEmpresaEmail = new System.Windows.Forms.Label();
            this.txtEmpresaEndereco = new System.Windows.Forms.TextBox();
            this.lblEmpresaEndereco = new System.Windows.Forms.Label();
            this.txtEmpresaCNPJ = new System.Windows.Forms.TextBox();
            this.lblEmpresaCNPJ = new System.Windows.Forms.Label();
            this.txtEmpresaName = new System.Windows.Forms.TextBox();
            this.lblEmpresaName = new System.Windows.Forms.Label();
            this.tabPageClientes = new System.Windows.Forms.TabPage();
            this.btnClienteClearFields = new System.Windows.Forms.Button();
            this.dgvClientes = new System.Windows.Forms.DataGridView();
            this.btnClienteDelete = new System.Windows.Forms.Button();
            this.btnClienteSave = new System.Windows.Forms.Button();
            this.btnClienteNew = new System.Windows.Forms.Button();
            this.grpClienteImage = new System.Windows.Forms.GroupBox();
            this.btnClienteClearImage = new System.Windows.Forms.Button();
            this.btnClienteLoadImage = new System.Windows.Forms.Button();
            this.picClienteImage = new System.Windows.Forms.PictureBox();
            this.grpClienteDetails = new System.Windows.Forms.GroupBox();
            this.cmbClienteEmpresa = new System.Windows.Forms.ComboBox();
            this.lblClienteEmpresa = new System.Windows.Forms.Label();
            this.txtClienteTelefone = new System.Windows.Forms.TextBox();
            this.lblClienteTelefone = new System.Windows.Forms.Label();
            this.txtClienteEmail = new System.Windows.Forms.TextBox();
            this.lblClienteEmail = new System.Windows.Forms.Label();
            this.txtClienteEndereco = new System.Windows.Forms.TextBox();
            this.lblClienteEndereco = new System.Windows.Forms.Label();
            this.txtClienteCPF = new System.Windows.Forms.TextBox();
            this.lblClienteCPF = new System.Windows.Forms.Label();
            this.txtClienteCNPJ = new System.Windows.Forms.TextBox();
            this.lblClienteCNPJ = new System.Windows.Forms.Label();
            this.txtClienteName = new System.Windows.Forms.TextBox();
            this.lblClienteName = new System.Windows.Forms.Label();
            this.openFileDialogImage = new System.Windows.Forms.OpenFileDialog();
            this.btnSair = new System.Windows.Forms.Button();
            this.tabControlMain.SuspendLayout();
            this.tabPageUsuarios.SuspendLayout();
            this.grpUserAccessLevel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).BeginInit();
            this.grpUserDetails.SuspendLayout();
            this.tabPageEmpresas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmpresas)).BeginInit();
            this.grpEmpresaImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picEmpresaImage)).BeginInit();
            this.grpEmpresaDetails.SuspendLayout();
            this.tabPageClientes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientes)).BeginInit();
            this.grpClienteImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picClienteImage)).BeginInit();
            this.grpClienteDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlMain
            // 
            this.tabControlMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlMain.Controls.Add(this.tabPageUsuarios);
            this.tabControlMain.Controls.Add(this.tabPageEmpresas);
            this.tabControlMain.Controls.Add(this.tabPageClientes);
            this.tabControlMain.Location = new System.Drawing.Point(16, 13);
            this.tabControlMain.Margin = new System.Windows.Forms.Padding(4);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(1389, 641);
            this.tabControlMain.TabIndex = 0;
            // 
            // tabPageUsuarios
            // 
            this.tabPageUsuarios.Controls.Add(this.grpUserAccessLevel);
            this.tabPageUsuarios.Controls.Add(this.btnUserClearFields);
            this.tabPageUsuarios.Controls.Add(this.dgvUsers);
            this.tabPageUsuarios.Controls.Add(this.btnUserDelete);
            this.tabPageUsuarios.Controls.Add(this.btnUserSave);
            this.tabPageUsuarios.Controls.Add(this.btnUserNew);
            this.tabPageUsuarios.Controls.Add(this.grpUserDetails);
            this.tabPageUsuarios.Location = new System.Drawing.Point(4, 25);
            this.tabPageUsuarios.Margin = new System.Windows.Forms.Padding(4);
            this.tabPageUsuarios.Name = "tabPageUsuarios";
            this.tabPageUsuarios.Padding = new System.Windows.Forms.Padding(4);
            this.tabPageUsuarios.Size = new System.Drawing.Size(1381, 612);
            this.tabPageUsuarios.TabIndex = 0;
            this.tabPageUsuarios.Text = "Usuários";
            this.tabPageUsuarios.UseVisualStyleBackColor = true;
            // 
            // grpUserAccessLevel
            // 
            this.grpUserAccessLevel.Controls.Add(this.lblUserAccessLevel);
            this.grpUserAccessLevel.Location = new System.Drawing.Point(21, 209);
            this.grpUserAccessLevel.Margin = new System.Windows.Forms.Padding(4);
            this.grpUserAccessLevel.Name = "grpUserAccessLevel";
            this.grpUserAccessLevel.Padding = new System.Windows.Forms.Padding(4);
            this.grpUserAccessLevel.Size = new System.Drawing.Size(507, 68);
            this.grpUserAccessLevel.TabIndex = 1;
            this.grpUserAccessLevel.TabStop = false;
            this.grpUserAccessLevel.Text = "Nível de Acesso Atual";
            // 
            // lblUserAccessLevel
            // 
            this.lblUserAccessLevel.AutoSize = true;
            this.lblUserAccessLevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserAccessLevel.Location = new System.Drawing.Point(20, 31);
            this.lblUserAccessLevel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUserAccessLevel.Name = "lblUserAccessLevel";
            this.lblUserAccessLevel.Size = new System.Drawing.Size(74, 20);
            this.lblUserAccessLevel.TabIndex = 0;
            this.lblUserAccessLevel.Text = "Usuário";
            // 
            // btnUserClearFields
            // 
            this.btnUserClearFields.Location = new System.Drawing.Point(420, 289);
            this.btnUserClearFields.Margin = new System.Windows.Forms.Padding(4);
            this.btnUserClearFields.Name = "btnUserClearFields";
            this.btnUserClearFields.Size = new System.Drawing.Size(100, 28);
            this.btnUserClearFields.TabIndex = 5;
            this.btnUserClearFields.Text = "Limpar";
            this.btnUserClearFields.UseVisualStyleBackColor = true;
            // 
            // dgvUsers
            // 
            this.dgvUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvUsers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUsers.Location = new System.Drawing.Point(21, 332);
            this.dgvUsers.Margin = new System.Windows.Forms.Padding(4);
            this.dgvUsers.Name = "dgvUsers";
            this.dgvUsers.RowHeadersWidth = 51;
            this.dgvUsers.Size = new System.Drawing.Size(1336, 259);
            this.dgvUsers.TabIndex = 6;
            // 
            // btnUserDelete
            // 
            this.btnUserDelete.Location = new System.Drawing.Point(300, 289);
            this.btnUserDelete.Margin = new System.Windows.Forms.Padding(4);
            this.btnUserDelete.Name = "btnUserDelete";
            this.btnUserDelete.Size = new System.Drawing.Size(100, 28);
            this.btnUserDelete.TabIndex = 4;
            this.btnUserDelete.Text = "Excluir";
            this.btnUserDelete.UseVisualStyleBackColor = true;
            // 
            // btnUserSave
            // 
            this.btnUserSave.Location = new System.Drawing.Point(180, 289);
            this.btnUserSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnUserSave.Name = "btnUserSave";
            this.btnUserSave.Size = new System.Drawing.Size(100, 28);
            this.btnUserSave.TabIndex = 3;
            this.btnUserSave.Text = "Salvar";
            this.btnUserSave.UseVisualStyleBackColor = true;
            // 
            // btnUserNew
            // 
            this.btnUserNew.Location = new System.Drawing.Point(60, 289);
            this.btnUserNew.Margin = new System.Windows.Forms.Padding(4);
            this.btnUserNew.Name = "btnUserNew";
            this.btnUserNew.Size = new System.Drawing.Size(100, 28);
            this.btnUserNew.TabIndex = 2;
            this.btnUserNew.Text = "Novo";
            this.btnUserNew.UseVisualStyleBackColor = true;
            // 
            // grpUserDetails
            // 
            this.grpUserDetails.Controls.Add(this.chkUserIsAdmin);
            this.grpUserDetails.Controls.Add(this.cmbUserEmpresa);
            this.grpUserDetails.Controls.Add(this.lblUserEmpresa);
            this.grpUserDetails.Controls.Add(this.txtUserName);
            this.grpUserDetails.Controls.Add(this.lblUserName);
            this.grpUserDetails.Location = new System.Drawing.Point(21, 18);
            this.grpUserDetails.Margin = new System.Windows.Forms.Padding(4);
            this.grpUserDetails.Name = "grpUserDetails";
            this.grpUserDetails.Padding = new System.Windows.Forms.Padding(4);
            this.grpUserDetails.Size = new System.Drawing.Size(507, 178);
            this.grpUserDetails.TabIndex = 0;
            this.grpUserDetails.TabStop = false;
            this.grpUserDetails.Text = "Detalhes do Usuário";
            // 
            // chkUserIsAdmin
            // 
            this.chkUserIsAdmin.AutoSize = true;
            this.chkUserIsAdmin.Location = new System.Drawing.Point(24, 129);
            this.chkUserIsAdmin.Margin = new System.Windows.Forms.Padding(4);
            this.chkUserIsAdmin.Name = "chkUserIsAdmin";
            this.chkUserIsAdmin.Size = new System.Drawing.Size(112, 20);
            this.chkUserIsAdmin.TabIndex = 2;
            this.chkUserIsAdmin.Text = "Administrador";
            this.chkUserIsAdmin.UseVisualStyleBackColor = true;
            // 
            // cmbUserEmpresa
            // 
            this.cmbUserEmpresa.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUserEmpresa.FormattingEnabled = true;
            this.cmbUserEmpresa.Location = new System.Drawing.Point(24, 92);
            this.cmbUserEmpresa.Margin = new System.Windows.Forms.Padding(4);
            this.cmbUserEmpresa.Name = "cmbUserEmpresa";
            this.cmbUserEmpresa.Size = new System.Drawing.Size(452, 24);
            this.cmbUserEmpresa.TabIndex = 1;
            // 
            // lblUserEmpresa
            // 
            this.lblUserEmpresa.AutoSize = true;
            this.lblUserEmpresa.Location = new System.Drawing.Point(20, 74);
            this.lblUserEmpresa.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUserEmpresa.Name = "lblUserEmpresa";
            this.lblUserEmpresa.Size = new System.Drawing.Size(65, 16);
            this.lblUserEmpresa.TabIndex = 2;
            this.lblUserEmpresa.Text = "Empresa:";
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(24, 43);
            this.txtUserName.Margin = new System.Windows.Forms.Padding(4);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(452, 22);
            this.txtUserName.TabIndex = 0;
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(20, 25);
            this.lblUserName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(47, 16);
            this.lblUserName.TabIndex = 0;
            this.lblUserName.Text = "Nome:";
            // 
            // tabPageEmpresas
            // 
            this.tabPageEmpresas.Controls.Add(this.btnEmpresaClearFields);
            this.tabPageEmpresas.Controls.Add(this.dgvEmpresas);
            this.tabPageEmpresas.Controls.Add(this.btnEmpresaDelete);
            this.tabPageEmpresas.Controls.Add(this.btnEmpresaSave);
            this.tabPageEmpresas.Controls.Add(this.btnEmpresaNew);
            this.tabPageEmpresas.Controls.Add(this.grpEmpresaImage);
            this.tabPageEmpresas.Controls.Add(this.grpEmpresaDetails);
            this.tabPageEmpresas.Location = new System.Drawing.Point(4, 25);
            this.tabPageEmpresas.Margin = new System.Windows.Forms.Padding(4);
            this.tabPageEmpresas.Name = "tabPageEmpresas";
            this.tabPageEmpresas.Padding = new System.Windows.Forms.Padding(4);
            this.tabPageEmpresas.Size = new System.Drawing.Size(1381, 612);
            this.tabPageEmpresas.TabIndex = 1;
            this.tabPageEmpresas.Text = "Empresas";
            this.tabPageEmpresas.UseVisualStyleBackColor = true;
            // 
            // btnEmpresaClearFields
            // 
            this.btnEmpresaClearFields.Location = new System.Drawing.Point(867, 271);
            this.btnEmpresaClearFields.Margin = new System.Windows.Forms.Padding(4);
            this.btnEmpresaClearFields.Name = "btnEmpresaClearFields";
            this.btnEmpresaClearFields.Size = new System.Drawing.Size(100, 28);
            this.btnEmpresaClearFields.TabIndex = 8;
            this.btnEmpresaClearFields.Text = "Limpar";
            this.btnEmpresaClearFields.UseVisualStyleBackColor = true;
            // 
            // dgvEmpresas
            // 
            this.dgvEmpresas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvEmpresas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEmpresas.Location = new System.Drawing.Point(21, 332);
            this.dgvEmpresas.Margin = new System.Windows.Forms.Padding(4);
            this.dgvEmpresas.Name = "dgvEmpresas";
            this.dgvEmpresas.RowHeadersWidth = 51;
            this.dgvEmpresas.Size = new System.Drawing.Size(1336, 256);
            this.dgvEmpresas.TabIndex = 9;
            // 
            // btnEmpresaDelete
            // 
            this.btnEmpresaDelete.Location = new System.Drawing.Point(747, 271);
            this.btnEmpresaDelete.Margin = new System.Windows.Forms.Padding(4);
            this.btnEmpresaDelete.Name = "btnEmpresaDelete";
            this.btnEmpresaDelete.Size = new System.Drawing.Size(100, 28);
            this.btnEmpresaDelete.TabIndex = 7;
            this.btnEmpresaDelete.Text = "Excluir";
            this.btnEmpresaDelete.UseVisualStyleBackColor = true;
            // 
            // btnEmpresaSave
            // 
            this.btnEmpresaSave.Location = new System.Drawing.Point(627, 271);
            this.btnEmpresaSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnEmpresaSave.Name = "btnEmpresaSave";
            this.btnEmpresaSave.Size = new System.Drawing.Size(100, 28);
            this.btnEmpresaSave.TabIndex = 6;
            this.btnEmpresaSave.Text = "Salvar";
            this.btnEmpresaSave.UseVisualStyleBackColor = true;
            // 
            // btnEmpresaNew
            // 
            this.btnEmpresaNew.Location = new System.Drawing.Point(507, 271);
            this.btnEmpresaNew.Margin = new System.Windows.Forms.Padding(4);
            this.btnEmpresaNew.Name = "btnEmpresaNew";
            this.btnEmpresaNew.Size = new System.Drawing.Size(100, 28);
            this.btnEmpresaNew.TabIndex = 5;
            this.btnEmpresaNew.Text = "Novo";
            this.btnEmpresaNew.UseVisualStyleBackColor = true;
            // 
            // grpEmpresaImage
            // 
            this.grpEmpresaImage.Controls.Add(this.btnEmpresaClearImage);
            this.grpEmpresaImage.Controls.Add(this.btnEmpresaLoadImage);
            this.grpEmpresaImage.Controls.Add(this.picEmpresaImage);
            this.grpEmpresaImage.Location = new System.Drawing.Point(507, 18);
            this.grpEmpresaImage.Margin = new System.Windows.Forms.Padding(4);
            this.grpEmpresaImage.Name = "grpEmpresaImage";
            this.grpEmpresaImage.Padding = new System.Windows.Forms.Padding(4);
            this.grpEmpresaImage.Size = new System.Drawing.Size(267, 234);
            this.grpEmpresaImage.TabIndex = 1;
            this.grpEmpresaImage.TabStop = false;
            this.grpEmpresaImage.Text = "Logomarca";
            // 
            // btnEmpresaClearImage
            // 
            this.btnEmpresaClearImage.Location = new System.Drawing.Point(133, 191);
            this.btnEmpresaClearImage.Margin = new System.Windows.Forms.Padding(4);
            this.btnEmpresaClearImage.Name = "btnEmpresaClearImage";
            this.btnEmpresaClearImage.Size = new System.Drawing.Size(113, 28);
            this.btnEmpresaClearImage.TabIndex = 1;
            this.btnEmpresaClearImage.Text = "Remover";
            this.btnEmpresaClearImage.UseVisualStyleBackColor = true;
            // 
            // btnEmpresaLoadImage
            // 
            this.btnEmpresaLoadImage.Location = new System.Drawing.Point(20, 191);
            this.btnEmpresaLoadImage.Margin = new System.Windows.Forms.Padding(4);
            this.btnEmpresaLoadImage.Name = "btnEmpresaLoadImage";
            this.btnEmpresaLoadImage.Size = new System.Drawing.Size(107, 28);
            this.btnEmpresaLoadImage.TabIndex = 0;
            this.btnEmpresaLoadImage.Text = "Carregar";
            this.btnEmpresaLoadImage.UseVisualStyleBackColor = true;
            // 
            // picEmpresaImage
            // 
            this.picEmpresaImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picEmpresaImage.Location = new System.Drawing.Point(20, 25);
            this.picEmpresaImage.Margin = new System.Windows.Forms.Padding(4);
            this.picEmpresaImage.Name = "picEmpresaImage";
            this.picEmpresaImage.Size = new System.Drawing.Size(226, 153);
            this.picEmpresaImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picEmpresaImage.TabIndex = 0;
            this.picEmpresaImage.TabStop = false;
            // 
            // grpEmpresaDetails
            // 
            this.grpEmpresaDetails.Controls.Add(this.txtEmpresaTelefone);
            this.grpEmpresaDetails.Controls.Add(this.lblEmpresaTelefone);
            this.grpEmpresaDetails.Controls.Add(this.txtEmpresaEmail);
            this.grpEmpresaDetails.Controls.Add(this.lblEmpresaEmail);
            this.grpEmpresaDetails.Controls.Add(this.txtEmpresaEndereco);
            this.grpEmpresaDetails.Controls.Add(this.lblEmpresaEndereco);
            this.grpEmpresaDetails.Controls.Add(this.txtEmpresaCNPJ);
            this.grpEmpresaDetails.Controls.Add(this.lblEmpresaCNPJ);
            this.grpEmpresaDetails.Controls.Add(this.txtEmpresaName);
            this.grpEmpresaDetails.Controls.Add(this.lblEmpresaName);
            this.grpEmpresaDetails.Location = new System.Drawing.Point(21, 18);
            this.grpEmpresaDetails.Margin = new System.Windows.Forms.Padding(4);
            this.grpEmpresaDetails.Name = "grpEmpresaDetails";
            this.grpEmpresaDetails.Padding = new System.Windows.Forms.Padding(4);
            this.grpEmpresaDetails.Size = new System.Drawing.Size(467, 283);
            this.grpEmpresaDetails.TabIndex = 0;
            this.grpEmpresaDetails.TabStop = false;
            this.grpEmpresaDetails.Text = "Detalhes da Empresa";
            // 
            // txtEmpresaTelefone
            // 
            this.txtEmpresaTelefone.Location = new System.Drawing.Point(24, 240);
            this.txtEmpresaTelefone.Margin = new System.Windows.Forms.Padding(4);
            this.txtEmpresaTelefone.Name = "txtEmpresaTelefone";
            this.txtEmpresaTelefone.Size = new System.Drawing.Size(412, 22);
            this.txtEmpresaTelefone.TabIndex = 4;
            // 
            // lblEmpresaTelefone
            // 
            this.lblEmpresaTelefone.AutoSize = true;
            this.lblEmpresaTelefone.Location = new System.Drawing.Point(20, 222);
            this.lblEmpresaTelefone.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblEmpresaTelefone.Name = "lblEmpresaTelefone";
            this.lblEmpresaTelefone.Size = new System.Drawing.Size(64, 16);
            this.lblEmpresaTelefone.TabIndex = 8;
            this.lblEmpresaTelefone.Text = "Telefone:";
            // 
            // txtEmpresaEmail
            // 
            this.txtEmpresaEmail.Location = new System.Drawing.Point(24, 191);
            this.txtEmpresaEmail.Margin = new System.Windows.Forms.Padding(4);
            this.txtEmpresaEmail.Name = "txtEmpresaEmail";
            this.txtEmpresaEmail.Size = new System.Drawing.Size(412, 22);
            this.txtEmpresaEmail.TabIndex = 3;
            // 
            // lblEmpresaEmail
            // 
            this.lblEmpresaEmail.AutoSize = true;
            this.lblEmpresaEmail.Location = new System.Drawing.Point(20, 172);
            this.lblEmpresaEmail.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblEmpresaEmail.Name = "lblEmpresaEmail";
            this.lblEmpresaEmail.Size = new System.Drawing.Size(48, 16);
            this.lblEmpresaEmail.TabIndex = 6;
            this.lblEmpresaEmail.Text = "E-mail:";
            // 
            // txtEmpresaEndereco
            // 
            this.txtEmpresaEndereco.Location = new System.Drawing.Point(24, 142);
            this.txtEmpresaEndereco.Margin = new System.Windows.Forms.Padding(4);
            this.txtEmpresaEndereco.Name = "txtEmpresaEndereco";
            this.txtEmpresaEndereco.Size = new System.Drawing.Size(412, 22);
            this.txtEmpresaEndereco.TabIndex = 2;
            // 
            // lblEmpresaEndereco
            // 
            this.lblEmpresaEndereco.AutoSize = true;
            this.lblEmpresaEndereco.Location = new System.Drawing.Point(20, 123);
            this.lblEmpresaEndereco.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblEmpresaEndereco.Name = "lblEmpresaEndereco";
            this.lblEmpresaEndereco.Size = new System.Drawing.Size(69, 16);
            this.lblEmpresaEndereco.TabIndex = 4;
            this.lblEmpresaEndereco.Text = "Endereço:";
            // 
            // txtEmpresaCNPJ
            // 
            this.txtEmpresaCNPJ.Location = new System.Drawing.Point(24, 92);
            this.txtEmpresaCNPJ.Margin = new System.Windows.Forms.Padding(4);
            this.txtEmpresaCNPJ.Name = "txtEmpresaCNPJ";
            this.txtEmpresaCNPJ.Size = new System.Drawing.Size(412, 22);
            this.txtEmpresaCNPJ.TabIndex = 1;
            // 
            // lblEmpresaCNPJ
            // 
            this.lblEmpresaCNPJ.AutoSize = true;
            this.lblEmpresaCNPJ.Location = new System.Drawing.Point(20, 74);
            this.lblEmpresaCNPJ.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblEmpresaCNPJ.Name = "lblEmpresaCNPJ";
            this.lblEmpresaCNPJ.Size = new System.Drawing.Size(45, 16);
            this.lblEmpresaCNPJ.TabIndex = 2;
            this.lblEmpresaCNPJ.Text = "CNPJ:";
            // 
            // txtEmpresaName
            // 
            this.txtEmpresaName.Location = new System.Drawing.Point(24, 43);
            this.txtEmpresaName.Margin = new System.Windows.Forms.Padding(4);
            this.txtEmpresaName.Name = "txtEmpresaName";
            this.txtEmpresaName.Size = new System.Drawing.Size(412, 22);
            this.txtEmpresaName.TabIndex = 0;
            // 
            // lblEmpresaName
            // 
            this.lblEmpresaName.AutoSize = true;
            this.lblEmpresaName.Location = new System.Drawing.Point(20, 25);
            this.lblEmpresaName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblEmpresaName.Name = "lblEmpresaName";
            this.lblEmpresaName.Size = new System.Drawing.Size(47, 16);
            this.lblEmpresaName.TabIndex = 0;
            this.lblEmpresaName.Text = "Nome:";
            // 
            // tabPageClientes
            // 
            this.tabPageClientes.Controls.Add(this.btnClienteClearFields);
            this.tabPageClientes.Controls.Add(this.dgvClientes);
            this.tabPageClientes.Controls.Add(this.btnClienteDelete);
            this.tabPageClientes.Controls.Add(this.btnClienteSave);
            this.tabPageClientes.Controls.Add(this.btnClienteNew);
            this.tabPageClientes.Controls.Add(this.grpClienteImage);
            this.tabPageClientes.Controls.Add(this.grpClienteDetails);
            this.tabPageClientes.Location = new System.Drawing.Point(4, 25);
            this.tabPageClientes.Margin = new System.Windows.Forms.Padding(4);
            this.tabPageClientes.Name = "tabPageClientes";
            this.tabPageClientes.Padding = new System.Windows.Forms.Padding(4);
            this.tabPageClientes.Size = new System.Drawing.Size(1381, 612);
            this.tabPageClientes.TabIndex = 2;
            this.tabPageClientes.Text = "Clientes";
            this.tabPageClientes.UseVisualStyleBackColor = true;
            // 
            // btnClienteClearFields
            // 
            this.btnClienteClearFields.Location = new System.Drawing.Point(867, 326);
            this.btnClienteClearFields.Margin = new System.Windows.Forms.Padding(4);
            this.btnClienteClearFields.Name = "btnClienteClearFields";
            this.btnClienteClearFields.Size = new System.Drawing.Size(100, 28);
            this.btnClienteClearFields.TabIndex = 10;
            this.btnClienteClearFields.Text = "Limpar";
            this.btnClienteClearFields.UseVisualStyleBackColor = true;
            // 
            // dgvClientes
            // 
            this.dgvClientes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvClientes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvClientes.Location = new System.Drawing.Point(21, 369);
            this.dgvClientes.Margin = new System.Windows.Forms.Padding(4);
            this.dgvClientes.Name = "dgvClientes";
            this.dgvClientes.RowHeadersWidth = 51;
            this.dgvClientes.Size = new System.Drawing.Size(1336, 219);
            this.dgvClientes.TabIndex = 11;
            // 
            // btnClienteDelete
            // 
            this.btnClienteDelete.Location = new System.Drawing.Point(747, 326);
            this.btnClienteDelete.Margin = new System.Windows.Forms.Padding(4);
            this.btnClienteDelete.Name = "btnClienteDelete";
            this.btnClienteDelete.Size = new System.Drawing.Size(100, 28);
            this.btnClienteDelete.TabIndex = 9;
            this.btnClienteDelete.Text = "Excluir";
            this.btnClienteDelete.UseVisualStyleBackColor = true;
            // 
            // btnClienteSave
            // 
            this.btnClienteSave.Location = new System.Drawing.Point(627, 326);
            this.btnClienteSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnClienteSave.Name = "btnClienteSave";
            this.btnClienteSave.Size = new System.Drawing.Size(100, 28);
            this.btnClienteSave.TabIndex = 8;
            this.btnClienteSave.Text = "Salvar";
            this.btnClienteSave.UseVisualStyleBackColor = true;
            // 
            // btnClienteNew
            // 
            this.btnClienteNew.Location = new System.Drawing.Point(507, 326);
            this.btnClienteNew.Margin = new System.Windows.Forms.Padding(4);
            this.btnClienteNew.Name = "btnClienteNew";
            this.btnClienteNew.Size = new System.Drawing.Size(100, 28);
            this.btnClienteNew.TabIndex = 7;
            this.btnClienteNew.Text = "Novo";
            this.btnClienteNew.UseVisualStyleBackColor = true;
            // 
            // grpClienteImage
            // 
            this.grpClienteImage.Controls.Add(this.btnClienteClearImage);
            this.grpClienteImage.Controls.Add(this.btnClienteLoadImage);
            this.grpClienteImage.Controls.Add(this.picClienteImage);
            this.grpClienteImage.Location = new System.Drawing.Point(507, 18);
            this.grpClienteImage.Margin = new System.Windows.Forms.Padding(4);
            this.grpClienteImage.Name = "grpClienteImage";
            this.grpClienteImage.Padding = new System.Windows.Forms.Padding(4);
            this.grpClienteImage.Size = new System.Drawing.Size(267, 234);
            this.grpClienteImage.TabIndex = 1;
            this.grpClienteImage.TabStop = false;
            this.grpClienteImage.Text = "Foto do Cliente";
            // 
            // btnClienteClearImage
            // 
            this.btnClienteClearImage.Location = new System.Drawing.Point(133, 191);
            this.btnClienteClearImage.Margin = new System.Windows.Forms.Padding(4);
            this.btnClienteClearImage.Name = "btnClienteClearImage";
            this.btnClienteClearImage.Size = new System.Drawing.Size(113, 28);
            this.btnClienteClearImage.TabIndex = 1;
            this.btnClienteClearImage.Text = "Remover";
            this.btnClienteClearImage.UseVisualStyleBackColor = true;
            // 
            // btnClienteLoadImage
            // 
            this.btnClienteLoadImage.Location = new System.Drawing.Point(20, 191);
            this.btnClienteLoadImage.Margin = new System.Windows.Forms.Padding(4);
            this.btnClienteLoadImage.Name = "btnClienteLoadImage";
            this.btnClienteLoadImage.Size = new System.Drawing.Size(107, 28);
            this.btnClienteLoadImage.TabIndex = 0;
            this.btnClienteLoadImage.Text = "Carregar";
            this.btnClienteLoadImage.UseVisualStyleBackColor = true;
            // 
            // picClienteImage
            // 
            this.picClienteImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picClienteImage.Location = new System.Drawing.Point(20, 25);
            this.picClienteImage.Margin = new System.Windows.Forms.Padding(4);
            this.picClienteImage.Name = "picClienteImage";
            this.picClienteImage.Size = new System.Drawing.Size(226, 153);
            this.picClienteImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picClienteImage.TabIndex = 0;
            this.picClienteImage.TabStop = false;
            // 
            // grpClienteDetails
            // 
            this.grpClienteDetails.Controls.Add(this.cmbClienteEmpresa);
            this.grpClienteDetails.Controls.Add(this.lblClienteEmpresa);
            this.grpClienteDetails.Controls.Add(this.txtClienteTelefone);
            this.grpClienteDetails.Controls.Add(this.lblClienteTelefone);
            this.grpClienteDetails.Controls.Add(this.txtClienteEmail);
            this.grpClienteDetails.Controls.Add(this.lblClienteEmail);
            this.grpClienteDetails.Controls.Add(this.txtClienteEndereco);
            this.grpClienteDetails.Controls.Add(this.lblClienteEndereco);
            this.grpClienteDetails.Controls.Add(this.txtClienteCPF);
            this.grpClienteDetails.Controls.Add(this.lblClienteCPF);
            this.grpClienteDetails.Controls.Add(this.txtClienteCNPJ);
            this.grpClienteDetails.Controls.Add(this.lblClienteCNPJ);
            this.grpClienteDetails.Controls.Add(this.txtClienteName);
            this.grpClienteDetails.Controls.Add(this.lblClienteName);
            this.grpClienteDetails.Location = new System.Drawing.Point(21, 18);
            this.grpClienteDetails.Margin = new System.Windows.Forms.Padding(4);
            this.grpClienteDetails.Name = "grpClienteDetails";
            this.grpClienteDetails.Padding = new System.Windows.Forms.Padding(4);
            this.grpClienteDetails.Size = new System.Drawing.Size(467, 338);
            this.grpClienteDetails.TabIndex = 0;
            this.grpClienteDetails.TabStop = false;
            this.grpClienteDetails.Text = "Detalhes do Cliente";
            // 
            // cmbClienteEmpresa
            // 
            this.cmbClienteEmpresa.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbClienteEmpresa.FormattingEnabled = true;
            this.cmbClienteEmpresa.Location = new System.Drawing.Point(24, 289);
            this.cmbClienteEmpresa.Margin = new System.Windows.Forms.Padding(4);
            this.cmbClienteEmpresa.Name = "cmbClienteEmpresa";
            this.cmbClienteEmpresa.Size = new System.Drawing.Size(412, 24);
            this.cmbClienteEmpresa.TabIndex = 6;
            // 
            // lblClienteEmpresa
            // 
            this.lblClienteEmpresa.AutoSize = true;
            this.lblClienteEmpresa.Location = new System.Drawing.Point(20, 271);
            this.lblClienteEmpresa.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblClienteEmpresa.Name = "lblClienteEmpresa";
            this.lblClienteEmpresa.Size = new System.Drawing.Size(128, 16);
            this.lblClienteEmpresa.TabIndex = 12;
            this.lblClienteEmpresa.Text = "Empresa Vinculada:";
            // 
            // txtClienteTelefone
            // 
            this.txtClienteTelefone.Location = new System.Drawing.Point(24, 240);
            this.txtClienteTelefone.Margin = new System.Windows.Forms.Padding(4);
            this.txtClienteTelefone.Name = "txtClienteTelefone";
            this.txtClienteTelefone.Size = new System.Drawing.Size(412, 22);
            this.txtClienteTelefone.TabIndex = 5;
            // 
            // lblClienteTelefone
            // 
            this.lblClienteTelefone.AutoSize = true;
            this.lblClienteTelefone.Location = new System.Drawing.Point(20, 222);
            this.lblClienteTelefone.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblClienteTelefone.Name = "lblClienteTelefone";
            this.lblClienteTelefone.Size = new System.Drawing.Size(64, 16);
            this.lblClienteTelefone.TabIndex = 10;
            this.lblClienteTelefone.Text = "Telefone:";
            // 
            // txtClienteEmail
            // 
            this.txtClienteEmail.Location = new System.Drawing.Point(24, 191);
            this.txtClienteEmail.Margin = new System.Windows.Forms.Padding(4);
            this.txtClienteEmail.Name = "txtClienteEmail";
            this.txtClienteEmail.Size = new System.Drawing.Size(412, 22);
            this.txtClienteEmail.TabIndex = 4;
            // 
            // lblClienteEmail
            // 
            this.lblClienteEmail.AutoSize = true;
            this.lblClienteEmail.Location = new System.Drawing.Point(20, 172);
            this.lblClienteEmail.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblClienteEmail.Name = "lblClienteEmail";
            this.lblClienteEmail.Size = new System.Drawing.Size(48, 16);
            this.lblClienteEmail.TabIndex = 8;
            this.lblClienteEmail.Text = "E-mail:";
            // 
            // txtClienteEndereco
            // 
            this.txtClienteEndereco.Location = new System.Drawing.Point(24, 142);
            this.txtClienteEndereco.Margin = new System.Windows.Forms.Padding(4);
            this.txtClienteEndereco.Name = "txtClienteEndereco";
            this.txtClienteEndereco.Size = new System.Drawing.Size(412, 22);
            this.txtClienteEndereco.TabIndex = 3;
            // 
            // lblClienteEndereco
            // 
            this.lblClienteEndereco.AutoSize = true;
            this.lblClienteEndereco.Location = new System.Drawing.Point(20, 123);
            this.lblClienteEndereco.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblClienteEndereco.Name = "lblClienteEndereco";
            this.lblClienteEndereco.Size = new System.Drawing.Size(69, 16);
            this.lblClienteEndereco.TabIndex = 6;
            this.lblClienteEndereco.Text = "Endereço:";
            // 
            // txtClienteCPF
            // 
            this.txtClienteCPF.Location = new System.Drawing.Point(240, 92);
            this.txtClienteCPF.Margin = new System.Windows.Forms.Padding(4);
            this.txtClienteCPF.Name = "txtClienteCPF";
            this.txtClienteCPF.Size = new System.Drawing.Size(196, 22);
            this.txtClienteCPF.TabIndex = 2;
            // 
            // lblClienteCPF
            // 
            this.lblClienteCPF.AutoSize = true;
            this.lblClienteCPF.Location = new System.Drawing.Point(236, 74);
            this.lblClienteCPF.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblClienteCPF.Name = "lblClienteCPF";
            this.lblClienteCPF.Size = new System.Drawing.Size(36, 16);
            this.lblClienteCPF.TabIndex = 4;
            this.lblClienteCPF.Text = "CPF:";
            // 
            // txtClienteCNPJ
            // 
            this.txtClienteCNPJ.Location = new System.Drawing.Point(24, 92);
            this.txtClienteCNPJ.Margin = new System.Windows.Forms.Padding(4);
            this.txtClienteCNPJ.Name = "txtClienteCNPJ";
            this.txtClienteCNPJ.Size = new System.Drawing.Size(196, 22);
            this.txtClienteCNPJ.TabIndex = 1;
            // 
            // lblClienteCNPJ
            // 
            this.lblClienteCNPJ.AutoSize = true;
            this.lblClienteCNPJ.Location = new System.Drawing.Point(20, 74);
            this.lblClienteCNPJ.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblClienteCNPJ.Name = "lblClienteCNPJ";
            this.lblClienteCNPJ.Size = new System.Drawing.Size(45, 16);
            this.lblClienteCNPJ.TabIndex = 2;
            this.lblClienteCNPJ.Text = "CNPJ:";
            // 
            // txtClienteName
            // 
            this.txtClienteName.Location = new System.Drawing.Point(24, 43);
            this.txtClienteName.Margin = new System.Windows.Forms.Padding(4);
            this.txtClienteName.Name = "txtClienteName";
            this.txtClienteName.Size = new System.Drawing.Size(412, 22);
            this.txtClienteName.TabIndex = 0;
            // 
            // lblClienteName
            // 
            this.lblClienteName.AutoSize = true;
            this.lblClienteName.Location = new System.Drawing.Point(20, 25);
            this.lblClienteName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblClienteName.Name = "lblClienteName";
            this.lblClienteName.Size = new System.Drawing.Size(47, 16);
            this.lblClienteName.TabIndex = 0;
            this.lblClienteName.Text = "Nome:";
            // 
            // openFileDialogImage
            // 
            this.openFileDialogImage.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";
            // 
            // btnSair
            // 
            this.btnSair.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSair.Location = new System.Drawing.Point(1261, 652);
            this.btnSair.Margin = new System.Windows.Forms.Padding(4);
            this.btnSair.Name = "btnSair";
            this.btnSair.Size = new System.Drawing.Size(140, 43);
            this.btnSair.TabIndex = 7;
            this.btnSair.Text = "Sair";
            this.btnSair.UseVisualStyleBackColor = true;
            this.btnSair.Click += new System.EventHandler(this.BtnSair_Click);
            // 
            // ContasW
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1421, 708);
            this.Controls.Add(this.btnSair);
            this.Controls.Add(this.tabControlMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ContasW";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gerenciamento Contas";
            this.tabControlMain.ResumeLayout(false);
            this.tabPageUsuarios.ResumeLayout(false);
            this.grpUserAccessLevel.ResumeLayout(false);
            this.grpUserAccessLevel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).EndInit();
            this.grpUserDetails.ResumeLayout(false);
            this.grpUserDetails.PerformLayout();
            this.tabPageEmpresas.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvEmpresas)).EndInit();
            this.grpEmpresaImage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picEmpresaImage)).EndInit();
            this.grpEmpresaDetails.ResumeLayout(false);
            this.grpEmpresaDetails.PerformLayout();
            this.tabPageClientes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientes)).EndInit();
            this.grpClienteImage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picClienteImage)).EndInit();
            this.grpClienteDetails.ResumeLayout(false);
            this.grpClienteDetails.PerformLayout();
            this.ResumeLayout(false);

        }

            #endregion

            private System.Windows.Forms.TabControl tabControlMain;
            private System.Windows.Forms.TabPage tabPageUsuarios;
            private System.Windows.Forms.TabPage tabPageEmpresas;
            private System.Windows.Forms.TabPage tabPageClientes;
            private System.Windows.Forms.GroupBox grpUserDetails;
            private System.Windows.Forms.Label lblUserName;
            private System.Windows.Forms.TextBox txtUserName;
            private System.Windows.Forms.Label lblUserEmpresa;
            private System.Windows.Forms.ComboBox cmbUserEmpresa;
            private System.Windows.Forms.CheckBox chkUserIsAdmin;
            private System.Windows.Forms.Button btnUserNew;
            private System.Windows.Forms.Button btnUserSave;
            private System.Windows.Forms.Button btnUserDelete;
            private System.Windows.Forms.DataGridView dgvUsers;
            private System.Windows.Forms.GroupBox grpEmpresaDetails;
            private System.Windows.Forms.Label lblEmpresaName;
            private System.Windows.Forms.TextBox txtEmpresaName;
            private System.Windows.Forms.Label lblEmpresaCNPJ;
            private System.Windows.Forms.TextBox txtEmpresaCNPJ;
            private System.Windows.Forms.Label lblEmpresaEndereco;
            private System.Windows.Forms.TextBox txtEmpresaEndereco;
            private System.Windows.Forms.Label lblEmpresaEmail;
            private System.Windows.Forms.TextBox txtEmpresaEmail;
            private System.Windows.Forms.Label lblEmpresaTelefone;
            private System.Windows.Forms.TextBox txtEmpresaTelefone;
            private System.Windows.Forms.GroupBox grpEmpresaImage;
            private System.Windows.Forms.PictureBox picEmpresaImage;
            private System.Windows.Forms.Button btnEmpresaLoadImage;
            private System.Windows.Forms.Button btnEmpresaClearImage;
            private System.Windows.Forms.Button btnEmpresaNew;
            private System.Windows.Forms.Button btnEmpresaSave;
            private System.Windows.Forms.Button btnEmpresaDelete;
            private System.Windows.Forms.DataGridView dgvEmpresas;
            private System.Windows.Forms.GroupBox grpClienteDetails;
            private System.Windows.Forms.Label lblClienteName;
            private System.Windows.Forms.TextBox txtClienteName;
            private System.Windows.Forms.Label lblClienteCNPJ;
            private System.Windows.Forms.TextBox txtClienteCNPJ;
            private System.Windows.Forms.Label lblClienteCPF;
            private System.Windows.Forms.TextBox txtClienteCPF;
            private System.Windows.Forms.Label lblClienteEndereco;
            private System.Windows.Forms.TextBox txtClienteEndereco;
            private System.Windows.Forms.Label lblClienteEmail;
            private System.Windows.Forms.TextBox txtClienteEmail;
            private System.Windows.Forms.Label lblClienteTelefone;
            private System.Windows.Forms.TextBox txtClienteTelefone;
            private System.Windows.Forms.Label lblClienteEmpresa;
            private System.Windows.Forms.ComboBox cmbClienteEmpresa;
            private System.Windows.Forms.GroupBox grpClienteImage;
            private System.Windows.Forms.PictureBox picClienteImage;
            private System.Windows.Forms.Button btnClienteLoadImage;
            private System.Windows.Forms.Button btnClienteClearImage;
            private System.Windows.Forms.Button btnClienteNew;
            private System.Windows.Forms.Button btnClienteSave;
            private System.Windows.Forms.Button btnClienteDelete;
            private System.Windows.Forms.DataGridView dgvClientes;
            private System.Windows.Forms.OpenFileDialog openFileDialogImage;
            private System.Windows.Forms.Button btnUserClearFields;
            private System.Windows.Forms.Button btnEmpresaClearFields;
            private System.Windows.Forms.Button btnClienteClearFields;
            private System.Windows.Forms.GroupBox grpUserAccessLevel;
            private System.Windows.Forms.Label lblUserAccessLevel;
        private System.Windows.Forms.Button btnSair;
    }
    
}