using System.Collections.Generic;

namespace DoDoModels
{
    public class WorkZone
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<Courier> PinnedCouriers { get; set; }
    }
}
