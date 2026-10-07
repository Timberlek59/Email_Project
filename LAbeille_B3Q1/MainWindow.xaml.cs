using System;
using System.Collections.ObjectModel;
using System.IO;                   // Requis pour File et Path
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.Json;           // Requis pour la sérialisation JSON
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace LAbeille_B3Q1
{
    public partial class MainWindow : Window
    {
        // Collection observable pour la To-Do List
        public ObservableCollection<TodoTask> Taches { get; set; } = new ObservableCollection<TodoTask>();

        // Chemin du fichier de sauvegarde
        private readonly string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "todolist.json");

        // CONSTRUCTEUR UNIQUE (Regroupé ici)
        public MainWindow()
        {
            InitializeComponent();

            // Attribuer le DataContext pour que le Chronomètre fonctionne
            this.DataContext = new LAbeille_B3Q1.ViewModel.ChronoViewModel();

            // Lier la collection à la ListBox
            lstTaches.ItemsSource = Taches;

            // Charger les tâches existantes au démarrage
            ChargerTodoList();
        }

        #region MODULE 1 : ENVOI DE MAIL
        private async void BtnEnvoyer_Click(object sender, RoutedEventArgs e)
        {
            string expediteur = txtExpediteur.Text.Trim();
            string motDePasseApp = txtMotDePasse.Password.Trim();
            string destinataire = txtDestinataire.Text.Trim();
            string objet = txtObjet.Text.Trim();
            string corps = txtCorps.Text;

            if (string.IsNullOrEmpty(expediteur) || string.IsNullOrEmpty(motDePasseApp) || string.IsNullOrEmpty(destinataire))
            {
                AfficherStatutMail("Veuillez remplir l'expéditeur, la clé d'application et le destinataire.", Colors.Red);
                return;
            }

            btnEnvoyer.IsEnabled = false;
            AfficherStatutMail("Envoi en cours...", Colors.DarkOrange);

            try
            {
                await Task.Run(() => EnvoyerEmail(expediteur, motDePasseApp, destinataire, objet, corps));
                AfficherStatutMail("E-mail envoyé avec succès !", Colors.Green);
            }
            catch (SmtpException ex)
            {
                AfficherStatutMail($"Erreur SMTP : {ex.Message}", Colors.Red);
            }
            catch (Exception ex)
            {
                AfficherStatutMail($"Erreur : {ex.Message}", Colors.Red);
            }
            finally
            {
                btnEnvoyer.IsEnabled = true;
            }
        }

        private void EnvoyerEmail(string exp, string mdp, string dest, string objet, string corps)
        {
            using (MailMessage mail = new MailMessage(exp, dest, objet, corps))
            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                smtp.Credentials = new NetworkCredential(exp, mdp);
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
        }

        private void AfficherStatutMail(string msg, Color col)
        {
            txtStatut.Text = msg;
            txtStatut.Foreground = new SolidColorBrush(col);
        }
        #endregion

        #region MODULE 2 : TO-DO LIST
        private void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            string titre = txtNouvelleTache.Text.Trim();
            if (!string.IsNullOrEmpty(titre))
            {
                Taches.Add(new TodoTask { Title = titre, IsDone = false });
                txtNouvelleTache.Clear();
                txtStatutTodo.Text = "Tâche ajoutée.";
                txtStatutTodo.Foreground = Brushes.Green;
            }
        }

        private void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (lstTaches.SelectedItem is TodoTask tacheSelectionnee)
            {
                Taches.Remove(tacheSelectionnee);
                txtStatutTodo.Text = "Tâche supprimée.";
                txtStatutTodo.Foreground = Brushes.Orange;
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une tâche à supprimer.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            SauvegarderTodoList();
        }

        private void SauvegarderTodoList()
        {
            try
            {
                string json = JsonSerializer.Serialize(Taches);
                File.WriteAllText(filePath, json);

                txtStatutTodo.Text = "To-Do list sauvegardée avec succès sur le disque !";
                txtStatutTodo.Foreground = Brushes.Green;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la sauvegarde : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ChargerTodoList()
        {
            if (File.Exists(filePath))
            {
                try
                {
                    string json = File.ReadAllText(filePath);
                    var list = JsonSerializer.Deserialize<TodoTask[]>(json);

                    Taches.Clear();
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            Taches.Add(item);
                        }
                    }
                    txtStatutTodo.Text = "To-Do list chargée automatiquement depuis le PC.";
                    txtStatutTodo.Foreground = Brushes.Blue;
                }
                catch (Exception ex)
                {
                    txtStatutTodo.Text = $"Impossible de charger le fichier : {ex.Message}";
                    txtStatutTodo.Foreground = Brushes.Red;
                }
            }
        }
        #endregion
    }
}