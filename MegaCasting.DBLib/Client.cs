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
    
    public partial class Client
    {
        public Client()
        {
            this.OffreCastings = new HashSet<OffreCasting>();
        }
    
        public long Identifiant { get; set; }
        public string Adresse { get; set; }
        public string Raison_Social { get; set; }
        public string Siret { get; set; }
    
        public virtual ICollection<OffreCasting> OffreCastings { get; set; }
    }
}
