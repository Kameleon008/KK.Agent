using KK.Agent.Library.Attributes;
using Newtonsoft.Json;

namespace KK.Agent.Library.Examples.Models;

public class CharacterWithPlanetInfo
{
    [Required]
    [JsonProperty("character", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    [PropertyDescription("Complete character profile with name, age, occupation, species, and personality")]
    public FictionalCharacter Character { get; set; } = null!;

    [Required]
    [JsonProperty("home_planet", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    [PropertyDescription("Detailed information about the planet where the character originates from")]
    public PlanetMetadata HomePlanet { get; set; } = null!;

    [JsonProperty("planet_connection", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    [PropertyDescription("How this character is connected to their home world - birthplace, exile, protector, etc.")]
    public string PlanetConnection { get; set; } = null!;

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}

public class FictionalCharacter
{
    [Required]
    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    [PropertyDescription("The full name of the character, including any titles or epithets")]
    public string Name { get; set; } = null!;

    [Required]
    [JsonProperty("age", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    [PropertyDescription("Age in years (positive integer)")]
    [Range(1, 500)]
    public int Age { get; set; }

    [Required]
    [JsonProperty("occupation", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    [PropertyDescription("The character's profession, role, or primary activity in life")]
    public string Occupation { get; set; } = null!;

    [Required]
    [JsonProperty("species", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    [PropertyDescription("The biological species or race the character belongs to")]
    public string Species { get; set; } = null!;

    [Required]
    [JsonProperty("personality", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    [PropertyDescription("A detailed description of personality, motivations, fears, and key character traits")]
    public string Personality { get; set; } = null!;
}

public class PlanetMetadata
{
    [Required]
    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    [PropertyDescription("The official name of the planet")]
    public required string Name { get; set; } = null!;

    [Required]
    [JsonProperty("planet_type", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    [PropertyDescription("Classification of the planet type")]
    [StringChoices("Terrestrial", "Gas Giant", "Ice Giant", "Rocky Planet", "Ocean World", "Desert Planet", "Volcanic World", "Tundra World", "Jungle Moon", "Space Station")]
    public required string PlanetType { get; set; } = null!;

    [Required]
    [JsonProperty("atmosphere", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    [PropertyDescription("Description of the atmosphere including composition and conditions")]
    [StringLength(50, 500)]
    public required string Atmosphere { get; set; } = null!;

    [Required]
    [JsonProperty("gravity", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    [PropertyDescription("Surface gravity as a multiplier of Earth's gravity")]
    [Range(0.1, 5.0)]
    public required double Gravity { get; set; }

    [Required]
    [JsonProperty("temperature_celsius", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    [PropertyDescription("Average temperature on the planet's surface")]
    [Range(-273, 6000)]
    public required double TemperatureCelsius { get; set; }

    [Required]
    [JsonProperty("has_life", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    [PropertyDescription("True if the planet supports any form of native life")]
    public required bool HasLife { get; set; }

    [Required]
    [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    [PropertyDescription("Notable geographical features, cities, historical events, or characteristics of this world")]
    [StringLength(100, 2000)]
    public required string Description { get; set; } = null!;
}


