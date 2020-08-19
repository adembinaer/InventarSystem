using DuplicateCheckerLib;

namespace LogSystem.Models
{
    public class Location : IEntity
    {
        public int Id { get; set; }
        public int FK_PointOfDelivery { get; set; }
        public int FK_Address { get; set; }
    }
}
