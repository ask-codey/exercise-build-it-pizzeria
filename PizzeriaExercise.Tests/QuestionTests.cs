using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace PizzeriaExercise.Tests;

// Runs your answers in Questions.sql against the BuildItPizzeria database your script built, and compares
// each answer with its question's Expect line. Run BuildItPizzeria.sql in SSMS first: the tests don't build
// the database. Nothing is changed: every answer runs inside a transaction that is rolled back.
// Your table and column names may differ from anyone else's; the answers won't.
// You don't need to change this file.
[TestClass]
public sealed class QuestionTests
{
    // The PIZZERIA_CONNECTION environment variable, if set, points the tests at another database.
    private static readonly string connectionString =
        Environment.GetEnvironmentVariable("PIZZERIA_CONNECTION")
        ?? "Server=localhost;Database=BuildItPizzeria;Integrated Security=True;TrustServerCertificate=True";

    // Runs your answer to one question: the lines under its "-- N." comment, up to the next question.
    private static DataRow[] Answer(int question)
    {
        string sql = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Queries", "Questions.sql"));
        Match part = Regex.Match(sql, $@"^(--\s*{question}\..*?)(?=^--\s*\d+\.|\z)", RegexOptions.Multiline | RegexOptions.Singleline);
        Assert.IsTrue(part.Success, $"Questions.sql has no question {question}.");
        var results = new List<DataTable>();
        using var connection = new SqlConnection(connectionString);
        try
        {
            connection.Open();
        }
        catch (SqlException e)
        {
            Assert.Fail($"The BuildItPizzeria database isn't there yet: run BuildItPizzeria.sql in SSMS first. ({e.Message})");
        }
        using var transaction = connection.BeginTransaction();
        foreach (string batch in Regex.Split(part.Groups[1].Value, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase))
        {
            if (string.IsNullOrWhiteSpace(batch)) continue;
            using var command = new SqlCommand(batch, connection, transaction);
            using var reader = command.ExecuteReader();
            while (!reader.IsClosed && reader.FieldCount > 0)
            {
                var result = new DataTable();
                result.Load(reader);   // reads one result, then moves on to the next
                results.Add(result);
            }
        }
        transaction.Rollback();
        Assert.IsNotEmpty(results, $"Question {question} has no answer yet. Write your query under it in Questions.sql.");
        return results[^1].Rows.Cast<DataRow>().ToArray();
    }

    // True when one of the row's values is the text, or the row reads it across its values (a first and a
    // last name in two columns), so your column names and order don't matter.
    private static bool Has(DataRow row, string value) =>
        row.ItemArray.Any(cell => string.Equals(cell?.ToString(), value, StringComparison.Ordinal))
        || string.Join(" ", row.ItemArray.Select(cell => cell?.ToString())).Contains(value, StringComparison.Ordinal);

    // True when one of the row's values is the number: 15.5000 is the same answer as 15.50.
    private static bool Has(DataRow row, decimal value) =>
        row.ItemArray.Any(cell => cell is not null and not DBNull && decimal.TryParse(Convert.ToString(cell, CultureInfo.InvariantCulture),
            NumberStyles.Number, CultureInfo.InvariantCulture, out decimal number) && Math.Abs(number - value) < 0.005m);

    [TestMethod]
    public void Question1_PizzasOrdered_IsSix()
    {
        DataRow[] rows = Answer(1);

        Assert.HasCount(1, rows, "1: one row, the number of pizzas.");
        Assert.IsTrue(Has(rows[0], 6m), "1: 6 pizzas. Two of the same on one order are two: add up the quantities.");
    }

    [TestMethod]
    public void Question2_PizzasPerSize_CountsEachSize()
    {
        DataRow[] rows = Answer(2);

        Assert.HasCount(3, rows, "2: one row per size, so 3 rows.");
        foreach (var (size, pizzas) in new[] { ("Large", 2m), ("Medium", 3m), ("Small", 1m) })
        {
            Assert.IsTrue(rows.Any(row => Has(row, size) && Has(row, pizzas)), $"2: {size}, {pizzas}.");
        }
    }

    [TestMethod]
    public void Question3_LineCosts_AddTheSizeCrustAndToppings()
    {
        DataRow[] rows = Answer(3);

        Assert.HasCount(5, rows, "3: one row per pizza line, so 5 rows. A pizza with no toppings needs a LEFT JOIN to stay in.");
        foreach (var (order, size, crust, cost) in new[]
        {
            (1m, "Large", "Classic", 15.50m), (1m, "Small", "Thin", 10.00m), (2m, "Medium", "Stuffed", 29.00m),
            (3m, "Large", "Thin", 16.00m), (4m, "Medium", "Classic", 11.00m),
        })
        {
            Assert.IsTrue(rows.Any(row => Has(row, order) && Has(row, size) && Has(row, crust) && Has(row, cost)),
                $"3: order {order}'s {size} {crust} line costs {cost:0.00}.");
        }
    }

    [TestMethod]
    public void Question4_MostPopularTopping_IsPepperoni()
    {
        DataRow[] rows = Answer(4);

        Assert.IsNotEmpty(rows, "4: show the topping and how many pizzas it's on.");
        Assert.IsTrue(Has(rows[0], "Pepperoni") && Has(rows[0], 3m), "4: Pepperoni, on 3 pizzas. Two of the same on one order are two.");
    }

    [TestMethod]
    public void Question5_ToppingsNobodyOrdered_IsExtraCheese()
    {
        DataRow[] rows = Answer(5);

        Assert.HasCount(1, rows, "5: one topping has never been ordered.");
        Assert.IsTrue(Has(rows[0], "Extra Cheese"), "5: Extra Cheese. A LEFT JOIN and IS NULL find it.");
    }

    [TestMethod]
    public void Question6_Bonus_SpendPerCustomer_BiggestFirst()
    {
        DataRow[] rows = Answer(6);

        Assert.HasCount(3, rows, "6: one row per customer, so 3 rows.");
        (string Customer, decimal Spent)[] expected = [("Maria Lopez", 45.00m), ("Sam Okafor", 32.50m), ("Priya Shah", 11.00m)];
        for (int i = 0; i < expected.Length; i++)
        {
            Assert.IsTrue(Has(rows[i], expected[i].Customer) && Has(rows[i], expected[i].Spent),
                $"6: row {i + 1} is {expected[i].Customer}, {expected[i].Spent:0.00}, delivery fees included.");
        }
    }
}
