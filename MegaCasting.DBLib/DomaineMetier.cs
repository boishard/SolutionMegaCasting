//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MegaCasting.DBLib
{
    using System;
    using System.Collections.Generic;
    
    public partial class DomaineMetier
    {
        public DomaineMetier()
        {
            this.Metiers = new HashSet<Metier>();
        }
    
        public long Identifiant { get; set; }
        public string Libelle { get; set; }
    
        public virtual ICollection<Metier> Metiers { get; set; }
    }
}
