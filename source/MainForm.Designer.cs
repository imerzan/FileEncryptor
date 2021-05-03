
namespace FileEncryptor
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.inputFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.outputFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.hashFileOpenDialog = new System.Windows.Forms.OpenFileDialog();
            this.button_SelectInputFile = new System.Windows.Forms.Button();
            this.textBox_InputPath = new System.Windows.Forms.TextBox();
            this.textBox_HashPath = new System.Windows.Forms.TextBox();
            this.button_SelectHash = new System.Windows.Forms.Button();
            this.checkBox_Hash = new System.Windows.Forms.CheckBox();
            this.radioButton_Encrypt = new System.Windows.Forms.RadioButton();
            this.radioButton_Decrypt = new System.Windows.Forms.RadioButton();
            this.textBox_OutputPath = new System.Windows.Forms.TextBox();
            this.button_SelectOutputFile = new System.Windows.Forms.Button();
            this.button_Execute = new System.Windows.Forms.Button();
            this.textBox_Password = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox_HidePW = new System.Windows.Forms.CheckBox();
            this.hashFileSaveDialog = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // inputFileDialog
            // 
            this.inputFileDialog.Filter = "All files (*.*)|*.*";
            // 
            // outputFileDialog
            // 
            this.outputFileDialog.Filter = "All files (*.*)|*.*";
            // 
            // hashFileOpenDialog
            // 
            this.hashFileOpenDialog.Filter = "All files (*.*)|*.*";
            // 
            // button_SelectInputFile
            // 
            this.button_SelectInputFile.Location = new System.Drawing.Point(12, 47);
            this.button_SelectInputFile.Name = "button_SelectInputFile";
            this.button_SelectInputFile.Size = new System.Drawing.Size(67, 40);
            this.button_SelectInputFile.TabIndex = 0;
            this.button_SelectInputFile.Text = "Input";
            this.button_SelectInputFile.UseVisualStyleBackColor = true;
            this.button_SelectInputFile.Click += new System.EventHandler(this.button_SelectInputFile_Click);
            // 
            // textBox_InputPath
            // 
            this.textBox_InputPath.Location = new System.Drawing.Point(85, 57);
            this.textBox_InputPath.Name = "textBox_InputPath";
            this.textBox_InputPath.Size = new System.Drawing.Size(327, 23);
            this.textBox_InputPath.TabIndex = 1;
            // 
            // textBox_HashPath
            // 
            this.textBox_HashPath.Enabled = false;
            this.textBox_HashPath.Location = new System.Drawing.Point(85, 115);
            this.textBox_HashPath.Name = "textBox_HashPath";
            this.textBox_HashPath.Size = new System.Drawing.Size(327, 23);
            this.textBox_HashPath.TabIndex = 3;
            // 
            // button_SelectHash
            // 
            this.button_SelectHash.Enabled = false;
            this.button_SelectHash.Location = new System.Drawing.Point(12, 105);
            this.button_SelectHash.Name = "button_SelectHash";
            this.button_SelectHash.Size = new System.Drawing.Size(67, 40);
            this.button_SelectHash.TabIndex = 2;
            this.button_SelectHash.Text = "Hashfile";
            this.button_SelectHash.UseVisualStyleBackColor = true;
            this.button_SelectHash.Click += new System.EventHandler(this.button_SelectHash_Click);
            // 
            // checkBox_Hash
            // 
            this.checkBox_Hash.AutoSize = true;
            this.checkBox_Hash.Location = new System.Drawing.Point(208, 13);
            this.checkBox_Hash.Name = "checkBox_Hash";
            this.checkBox_Hash.Size = new System.Drawing.Size(135, 19);
            this.checkBox_Hash.TabIndex = 4;
            this.checkBox_Hash.Text = "Use HMAC-SHA-512";
            this.checkBox_Hash.UseVisualStyleBackColor = true;
            this.checkBox_Hash.CheckedChanged += new System.EventHandler(this.checkBox_Hash_CheckedChanged);
            // 
            // radioButton_Encrypt
            // 
            this.radioButton_Encrypt.AutoSize = true;
            this.radioButton_Encrypt.Checked = true;
            this.radioButton_Encrypt.Location = new System.Drawing.Point(12, 12);
            this.radioButton_Encrypt.Name = "radioButton_Encrypt";
            this.radioButton_Encrypt.Size = new System.Drawing.Size(65, 19);
            this.radioButton_Encrypt.TabIndex = 5;
            this.radioButton_Encrypt.TabStop = true;
            this.radioButton_Encrypt.Text = "Encrypt";
            this.radioButton_Encrypt.UseVisualStyleBackColor = true;
            this.radioButton_Encrypt.CheckedChanged += new System.EventHandler(this.radioButton_Encrypt_CheckedChanged);
            // 
            // radioButton_Decrypt
            // 
            this.radioButton_Decrypt.AutoSize = true;
            this.radioButton_Decrypt.Location = new System.Drawing.Point(112, 12);
            this.radioButton_Decrypt.Name = "radioButton_Decrypt";
            this.radioButton_Decrypt.Size = new System.Drawing.Size(66, 19);
            this.radioButton_Decrypt.TabIndex = 6;
            this.radioButton_Decrypt.Text = "Decrypt";
            this.radioButton_Decrypt.UseVisualStyleBackColor = true;
            this.radioButton_Decrypt.CheckedChanged += new System.EventHandler(this.radioButton_Decrypt_CheckedChanged);
            // 
            // textBox_OutputPath
            // 
            this.textBox_OutputPath.Location = new System.Drawing.Point(85, 175);
            this.textBox_OutputPath.Name = "textBox_OutputPath";
            this.textBox_OutputPath.Size = new System.Drawing.Size(327, 23);
            this.textBox_OutputPath.TabIndex = 8;
            // 
            // button_SelectOutputFile
            // 
            this.button_SelectOutputFile.Location = new System.Drawing.Point(12, 165);
            this.button_SelectOutputFile.Name = "button_SelectOutputFile";
            this.button_SelectOutputFile.Size = new System.Drawing.Size(67, 40);
            this.button_SelectOutputFile.TabIndex = 7;
            this.button_SelectOutputFile.Text = "Output";
            this.button_SelectOutputFile.UseVisualStyleBackColor = true;
            this.button_SelectOutputFile.Click += new System.EventHandler(this.button_SelectOutputFile_Click);
            // 
            // button_Execute
            // 
            this.button_Execute.Location = new System.Drawing.Point(188, 279);
            this.button_Execute.Name = "button_Execute";
            this.button_Execute.Size = new System.Drawing.Size(108, 49);
            this.button_Execute.TabIndex = 9;
            this.button_Execute.Text = "Encrypt";
            this.button_Execute.UseVisualStyleBackColor = true;
            this.button_Execute.Click += new System.EventHandler(this.button_Execute_Click);
            // 
            // textBox_Password
            // 
            this.textBox_Password.Location = new System.Drawing.Point(85, 224);
            this.textBox_Password.Name = "textBox_Password";
            this.textBox_Password.Size = new System.Drawing.Size(231, 23);
            this.textBox_Password.TabIndex = 10;
            this.textBox_Password.UseSystemPasswordChar = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 227);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 15);
            this.label1.TabIndex = 11;
            this.label1.Text = "Password";
            // 
            // checkBox_HidePW
            // 
            this.checkBox_HidePW.AutoSize = true;
            this.checkBox_HidePW.Checked = true;
            this.checkBox_HidePW.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_HidePW.Location = new System.Drawing.Point(322, 228);
            this.checkBox_HidePW.Name = "checkBox_HidePW";
            this.checkBox_HidePW.Size = new System.Drawing.Size(104, 19);
            this.checkBox_HidePW.TabIndex = 12;
            this.checkBox_HidePW.Text = "Hide Password";
            this.checkBox_HidePW.UseVisualStyleBackColor = true;
            this.checkBox_HidePW.CheckedChanged += new System.EventHandler(this.checkBox_HidePW_CheckedChanged);
            // 
            // hashFileSaveDialog
            // 
            this.hashFileSaveDialog.Filter = "All files (*.*)|*.*";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 360);
            this.Controls.Add(this.checkBox_HidePW);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_Password);
            this.Controls.Add(this.button_Execute);
            this.Controls.Add(this.textBox_OutputPath);
            this.Controls.Add(this.button_SelectOutputFile);
            this.Controls.Add(this.radioButton_Decrypt);
            this.Controls.Add(this.radioButton_Encrypt);
            this.Controls.Add(this.checkBox_Hash);
            this.Controls.Add(this.textBox_HashPath);
            this.Controls.Add(this.button_SelectHash);
            this.Controls.Add(this.textBox_InputPath);
            this.Controls.Add(this.button_SelectInputFile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "File Encryptor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog inputFileDialog;
        private System.Windows.Forms.SaveFileDialog outputFileDialog;
        private System.Windows.Forms.OpenFileDialog hashFileOpenDialog;
        private System.Windows.Forms.Button button_SelectInputFile;
        private System.Windows.Forms.TextBox textBox_InputPath;
        private System.Windows.Forms.TextBox textBox_HashPath;
        private System.Windows.Forms.Button button_SelectHash;
        private System.Windows.Forms.CheckBox checkBox_Hash;
        private System.Windows.Forms.RadioButton radioButton_Encrypt;
        private System.Windows.Forms.RadioButton radioButton_Decrypt;
        private System.Windows.Forms.TextBox textBox_OutputPath;
        private System.Windows.Forms.Button button_SelectOutputFile;
        private System.Windows.Forms.Button button_Execute;
        private System.Windows.Forms.TextBox textBox_Password;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox_HidePW;
        private System.Windows.Forms.SaveFileDialog hashFileSaveDialog;
    }
}

