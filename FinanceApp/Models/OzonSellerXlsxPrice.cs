
namespace FinanceApp.Models
{
    public class OzonSellerXlsxPrice
    {
        public string? Article {  get; set; }
        public decimal Price {  get; set; }
        public decimal Acquiring { get; set; }
        public decimal OzonRewardFBS { get; set; }
        public decimal ShipmentProcessingFBS { get; set; }
        public decimal OzonLogisticsFBS { get; set; }
        public decimal DeliveryPickupPointFBS { get; set; }

        public decimal ExpensesSum => Math.Round(Acquiring + ShipmentProcessingFBS + OzonLogisticsFBS + DeliveryPickupPointFBS + ((Price * OzonRewardFBS) / 100m), 2);
    }
}
