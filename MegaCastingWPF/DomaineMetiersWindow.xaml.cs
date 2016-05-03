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
    /// Logique d'interaction pour DomaineMetiersWindow.xaml
    /// </summary>
    public partial class DomaineMetiersWindow : Window
    {
        private MegaProductionEntities2 db;

        public DomaineMetier DomaineMetier { get; set; }


        public DomaineMetiersWindow(MegaProductionEntities2 context)
        {
            InitializeComponent();
            db = context;
            this.DomaineMetier = new DomaineMetier();
            this.DataContext = this;
        }

        private void Validate_click(object sender, RoutedEventArgs e)
        {

            //On ajoute d'abord le domine de metier en "base"
            db.DomaineMetiers.Add(DomaineMetier);

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

                db.DomaineMetiers.Remove(DomaineMetier);
            }

        }

        private void Annuler_Click(object sender, RoutedEventArgs e)
        {
        
            this.DialogResult = false;
        }
    }
}
