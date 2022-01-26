using Features.Models;
using System;
using System.Linq;
using System.Collections.Generic;

// Constant Interpolated Strings





List<City> cities = new List<City>()
{
	new City() { Name = "Los Angeles", Country = "United States", Population = 3_900_000 },
	new City() { Name = "Kyev", Country = "Ukraine", Population = 2_800_000 },
	new City() { Name = "Campinas", Country = "Brazil", Population = 1_200_000 },
	new City() { Name = "Omaha", Country = "United States", Population = 475_000 },
	new City() { Name = "Sao Paulo", Country = "Brazil", Population = 12_000_000 },
	new City() { Name = "Odessa", Country = "Ukraine", Population = 900_000 }
};

// LINQ: New features and improvements

/*
// How can we get the city with the largest population?

var bigestPopulationCity = ?

Console.WriteLine(bigestPopulationCity);

// How can we get the city with the smallest population?

var smallestPopulationCity = ?

Console.WriteLine(smallestPopulationCity);
*/

// FirstOrDefault, LastOrDefault

string[] names = new string[] 
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

// Take and Ranges
Range r = 1..^1;
var results = names.Take(r);

foreach (var name in results)
	Console.WriteLine(name);

// File-scoped namespaces and Global Usings
