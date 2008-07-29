using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace WOP {
    internal partial class AboutWOP {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private IContainer components = null;

        private Label labelCompanyName;
        private Label labelCopyright;

        private Label labelProductName;
        private Label labelVersion;
        private PictureBox logoPictureBox;
        private Button okButton;
        private TableLayoutPanel tableLayoutPanel;
        private TextBox textBoxDescription;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof (AboutWOP));
            tableLayoutPanel = new TableLayoutPanel();
            logoPictureBox = new PictureBox();
            labelProductName = new Label();
            labelVersion = new Label();
            labelCopyright = new Label();
            labelCompanyName = new Label();
            textBoxDescription = new TextBox();
            okButton = new Button();
            tableLayoutPanel.SuspendLayout();
            ((ISupportInitialize) (logoPictureBox)).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 67F));
            tableLayoutPanel.Controls.Add(logoPictureBox, 0, 0);
            tableLayoutPanel.Controls.Add(labelProductName, 1, 0);
            tableLayoutPanel.Controls.Add(labelVersion, 1, 1);
            tableLayoutPanel.Controls.Add(labelCopyright, 1, 2);
            tableLayoutPanel.Controls.Add(labelCompanyName, 1, 3);
            tableLayoutPanel.Controls.Add(textBoxDescription, 1, 4);
            tableLayoutPanel.Controls.Add(okButton, 1, 5);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(9, 9);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 6;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel.Size = new Size(417, 265);
            tableLayoutPanel.TabIndex = 0;
            // 
            // logoPictureBox
            // 
            logoPictureBox.Dock = DockStyle.Fill;
            logoPictureBox.Image = ((Image) (resources.GetObject("logoPictureBox.Image")));
            logoPictureBox.Location = new Point(3, 3);
            logoPictureBox.Name = "logoPictureBox";
            tableLayoutPanel.SetRowSpan(logoPictureBox, 6);
            logoPictureBox.Size = new Size(131, 259);
            logoPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            logoPictureBox.TabIndex = 12;
            logoPictureBox.TabStop = false;
            // 
            // labelProductName
            // 
            labelProductName.Dock = DockStyle.Fill;
            labelProductName.Location = new Point(143, 0);
            labelProductName.Margin = new Padding(6, 0, 3, 0);
            labelProductName.MaximumSize = new Size(0, 17);
            labelProductName.Name = "labelProductName";
            labelProductName.Size = new Size(271, 17);
            labelProductName.TabIndex = 19;
            labelProductName.Text = "wop";
            labelProductName.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // labelVersion
            // 
            labelVersion.Dock = DockStyle.Fill;
            labelVersion.Location = new Point(143, 26);
            labelVersion.Margin = new Padding(6, 0, 3, 0);
            labelVersion.MaximumSize = new Size(0, 17);
            labelVersion.Name = "labelVersion";
            labelVersion.Size = new Size(271, 17);
            labelVersion.TabIndex = 0;
            labelVersion.Text = "v0.1";
            labelVersion.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // labelCopyright
            // 
            labelCopyright.Dock = DockStyle.Fill;
            labelCopyright.Location = new Point(143, 52);
            labelCopyright.Margin = new Padding(6, 0, 3, 0);
            labelCopyright.MaximumSize = new Size(0, 17);
            labelCopyright.Name = "labelCopyright";
            labelCopyright.Size = new Size(271, 17);
            labelCopyright.TabIndex = 21;
            labelCopyright.Text = "dotob";
            labelCopyright.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // labelCompanyName
            // 
            labelCompanyName.Dock = DockStyle.Fill;
            labelCompanyName.Location = new Point(143, 78);
            labelCompanyName.Margin = new Padding(6, 0, 3, 0);
            labelCompanyName.MaximumSize = new Size(0, 17);
            labelCompanyName.Name = "labelCompanyName";
            labelCompanyName.Size = new Size(271, 17);
            labelCompanyName.TabIndex = 22;
            labelCompanyName.Text = "build 4 lichtographie and sportograph";
            labelCompanyName.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // textBoxDescription
            // 
            textBoxDescription.Dock = DockStyle.Fill;
            textBoxDescription.Location = new Point(143, 107);
            textBoxDescription.Margin = new Padding(6, 3, 3, 3);
            textBoxDescription.Multiline = true;
            textBoxDescription.Name = "textBoxDescription";
            textBoxDescription.ReadOnly = true;
            textBoxDescription.ScrollBars = ScrollBars.Both;
            textBoxDescription.Size = new Size(271, 126);
            textBoxDescription.TabIndex = 23;
            textBoxDescription.TabStop = false;
            textBoxDescription.Text = "working on pictures is a workflow tool for processing images/pictures";
            // 
            // okButton
            // 
            okButton.Anchor = ((AnchorStyles) ((AnchorStyles.Bottom | AnchorStyles.Right)));
            okButton.DialogResult = DialogResult.Cancel;
            okButton.Location = new Point(339, 239);
            okButton.Name = "okButton";
            okButton.Size = new Size(75, 23);
            okButton.TabIndex = 24;
            okButton.Text = "&OK";
            // 
            // AboutWOP
            // 
            AcceptButton = okButton;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(435, 283);
            Controls.Add(tableLayoutPanel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutWOP";
            Padding = new Padding(9);
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "über wop...";
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            ((ISupportInitialize) (logoPictureBox)).EndInit();
            ResumeLayout(false);
        }

        #endregion
    }
}