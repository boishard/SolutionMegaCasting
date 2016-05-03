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
    /// Logique d'interaction pour MetiersWindow.xaml
    /// </summary>
    public partial class MetiersWindow : Window
    {
        private MegaProductionEntities2 db;
        private DomaineMetier currentDomaineMetier;

        public Metier metier { get; set; }


        public MetiersWindow(MegaProductionEntities2 context, DomaineMetier currentDomaineMetier)
        {
            InitializeComponent();
            db = context;
            this.metier = new Metier();
            this.DataContext = this;
            this.currentDomaineMetier = currentDomaineMetier;
        }

        private void Validate_click(object sender, RoutedEventArgs e)
        {

            metier.IdentifiantDomaineMetier = currentDomaineMetier.Identifiant; //on rattache le metier a un domaine de metier

            //On ajoute d'abord le metier en base
            db.Metiers.Add(metier);

            // on essai d'enregistrer, si ça ne marche pas on enleve le metier de la base et on affiche le message d'erreur 
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

                db.Metiers.Remove(metier);
            }

        }

        private void Annuler_Click(object sender, RoutedEventArgs e)
        {
        
            this.DialogResult = false;
        }
    }
}
