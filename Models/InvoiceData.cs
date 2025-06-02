using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace testbills.Models
{
    public class InvoiceData
    {
        [Key]
        public int Id { get; set; }

        public string? Libelle { get; set; } = null;
        public string? Date { get; set; } = null;   
        public string? Montant_HT { get; set; }=null;
        public string? Montant_TTC { get; set; } = null;    
        public string? TVA { get; set; } =null;
      
        public List<TaxDetail>? TaxDetails { get; set; } = new();
    }
}
