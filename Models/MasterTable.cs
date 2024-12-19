namespace ucsUpdatedApp.Models
{
    public class MasterTable
    {
        public int MasterId { get; set; }
        public int EmployeeId { get; set; }
        public string Employeename { get; set; }
        public string FingerPrintData { get; set; }
        public DateTime? LastTransactionDate { get; set; }

        // Navigation property
        public ICollection<TransactionTable> Transactions { get; set; }
    }
}
