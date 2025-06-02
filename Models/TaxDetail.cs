using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace testbills.Models
{
    public class TaxDetail
    {
        [Key]
        public int Id { get; set; }

        public string TaxAmount { get; set; }
        public string TaxRate { get; set; }

        public int InvoiceDataId { get; set; }

        [ForeignKey("InvoiceDataId")]
        public InvoiceData InvoiceData { get; set; }
    }
}
