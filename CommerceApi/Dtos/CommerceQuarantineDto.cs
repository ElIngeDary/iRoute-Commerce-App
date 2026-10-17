namespace CommerceApi.Dtos
{
    public class CommerceQuarantineDto
    {
        public int Id { get; set; }
        public string? PcNomcomred { get; set; }
        public string? PcNumdoc { get; set; }
        public DateTime? PcProcessdate { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public DateTime FechaCuarentena { get; set; }
    }
}