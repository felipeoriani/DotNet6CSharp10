using Features.Models;
using Features.Clients;

// Constant Interpolated Strings
ApiClient client = new ApiClient();

// LINQ: New features and improvements

List<City> cities = new List<City>()
{
	new City() { Name = "Los Angeles", Country = "United States", Population = 3_900_000 },
	new City() { Name = "Kyev", Country = "Ukraine", Population = 2_800_000 },
	new City() { Name = "Campinas", Country = "Brazil", Population = 1_200_000 },
	new City() { Name = "Omaha", Country = "United States", Population = 475_000 },
	new City() { Name = "Sao Paulo", Country = "Brazil", Population = 12_000_000 },
	new City() { Name = "Odessa", Country = "Ukraine", Population = 900_000 }
};

// How can we get the city with the largest population?

City? bigestPopulationCity = cities.MaxBy(x => x.Population);

Console.WriteLine(bigestPopulationCity);

// How can we get the city with the smallest population?

City? smallestPopulationCity = cities.MinBy(x => x.Population);

Console.WriteLine(smallestPopulationCity);

string[] names =
{
	"Jacob",
	"Hendra",
	"Jeff",
	"Lucas",
	"Daniel",
	"Felipe",
	"Fernando",
	"Dasha"
};

string[] team =
{
	"Dasha",
	"Denis",
	"Felipe"
};

// FirstOrDefault: Now it contains a default value

var firstNameWithA = names.FirstOrDefault(x => x.StartsWith("A"), "### No name found ###");

// LastOrDefault: Now it contains a default value in case 

var lastNameWithB = names.LastOrDefault(x => x.StartsWith("B"), "### No name found ###");


// New method inspired on set theory
Console.WriteLine("Union");
foreach (var name in names.UnionBy(team, x => x))
	Console.WriteLine(name);

Console.WriteLine("Intersect");
foreach (var name in names.IntersectBy(team, x => x))
	Console.WriteLine(name);

Console.WriteLine("Except");
foreach (var name in names.ExceptBy(team, x => x))
	Console.WriteLine(name);

// Take and Ranges


Console.WriteLine("2..5: start on the 2nd and end on 5th element");
foreach (var name in names.Take(2..^5))
	Console.WriteLine(name);

Console.WriteLine("1..^1: start on the 1st and end on the penultimate");
foreach (var name in names.Take(1..^1))
	Console.WriteLine(name);

Console.WriteLine("^4..^2: counting backwards, start on the 4th and end on the 2nd.");
foreach (var name in names.Take(^4..^2))
	Console.WriteLine(name);
