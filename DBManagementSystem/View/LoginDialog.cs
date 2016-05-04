using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBManagementSystem.Security;

namespace DBManagementSystem.View
{
    class LoginDialog : NewDialog
    {
        public override string ShowDialog(string caption)
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 400,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };

            string adresaLabelText = "IP, port number";
            string userLabelText = "User name";
            string passwordLabelText = "Password";
            string initialCatalogLabelText = "Initial catalog";

            Label adresaLabel = new Label() {Left = 50, Top = 20, Text = adresaLabelText};
            TextBox adresaText = new TextBox() {Left = 50, Top = 50, Width = 400};

            Label userLabel = new Label() {Left = 50, Top = 80, Text = userLabelText};
            TextBox userText = new TextBox() {Left = 50, Top = 110, Width = 400};

            Label passwordLabel = new Label() {Left = 50, Top = 140, Text = passwordLabelText};
            TextBox passwordText = new TextBox() {Left = 50, Top = 170, Width = 400};

            Label catalogLabel = new Label() {Left = 50, Top = 200, Text = initialCatalogLabelText};
            TextBox catalogText = new TextBox() {Left = 50, Top = 230, Width = 400};

            Button confirmation = new Button()
            {
                Text = "Connect",
                Left = 350,
                Width = 100,
                Top = 300,
                DialogResult = DialogResult.OK
            };

            string connectionString = "";

            confirmation.Click += delegate {
                NewConnection test = new NewConnection(userText.Text, passwordText.Text, adresaText.Text, catalogText.Text);
                if (test.IsConnected)
                {
                    test.Connection.Close();
                    prompt.Close();
                }
                else
                {
                    this.ShowDialog("Login Form - Incorrect Data");
                }
            };

            prompt.Controls.Add(adresaText);
            prompt.Controls.Add(adresaLabel);
            prompt.Controls.Add(userText);
            prompt.Controls.Add(userLabel);
            prompt.Controls.Add(passwordLabel);
            prompt.Controls.Add(passwordText);
            prompt.Controls.Add(catalogLabel);
            prompt.Controls.Add(catalogText);
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            connectionString = "Data Source=" + adresaText.Text + ";Initial Catalog=" + catalogText.Text + ";User ID=" + userText.Text + ";Password=" + passwordText.Text;

            return prompt.ShowDialog() == DialogResult.OK ? connectionString : "";
        }



        public bool CheckConnection()
        {
            return true;
        }

    }
}
