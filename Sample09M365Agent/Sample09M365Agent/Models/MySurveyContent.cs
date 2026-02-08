using System.Text.Json.Serialization;

namespace Sample09M365Agent.Models
{
    public class MySurveyContent
    {
        [JsonIgnore]
        public string Id { get; set; }
        [JsonIgnore]
        public string Message { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string FavoriteFood { get; set; }
        public string Hobby { get; set; }
    }
}
