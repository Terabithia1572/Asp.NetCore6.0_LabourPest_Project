using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Log
    {
		public int Id { get; set; }
		public string UserName { get; set; }
		public DateTime Date { get; set; }
		public string Action { get; set; }
		public bool Success { get; set; }

		// Yeni eklenen özellik:
		public string IPAddress { get; set; }
	}
}
