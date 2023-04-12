namespace HW_163
{
    public class SqlProduct
    {
        public int ID { get; set; }
        public short ProductCode { get; set; }
        public string ProductName { get; set; }

        public override string ToString()
        {
            return $"{ProductCode} {ProductName}";
        }
    }
}
