namespace MayNghien.Common.Models
{
    public class BaseDto
    {
        public Guid? Id { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? CreatedOn { get; set; }

    }
}
