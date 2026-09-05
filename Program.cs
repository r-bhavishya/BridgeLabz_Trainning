using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

const string DataFolder = "data";
Directory.CreateDirectory(DataFolder);
CreateSampleFiles();

while (true)
{
	Console.WriteLine("\n=== CSV Data Handling Practice ===");
	Console.WriteLine("1. Read and print students");
	Console.WriteLine("2. Write employee CSV");
	Console.WriteLine("3. Count CSV rows");
	Console.WriteLine("4. Filter students with marks over 80");
	Console.WriteLine("5. Search for an employee");
	Console.WriteLine("6. Increase IT salaries by 10%");
	Console.WriteLine("7. Show top 5 salaries");
	Console.WriteLine("8. Validate email and phone data");
	Console.WriteLine("9. Convert CSV rows into Student objects");
	Console.WriteLine("10. Merge two CSV files by ID");
	Console.WriteLine("11. Read a large CSV in chunks");
	Console.WriteLine("12. Detect duplicate IDs");
	Console.WriteLine("13. Create a report from database-like data");
	Console.WriteLine("14. Convert JSON to CSV and back");
	Console.WriteLine("15. Encrypt and decrypt sensitive CSV fields");
	Console.WriteLine("0. Exit");
	Console.Write("Choose an exercise: ");

	string choice = Console.ReadLine() ?? "0";
	Console.WriteLine();

	switch (choice)
	{
		case "1": ReadAndPrintStudents(); break;
		case "2": WriteEmployeeFile(); break;
		case "3": CountRows(); break;
		case "4": FilterStudents(); break;
		case "5": SearchEmployee(); break;
		case "6": IncreaseItSalaries(); break;
		case "7": SortEmployees(); break;
		case "8": ValidateContacts(); break;
		case "9": ConvertStudentsToObjects(); break;
		case "10": MergeFiles(); break;
		case "11": ReadLargeFileInChunks(); break;
		case "12": FindDuplicateIds(); break;
		case "13": CreateDatabaseReport(); break;
		case "14": ConvertJsonAndCsv(); break;
		case "15": EncryptAndDecryptFile(); break;
		case "0": return;
		default: Console.WriteLine("Please choose a number from 0 to 15."); break;
	}

	Console.WriteLine("\nPress Enter to return to the menu.");
	Console.ReadLine();
}

static void ReadAndPrintStudents()
{
	Console.WriteLine("Students:");
	foreach (string[] row in ReadCsv("students.csv"))
	{
		Console.WriteLine($"ID: {row[0]}, Name: {row[1]}, Age: {row[2]}, Marks: {row[3]}");
	}
}

static void WriteEmployeeFile()
{
	List<string[]> employees = new()
	{
		new[] { "101", "Alice", "IT", "65000" },
		new[] { "102", "Bob", "HR", "52000" },
		new[] { "103", "Carla", "IT", "72000" },
		new[] { "104", "David", "Sales", "58000" },
		new[] { "105", "Eva", "Finance", "81000" }
	};

	WriteCsv("created-employees.csv", new[] { "ID", "Name", "Department", "Salary" }, employees);
	Console.WriteLine("Created data/created-employees.csv with 5 records.");
}

static void CountRows()
{
	int count = ReadCsv("students.csv").Count;
	Console.WriteLine($"There are {count} student records, excluding the header.");
}

static void FilterStudents()
{
	Console.WriteLine("Students with marks over 80:");
	foreach (string[] row in ReadCsv("students.csv"))
	{
		if (double.Parse(row[3], CultureInfo.InvariantCulture) > 80)
		{
			Console.WriteLine(string.Join(" | ", row));
		}
	}
}

static void SearchEmployee()
{
	Console.Write("Enter an employee name: ");
	string name = Console.ReadLine() ?? "";
	string[]? employee = ReadCsv("employees.csv")
		.FirstOrDefault(row => row[1].Equals(name, StringComparison.OrdinalIgnoreCase));

	if (employee is null)
	{
		Console.WriteLine("Employee not found.");
		return;
	}

	Console.WriteLine($"Department: {employee[2]}, Salary: {employee[3]}");
}

