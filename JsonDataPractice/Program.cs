using System.Globalization;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

Console.WriteLine("JSON and Data Handling in C#");
Console.WriteLine(new string('=', 32));

RunBasicJsonExamples();
RunHandsOnExamples();
RunIplAnalyzer();

static void RunBasicJsonExamples()
{
	PrintSection("1. Basic JSON Handling");

	var student = new Student
	{
		Name = "Aisha",
		Age = 20,
		Subjects = new List<string> { "C#", "Database", "JSON" }
	};

	string studentJson = JsonConvert.SerializeObject(student, Formatting.Indented);
	Console.WriteLine("Student object as JSON:");
	Console.WriteLine(studentJson);

	var car = new Car { Brand = "Toyota", Model = "Corolla", Year = 2024 };
	Console.WriteLine($"Car object as JSON: {JsonConvert.SerializeObject(car)}");

	var students = new List<Student>
	{
		student,
		new Student { Name = "Ben", Age = 22, Subjects = new List<string> { "C#", "XML" } }
	};
	Console.WriteLine("List as a JSON array:");
	Console.WriteLine(JsonConvert.SerializeObject(students, Formatting.Indented));

	var olderStudents = students.Where(item => item.Age > 20).ToList();
	Console.WriteLine($"Students older than 20: {string.Join(", ", olderStudents.Select(item => item.Name))}");

	JObject firstObject = JObject.Parse("{ \"name\": \"Aisha\", \"age\": 20 }");
	JObject secondObject = JObject.Parse("{ \"email\": \"aisha@example.com\", \"course\": \"C#\" }");
	firstObject.Merge(secondObject, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
	Console.WriteLine($"Merged JSON objects: {firstObject}");

	string schemaText = """
	{
	  "type": "object",
	  "properties": {
		"name": { "type": "string" },
		"email": { "type": "string", "format": "email" }
	  },
	  "required": [ "name", "email" ]
	}
	""";
	JSchema schema = JSchema.Parse(schemaText);
	JObject profile = JObject.Parse("{ \"name\": \"Aisha\", \"email\": \"aisha@example.com\" }");
	Console.WriteLine($"Profile is valid: {profile.IsValid(schema, out IList<string> errors)}");
	if (errors.Count > 0)
	{
		Console.WriteLine(string.Join(Environment.NewLine, errors));
	}
}

static void RunHandsOnExamples()
{
	PrintSection("2. Hands-on Practice");

	JArray users = JArray.Parse(File.ReadAllText(Path.Combine("Data", "users.json")));
	Console.WriteLine("All keys and values in users.json:");
	foreach (JObject user in users.Cast<JObject>())
	{
		foreach (JProperty property in user.Properties())
		{
			Console.WriteLine($"  {property.Name}: {property.Value}");
		}
	}

	Console.WriteLine("Users older than 25:");
	foreach (JObject user in users.Where(item => (int)item["age"]! > 25))
	{
		Console.WriteLine($"  {user["name"]} ({user["age"]})");
	}

	JObject firstFile = JObject.Parse(File.ReadAllText(Path.Combine("Data", "merge-one.json")));
	JObject secondFile = JObject.Parse(File.ReadAllText(Path.Combine("Data", "merge-two.json")));
	firstFile.Merge(secondFile);
	File.WriteAllText("merged.json", firstFile.ToString(Formatting.Indented));
	Console.WriteLine("Merged JSON files into merged.json");

	XDocument xml = JsonConvert.DeserializeXNode(users.ToString(Formatting.None), "Users")!;
	File.WriteAllText("users.xml", xml.ToString());
	Console.WriteLine("Converted users.json into users.xml");

	var csvUsers = File.ReadAllLines(Path.Combine("Data", "users.csv"))
		.Skip(1)
		.Select(line => line.Split(','))
		.Select(columns => new User { Name = columns[0], Email = columns[1], Age = int.Parse(columns[2], CultureInfo.InvariantCulture) })
		.ToList();
	File.WriteAllText("users-from-csv.json", JsonConvert.SerializeObject(csvUsers, Formatting.Indented));
	Console.WriteLine("Converted users.csv into users-from-csv.json");

	var reportRows = new List<ReportRow>
	{
		new ReportRow { Category = "C#", Count = 12 },
		new ReportRow { Category = "JSON", Count = 8 }
	};
	File.WriteAllText("database-report.json", JsonConvert.SerializeObject(reportRows, Formatting.Indented));
	Console.WriteLine("Created database-report.json from sample database records");
}

static void RunIplAnalyzer()
{
	PrintSection("3. IPL and Censorship Analyzer");
	List<IplMatch> matches = JsonConvert.DeserializeObject<List<IplMatch>>(
		File.ReadAllText(Path.Combine("Data", "ipl-matches.json")))!;

	foreach (IplMatch match in matches)
	{
		match.Team1 = MaskTeamName(match.Team1);
		match.Team2 = MaskTeamName(match.Team2);
		match.Winner = MaskTeamName(match.Winner);
		match.PlayerOfMatch = "REDACTED";
		match.Score = match.Score.ToDictionary(score => MaskTeamName(score.Key), score => score.Value);
	}

	File.WriteAllText("ipl-matches-censored.json", JsonConvert.SerializeObject(matches, Formatting.Indented));

	List<IplCsvRow> csvMatches = File.ReadAllLines(Path.Combine("Data", "ipl-matches.csv"))
		.Skip(1)
		.Select(line => line.Split(','))
		.Select(columns => new IplCsvRow
		{
			MatchId = int.Parse(columns[0], CultureInfo.InvariantCulture),
			Team1 = MaskTeamName(columns[1]),
			Team2 = MaskTeamName(columns[2]),
			ScoreTeam1 = int.Parse(columns[3], CultureInfo.InvariantCulture),
			ScoreTeam2 = int.Parse(columns[4], CultureInfo.InvariantCulture),
			Winner = MaskTeamName(columns[5]),
			PlayerOfMatch = "REDACTED"
		})
		.ToList();

	var csvLines = new List<string> { "match_id,team1,team2,score_team1,score_team2,winner,player_of_match" };
	csvLines.AddRange(csvMatches.Select(match => string.Join(",", match.MatchId, match.Team1, match.Team2, match.ScoreTeam1, match.ScoreTeam2, match.Winner, match.PlayerOfMatch)));
	File.WriteAllLines("ipl-matches-censored.csv", csvLines);
	Console.WriteLine("Created ipl-matches-censored.json and ipl-matches-censored.csv");
}

static string MaskTeamName(string teamName)
{
	string[] words = teamName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
	if (words.Length == 1)
	{
		return "***";
	}

	words[1] = "***";
	return string.Join(' ', words);
}

static void PrintSection(string title)
{
	Console.WriteLine();
	Console.WriteLine(title);
	Console.WriteLine(new string('-', title.Length));
}

public class Student
{
	[JsonProperty("name")]
	public string Name { get; set; } = "";
	[JsonProperty("age")]
	public int Age { get; set; }
	[JsonProperty("subjects")]
	public List<string> Subjects { get; set; } = new();
}

public class Car
{
	public string Brand { get; set; } = "";
	public string Model { get; set; } = "";
	public int Year { get; set; }
}

public class User
{
	public string Name { get; set; } = "";
	public string Email { get; set; } = "";
	public int Age { get; set; }
}

public class ReportRow
{
	public string Category { get; set; } = "";
	public int Count { get; set; }
}

public class IplMatch
{
	[JsonProperty("match_id")]
	public int MatchId { get; set; }
	[JsonProperty("team1")]
	public string Team1 { get; set; } = "";
	[JsonProperty("team2")]
	public string Team2 { get; set; } = "";
	public Dictionary<string, int> Score { get; set; } = new();
	public string Winner { get; set; } = "";
	[JsonProperty("player_of_match")]
	public string PlayerOfMatch { get; set; } = "";
}

public class IplCsvRow
{
	public int MatchId { get; set; }
	public string Team1 { get; set; } = "";
	public string Team2 { get; set; } = "";
	public int ScoreTeam1 { get; set; }
	public int ScoreTeam2 { get; set; }
	public string Winner { get; set; } = "";
	public string PlayerOfMatch { get; set; } = "";
}
