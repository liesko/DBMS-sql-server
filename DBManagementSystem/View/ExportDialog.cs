using System.Windows.Forms;
using DBManagementSystem.DataHandler;

namespace DBManagementSystem.View
{
    class ExportDialog : NewDialog
    {
        public override string ShowDialog(string caption)
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 240,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };

            string location = "Location";
            string fileName = "File Name";

            Label locationLabel = new Label() { Left = 50, Top = 20, Text = location };
            TextBox locationText = new TextBox() { Left = 50, Top = 50, Width = 400 };

            Label fileNameLabel = new Label() { Left = 50, Top = 80, Text = fileName };
            TextBox fileNameText = new TextBox() { Left = 50, Top = 110, Width = 400 };

            Button confirmation = new Button()
            {
                Text = "Export",
                Left = 350,
                Width = 100,
                Top = 150,
                DialogResult = DialogResult.OK
            };

            confirmation.Click += delegate {
                Exporter.FileName = fileNameText.Text;
                Exporter.Path = locationText.Text;
            };

            prompt.Controls.Add(locationText);
            prompt.Controls.Add(locationLabel);
            prompt.Controls.Add(fileNameText);
            prompt.Controls.Add(fileNameLabel);

            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? "done" : "error";
        }
    }
}
