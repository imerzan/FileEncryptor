using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileEncryptor
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void radioButton_Encrypt_CheckedChanged(object sender, EventArgs e) // Selected 'Encrypt'
        {
            if (this.radioButton_Encrypt.Checked)
            {
                this.Clear();
                this.button_Execute.Text = "Encrypt";
            }
        }

        private void radioButton_Decrypt_CheckedChanged(object sender, EventArgs e) // Selected 'Decrypt'
        {
            if (this.radioButton_Decrypt.Checked)
            {
                this.Clear();
                this.button_Execute.Text = "Decrypt";
            }
        }

        private void button_SelectInputFile_Click(object sender, EventArgs e) // Clicked Button 'Input'
        {
            if (this.inputFileDialog.ShowDialog() == DialogResult.OK) // Dialog 'OK' Pressed
            {
                this.textBox_InputPath.Text = this.inputFileDialog.FileName; // Set textbox
                if (this.checkBox_Hash.Checked & this.radioButton_Encrypt.Checked) // If: Hashing = true , Mode = Encrypt
                {
                    this.textBox_HashPath.Text = this.inputFileDialog.FileName + ".hash"; // Set default hashfile
                }
                if (this.textBox_OutputPath.Text == String.Empty) // If: Output is empty
                {
                    if (this.radioButton_Encrypt.Checked) // If: Mode = Encrypt
                        this.textBox_OutputPath.Text = this.inputFileDialog.FileName + ".encrypted"; // Set default output
                    else // Mode = Decrypt
                        this.textBox_OutputPath.Text = this.inputFileDialog.FileName + ".plaintext"; // Set default output
                }
            }
        }

        private void button_SelectHash_Click(object sender, EventArgs e) // Clicked Button 'Hashfile'
        {
            if (this.radioButton_Encrypt.Checked) // Hash File Save
            {
                if (this.hashFileSaveDialog.ShowDialog() == DialogResult.OK) // Dialog 'OK' Pressed
                    this.textBox_HashPath.Text = this.hashFileSaveDialog.FileName; // Set textbox
            }
            else // Hash File Open
            {
                if (this.hashFileOpenDialog.ShowDialog() == DialogResult.OK) // Dialog 'OK' Pressed
                    this.textBox_HashPath.Text = this.hashFileOpenDialog.FileName; // Set textbox
            }
        }

        private void button_SelectOutputFile_Click(object sender, EventArgs e) // Clicked Button 'Output'
        {
            if (this.outputFileDialog.ShowDialog() == DialogResult.OK) // Dialog 'OK' Pressed
                this.textBox_OutputPath.Text = this.outputFileDialog.FileName; // Set textbox
        }

        private void checkBox_Hash_CheckedChanged(object sender, EventArgs e) // Clicked Checkbox 'Use HMAC-SHA-512'
        {
            if (this.checkBox_Hash.Checked & this.radioButton_Encrypt.Checked & this.textBox_InputPath.Text != String.Empty) // If: Hashing = true , Mode = Encrypt, Textbox not empty
                this.textBox_HashPath.Text = this.textBox_InputPath.Text + ".hash"; // Set default hashfile
            this.textBox_HashPath.Enabled = this.checkBox_Hash.Checked; // Enable textbox
            this.button_SelectHash.Enabled = this.checkBox_Hash.Checked; // Enable button 'Hashfile'
        }

        private void Clear() // Clear text input fields
        {
            this.textBox_InputPath.Text = null;
            this.textBox_HashPath.Text = null;
            this.textBox_OutputPath.Text = null;
        }

        private void checkBox_HidePW_CheckedChanged(object sender, EventArgs e) // Clicked Checkbox 'Hide Password'
        {
            if (this.checkBox_HidePW.Checked) // true (checked)
                this.textBox_Password.UseSystemPasswordChar = true; // Hide password
            else // false (unchecked)
                this.textBox_Password.UseSystemPasswordChar = false; // Show password
        }

        private async void button_Execute_Click(object sender, EventArgs e) // Clicked Button(s) 'Encrypt' / 'Decrypt'
        {
            try
            {
                this.Enabled = false; // Lock Main Window
                this.button_Execute.Text = "Please wait..."; // Change button name to 'Please wait...'
                if (this.radioButton_Decrypt.Checked & this.checkBox_Hash.Checked & this.textBox_HashPath.Text.Trim() == String.Empty) // If: Mode = Decrypt , Hashing = true, Hashfile input is empty
                {
                    throw new Exception("Hashfile not selected!"); // Missing hashfile, throw exception
                }

                if (this.radioButton_Encrypt.Checked) // If: Mode = Encrypt
                {
                    await Task.Run(() => Crypt.EncryptFile(this.textBox_Password.Text, this.textBox_InputPath.Text, this.textBox_OutputPath.Text, this.checkBox_Hash.Checked, this.textBox_HashPath.Text));
                    MessageBox.Show("Encryption completed!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information); // Success MessageBox
                }
                else // Decrypt
                {
                    await Task.Run(() => Crypt.DecryptFile(this.textBox_Password.Text, this.textBox_InputPath.Text, this.textBox_OutputPath.Text, this.textBox_HashPath.Text));
                    MessageBox.Show("Decryption completed!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information); // Success MessageBox
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error); // Error MessageBox
            }
            finally
            {
                this.Enabled = true; // Unlock Main Window
                if (this.radioButton_Encrypt.Checked) // Set button name back to 'Encrypt' or 'Decrypt'
                    this.button_Execute.Text = "Encrypt";
                else
                    this.button_Execute.Text = "Decrypt";
            }
        }

    }
}
