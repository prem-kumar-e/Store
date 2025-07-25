namespace Store.Models
{
    public class Setting
    {
        public int Id { get; set; }
        public required string Key { get; set; }
        public required string Value { get; set; }
       
    }
    public class AdminSetting
    {
        public int Id { get; set; }
        public int MaxUsers { get; set; }
        public decimal Tax { get; set; }
        public string? Name { get; set; }
        public string? Currency { get; set; }
    }
}