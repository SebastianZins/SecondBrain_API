using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecondBrain.Models.DatabaseModels.Neo4j
{
    public class BaseNode
    {
        public Guid id { get; set; } = Guid.Empty;
    }
}
