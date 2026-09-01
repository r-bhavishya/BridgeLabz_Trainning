using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace TestProject1;

[TestFixture]
public class PipelineTests
{
    [Test]
    public void Process_ArchivesValidDocumentsAndRejectsInvalid(){
        var docs = new List<DocumentRecord>{
            new() { DocumentId = "1", DocumentType = "Invoice", ParsedFields = new() { ["InvoiceNumber"] = "INV001", ["Date"] = "2026"}},
            new() { DocumentId = "2", DocumentType = "Invoice", ParsedFields = new() { ["Date"] = "2026" } },
            new() { DocumentId = "3", DocumentType = "Invoice", ParsedFields = new() { ["InvoiceNumber"] = "INV003", ["Date"] = "2026"}}
        };

        var pipeline = new DocumentIngestionPipeline();
        var archivedIds = new List<string>();
        var rejectedIds = new List<string>();

        pipeline.DocumentArchived += d => archivedIds.Add(d.DocumentId);
        pipeline.DocumentRejected += d => rejectedIds.Add(d.DocumentId);

        using (var session = new DocumentIngestionSession()){
            pipeline.Process(docs, session);
            Assert.That(session.Archive.Data.Count, Is.EqualTo(2));
            Assert.That(session.Archive.Data.Any(d => d.DocumentId == "1"), Is.True);
            Assert.That(session.Archive.Data.Any(d => d.DocumentId == "3"), Is.True);
            Assert.That(pipeline.Rejects.Any(r => r.StartsWith("2: ") && r.Contains("Missing required field")), Is.True);
            Assert.That(rejectedIds, Does.Contain("2"));
            Assert.That(archivedIds.Count, Is.EqualTo(2));
        }
    }

    [Test]
    public void Session_Dispose_MovesArchiveAndCloses()
    {
        var session = new DocumentIngestionSession();
        var doc = new DocumentRecord { DocumentId = "X", DocumentType = "Invoice", ParsedFields = new() { ["InvoiceNumber"] = "1", ["Date"] = "2026" } };

        session.Archive.Add(doc);
        Assert.That(session.Archive.Open, Is.True);
        Assert.That(session.Archive.Data.Count, Is.EqualTo(1));

        session.Dispose();

        Assert.That(session.Archive.Open, Is.False);
        Assert.That(session.Archived.Count, Is.EqualTo(1));
        Assert.That(session.Archived[0].DocumentId, Is.EqualTo("X"));
    }

    [Test]
    public void Process_DuplicateDocumentId_IsRejected()
    {
        var docs = new List<DocumentRecord>
        {
            new() { DocumentId = "dup", DocumentType = "Invoice", ParsedFields = new() { ["InvoiceNumber"] = "1", ["Date"] = "2026" } },
            new() { DocumentId = "dup", DocumentType = "Invoice", ParsedFields = new() { ["InvoiceNumber"] = "2", ["Date"] = "2026" } }
        };
        var pipeline = new DocumentIngestionPipeline();
        using (var session = new DocumentIngestionSession())
{
            pipeline.Process(docs, session);

            Assert.That(pipeline.Rejects.Any(r => r.Contains("Duplicate DocumentId")), Is.True);
        }
    }
}
