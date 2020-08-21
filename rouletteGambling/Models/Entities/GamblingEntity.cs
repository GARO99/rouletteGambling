namespace rouletteGambling.Models.Entities
{
    public class GamblingEntity
    {
        public int BetId { get; set; }
        public string GamblerId { get; set; }
        public decimal CreditsBet { get; set; }
        public int BetType { get; set; }
        public int? BetNumber { get; set; }
        public int? BetColor { get; set; }
        public bool? WonBet { get; set; }
    }
}
