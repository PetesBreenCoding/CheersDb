using System.Text.Json.Serialization;

namespace CheersDb.Api.Dtos;

public class ProblemDetailsDto
{
	public string? Type { get; set; }

	public string? Title { get; set; }

	public int? Status { get; set; }
	
	public string? Detail { get; set; }

	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	[JsonPropertyOrder(-1)]
	[JsonPropertyName("instance")]
	public string? Instance { get; set; }
}
