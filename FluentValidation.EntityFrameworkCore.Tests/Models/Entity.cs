#nullable disable

namespace FluentValidation.EntityFrameworkCore.Tests.Models
{
    public class Entity
    {
        public int Id { get; set; }
        public string Null { get; set; }
        public string NotNull { get; set; }

        public string MaxLength10 { get; set; }
        public string FixedLength10 { get; set; }

        public decimal Precision5_Scale4 { get; set; }
    }
}