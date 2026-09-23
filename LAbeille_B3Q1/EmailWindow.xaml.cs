using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LAbeille_B3Q1
{
    /// <summary>
    /// Logique d'interaction pour EmailWindow.xaml
    /// </summary>
    public partial class EmailWindow : Window
    {
        public EmailWindow()
        {
            InitializeComponent();
        }

        private async void BtnEnvoyer_Click(object sender, RoutedEventArgs e)
        {
            string expediteur = TxtExpediteur.Text.Trim();
            string password = TxtPassword.Password.Trim();
            string destinataire = TxtDestinataire.Text.Trim();
            string objet = TxtObjet.Text.Trim();
            string corps = TxtMessage.Text.Trim();

            if (string.IsNullOrEmpty(expediteur) || string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(destinataire) || string.IsNullOrEmpty(objet))
            {
                ShowStatus("Veuillez remplir tous les champs obligatoires.", Colors.OrangeRed);
                return;
            }

            BtnEnvoyer.IsEnabled = false;
            ShowStatus("Envoi en cours...", Colors.DodgerBlue);

            try
            {
                using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(expediteur, password);

                    using (MailMessage mail = new MailMessage())
                    {
                        mail.From = new MailAddress(expediteur);
                        mail.To.Add(destinataire);
                        mail.Subject = objet;
                        mail.Body = corps;
                        mail.IsBodyHtml = false;

                        await smtpClient.SendMailAsync(mail);
                    }
                }

                ShowStatus("E-mail envoyé avec succès !", Colors.Green);
                ClearFields();
            }
            catch (SmtpException smtpEx)
            {
                ShowStatus($"Erreur SMTP : {smtpEx.Message}", Colors.Red);
                MessageBox.Show($"Détails de l'erreur SMTP :\n{smtpEx.Message}", "Échec de l'envoi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                ShowStatus($"Erreur : {ex.Message}", Colors.Red);
                MessageBox.Show($"Une erreur est survenue :\n{ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                BtnEnvoyer.IsEnabled = true;
            }
        }

        private void ShowStatus(string message, Color color)
        {
            TxtStatus.Text = message;
            TxtStatus.Foreground = new SolidColorBrush(color);
        }

        private void ClearFields()
        {
            TxtDestinataire.Clear();
            TxtObjet.Clear();
            TxtMessage.Clear();
        }
    }
}
