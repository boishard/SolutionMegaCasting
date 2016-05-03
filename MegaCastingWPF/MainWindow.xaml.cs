using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MegaCasting.DBLib;
using System.Collections.ObjectModel;
using System.Data.Entity.Validation;
using System.Xml;

namespace MegaCastingWPF
{
    public partial class MainWindow : Window
    {

        MegaProductionEntities2 db = new MegaProductionEntities2();

        public ObservableCollection<Client> Clients { get; set; }
        public ObservableCollection<OffreCasting> OffreCastings { get; set; }
        public ObservableCollection<TypeContrat> TypeContrats { get; set; }
        public ObservableCollection<PartenaireDiffusion> PartenaireDiffusions { get; set; }
        public ObservableCollection<DomaineMetier> DomaineMetiers { get; set; }
        public ObservableCollection<Metier> Metiers { get; set; }



        public MainWindow()
        {
            InitializeComponent();
            this.Clients = new ObservableCollection<Client>(db.Clients.ToList());
            this.OffreCastings = new ObservableCollection<OffreCasting>(db.OffreCastings.ToList());
            this.TypeContrats = new ObservableCollection<TypeContrat>(db.TypeContrats.ToList());
            this.PartenaireDiffusions = new ObservableCollection<PartenaireDiffusion>(db.PartenaireDiffusions.ToList());
            this.DomaineMetiers = new ObservableCollection<DomaineMetier>(db.DomaineMetiers.ToList());
            this.Metiers = new ObservableCollection<Metier>(db.Metiers.ToList());
            this.DataContext = this;


        }

        public void Sauvegarde()
        {
            try
            {
                db.SaveChanges();

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
            }
        }

