using System.ComponentModel.DataAnnotations;

namespace DunakanyarHouseIngatlan.DataAccess.Entities
{
    public class Popularity
    {
        [Key]
        public int Id { get; set; }
        public int DaySpan { get; set; }
        public int WeekSpan { get; set; }
        public int MonthSpan { get; set; }
        public int AllTime { get; set; }
        public int EstateId { get;  set; }
    }
}