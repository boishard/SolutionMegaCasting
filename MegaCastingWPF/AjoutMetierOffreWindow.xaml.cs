﻿using MegaCasting.DBLib;
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
using System.Windows.Shapes;

namespace MegaCastingWPF
{
    /// <summary>
    /// Logique d'interaction pour AjoutMetierOffreWindow.xaml
    /// </summary>
    public partial class AjoutMetierOffreWindow : Window
    {
        private MegaProductionEntities2 db;

        public OffreCasting OffreCasting { get; set; }

        public AjoutMetierOffreWindow(MegaProductionEntities2 context)
        {
            InitializeComponent();
            db = context;

            //Remplissage de la liste d'offre, les donnée sont récupéré de la base
            List<OffreCasting> items = new List<OffreCasting>();
            foreach (OffreCasting offreCasting in db.OffreCastings)
            {
                items.Add(offreCasting);
            }
            listOffreMetierWindow.ItemsSource = items;


            this.OffreCasting = new OffreCasting();
            this.DataContext = this;
        }

        private void Validate_click(object sender, RoutedEventArgs e)
        {
            //Verifie qu'une offre est bien selectionnée, si oui on envoie true
            OffreCasting = listOffreMetierWindow.SelectedItem as OffreCasting;
            if (OffreCasting != null)
            {
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une offre de casting.", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            
        }

        private void Annuler_Click(object sender, RoutedEventArgs e)
        {

            this.DialogResult = false;
        }
    }
}
