namespace rouletteGambling.Utils.Responses
{
    public class BetResponse
    {
        public string GamblerId { get; set; }
        public string GamblerFullName { get; set; }
        public decimal CreditsBet { get; set; }
        public string BetType { get; set; }
        public int? BetNumber { get; set; }
        public string BetColor { get; set; }
    }
}
