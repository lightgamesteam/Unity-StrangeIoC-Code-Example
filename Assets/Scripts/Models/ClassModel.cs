using Newtonsoft.Json;

namespace PFS.Assets.Scripts.Models
{
    public class ClassModel
    {
        [JsonProperty("displayName")]
        public string name;

        [JsonProperty("childrenCount")]
        public int studentsCount;

        [JsonProperty("allHomeworks")]
        public int allHomeworksCount;

        [JsonProperty("allFinished")]
        public int doneHomeworksCount;
    }
}