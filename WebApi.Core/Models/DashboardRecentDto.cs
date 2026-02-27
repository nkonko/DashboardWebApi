namespace WebApi.Core.Models
{
    public class DashboardRecentDto
    {
        public IList<string> RecentActivities { get; set; } = new List<string>();
    }
}
