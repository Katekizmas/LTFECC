using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTFECC.Models
{
    public class Donators
    {
        public string Id_server { get; set; }
        public string Id_user { get; set; }
        public string Username { get; set; }
        public string Amount { get; set; }
        public Donators(string Id_server, string Id_user, string Username, string Amount)
        {
            this.Id_server = Id_server;
            this.Id_user = Id_user;
            this.Username = Username;
            this.Amount = Amount;
        }
        public Donators()
        { }
    }
}
