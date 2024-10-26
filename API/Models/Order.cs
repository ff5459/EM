namespace API.Models
{
    public class Order
    {
        public int Id { get; set; }
        public double Kg { get; set; }
        public string Region { get; set; }
        public DateTime Date { get; set; }
    }
}