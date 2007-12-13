using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Zeus;
using Zeus.Projects;

namespace MyGeneration
{
	/// <summary>
	/// Summary description for AddEditModule.
	/// </summary>
	public class FormAddEditModule : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label labelName;
		private System.Windows.Forms.Label labelDescription;
		private System.Windows.Forms.TextBox textBoxName;
		private System.Windows.Forms.TextBox textBoxDescription;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.ErrorProvider errorProviderRequiredFields;

		private ZeusModule _module;
        private bool _isActivated = false;
        private CheckBox checkBox1;
        private Label labelUserData;
        private DataGridView dataGridViewUserData;
        private DataGridViewTextBoxColumn ColumnName;
        private DataGridViewTextBoxColumn ColumnValue;
        private IContainer components;

		public FormAddEditModule()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.labelName = new System.Windows.Forms.Label();
            this.labelDescription = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.errorProviderRequiredFields = new System.Windows.Forms.ErrorProvider(this.components);
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.labelUserData = new System.Windows.Forms.Label();
            this.dataGridViewUserData = new System.Windows.Forms.DataGridView();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderRequiredFields)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUserData)).BeginInit();
            this.SuspendLayout();
            // 
            // labelName
            // 
            this.labelName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelName.Location = new System.Drawing.Point(16, 8);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(303, 23);
            this.labelName.TabIndex = 0;
            this.labelName.Text = "Name:";
            this.labelName.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // labelDescription
            // 
            this.labelDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDescription.Location = new System.Drawing.Point(16, 56);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(303, 23);
            this.labelDescription.TabIndex = 1;
            this.labelDescription.Text = "Description:";
            this.labelDescription.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // textBoxName
            // 
            this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxName.Location = new System.Drawing.Point(16, 32);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(303, 20);
            this.textBoxName.TabIndex = 2;
            this.textBoxName.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxName_Validating);
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.AcceptsReturn = true;
            this.textBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDescription.Location = new System.Drawing.Point(16, 80);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(303, 60);
            this.textBoxDescription.TabIndex = 3;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.CausesValidation = false;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(255, 317);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(175, 317);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // errorProviderRequiredFields
            // 
            this.errorProviderRequiredFields.ContainerControl = this;
            // 
            // checkBox1
            // 
            this.checkBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(19, 294);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(229, 17);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.Text = "Override Saved Data With Default Settings";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // labelUserData
            // 
            this.labelUserData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelUserData.Location = new System.Drawing.Point(12, 143);
            this.labelUserData.Name = "labelUserData";
            this.labelUserData.Size = new System.Drawing.Size(303, 23);
            this.labelUserData.TabIndex = 7;
            this.labelUserData.Text = "User Override Data:";
            this.labelUserData.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // dataGridViewUserData
            // 
            this.dataGridViewUserData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewUserData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewUserData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnName,
            this.ColumnValue});
            this.dataGridViewUserData.Location = new System.Drawing.Point(15, 169);
            this.dataGridViewUserData.Name = "dataGridViewUserData";
            this.dataGridViewUserData.Size = new System.Drawing.Size(304, 119);
            this.dataGridViewUserData.TabIndex = 8;
            // 
            // ColumnName
            // 
            this.ColumnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnName.HeaderText = "Name";
            this.ColumnName.Name = "ColumnName";
            // 
            // ColumnValue
            // 
            this.ColumnValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnValue.HeaderText = "Value";
            this.ColumnValue.Name = "ColumnValue";
            // 
            // FormAddEditModule
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(339, 347);
            this.Controls.Add(this.dataGridViewUserData);
            this.Controls.Add(this.labelUserData);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.textBoxDescription);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.labelDescription);
            this.Controls.Add(this.labelName);
            this.Name = "FormAddEditModule";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add/Edit Project Folder";
            this.Activated += new System.EventHandler(this.FormAddEditModule_Activated);
            this.Load += new System.EventHandler(this.FormAddEditModule_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderRequiredFields)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUserData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		public ZeusModule Module 
		{
			get 
			{
				return _module;
			}
			set 
			{
				_module = value;

				if(_module.Name != null) 
				{
					this.textBoxName.Text = _module.Name;
					this.Text = "Folder: " + _module.Name;
				}
				else 
				{
					this.textBoxName.Text = string.Empty;
                    this.Text = "Folder: [New] ";
				}

				if(_module.Description != null)
					this.textBoxDescription.Text = _module.Description;
				else 
					this.textBoxDescription.Text = string.Empty;


                this.checkBox1.Checked = this.Module.DefaultSettingsOverride;

                foreach (InputItem item in this._module.UserSavedItems)
                {
                    DataGridViewRow r = new DataGridViewRow();
                    r.Cells[0].Value = item.VariableName;
                    r.Cells[1].Value = item.Data;
                    this.dataGridViewUserData.Rows.Add(r);
                }

				this._isActivated = false;
			}
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			CancelEventArgs args = new CancelEventArgs();
			this.textBoxName_Validating(this, args);

			if (!args.Cancel)  
			{
				this.Module.Name = this.textBoxName.Text;
				if (this.textBoxDescription.Text.Length > 0)
				{
                    this.Module.Description = this.textBoxDescription.Text;
                }

                this.Module.DefaultSettingsOverride = this.checkBox1.Checked;
                this.Module.UserSavedItems.Clear();
                foreach (DataGridViewRow r in dataGridViewUserData.Rows) 
                {
                    if (r.Cells.Count >= 2)
                    {
                        InputItem item = new InputItem();
                        item.VariableName = r.Cells[0].Value.ToString();
                        item.Data = r.Cells[1].Value.ToString();
                        item.DataType = typeof(String);
                        _module.UserSavedItems.Add(item);
                    }
                }

				this.DialogResult = DialogResult.OK;
				this.Close();
			}
		}

		private void textBoxName_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (this.textBoxName.Text.Trim() == string.Empty) 
			{
				e.Cancel = true;
				this.errorProviderRequiredFields.SetError(this.textBoxName, "Name is Required!");
			}
			else 
			{
				this.errorProviderRequiredFields.SetError(this.textBoxName, string.Empty);
			}
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void FormAddEditModule_Load(object sender, System.EventArgs e)
		{
			this.errorProviderRequiredFields.SetError(this.textBoxName, string.Empty);
			this.errorProviderRequiredFields.SetIconAlignment(this.textBoxName, ErrorIconAlignment.TopRight);
		}

		private void FormAddEditModule_Activated(object sender, System.EventArgs e)
		{
			if (!this._isActivated) 
			{
				this.textBoxName.Focus();
				this._isActivated = true;
			}
		}
	}
}
