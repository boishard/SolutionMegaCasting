using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using MegaCasting.DBLib;
using System.Data.Entity.Validation;

namespace MegaCastingWPF
{
    /// <summary>
    /// Logique d'interaction pour ClientsWindow.xaml
    /// </summary>
    public partial class ClientsWindow : Window
    {
        private MegaProductionEntities2 db;

        public Client Client { get; set; }


        public ClientsWindow(MegaProductionEntities2 context)
        {
            InitializeComponent();
            db = context;
            this.Client = new Client();
            this.DataContext = this;
        }

        private void Validate_click(object sender, RoutedEventArgs e)
        {
            //On ajoute d'abord le client en base
            db.Clients.Add(Client);

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

                db.Clients.Remove(Client);
            }
        }

        private void Annuler_Click(object sender, RoutedEventArgs e)
        {
        
            this.DialogResult = false;
        }
    }
}
