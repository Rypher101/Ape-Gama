namespace ApeGama.Shared
{
    public class ConfirmOrderModel
    {
        public int shopID { get; set; }
        public string shop { get; set; }
        public string address { get; set; }
        public string contact { get; set; }
        public decimal total { get; set; } = 0;
    }
}
