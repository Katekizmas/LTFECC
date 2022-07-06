using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTFECC.Models
{
    public class QueueContendor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string IdUser { get; set; }
        public string Username { get; set; }
        public string Discriminator { get; set; }
        public string Raid { get; set; }
        public string TimeAdded { get; set; }
        public bool Completed { get; set; } 

        public QueueContendor() { }
    }
}
