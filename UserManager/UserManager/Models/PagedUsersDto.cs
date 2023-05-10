namespace UserManager.Models
{
    public class PagedUsersDto
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public UserDto[] Users { get; set; }
    }
}
