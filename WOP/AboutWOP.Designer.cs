using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace WOP
{
  internal partial class AboutWOP
  {
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
      if (disposing && (this.components != null)) {
        this.components.Dispose();
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
      ComponentResourceManager resources = new ComponentResourceManager(typeof(AboutWOP));
      this.tableLayoutPanel = new TableLayoutPanel();
      this.logoPictureBox = new PictureBox();
      this.labelProductName = new Label();
      this.labelVersion = new Label();
      this.labelCopyright = new Label();
      this.labelCompanyName = new Label();
      this.textBoxDescription = new TextBox();
      this.okButton = new Button();
      this.tableLayoutPanel.SuspendLayout();
      ((ISupportInitialize)(this.logoPictureBox)).BeginInit();
      this.SuspendLayout();
      // 
      // tableLayoutPanel
      // 
      this.tableLayoutPanel.ColumnCount = 2;
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
      this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 67F));
      this.tableLayoutPanel.Controls.Add(this.logoPictureBox, 0, 0);
      this.tableLayoutPanel.Controls.Add(this.labelProductName, 1, 0);
      this.tableLayoutPanel.Controls.Add(this.labelVersion, 1, 1);
      this.tableLayoutPanel.Controls.Add(this.labelCopyright, 1, 2);
      this.tableLayoutPanel.Controls.Add(this.labelCompanyName, 1, 3);
      this.tableLayoutPanel.Controls.Add(this.textBoxDescription, 1, 4);
      this.tableLayoutPanel.Controls.Add(this.okButton, 1, 5);
      this.tableLayoutPanel.Dock = DockStyle.Fill;
      this.tableLayoutPanel.Location = new Point(9, 9);
      this.tableLayoutPanel.Name = "tableLayoutPanel";
      this.tableLayoutPanel.RowCount = 6;
      this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
      this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
      this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
      this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
      this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
      this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
      this.tableLayoutPanel.Size = new Size(417, 265);
      this.tableLayoutPanel.TabIndex = 0;
      // 
      // logoPictureBox
      // 
      this.logoPictureBox.Dock = DockStyle.Fill;
      this.logoPictureBox.Image = ((Image)(resources.GetObject("logoPictureBox.Image")));
      this.logoPictureBox.Location = new Point(3, 3);
      this.logoPictureBox.Name = "logoPictureBox";
      this.tableLayoutPanel.SetRowSpan(this.logoPictureBox, 6);
      this.logoPictureBox.Size = new Size(131, 259);
      this.logoPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
      this.logoPictureBox.TabIndex = 12;
      this.logoPictureBox.TabStop = false;
      // 
      // labelProductName
      // 
      this.labelProductName.Dock = DockStyle.Fill;
      this.labelProductName.Location = new Point(143, 0);
      this.labelProductName.Margin = new Padding(6, 0, 3, 0);
      this.labelProductName.MaximumSize = new Size(0, 17);
      this.labelProductName.Name = "labelProductName";
      this.labelProductName.Size = new Size(271, 17);
      this.labelProductName.TabIndex = 19;
      this.labelProductName.Text = "wop";
      this.labelProductName.TextAlign = ContentAlignment.MiddleLeft;
      // 
      // labelVersion
      // 
      this.labelVersion.Dock = DockStyle.Fill;
      this.labelVersion.Location = new Point(143, 26);
      this.labelVersion.Margin = new Padding(6, 0, 3, 0);
      this.labelVersion.MaximumSize = new Size(0, 17);
      this.labelVersion.Name = "labelVersion";
      this.labelVersion.Size = new Size(271, 17);
      this.labelVersion.TabIndex = 0;
      this.labelVersion.Text = "v0.1";
      this.labelVersion.TextAlign = ContentAlignment.MiddleLeft;
      // 
      // labelCopyright
      // 
      this.labelCopyright.Dock = DockStyle.Fill;
      this.labelCopyright.Location = new Point(143, 52);
      this.labelCopyright.Margin = new Padding(6, 0, 3, 0);
      this.labelCopyright.MaximumSize = new Size(0, 17);
      this.labelCopyright.Name = "labelCopyright";
      this.labelCopyright.Size = new Size(271, 17);
      this.labelCopyright.TabIndex = 21;
      this.labelCopyright.Text = "dotob";
      this.labelCopyright.TextAlign = ContentAlignment.MiddleLeft;
      // 
      // labelCompanyName
      // 
      this.labelCompanyName.Dock = DockStyle.Fill;
      this.labelCompanyName.Location = new Point(143, 78);
      this.labelCompanyName.Margin = new Padding(6, 0, 3, 0);
      this.labelCompanyName.MaximumSize = new Size(0, 17);
      this.labelCompanyName.Name = "labelCompanyName";
      this.labelCompanyName.Size = new Size(271, 17);
      this.labelCompanyName.TabIndex = 22;
      this.labelCompanyName.Text = "build 4 lichtographie and sportograph";
      this.labelCompanyName.TextAlign = ContentAlignment.MiddleLeft;
      // 
      // textBoxDescription
      // 
      this.textBoxDescription.Dock = DockStyle.Fill;
      this.textBoxDescription.Location = new Point(143, 107);
      this.textBoxDescription.Margin = new Padding(6, 3, 3, 3);
      this.textBoxDescription.Multiline = true;
      this.textBoxDescription.Name = "textBoxDescription";
      this.textBoxDescription.ReadOnly = true;
      this.textBoxDescription.ScrollBars = ScrollBars.Both;
      this.textBoxDescription.Size = new Size(271, 126);
      this.textBoxDescription.TabIndex = 23;
      this.textBoxDescription.TabStop = false;
      this.textBoxDescription.Text = "working on pictures is a workflow tool for processing images/pictures";
      // 
      // okButton
      // 
      this.okButton.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Right)));
      this.okButton.DialogResult = DialogResult.Cancel;
      this.okButton.Location = new Point(339, 239);
      this.okButton.Name = "okButton";
      this.okButton.Size = new Size(75, 23);
      this.okButton.TabIndex = 24;
      this.okButton.Text = "&OK";
      // 
      // AboutWOP
      // 
      this.AcceptButton = this.okButton;
      this.AutoScaleDimensions = new SizeF(6F, 13F);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(435, 283);
      this.Controls.Add(this.tableLayoutPanel);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "AboutWOP";
      this.Padding = new Padding(9);
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "über wop...";
      this.tableLayoutPanel.ResumeLayout(false);
      this.tableLayoutPanel.PerformLayout();
      ((ISupportInitialize)(this.logoPictureBox)).EndInit();
      this.ResumeLayout(false);
    }

    #endregion
  }
}