namespace MailFlow.Dtos
{
    public class DashboardDto
    {
        public int TotalMessages { get; set; }
        public int StarredMessages { get; set; }
        public int ArchivedMessages { get; set; }
        public int UnreadMessages { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ImageUrl { get; set; }

    }
}
