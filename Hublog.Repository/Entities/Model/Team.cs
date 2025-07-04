﻿namespace Hublog.Repository.Entities.Model
{
    public class Team
    {
        public int Id { get; set; } //(int, not null)
        public string Name { get; set; } //(varchar(100), not null)
        public bool Active { get; set; } //(bit, not null)
        public string Description { get; set; } //(varchar(200), null)
        public int OrganizationId { get; set; } //(int, not null)
        public int? ShiftId { get; set; } //(int, not null)
        public Nullable<int> Parentid { get; set; } //(int, null)
    }
}
