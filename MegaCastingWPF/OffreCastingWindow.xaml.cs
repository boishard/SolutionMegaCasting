using MegaCasting.DBLib;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
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

namespace MegaCastingWPF
{
    /// <summary>
    /// Logique d'interaction pour OffreCastingWindow.xaml
    /// </summary>
    public partial class OffreCastingWindow : Window
    {

        private MegaProductionEntities2 db;

        public OffreCasting OffreCasting { get; set; }

        public OffreCastingWindow(MegaProductionEntities2 context)
        {
            InitializeComponent();
            db = context;
            this.OffreCasting = new OffreCasting();
            this.DataContext = this;

            //on remplie les combobox avec des client (le premier etant un factice)
            List<Client> itemsClient = new List<Client>();
            Client clientNull = new Client();
            clientNull.Identifiant = -1;
            clientNull.Raison_Social = "(Aucun)";
            itemsClient.Add(clientNull);
            foreach (Client client in db.Clients)
            {
                itemsClient.Add(client);
            }
            ComboBoxOffreClient.ItemsSource = itemsClient;
            ComboBoxOffreClient.SelectedIndex = 0;

            //on remplie les combobox avec des type de contrat (le premier etant un factice)
            List<TypeContrat> itemsTypeContrat = new List<TypeContrat>();
            TypeContrat typeContratNull = new TypeContrat();
            typeContratNull.Identifiant = -1;
            typeContratNull.LibelleType = "(Aucun)";
            itemsTypeContrat.Add(typeContratNull);
            foreach (TypeContrat typeContrat in db.TypeContrats)
            {
                itemsTypeContrat.Add(typeContrat);
            }
            ComboBoxOffreTypeContrat.ItemsSource = itemsTypeContrat;
            ComboBoxOffreTypeContrat.SelectedIndex = 0;

            //on remplie les combobox avec des metier (le premier etant un factice)
            List<Metier> itemsMetier = new List<Metier>();
            Metier metierNull = new Metier();
            metierNull.Identifiant = -1;
            metierNull.Libelle = "(Aucun)";
            itemsMetier.Add(metierNull);
            foreach (Metier metier in db.Metiers)
            {
                itemsMetier.Add(metier);
            }
            ComboBoxOffreMetier.ItemsSource = itemsMetier;
            ComboBoxOffreMetier.SelectedIndex = 0;
        }

        private void Validate_click(object sender, RoutedEventArgs e)
        {
            //on recupere le client selectionné et le rattache a l'offre de casting
            Client currentClient = ComboBoxOffreClient.SelectedItem as Client;
            if (currentClient.Identifiant == -1)
            {
                OffreCasting.IdentifiantClient = null;
            }
            else
            {
                OffreCasting.IdentifiantClient = currentClient.Identifiant;
            }

            //on recupere le type de contrat selectionné et le rattache a l'offre de casting
            TypeContrat currentTypeContrat = ComboBoxOffreTypeContrat.SelectedItem as TypeContrat;
            if (currentTypeContrat.Identifiant == -1)
            {
                OffreCasting.IdentifiantTypeContrat = null;
            }
            else
            {
                OffreCasting.IdentifiantTypeContrat = currentTypeContrat.Identifiant;
            }

            //on recupere le metier selectionné et le rattache a l'offre de casting
            Metier currentMetier = ComboBoxOffreMetier.SelectedItem as Metier;
            if (currentMetier.Identifiant == -1)
            {
                OffreCasting.IdentifiantMetier = null;
            }
            else
            {
                OffreCasting.IdentifiantMetier = currentMetier.Identifiant;
            }

            //On ajoute d'abord l'offre en base
            db.OffreCastings.Add(OffreCasting);


            // on essai d'enregistrer, si ça ne marche pas on enleve le client de la base et on affiche le message d'erreur 
            try
            {
                db.SaveChanges();

                this.DialogResult = true;

            }
            catch (DbEntityValidationException dbEx)
            {
                String errors = "";

                foreach (DbEntityValidationResult dbEntityValidationResult in dbEx.EntityValidationErrors)
                {
                    foreach (DbValidationError dbValidationError in dbEntityValidationResult.ValidationErrors)
                    {
                        errors += dbValidationError.ErrorMessage + "\n";
                    }
                }

                MessageBox.Show(errors);

                db.OffreCastings.Remove(OffreCasting);
            }
            
        }

        private void Annuler_Click(object sender, RoutedEventArgs e)
        {

            this.DialogResult = false;
        }
    }
}
