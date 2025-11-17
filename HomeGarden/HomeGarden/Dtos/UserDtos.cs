namespace HomeGarden.Dtos
{
    public class UserListDto
    {
        public long UserId { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string RoleName { get; set; }
        public string Status { get; set; }
    }

    public class UserUpdateDto
    {
        public string? Fullname { get; set; }
        public string? Phone { get; set; }
        public int? StatusId { get; set; }
    }
}
