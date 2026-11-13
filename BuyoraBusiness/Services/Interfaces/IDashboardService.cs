using BuyoraModels.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuyoraBusiness.Services.Interfaces
{
    public interface IDashboardService
    {
        public RevenueOverviewDto GetChartData();
    }
}
