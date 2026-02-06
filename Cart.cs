using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace ShopEaseApp;

public class Cart
{
	private readonly List<Product> _products = new();
	private const string ConnectionString = "Data Source=Shop.db";
	private static readonly object DbLock = new();
	private static bool _databaseInitialized;

	public void AddProduct(Product product)
	{
		if (product == null)
		{
			throw new ArgumentNullException(nameof(product));
		}

		_products.Add(product);
		SaveProductToDatabase(product);
	}

	public bool RemoveProduct(int productId)
	{
		var product = _products.Find(p => p.ProductID == productId);
		var removedFromList = product != null && _products.Remove(product);
		var removedFromDatabase = RemoveProductFromDatabase(productId) > 0;

		return removedFromList || removedFromDatabase;
	}

	public void DisplayCartItems()
	{
		foreach (var product in _products)
		{
			product.PrintDetails();
		}
	}

	public float CalculateTotal()
	{
		float total = 0f;

		foreach (var product in _products)
		{
			total += product.Price;
		}

		return total;
	}

	private static void EnsureDatabase()
	{
		if (_databaseInitialized)
		{
			return;
		}

		lock (DbLock)
		{
			if (_databaseInitialized)
			{
				return;
			}

			using var connection = new SqliteConnection(ConnectionString);
			connection.Open();

			using var command = connection.CreateCommand();
			command.CommandText =
				"CREATE TABLE IF NOT EXISTS Products (" +
				"ProductID INTEGER PRIMARY KEY, " +
				"Name TEXT NOT NULL, " +
				"Price REAL NOT NULL, " +
				"Category TEXT NOT NULL" +
				");";
			command.ExecuteNonQuery();

			_databaseInitialized = true;
		}
	}

	private static void SaveProductToDatabase(Product product)
	{
		EnsureDatabase();

		using var connection = new SqliteConnection(ConnectionString);
		connection.Open();

		using var command = connection.CreateCommand();
		command.CommandText =
			"INSERT INTO Products (ProductID, Name, Price, Category) " +
			"VALUES ($id, $name, $price, $category) " +
			"ON CONFLICT(ProductID) DO UPDATE SET " +
			"Name = excluded.Name, " +
			"Price = excluded.Price, " +
			"Category = excluded.Category;";
		command.Parameters.AddWithValue("$id", product.ProductID);
		command.Parameters.AddWithValue("$name", product.Name);
		command.Parameters.AddWithValue("$price", product.Price);
		command.Parameters.AddWithValue("$category", product.Category);
		command.ExecuteNonQuery();
	}

	private static int RemoveProductFromDatabase(int productId)
	{
		EnsureDatabase();

		using var connection = new SqliteConnection(ConnectionString);
		connection.Open();

		using var command = connection.CreateCommand();
		command.CommandText = "DELETE FROM Products WHERE ProductID = $id;";
		command.Parameters.AddWithValue("$id", productId);
		return command.ExecuteNonQuery();
	}
}