        private void Sauvegarder_Click(object sender, RoutedEventArgs e)
        {
            Sauvegarde();
            MessageBox.Show("Sauvegarde réussie !", "Sauvegarde", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        #region Client

        //Methode lors de l'événement click du bouton d'ajout d'un client
        private void AddClientButton_Click(object sender, RoutedEventArgs e)
        {
            //appel la fentre d'ajout d'un client
            ClientsWindow clientsWindow = new ClientsWindow(db);

            //Ajoute le client dans la liste
            if (clientsWindow.ShowDialog() == true)
            {
                this.Clients.Add(clientsWindow.Client);
            }
        }

        //Methode lors de l'événement click du bouton de suppression d'un client
        private void DellClientButton_Click(object sender, RoutedEventArgs e)
        {
            //verifie sur un client est bien selectionné
            if (listClients.SelectedItem != null)
            {
                bool clientToDelete = true;

                Client client = listClients.SelectedItem as Client; //recupere le client selectionné

                MessageBoxResult result = MessageBox.Show("Êtes-vous sûr de vouloir supprimer définitivement le client '" + client.Raison_Social + "' ?", "Suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning); //Demande une confirmation
                if (result == MessageBoxResult.Yes)
                {
                    //Cherche les offre de casting rattaché au client pour les enlever
                    foreach (OffreCasting offreCasting in db.OffreCastings)
                    {
                        if (offreCasting.IdentifiantClient == client.Identifiant)
                        {
                            MessageBoxResult dellClientResult = MessageBox.Show("Le client est lié à l'offre '" + offreCasting.Intitulé + "', la supprimer ?", "Suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                            if (dellClientResult == MessageBoxResult.Yes)
                            {
                                db.OffreCastings.Remove(offreCasting);
                                this.OffreCastings.Remove(offreCasting);
                            }
                            else if (dellClientResult == MessageBoxResult.No)
                            {
                                clientToDelete = false;
                            }
                        }
                    }

                    if (clientToDelete)
                    {
                        int currentIndex = listClients.SelectedIndex;
                        db.Clients.Remove(client);
                        this.Clients.Remove(client);
                        listClients.Focus();
                        listClients.SelectedIndex = currentIndex;

                    }
                    Sauvegarde();
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un client.", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }

        }

        //Methode lorsque on selectionne un client dans la liste
        private void listClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


            if (listClients.SelectedItem != null)
            {
                List<OffreCasting> items = new List<OffreCasting>();
                Client currentClient = listClients.SelectedItem as Client;
                foreach (OffreCasting offreCasting in db.OffreCastings)
                {
                    if (offreCasting.IdentifiantClient == currentClient.Identifiant)
                    {
                        items.Add(offreCasting);
                    }
                }



                ListBoxClientOffre.ItemsSource = items;
            }

        }

        //Methode lors de l'événement click du bouton d'ajout d'offre a un client
        private void btnAddClientOffre(object sender, RoutedEventArgs e)
        {

            Client currentClient = listClients.SelectedItem as Client; //recupere le client selectionnee

            if (currentClient != null)
            {
                AjoutClientOffreWindow ajoutClientOffreWindow = new AjoutClientOffreWindow(db); //appel la fenetre d'ajout d'offre au client

                if (ajoutClientOffreWindow.ShowDialog() == true)
                {
                    //Cherche l'offre a ajouter en base et la rattache au client
                    foreach (OffreCasting offreCasting in db.OffreCastings)
                    {
                        if (offreCasting.Identifiant == ajoutClientOffreWindow.OffreCasting.Identifiant)
                        {
                            offreCasting.IdentifiantClient = currentClient.Identifiant;
                            (this.ListBoxClientOffre.ItemsSource as List<OffreCasting>).Add(offreCasting);
                            this.ListBoxClientOffre.Items.Refresh();
                        }
                    }
                }
            }

        }

        //Methode lors de l'événement click du bouton de suppresion d'une offre a un client
        private void btnDellClientOffre(object sender, RoutedEventArgs e)
        {
            if (ListBoxClientOffre.SelectedItem != null)
            {
                OffreCasting currentoffreCasting = ListBoxClientOffre.SelectedItem as OffreCasting; //recupere l'offre selectionne
                int currentIndex = ListBoxClientOffre.SelectedIndex;

                //cherche l'offre en base et enleve le lien avec le client 
                foreach (OffreCasting offreCasting in db.OffreCastings)
                {
                    if (currentoffreCasting.Identifiant == offreCasting.Identifiant)
                    {
                        offreCasting.IdentifiantClient = null;
                        (this.ListBoxClientOffre.ItemsSource as List<OffreCasting>).Remove(offreCasting);
                        this.ListBoxClientOffre.Items.Refresh();
                    }
                }



                ListBoxClientOffre.Focus();
                ListBoxClientOffre.SelectedIndex = currentIndex;
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une offre de casting.", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }

        #endregion


        #region OffreCasting

        //Methode lors de l'événement click du bouton d'ajout d'une offre de casting
        private void AddOffreCastingButton_Click(object sender, RoutedEventArgs e)
        {
            if (!db.Clients.Any() || !db.TypeContrats.Any() || !db.Metiers.Any())
            {
                MessageBox.Show("Veuillez d'abord ajouter des clients, des types de contrats ou des métiers", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            else
            {
                //appel le fenetre pour ajouter une offre
                OffreCastingWindow OffreCastingsWindow = new OffreCastingWindow(db);

                if (OffreCastingsWindow.ShowDialog() == true)
                {
                    this.OffreCastings.Add(OffreCastingsWindow.OffreCasting);
                }
            }
        }

        //Methode lors de l'événement click du bouton de suppression d'offre de casting
        private void DellOffreCastingButton_Click(object sender, RoutedEventArgs e)
        {
            if (listOffreCastings.SelectedItem != null)
            {
                OffreCasting offreCasting = listOffreCastings.SelectedItem as OffreCasting; //recupere l'offre de casting selectionnée

                MessageBoxResult result = MessageBox.Show("Êtes-vous sûr de vouloir supprimer définitivement l'offre de casting '" + offreCasting.Intitulé + "' ?", "Suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning); //demande un confiration de suppression
                if (result == MessageBoxResult.Yes)
                {
                    //supprime l'offre de la base et de la liste, et selectionne une autre offre
                    int currentIndex = listOffreCastings.SelectedIndex;
                    db.OffreCastings.Remove(offreCasting);
                    this.OffreCastings.Remove(offreCasting);
                    listOffreCastings.Focus();
                    listOffreCastings.SelectedIndex = currentIndex;

                    Sauvegarde();
                }
            }
            else
            {
                MessageBox.Show("Veuillez selectionner une offre de stage.", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }

        }

        //Méthode lors du changement d'offre de casting
        private void listOffreCastings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listOffreCastings.SelectedItem != null)
            {
                //Affiche la grid avec les infos sur l'offre
                Grid_InfoOffre.Visibility = Visibility.Visible;

                //Reinisialise chaque combobox
                ComboBoxOffreClient.ItemsSource = null;
                ComboBoxOffreTypeContrat.ItemsSource = null;
                ComboBoxOffreMetier.ItemsSource = null;

                ComboBoxOffreClient.Items.Clear();
                ComboBoxOffreTypeContrat.Items.Clear();
                ComboBoxOffreMetier.Items.Clear();

                //instancie les listes sources pour les combobox
                List<Client> itemsClient = new List<Client>();
                List<TypeContrat> itemsTypeContrat = new List<TypeContrat>();
                List<Metier> itemsMetier = new List<Metier>();

                OffreCasting currentOffreCasting = listOffreCastings.SelectedItem as OffreCasting; // recupere l'offre selectionnée

                //instancie les indexs
                int indexClient = 0;
                int tempIndexClient = 0;
                int indexTypeContrat = 0;
                int tempIndexTypeContrat = 0;
                int indexMetier = 0;
                int tempIndexMetier = 0;

                //Remplie la combobox client
                foreach (Client client in db.Clients)
                {
                    itemsClient.Add(client);
                    if (client.Identifiant == currentOffreCasting.IdentifiantClient)
                    {
                        indexClient = tempIndexClient;
                    }
                    tempIndexClient++;
                }
                ComboBoxOffreClient.ItemsSource = itemsClient;
                ComboBoxOffreClient.SelectedIndex = indexClient;

                //Remplie la combobox type de contrat
                foreach (TypeContrat typeContrat in db.TypeContrats)
                {
                    itemsTypeContrat.Add(typeContrat);
                    if (typeContrat.Identifiant == currentOffreCasting.IdentifiantTypeContrat)
                    {
                        indexTypeContrat = tempIndexTypeContrat;
                    }
                    tempIndexTypeContrat++;
                }
                ComboBoxOffreTypeContrat.ItemsSource = itemsTypeContrat;
                ComboBoxOffreTypeContrat.SelectedIndex = indexTypeContrat;

                //Remplie la combobox metier
                foreach (Metier metier in db.Metiers)
                {
                    itemsMetier.Add(metier);
                    if (metier.Identifiant == currentOffreCasting.IdentifiantMetier)
                    {
                        indexMetier = tempIndexMetier;
                    }
                    tempIndexMetier++;
                }
                ComboBoxOffreMetier.ItemsSource = itemsMetier;
                ComboBoxOffreMetier.SelectedIndex = indexMetier;
            }
            else
            {

                Grid_InfoOffre.Visibility = Visibility.Hidden;
            }

        }

        //Methode lors d'un changement d'item de la combobox client
        private void ComboBoxOffreClient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OffreCasting currentOffreCasting = listOffreCastings.SelectedItem as OffreCasting; //recupere l'offre de casting sélectionnée

            Client currentClient = ComboBoxOffreClient.SelectedItem as Client; //recupere le client selectionnée

            if ((currentClient != null) && (currentOffreCasting != null))
            {
                //Cherche l'offre de casting en question dans la base et la rattache a un client
                foreach (OffreCasting offreCasting in db.OffreCastings)
                {
                    if (offreCasting.Identifiant == currentOffreCasting.Identifiant)
                    {
                        if (currentClient.Identifiant == -1)
                        {
                            offreCasting.IdentifiantClient = null;
                            ComboBoxOffreClient.SelectedIndex = 0;
                        }
                        else
                        {
                            offreCasting.IdentifiantClient = currentClient.Identifiant;
                        }
                    }
                }
            }
        }

        //Methode lors d'un changement d'item de la combobox client
        private void ComboBoxOffreTypeContrat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OffreCasting currentOffreCasting = listOffreCastings.SelectedItem as OffreCasting; //recupere l'offre de casting sélectionnée

            TypeContrat currentTypeContrat = ComboBoxOffreTypeContrat.SelectedItem as TypeContrat; //recupere le type de contrat sélectionnée

            if ((currentTypeContrat != null) && (currentOffreCasting != null))
            {
                //Cherche l'offre de casting en question dans la base et la rattache a un type de contrat
                foreach (OffreCasting offreCasting in db.OffreCastings)
                {
                    if (offreCasting.Identifiant == currentOffreCasting.Identifiant)
                    {
                        if (currentTypeContrat.Identifiant == -1)
                        {
                            offreCasting.IdentifiantTypeContrat = null;
                            ComboBoxOffreTypeContrat.SelectedIndex = 0;
                        }
                        else
                        {
                            offreCasting.IdentifiantTypeContrat = currentTypeContrat.Identifiant;
                        }
                    }
                }
            }
        }
        //Methode lors d'un changement d'item de la combobox client
        private void ComboBoxOffreMetier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OffreCasting currentOffreCasting = listOffreCastings.SelectedItem as OffreCasting; //recupere l'offre de casting sélectionnée

            Metier currentMetier = ComboBoxOffreMetier.SelectedItem as Metier; //recupere le metier sélectionnée

            if ((currentMetier != null) && (currentOffreCasting != null))
            {

                //Cherche l'offre de casting en question dans la base et la rattache a un metier
                foreach (OffreCasting offreCasting in db.OffreCastings)
                {
                    if (offreCasting.Identifiant == currentOffreCasting.Identifiant)
                    {
                        if (currentMetier.Identifiant == -1)
                        {
                            offreCasting.IdentifiantMetier = null;
                            ComboBoxOffreMetier.SelectedIndex = 0;
                        }
                        else
                        {
                            offreCasting.IdentifiantMetier = currentMetier.Identifiant;
                        }
                    }
                }
            }
        }

        #endregion


        #region TypeContrat

        //Méthode lors du changement de type de contrat
        private void listTypeContrats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listTypeContrats.SelectedItem != null)
            {

                //Remplie la liste des offres de castings rattachées au type de contrat selectionné
                List<OffreCasting> items = new List<OffreCasting>();
                TypeContrat currentTypeContrat = listTypeContrats.SelectedItem as TypeContrat;
                foreach (OffreCasting offreCasting in db.OffreCastings)
                {
                    if (offreCasting.IdentifiantTypeContrat == currentTypeContrat.Identifiant)
                    {
                        items.Add(offreCasting);
                    }
                }
                ListBoxTypeContratOffre.ItemsSource = items;
            }

        }


        //Methode lors de l'événement click du bouton d'ajout d'un type de contrat
        private void AddTypeContratButton_Click(object sender, RoutedEventArgs e)
        {
            TypeContratWindow typeContratWindow = new TypeContratWindow(db); //appel la fenetre pour ajouter un type de contrat

            if (typeContratWindow.ShowDialog() == true)
            {
                this.TypeContrats.Add(typeContratWindow.TypeContrat);
            }
        }

        //Methode lors de l'événement click du bouton suppression d'un type de contrat
        private void DellTypeContratButton_Click(object sender, RoutedEventArgs e)
        {
            if (listTypeContrats.SelectedItem != null)
            {
                bool typeContratToDelete = true;

                TypeContrat typeContrat = listTypeContrats.SelectedItem as TypeContrat;

                MessageBoxResult result = MessageBox.Show("Voulez-vous vraiment supprimmer ce type de contrat'" + typeContrat.LibelleType + "' ? ",
                "Suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    foreach (OffreCasting offreCasting in db.OffreCastings)
                    {
                        if (offreCasting.IdentifiantTypeContrat == typeContrat.Identifiant)
                        {
                            MessageBoxResult dellTypeContratResult = MessageBox.Show("Le type contrat est lié à l'offre '" + offreCasting.Intitulé + "', la supprimer ?", "Suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                            if (dellTypeContratResult == MessageBoxResult.Yes)
                            {
                                db.OffreCastings.Remove(offreCasting);
                                this.OffreCastings.Remove(offreCasting);
                            }
                            else if (dellTypeContratResult == MessageBoxResult.No)
                            {
                                typeContratToDelete = false;
                            }
                        }
                    }

                    if (typeContratToDelete)
                    {
                        //supprime l'offre selectionne et reselectionne une offre dans la liste 
                        int currentIndex = listTypeContrats.SelectedIndex;
                        db.TypeContrats.Remove(typeContrat);
                        this.TypeContrats.Remove(typeContrat);
                        listTypeContrats.Focus();
                        listTypeContrats.SelectedIndex = currentIndex;
                    }

                    Sauvegarde();
                }
            }
            else
            {
                MessageBox.Show("Veuillez selectionner un type de contrat.", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }

        //Methode lors de l'événement click du bouton d'ajout d'une offre pour un type de contrat
        private void AddTypeContratOffre(object sender, RoutedEventArgs e)
        {
            TypeContrat currentTypeContrat = listTypeContrats.SelectedItem as TypeContrat; //recupere le type de contrat selectionnée

            if (currentTypeContrat != null)
            {
                AjoutTypeContratOffreWindow ajoutTypeContratOffreWindow = new AjoutTypeContratOffreWindow(db); //appel la fenetre d'ajout d'une offre pour un type de contrat

                if (ajoutTypeContratOffreWindow.ShowDialog() == true)
                {

                    //cherche l'offre selectionnée dans la base et la rattache a un type de contrat
                    foreach (OffreCasting offreCasting in db.OffreCastings)
                    {
                        if (offreCasting.Identifiant == ajoutTypeContratOffreWindow.OffreCasting.Identifiant)
                        {
                            offreCasting.IdentifiantTypeContrat = currentTypeContrat.Identifiant;
                            (this.ListBoxTypeContratOffre.ItemsSource as List<OffreCasting>).Add(offreCasting);
                            this.ListBoxTypeContratOffre.Items.Refresh();
                        }
                    }
                }
            }
        }


        //Methode lors de l'événement click du bouton suppression d'une offre rattachée a un type de contrat
        private void DellTypeContratOffre(object sender, RoutedEventArgs e)
        {
            if (ListBoxTypeContratOffre.SelectedItem != null)
            {
                OffreCasting currentoffreCasting = ListBoxTypeContratOffre.SelectedItem as OffreCasting; //recupere l'offre de casting selectionnée

                int currentIndex = ListBoxTypeContratOffre.SelectedIndex;

                //cherche l'offre selectionnée et enleve le lien avec le type de contrat
                foreach (OffreCasting offreCasting in db.OffreCastings)
                {
                    if (currentoffreCasting.Identifiant == offreCasting.Identifiant)
                    {
                        offreCasting.IdentifiantTypeContrat = null;
                        (this.ListBoxTypeContratOffre.ItemsSource as List<OffreCasting>).Remove(offreCasting);
                        this.ListBoxTypeContratOffre.Items.Refresh();
                    }
                }



                ListBoxTypeContratOffre.Focus();
                ListBoxTypeContratOffre.SelectedIndex = currentIndex;
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une offre de casting.", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }

        #endregion


        #region DomaineMetier
        //Methode lors d'un changement d'item dans la liste domaine metier
        private void Listbox_DomaineMetier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Listbox_DomaineMetiers.SelectedItem != null)
            {
                //affiche les infos concernant le domaine metier et affiche la liste des metier liée au domaine metier selectionné, en dessous

                //remplie la liste métier en fonction du domaine metier selectionné
                List<Metier> items = new List<Metier>();
                DomaineMetier currentDomaineMetier = Listbox_DomaineMetiers.SelectedItem as DomaineMetier;
                foreach (Metier metier in db.Metiers)
                {
                    if (metier.IdentifiantDomaineMetier == currentDomaineMetier.Identifiant)
                    {
                        items.Add(metier);
                    }
                }
                Listbox_Metiers.ItemsSource = items;
            }
        }

        //Methode lors de l'événement click du bouton d'ajout d'un domaine metier
        private void AddDomaineMetierButton_Click(object sender, RoutedEventArgs e)
        {
            DomaineMetiersWindow domaineMetiersWindow = new DomaineMetiersWindow(db); //appel la fenetre pour ajouter un domaine metier

            if (domaineMetiersWindow.ShowDialog() == true)
            {
                this.DomaineMetiers.Add(domaineMetiersWindow.DomaineMetier);
            }
        }

        //Methode lors de l'événement click du bouton suppression d'un domaine metier
        private void DellDomaineMetierButton_Click(object sender, RoutedEventArgs e)
        {
            if (Listbox_DomaineMetiers.SelectedItem != null)
            {
                bool domaineMetierToDelete = true;

                DomaineMetier domaineMetier = Listbox_DomaineMetiers.SelectedItem as DomaineMetier;

                MessageBoxResult result = MessageBox.Show("Voulez-vous vraiment supprimmer ce domaine de métier'" + domaineMetier.Libelle + "' ? ",
                "Suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    foreach (Metier metier in db.Metiers)
                    {
                        if (metier.IdentifiantDomaineMetier == domaineMetier.Identifiant)
                        {
                            MessageBoxResult dellTypeContratResult = MessageBox.Show("Le domaine de métier est lié au metier '" + metier.Libelle + "', la supprimer ?", "Suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                            if (dellTypeContratResult == MessageBoxResult.Yes)
                            {
                                db.Metiers.Remove(metier);
                                this.Metiers.Remove(metier);
                            }
                            else if (dellTypeContratResult == MessageBoxResult.No)
                            {
                                domaineMetierToDelete = false;
                            }
                        }
                    }

                    if (domaineMetierToDelete)
                    {
                        //supprime l'offre selectionne et reselectionne une offre dans la liste 
                        int currentIndex = Listbox_DomaineMetiers.SelectedIndex;
                        db.DomaineMetiers.Remove(domaineMetier);
                        this.DomaineMetiers.Remove(domaineMetier);
                        Listbox_DomaineMetiers.Focus();
                        Listbox_DomaineMetiers.SelectedIndex = currentIndex;
                    }
                }

                else
                {
                    MessageBox.Show("Veuillez selectionner un domaine de métier.", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
                Sauvegarde();
            }
        }
        #endregion


        #region Metier
        //Methode lors de l'événement click du bouton d'ajout d'un métier
        private void AddMetierButton_Click(object sender, RoutedEventArgs e)
        {

            DomaineMetier currentDomaineMetier = Listbox_DomaineMetiers.SelectedItem as DomaineMetier; //recupere le metier selectionné
            MetiersWindow MetiersWindow = new MetiersWindow(db, currentDomaineMetier); //appel la fenetre d'ajout d'un metier

            if (MetiersWindow.ShowDialog() == true)
            {
                //cherche le metier selectionné dans la base, le rattache a un domaine metier et l'ajoute dans le liste
                foreach (Metier metier in db.Metiers)
                {
                    if (metier.Identifiant == MetiersWindow.metier.Identifiant)
                    {
                        metier.IdentifiantDomaineMetier = currentDomaineMetier.Identifiant;
                        (this.Listbox_Metiers.ItemsSource as List<Metier>).Add(metier);
                        this.Listbox_Metiers.Items.Refresh();
                    }
                }
            }
        }

        //Methode lors de l'événement click du bouton suppresion d'un metier
        private void DellMetierButton_Click(object sender, RoutedEventArgs e)
        {
            if (Listbox_Metiers.SelectedItem != null)
            {
                Metier currentMetier = Listbox_Metiers.SelectedItem as Metier; //recupere le metier selectionné

                MessageBoxResult result = MessageBox.Show("Êtes-vous sûr de vouloir supprimer définitivement le métier '" + currentMetier.Libelle + "' ?", "Suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning); //demande une confirmation de suppression du metier
                if (result == MessageBoxResult.Yes)
                {
                    //supprime le metier de la base et de la liste et reselectionne un autre metier dans la liste
                    int currentIndex = Listbox_Metiers.SelectedIndex;
                    db.Metiers.Remove(currentMetier);
                    Sauvegarde();
                    (this.Listbox_Metiers.ItemsSource as List<Metier>).Remove(currentMetier);
                    this.Listbox_Metiers.Items.Refresh();

                    Listbox_Metiers.Focus();
                    Listbox_Metiers.SelectedIndex = currentIndex;

                }
            }
            else
            {
                MessageBox.Show("Veuillez selectionner un métier.", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }

        //Methode lors d'un changement d'item dans la liste metier
        private void Listbox_Metiers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Listbox_Metiers.SelectedItem != null)
            {
                //affiche les infos du metier selectionné
                Grid_InfoMetier.Visibility = Visibility.Visible;

                //remplie la liste des offres rattachée au metier selectionnée
                List<OffreCasting> items = new List<OffreCasting>();
                Metier currentMetier = Listbox_Metiers.SelectedItem as Metier;
                foreach (OffreCasting offreCasting in db.OffreCastings)
                {
                    if (offreCasting.IdentifiantMetier == currentMetier.Identifiant)
                    {
                        items.Add(offreCasting);
                    }
                }
                Listbox_MetierOffre.ItemsSource = items;

            }
            else
            {
                Grid_InfoMetier.Visibility = Visibility.Hidden;
            }
        }

        //Methode lors de l'événement click du bouton d'ajout d'une offre a un metier
        private void AddMetierOffre(object sender, RoutedEventArgs e)
        {
            Metier currentMetier = Listbox_Metiers.SelectedItem as Metier; //recupere le metier selectionné

            if (currentMetier != null)
            {
                AjoutMetierOffreWindow ajoutMetierOffreWindow = new AjoutMetierOffreWindow(db); //appel la fenetre d'ajout d'une offre

                if (ajoutMetierOffreWindow.ShowDialog() == true)
                {

                    //cherche l'offre dans la base et la rattache au metier
                    foreach (OffreCasting offreCasting in db.OffreCastings)
                    {
                        if (offreCasting.Identifiant == ajoutMetierOffreWindow.OffreCasting.Identifiant)
                        {
                            offreCasting.IdentifiantMetier = currentMetier.Identifiant;
                            (this.Listbox_MetierOffre.ItemsSource as List<OffreCasting>).Add(offreCasting);
                            this.Listbox_MetierOffre.Items.Refresh();
                        }
                    }
                }
            }
        }

        //Methode lors de l'événement click du bouton suppression d'une offre a un metier
        private void DellMetierOffre(object sender, RoutedEventArgs e)
        {
            if (Listbox_MetierOffre.SelectedItem != null)
            {
                OffreCasting currentoffreCasting = Listbox_MetierOffre.SelectedItem as OffreCasting; //recupere l'offre selectionnée
                int currentIndex = Listbox_MetierOffre.SelectedIndex;

                //cherche l'offre dans la base et enleve le lient avec la metier selectionné
                foreach (OffreCasting offreCasting in db.OffreCastings)
                {
                    if (currentoffreCasting.Identifiant == offreCasting.Identifiant)
                    {
                        offreCasting.IdentifiantMetier = null;
                        (this.Listbox_MetierOffre.ItemsSource as List<OffreCasting>).Remove(offreCasting);
                        this.Listbox_MetierOffre.Items.Refresh();
                    }
                }



                Listbox_MetierOffre.Focus();
                Listbox_MetierOffre.SelectedIndex = currentIndex;
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une offre de casting.", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }
        #endregion


        #region PartenaireDiffusion

        private void listPartenaireDiffusions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        //Methode lors de l'événement click du bouton d'ajout d'un partenaire de diffusion
        private void AddPartenaireDiffusionButton_Click(object sender, RoutedEventArgs e)
        {
            PartenaireDiffusionsWindow partenaireDiffusionsWindow = new PartenaireDiffusionsWindow(db); //appel la fenetre d'ajout d'un partenaire de diffusion

            if (partenaireDiffusionsWindow.ShowDialog() == true)
            {
                //ajoute le partenaire de diffusion a la liste
                this.PartenaireDiffusions.Add(partenaireDiffusionsWindow.PartenaireDiffusion);
            }
        }

        //Methode lors de l'événement click du bouton suppression d'un partenaire de diffusion
        private void DellPartenaireDiffusionButton_Click(object sender, RoutedEventArgs e)
        {
            if (listPartenaireDiffusions.SelectedItem != null)
            {
                PartenaireDiffusion partenaireDiffusion = listPartenaireDiffusions.SelectedItem as PartenaireDiffusion; //recupere le partenaire de diffusion selectionné


                MessageBoxResult result = MessageBox.Show("Êtes-vous sûr de vouloir supprimer définitivement le partenaire de diffusion '" + partenaireDiffusion.Raison_Social + "' ?", "Suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning); //demande une confirmation de suppression du partenaire de diffusion

                if (result == MessageBoxResult.Yes)
                {
                    //supprime le partenaire de la base et de la liste
                    int currentIndex = listClients.SelectedIndex;
                    db.PartenaireDiffusions.Remove(partenaireDiffusion);
                    this.PartenaireDiffusions.Remove(partenaireDiffusion);
                    listClients.Focus();
                    listClients.SelectedIndex = currentIndex;

                    Sauvegarde();
                }

            }
            else
            {
                MessageBox.Show("Veuillez selectionner un partenaire de diffusion.", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
        }

        //Methode lors d'un changement d'item de la liste des partenaire de diffusion
        #endregion

        }
    }