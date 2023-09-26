using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels;

public class DashboardVm
{
    public int TotalOrderCount { get; set; }
    public double TotalOrderPrice { get; set; }
    public int TotalTableCount { get; set; }
    public int TotalUserCount { get; set; }
}
