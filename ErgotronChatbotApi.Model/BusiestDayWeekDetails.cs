using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErgotronChatbotApi.Model
{
    public class BusiestDayWeekDetails
    {
        public string PeakDayName { get; set; }
        public int PeakCarts { get; set; }
        public int AvgCarts { get; set; }
        public int MaxCarts { get; set; }
    }
}
