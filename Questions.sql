-- Build-It Pizzeria: questions about your database.
-- Run BuildItPizzeria.sql first, then answer each question with a query.
-- Your table and column names may differ from anyone else's; the answers won't.
USE BuildItPizzeria;
GO

-- 1. How many pizzas were ordered in all? (Count every pizza: two of the same on one order are two.)
-- Expect: 6.

-- 2. How many pizzas were ordered in each size? Show the size's name.
-- Expect: Large 2, Medium 3, Small 1.

-- 3. What does each pizza line cost? A pizza costs its size's base price, plus its crust's extra price,
--    plus the price of each of its toppings; a line costs that times its quantity. Show the order, the size,
--    the crust, the quantity and the line's cost.
-- Expect: 5 rows: order 1 Large Classic 15.50 and Small Thin 10.00; order 2 two Medium Stuffed 29.00;
--         order 3 Large Thin 16.00; order 4 Medium Classic 11.00.

-- 4. Which topping is on the most pizzas? (Count every pizza: two of the same on one order are two.)
-- Expect: Pepperoni, on 3 pizzas.

-- 5. Which toppings has nobody ordered?
-- Expect: Extra Cheese.

-- 6. Bonus: what did each customer spend in all, delivery fees included? Biggest spender first.
-- Expect: Maria Lopez 45.00, Sam Okafor 32.50, Priya Shah 11.00.
