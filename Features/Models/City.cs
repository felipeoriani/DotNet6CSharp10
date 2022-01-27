namespace Features.Models;

using SubModels;

internal class City
{
	public string? Name { get; set; }
	public string? Country { get; set; }
	public decimal Population { get; set; }
	public Neighborhood[]? Neighborhoods { get; set; }

	public override string ToString()
	{
		return $"{Name} - {Country}. Population: {Population}";
	}
}
