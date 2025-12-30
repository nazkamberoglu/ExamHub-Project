using System.Text.Json.Serialization;

namespace ExamHubProject.Models.Entities
{
}

public class ExamSubmitDTO
{
    public int ExamId { get; set; }
    public List<AnswerDTO> Answers { get; set; }
}

public class AnswerDTO
{
    [JsonPropertyName("QuestionNo")]
    public int QuestionNo { get; set; }

    [JsonPropertyName("Answer")]
    public string Answer { get; set; }

   
}