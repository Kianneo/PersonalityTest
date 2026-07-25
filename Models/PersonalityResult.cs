namespace PersonalityTest.Models
{
    public class PersonalityResult
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        // New: percentage strength (0-100) of each side, used for the bar graphic
        public int PctE { get; set; }
        public int PctI { get; set; }
        public int PctN { get; set; }
        public int PctS { get; set; }
        public int PctT { get; set; }
        public int PctF { get; set; }
        public int PctJ { get; set; }
        public int PctP { get; set; }
    }
}