static void IncreaseItSalaries()
{
	List<string[]> employees = ReadCsv("employees.csv");
	foreach (string[] employee in employees)
	{
		if (employee[2].Equals("IT", StringComparison.OrdinalIgnoreCase))
		{
			decimal salary = decimal.Parse(employee[3], CultureInfo.InvariantCulture);
			employee[3] = (salary * 1.10m).ToString("0.00", CultureInfo.InvariantCulture);
		}
	}

	WriteCsv("updated-employees.csv", new[] { "ID", "Name", "Department", "Salary" }, employees);
	Console.WriteLine("Saved the updated file to data/updated-employees.csv.");
}

static void SortEmployees()
{
	List<string[]> employees = ReadCsv("employees.csv")
		.OrderByDescending(row => decimal.Parse(row[3], CultureInfo.InvariantCulture))
		.ToList();

	Console.WriteLine("Top 5 highest-paid employees:");
	foreach (string[] employee in employees.Take(5))
	{
		Console.WriteLine($"{employee[1]} - {employee[3]}");
	}
}

static void ValidateContacts()
{
	Regex emailPattern = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
	Regex phonePattern = new(@"^\d{10}$");

	foreach (string[] row in ReadCsv("contacts.csv"))
	{
		List<string> errors = new();
		if (!emailPattern.IsMatch(row[2])) errors.Add("invalid email");
		if (!phonePattern.IsMatch(row[3])) errors.Add("phone must contain exactly 10 digits");
		if (errors.Count > 0) Console.WriteLine($"Row {row[0]}: {string.Join(", ", errors)}");
	}
}

static void ConvertStudentsToObjects()
{
	List<Student> students = ReadCsv("students.csv")
		.Select(row => new Student(int.Parse(row[0]), row[1], int.Parse(row[2]), double.Parse(row[3], CultureInfo.InvariantCulture)))
		.ToList();

	foreach (Student student in students)
	{
		Console.WriteLine($"{student.Id}: {student.Name}, age {student.Age}, marks {student.Marks}");
	}
}

static void MergeFiles()
{
	Dictionary<string, string[]> firstFile = ReadCsv("part-one.csv").ToDictionary(row => row[0]);
	List<string[]> merged = new();

	foreach (string[] secondRow in ReadCsv("part-two.csv"))
	{
		if (firstFile.TryGetValue(secondRow[0], out string[]? firstRow))
		{
			merged.Add(firstRow.Concat(secondRow.Skip(1)).ToArray());
		}
	}

	WriteCsv("merged.csv", new[] { "ID", "Name", "Department", "Salary" }, merged);
	Console.WriteLine("Merged matching IDs into data/merged.csv.");
}

static void ReadLargeFileInChunks()
{
	int processed = 0;
	using StreamReader reader = new(Path.Combine(DataFolder, "large.csv"));
	reader.ReadLine();
	while (reader.ReadLine() is not null)
	{
		processed++;
		if (processed % 100 == 0) Console.WriteLine($"Processed {processed} records...");
	}

	Console.WriteLine($"Finished. Total records processed: {processed}.");
}

static void FindDuplicateIds()
{
	List<string[]> rows = ReadCsv("duplicates.csv");
	foreach (var group in rows.GroupBy(row => row[0]).Where(group => group.Count() > 1))
	{
		Console.WriteLine($"Duplicate ID: {group.Key}");
		foreach (string[] row in group) Console.WriteLine("  " + string.Join(" | ", row));
	}
}

static void CreateDatabaseReport()
{
	// Replace this list with rows returned by a real database query later.
	List<string[]> databaseRows = ReadCsv("employees.csv");
	WriteCsv("database-report.csv", new[] { "Employee ID", "Name", "Department", "Salary" }, databaseRows);
	Console.WriteLine("Created data/database-report.csv from sample database-like data.");
}

static void ConvertJsonAndCsv()
{
	List<Student> students = JsonSerializer.Deserialize<List<Student>>(
		File.ReadAllText(Path.Combine(DataFolder, "students.json"))) ?? new();
	List<string[]> rows = students.Select(student => new[]
	{
		student.Id.ToString(), student.Name, student.Age.ToString(), student.Marks.ToString(CultureInfo.InvariantCulture)
	}).ToList();

	WriteCsv("students-from-json.csv", new[] { "ID", "Name", "Age", "Marks" }, rows);
	List<Student> studentsAgain = ReadCsv("students-from-json.csv")
		.Select(row => new Student(int.Parse(row[0]), row[1], int.Parse(row[2]), double.Parse(row[3], CultureInfo.InvariantCulture)))
		.ToList();
	File.WriteAllText(Path.Combine(DataFolder, "students-again.json"), JsonSerializer.Serialize(studentsAgain, new JsonSerializerOptions { WriteIndented = true }));
	Console.WriteLine("Created students-from-json.csv and students-again.json.");
}

