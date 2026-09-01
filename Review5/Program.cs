using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
class RequiredFieldAttribute : Attribute{
    public string Name;
    public RequiredFieldAttribute(string name) => Name = name;
}

[RequiredField("InvoiceNumber")]
[RequiredField("Date")]
public class InvoiceRules { }

[RequiredField("ContractNumber")]
[RequiredField("StartDate")]
public class ContractRules { }

public class DocumentRecord
{
    public string DocumentId, DocumentType;
    public Dictionary<string, string> ParsedFields;
}

public class MissingRequiredFieldException : Exception
{
    public MissingRequiredFieldException(string id, string field)
        : base($"Missing required field: {field}") { }
}

public class MalformedDocumentException : Exception
{
    public MalformedDocumentException(string id)
        : base($"Malformed document: {id}") { }
}

public class ArchiveIndex
{
    public bool Open = true;
    public List<DocumentRecord> Data = new();

    public void Add(DocumentRecord d)
    {
        if (!Open) throw new ObjectDisposedException("ArchiveIndex");
        Data.Add(d);
    }

    public void Close() => Open = false;
}

public class DocumentIngestionSession : IDisposable
{
    public ArchiveIndex Archive = new();
    public List<DocumentRecord> Archived = new();

    public void Dispose()
    {
        Archived.AddRange(Archive.Data);
        Archive.Close();
        GC.SuppressFinalize(this);
    }

    ~DocumentIngestionSession()
    {
        if (Archive.Open) Console.WriteLine("WARNING: Dispose not called");
    }
}

public class DocumentIngestionPipeline
{
    public event Action<DocumentRecord> DocumentArchived;
    public event Action<DocumentRecord> DocumentRejected;

    public List<string> Rejects = new();
    HashSet<string> ids = new();

    static string[] RequiredFields(string type)
    {
        Type t = type == "Invoice"
            ? typeof(InvoiceRules)
            : typeof(ContractRules);

        return t.GetCustomAttributes<RequiredFieldAttribute>()
                .Select(a => a.Name).ToArray();
    }
    static Predicate<DocumentRecord> RequiredRule(string[] fields){
        return d =>
        {
            foreach (string f in fields)
                if (!d.ParsedFields.ContainsKey(f))
                    throw new MissingRequiredFieldException(d.DocumentId, f);

            return true;
        };
    }

    public void Process(List<DocumentRecord> docs, DocumentIngestionSession s)
    {
        try
        {
            if (docs == null)
                throw new ArgumentNullException("docs", "Batch cannot be null.");

            foreach (var d in docs)
            {
                try
                {
                    if (!ids.Add(d.DocumentId))
                        throw new Exception("Duplicate DocumentId: " + d.DocumentId);

                    if (d.ParsedFields == null || d.ParsedFields.Count == 0)
                        throw new MalformedDocumentException(d.DocumentId);
                    d.ParsedFields = d.ParsedFields
                       .ToDictionary(x => x.Key, x => x.Value?.Trim());
                    RequiredRule(RequiredFields(d.DocumentType))(d);
                    Predicate<string> validDate =
                        x => !string.IsNullOrEmpty(x);

                    Console.WriteLine("Validated: " + d.DocumentId);

                    s.Archive.Add(d);
                    DocumentArchived?.Invoke(d);
                }
                catch (Exception e){
                    Rejects.Add(d.DocumentId + ": " + e.Message);
                    DocumentRejected?.Invoke(d);
                }
            }
        }
        catch (ArgumentNullException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public void Statistics(List<DocumentRecord> docs)
    {

        var rates = docs.GroupBy(d => d.DocumentType)
            .Select(g => new
            {
                Type = g.Key,
                Rate = (double)g.Count(d =>
                    Rejects.Any(r => r.StartsWith(d.DocumentId + ":"))) / g.Count()
            });
        var missing = Rejects
            .Where(x => x.Contains("Missing required field"))
            .Select(x => x.Split(':').Last().Trim())
            .GroupBy(x => x)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault()?.Key;

        foreach (var r in rates)
            Console.WriteLine($"{r.Type}: {r.Rate:P}");

        Console.WriteLine("Most missing: " + missing);
    }
}

public class Program
{
    static void Main()
    {
        var docs = new List<DocumentRecord>
        {
            new() {
                DocumentId = "1", DocumentType = "Invoice",
                ParsedFields = new()
                {
                    ["InvoiceNumber"] = " INV001 ",
                    ["Date"] = "2026"
                }
            },

            new() {
                DocumentId = "2", DocumentType = "Invoice",
                ParsedFields = new()
                {
                    ["Date"] = "2026"
                }
            },

            new() {
                DocumentId = "3", DocumentType = "Invoice",
                ParsedFields = new()
                {
                    ["InvoiceNumber"] = "INV003",
                    ["Date"] = "2026"
                }
            }
        };

        var pipeline = new DocumentIngestionPipeline();

        pipeline.DocumentArchived +=
            d => Console.WriteLine("Archived: " + d.DocumentId);

        pipeline.DocumentRejected +=
            d => Console.WriteLine("Rejected: " + d.DocumentId);

        using (var session = new DocumentIngestionSession())
        {
            pipeline.Process(docs, session);
            var entries = session.Archived
                .Select(d => new { d.DocumentId, d.DocumentType })
                .ToList();

            pipeline.Statistics(docs);
        }
    }
}