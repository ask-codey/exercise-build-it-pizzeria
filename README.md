# Exercise: Build-It Pizzeria

Exercise SQL for the lesson **Exercise: Build-It Pizzeria** in the C# Full-Stack Bootcamp on [ask-codey.com](https://www.ask-codey.com).

Build-It Pizzeria lets customers build their own pizza: pick a size, a crust and any toppings. Until now, orders were written on paper slips. You'll design the database for them, write the script that builds it, fill it with the first four orders, and answer the owner's questions.

## The menu

| Size | Inches | Base price |
|---|---|---|
| Small | 10 | $9.00 |
| Medium | 12 | $11.00 |
| Large | 14 | $13.00 |

| Crust | Extra |
|---|---|
| Thin | $0.00 |
| Classic | $0.00 |
| Stuffed | $2.00 |

| Topping | Price per pizza |
|---|---|
| Pepperoni | $1.50 |
| Mushrooms | $1.00 |
| Olives | $1.00 |
| Green Peppers | $1.00 |
| Extra Cheese | $1.25 |

A pizza costs its size's base price, plus its crust's extra, plus each of its toppings. Delivery costs $3.50; pickup is free.

## The customers

| Name | Phone | Email | Address |
|---|---|---|---|
| Maria Lopez | (503) 555-0142 | maria.lopez@example.com | 12 Alder Street |
| Sam Okafor | (503) 555-0187 | sam.okafor@example.com | 480 River Road |
| Priya Shah | (503) 555-0123 | (none) | (none) |

## The order slips

1. **Maria Lopez**, March 6, 2026, 6:05 PM, pickup: one Large Classic with Pepperoni and Mushrooms; one Small Thin with Olives.
2. **Sam Okafor**, March 6, 2026, 6:40 PM, delivery: two Medium Stuffed with Pepperoni.
3. **Maria Lopez**, March 7, 2026, 12:15 PM, delivery: one Large Thin with Mushrooms, Olives and Green Peppers.
4. **Priya Shah**, March 7, 2026, 7:00 PM, pickup: one Medium Classic, cheese only (no toppings).

## What to do

1. **Design.** On paper or in a drawing tool, sketch the tables: one subject per table, a primary key in each, foreign keys for every link. Watch for the many-to-many: a pizza can have many toppings, and a topping can be on many pizzas.
2. **Script.** In `BuildItPizzeria.sql`, create the database and its tables, with types, `NOT NULL`, `IDENTITY`, defaults, and primary key, foreign key, `UNIQUE` and `CHECK` constraints. Start with `USE master;`, `GO` and `DROP DATABASE IF EXISTS BuildItPizzeria;`, so you can run it again. (Close any other window that's using BuildItPizzeria first: SQL Server can't drop a database that's in use.)
3. **Data.** At the end of the same script, insert the menu, the customers and the four orders.
4. **Questions.** Run your script, then answer each question in `Questions.sql` with a query, and compare with its **Expect** line.
5. **Diagram.** In SSMS, make a database diagram of your finished database, and check it matches your sketch.

## Check your answers

`PizzeriaExercise.Tests` runs your answers in `Questions.sql` against the BuildItPizzeria database your script built, and compares each one with its question's **Expect** line. Run `BuildItPizzeria.sql` in SSMS first: the tests don't build the database. Nothing is changed: every answer runs inside a transaction that is rolled back. It needs the .NET SDK, which you installed in the first week. Open a terminal in this folder and run:

```bash
dotnet test PizzeriaExercise.Tests
```

At first every test fails: that is expected. Each question you answer turns its test green, and a failing test's message says what the right answer looks like. Your table and column names don't matter. The tests use the BuildItPizzeria database on `localhost`; to use another one, set the `PIZZERIA_CONNECTION` environment variable to its connection string.

## Tips

- Create parents before children: the menu and customers before orders, orders before their pizzas.
- An order line with a quantity of 2 is one row, not two.
- A pizza with no toppings has no rows in the topping link table. Questions that add up topping prices need a `LEFT JOIN` so it isn't left out.
- Your names don't have to match anyone else's: the answers to the questions are the same for any sound design.

## Hand it in

When it works, push your solution to a public repo of your own on GitHub, then paste that repo's address into **Submit your exercise** on the lesson page at [ask-codey.com](https://www.ask-codey.com). That's what completes the lesson. The lesson page shows the Git commands, step by step.