static void EncryptAndDecryptFile()
{
	List<string[]> encryptedRows = ReadCsv("employees.csv").Select(row => new[]
	{
		row[0], row[1], row[2], Encrypt(row[3])
	}).ToList();
	WriteCsv("encrypted-employees.csv", new[] { "ID", "Name", "Department", "EncryptedSalary" }, encryptedRows);

	Console.WriteLine("Encrypted salary values:");
	foreach (string[] row in encryptedRows)
	{
		Console.WriteLine($"{row[1]}: {Decrypt(row[3])}");
	}
}

static List<string[]> ReadCsv(string fileName)
{
	string path = Path.Combine(DataFolder, fileName);
	return File.ReadLines(path).Skip(1)
		.Where(line => !string.IsNullOrWhiteSpace(line))
		.Select(line => line.Split(','))
		.ToList();
}

static void WriteCsv(string fileName, string[] headers, IEnumerable<string[]> rows)
{
	string path = Path.Combine(DataFolder, fileName);
	using StreamWriter writer = new(path);
	writer.WriteLine(string.Join(',', headers));
	foreach (string[] row in rows) writer.WriteLine(string.Join(',', row));
}

static string Encrypt(string value)
{
	using Aes aes = Aes.Create();
	aes.Key = Encoding.UTF8.GetBytes("12345678901234567890123456789012");
	aes.IV = new byte[16];
	using ICryptoTransform encryptor = aes.CreateEncryptor();
	byte[] bytes = encryptor.TransformFinalBlock(Encoding.UTF8.GetBytes(value), 0, value.Length);
	return Convert.ToBase64String(bytes);
}

static string Decrypt(string value)
{
	using Aes aes = Aes.Create();
	aes.Key = Encoding.UTF8.GetBytes("12345678901234567890123456789012");
	aes.IV = new byte[16];
	using ICryptoTransform decryptor = aes.CreateDecryptor();
	byte[] encryptedBytes = Convert.FromBase64String(value);
	byte[] bytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
	return Encoding.UTF8.GetString(bytes);
}

static void CreateSampleFiles()
{
	WriteCsv("students.csv", new[] { "ID", "Name", "Age", "Marks" }, new[]
	{
		new[] { "1", "Asha", "20", "91" }, new[] { "2", "Ben", "21", "75" }, new[] { "3", "Chen", "20", "88" }
	});
	WriteCsv("employees.csv", new[] { "ID", "Name", "Department", "Salary" }, new[]
	{
		new[] { "101", "Alice", "IT", "65000" }, new[] { "102", "Bob", "HR", "52000" }, new[] { "103", "Carla", "IT", "72000" },
		new[] { "104", "David", "Sales", "58000" }, new[] { "105", "Eva", "Finance", "81000" }, new[] { "106", "Alice", "IT", "69000" }
	});
	WriteCsv("contacts.csv", new[] { "ID", "Name", "Email", "Phone" }, new[]
	{
		new[] { "1", "Asha", "asha@example.com", "1234567890" }, new[] { "2", "Ben", "wrong-email", "12345" }
	});
	WriteCsv("part-one.csv", new[] { "ID", "Name" }, new[] { new[] { "1", "Asha" }, new[] { "2", "Ben" } });
	WriteCsv("part-two.csv", new[] { "ID", "Department", "Salary" }, new[] { new[] { "1", "IT", "65000" }, new[] { "2", "HR", "52000" } });
	WriteCsv("duplicates.csv", new[] { "ID", "Name" }, new[] { new[] { "1", "Asha" }, new[] { "2", "Ben" }, new[] { "1", "Asha again" } });
	WriteCsv("large.csv", new[] { "ID", "Value" }, Enumerable.Range(1, 250).Select(number => new[] { number.ToString(), $"Value {number}" }));
	File.WriteAllText(Path.Combine(DataFolder, "students.json"), JsonSerializer.Serialize(new[]
	{
		new Student(1, "Asha", 20, 91), new Student(2, "Ben", 21, 75)
	}, new JsonSerializerOptions { WriteIndented = true }));
}

public record Student(int Id, string Name, int Age, double Marks);
