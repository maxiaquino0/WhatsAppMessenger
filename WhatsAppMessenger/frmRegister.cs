using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WhatsAppMessenger
{
    public partial class frmRegister : Form
    {
        string password;
        string error;

        public frmRegister()
        {
            InitializeComponent();
        }

        private void btnRequest_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPhoneNumber.Text))
            {
                MessageBox.Show("Por favor ingrese su número de teléfono.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtPhoneNumber.Focus();
                return;
            }
            if (string.IsNullOrEmpty(txtFullName.Text))
            {
                MessageBox.Show("por favor ingrese su nombre.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtFullName.Focus();
                return;
            }
            if (WhatsAppApi.Register.WhatsRegisterV2.RequestCode(txtPhoneNumber.Text, out password, out error, "sms"))
            {
                if (!string.IsNullOrEmpty(password))
                    Save();
                else
                {
                    grbRequestCode.Enabled = false;
                    grbConfirmCode.Enabled = true;
                }
            }
            else
            {
                MessageBox.Show("No se puede generar password", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Save() {
            this.grbRequestCode.Enabled = false;
            this.grbConfirmCode.Enabled = false;
            Properties.Settings.Default.PhoneNumber = txtPhoneNumber.Text;
            Properties.Settings.Default.Password = password;
            Properties.Settings.Default.FullName = txtFullName.Text;
            Properties.Settings.Default.Save();
            if (Globals.DB.Accounts.FindByAccountId(txtPhoneNumber.Text) == null)
            {
                AppData.AccountsRow row = Globals.DB.Accounts.NewAccountsRow();
                row.AccountId = txtPhoneNumber.Text;
                row.FullName = txtFullName.Text;
                row.Password = password;
                Globals.DB.Accounts.AddAccountsRow(row);
                Globals.DB.Accounts.AcceptChanges();
                Globals.DB.Accounts.WriteXml(string.Format("{0}\\accounts.dat", Application.StartupPath));
            }
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSMSCode.Text))
            {
                MessageBox.Show("Por favor ingrese su codigo sms.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtSMSCode.Focus();
                return;
            }
            password = WhatsAppApi.Register.WhatsRegisterV2.RegisterCode(txtPhoneNumber.Text, txtSMSCode.Text, out error);
            Save();
            
        }
    }
}
