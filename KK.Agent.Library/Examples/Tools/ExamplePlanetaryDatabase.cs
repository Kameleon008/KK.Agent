using KK.Agent.Library.Attributes;

namespace KK.Agent.Library.Examples.Tools;

public class ExamplePlanetaryDatabase
{
    [AgentTool("Retrieve detailed information about a specific planet from the galactic database.")]
    public async Task<PlanetData> get_planet_info(string planet_name)
    {
        await Task.Delay(300);

        // Simulated planetary database
        var planets = new Dictionary<string, PlanetData>
        {
            ["Tatooine"] = new()
            {
                Name = "Tatooine",
                Type = "Desert",
                Population = 200000,
                Climate = "Arid, temperate",
                Terrain = "Deserts, mountains, mesas",
                Gravity = "1 standard",
                Description = "A harsh desert planet in the Outer Rim Territories. Home to Luke Skywalker and various moisture farmers."
            },
            ["Coruscant"] = new()
            {
                Name = "Coruscant",
                Type = "Industrial",
                Population = 1000000000000L,
                Climate = "Temperate",
                Terrain = "Cityscape (entire planet)",
                Gravity = "1 standard",
                Description = "The capital world of the Galactic Republic and later Empire. A fully urbanized planet with no natural landscape."
            },
            ["Hoth"] = new()
            {
                Name = "Hoth",
                Type = "Frozen",
                Population = 0,
                Climate = "Polar cold",
                Terrain = "Ice plains, glaciers, mountains",
                Gravity = "1.25 standard",
                Description = "A remote ice planet in the Outer Rim. Site of Rebel Alliance base Echo Base and a major Imperial defeat."
            },
            ["Naboo"] = new()
            {
                Name = "Naboo",
                Type = "Temperate",
                Population = 4500000000L,
                Climate = "Temperate, tropical",
                Terrain = "Plains, forests, mountains, swamps",
                Gravity = "1 standard",
                Description = "A peaceful planet known for its grassy plains and Gungan cities. Home of Padmé Amidala and Queen Amidala."
            },
            ["Endor"] = new()
            {
                Name = "Endor",
                Type = "Temperate",
                Population = 20000000,
                Climate = "Temperate",
                Terrain = "Forests, grasslands, mountains",
                Gravity = "1 standard",
                Description = "A forest moon with Ewok civilization. Site of the Empire's Death Star II and final battle in Return of the Jedi."
            }
        };

        if (planets.TryGetValue(planet_name, out var planet))
        {
            return planet;
        }

        // Default fallback response for unknown planets
        return new PlanetData
        {
            Name = planet_name,
            Type = "Unknown",
            Population = 0,
            Climate = "Unexplored",
            Terrain = "Unknown",
            Gravity = "Variable",
            Description = $"No information found for planet '{planet_name}' in the galactic database."
        };
    }

    [AgentTool("Search for planets matching specific criteria from the planetary database.")]
    public async Task<List<PlanetData>> search_planets(
        [ParameterDescription("Type of climate to filter by (e.g., 'desert', 'frozen', 'temperate')")] string? climate = null,
        [ParameterDescription("Minimum population threshold")] long minPopulation = 0)
    {
        await Task.Delay(200);

        var planets = new List<PlanetData>
        {
            new() { Name = "Tatooine", Type = "Desert", Population = 200000, Climate = "Arid", Terrain = "Deserts", Gravity = "1 standard" },
            new() { Name = "Coruscant", Type = "Industrial", Population = 1000000000000L, Climate = "Temperate", Terrain = "Cityscape", Gravity = "1 standard" },
            new() { Name = "Hoth", Type = "Frozen", Population = 0, Climate = "Polar cold", Terrain = "Ice plains", Gravity = "1.25 standard" },
            new() { Name = "Naboo", Type = "Temperate", Population = 4500000000L, Climate = "Temperate", Terrain = "Plains", Gravity = "1 standard" },
            new() { Name = "Endor", Type = "Temperate", Population = 20000000, Climate = "Temperate", Terrain = "Forests", Gravity = "1 standard" }
        };

        var results = planets.Where(p => 
            (climate == null || p.Climate.Contains(climate, StringComparison.OrdinalIgnoreCase)) &&
            p.Population >= minPopulation).ToList();

        return results.Count > 0 ? results : new List<PlanetData> { 
            new() { Name = "No Matches", Type = "N/A", Population = 0, Climate = climate ?? "Any", Terrain = "N/A", Gravity = "N/A" }
        };
    }
}
