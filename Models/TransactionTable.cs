namespace ucsUpdatedApp.Models
{
    public class TransactionTable
    {
        public int Id { get; set; }
        public int MasterId { get; set; }

        public int Op { get; set; } // 1 for Check-In, 0 for Check-Out
        public DateTime OpDateTime { get; set; }
        public string CheckInMethod { get; set; }  // "UserId" or "FingerPrint"

        public DateTime? LatestTransactionDate { get; set; }
        // Foreign key relationship
        public MasterTable Master { get; set; }
    }
}
