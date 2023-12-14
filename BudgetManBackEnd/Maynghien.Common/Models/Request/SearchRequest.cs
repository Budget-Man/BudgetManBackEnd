using System.ComponentModel.DataAnnotations;

namespace MayNghien.Models.Request.Base
{
    public class SearchRequest
    {
        public List<Filter>? Filters { get; set; }

        public SortByInfo? SortBy { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int? PageIndex { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int? PageSize { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
