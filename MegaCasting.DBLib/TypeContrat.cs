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
    
    public partial class TypeContrat
    {
        public TypeContrat()
        {
            this.OffreCastings = new HashSet<OffreCasting>();
        }
    
        public long Identifiant { get; set; }
        public string LibelleType { get; set; }
    
        public virtual ICollection<OffreCasting> OffreCastings { get; set; }
    }
}
