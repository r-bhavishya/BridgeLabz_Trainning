# CSV Data Handling Practice in C#

This is a beginner-friendly console program covering the 15 requested CSV exercises.

## Run it

```powershell
dotnet run
```

Choose an exercise from the menu. The program creates a `data` folder with sample files automatically. Output files such as `updated-employees.csv`, `merged.csv`, and `database-report.csv` are also placed there.

## Notes

- Exercise 9 uses a C# `Student` record. The original question says Java object, but this project is C#.
- Exercise 13 uses sample employee rows in place of a real database. Replace that list with rows from your database query when you are ready.
- The CSV reader is intentionally simple for practice. It expects values without commas inside quoted fields. Real applications should use a CSV library for full CSV rules.
- Exercise 15 demonstrates AES encryption with a sample key. Do not store a real encryption key directly in source code in a production application.