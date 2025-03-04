using System;
using System.Data;
using MSTest = Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using PersonalFinances.BLL.Entities.Models.SavingPlan;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PersonalFinances.BLL.Entities.Models.SavingPlan.Tests
{
    public class BudgetHistoryModelTests
    {

        [Test]
        public void Should_Set_BudgetId_To_EmptyString_When_BudgetId_Column_Is_Missing()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("transaction_id", typeof(string));
            table.Columns.Add("valor_gasto", typeof(decimal));
            table.Columns.Add("data_registro", typeof(DateTime));

            var row = table.NewRow();
            row["transaction_id"] = "trans123";
            row["valor_gasto"] = 100.0m;
            row["data_registro"] = DateTime.Now;
            table.Rows.Add(row);

            // Act
            var model = new BudgetHistoryModel(row);

            // Assert
            MSTest.Assert.AreEqual(string.Empty, model.BudgetId);
        }

        [Test]
        public void Should_Set_TransactionId_To_EmptyString_When_TransactionId_Column_Is_Missing()
        {
            // Arrange
            DataTable table = new DataTable();
            table.Columns.Add("budget_id", typeof(string));
            table.Columns.Add("valor_gasto", typeof(decimal));
            table.Columns.Add("data_registro", typeof(DateTime));
            
            DataRow row = table.NewRow();
            row["budget_id"] = "B123";
            row["valor_gasto"] = 100.50m;
            row["data_registro"] = DateTime.Now;
            table.Rows.Add(row);

            // Act
            BudgetHistoryModel budgetHistory = new BudgetHistoryModel(row);

            // Assert
            MSTest.Assert.AreEqual(string.Empty, budgetHistory.TransactionId);
        }

        [TestMethod]
        public void BudgetHistoryModel_ShouldSetValorGastoToZero_WhenValorGastoColumnIsMissing()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("budget_id", typeof(string));
            table.Columns.Add("transaction_id", typeof(string));
            table.Columns.Add("data_registro", typeof(DateTime));

            var row = table.NewRow();
            row["budget_id"] = "B123";
            row["transaction_id"] = "T456";
            row["data_registro"] = DateTime.Now;
            table.Rows.Add(row);

            // Act
            var budgetHistoryModel = new BudgetHistoryModel(row);

            // Assert
            MSTest.Assert.AreEqual(0, budgetHistoryModel.ValorGasto);
        }

        [Test]
        public void BudgetHistoryModel_ShouldSetDataRegistroToCurrentDateTime_WhenDataRegistroColumnIsMissing()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("budget_id", typeof(string));
            table.Columns.Add("transaction_id", typeof(string));
            table.Columns.Add("valor_gasto", typeof(decimal));
            var row = table.NewRow();
            row["budget_id"] = "B123";
            row["transaction_id"] = "T456";
            row["valor_gasto"] = 100.50m;
            table.Rows.Add(row);

            // Act
            var model = new BudgetHistoryModel(row);

            // Assert
            NUnit.Framework.Assert.That(model.DataRegistro, Is.EqualTo(DateTime.Now).Within(TimeSpan.FromSeconds(1)));
        }

        [Test]
        public void BudgetHistoryModel_ShouldAssignBudgetId_WhenBudgetIdColumnIsPresentAndNotNull()
        {
            // Arrange
            var dataTable = new DataTable();
            dataTable.Columns.Add("budget_id", typeof(string));
            dataTable.Columns.Add("transaction_id", typeof(string));
            dataTable.Columns.Add("valor_gasto", typeof(decimal));
            dataTable.Columns.Add("data_registro", typeof(DateTime));

            var dataRow = dataTable.NewRow();
            dataRow["budget_id"] = "TestBudgetId";
            dataRow["transaction_id"] = "TestTransactionId";
            dataRow["valor_gasto"] = 100m;
            dataRow["data_registro"] = DateTime.Now;
            dataTable.Rows.Add(dataRow);

            // Act
            var budgetHistoryModel = new BudgetHistoryModel(dataRow);

            // Assert
            MSTest.Assert.AreEqual("TestBudgetId", budgetHistoryModel.BudgetId);
        }

        [Test]
        public void ShouldCorrectlyAssignTransactionIdWhenTransactionIdColumnIsPresentAndNotNull()
        {
            // Arrange
            var dataTable = new DataTable();
            dataTable.Columns.Add("budget_id", typeof(string));
            dataTable.Columns.Add("transaction_id", typeof(string));
            dataTable.Columns.Add("valor_gasto", typeof(decimal));
            dataTable.Columns.Add("data_registro", typeof(DateTime));

            var expectedTransactionId = "trans123";
            var row = dataTable.NewRow();
            row["transaction_id"] = expectedTransactionId;
            dataTable.Rows.Add(row);

            // Act
            var budgetHistoryModel = new BudgetHistoryModel(row);

            // Assert
            MSTest.Assert.AreEqual(expectedTransactionId, budgetHistoryModel.TransactionId);
        }

        [Test]
        public void ShouldCorrectlyAssignValorGastoWhenValorGastoColumnIsPresentAndNotNull()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("budget_id", typeof(string));
            table.Columns.Add("transaction_id", typeof(string));
            table.Columns.Add("valor_gasto", typeof(decimal));
            table.Columns.Add("data_registro", typeof(DateTime));

            var row = table.NewRow();
            row["budget_id"] = "B123";
            row["transaction_id"] = "T456";
            row["valor_gasto"] = 150.75m;
            row["data_registro"] = DateTime.Now;
            table.Rows.Add(row);

            // Act
            var budgetHistoryModel = new BudgetHistoryModel(row);

            // Assert
            MSTest.Assert.AreEqual(150.75m, budgetHistoryModel.ValorGasto);
        }

        [Test]
        public void BudgetHistoryModel_ShouldAssignDataRegistro_WhenDataRegistroColumnIsPresentAndNotNull()
        {
            // Arrange
            var dataTable = new DataTable();
            dataTable.Columns.Add("budget_id", typeof(string));
            dataTable.Columns.Add("transaction_id", typeof(string));
            dataTable.Columns.Add("valor_gasto", typeof(decimal));
            dataTable.Columns.Add("data_registro", typeof(DateTime));

            var expectedDate = new DateTime(2023, 10, 5);
            var dataRow = dataTable.NewRow();
            dataRow["budget_id"] = "B123";
            dataRow["transaction_id"] = "T456";
            dataRow["valor_gasto"] = 100.50m;
            dataRow["data_registro"] = expectedDate;

            // Act
            var budgetHistoryModel = new BudgetHistoryModel(dataRow);

            // Assert
            MSTest.Assert.AreEqual(expectedDate, budgetHistoryModel.DataRegistro);
        }

        [Test]
        public void BudgetHistoryModel_ShouldHandleNullValorGastoBySettingToZero()
        {
            // Arrange
            var dataTable = new DataTable();
            dataTable.Columns.Add("budget_id", typeof(string));
            dataTable.Columns.Add("transaction_id", typeof(string));
            dataTable.Columns.Add("valor_gasto", typeof(decimal));
            dataTable.Columns.Add("data_registro", typeof(DateTime));

            var dataRow = dataTable.NewRow();
            dataRow["budget_id"] = "B001";
            dataRow["transaction_id"] = "T001";
            dataRow["valor_gasto"] = DBNull.Value; // Simulate null value
            dataRow["data_registro"] = DateTime.Now;

            // Act
            var budgetHistoryModel = new BudgetHistoryModel(dataRow);

            // Assert
            MSTest.Assert.AreEqual(0, budgetHistoryModel.ValorGasto);
        }

        [Test]
        public void BudgetHistoryModel_ShouldHandleNullDataRegistroBySettingToCurrentDateTime()
        {
            // Arrange
            var dataTable = new DataTable();
            dataTable.Columns.Add("budget_id", typeof(string));
            dataTable.Columns.Add("transaction_id", typeof(string));
            dataTable.Columns.Add("valor_gasto", typeof(decimal));
            dataTable.Columns.Add("data_registro", typeof(DateTime));

            var row = dataTable.NewRow();
            row["budget_id"] = "B001";
            row["transaction_id"] = "T001";
            row["valor_gasto"] = 100.00m;
            row["data_registro"] = DBNull.Value; // Simulate null value

            // Act
            var budgetHistoryModel = new BudgetHistoryModel(row);

            // Assert
            MSTest.Assert.AreEqual("B001", budgetHistoryModel.BudgetId);
            MSTest.Assert.AreEqual("T001", budgetHistoryModel.TransactionId);
            MSTest.Assert.AreEqual(100.00m, budgetHistoryModel.ValorGasto);
            NUnit.Framework.Assert.That(budgetHistoryModel.DataRegistro, Is.EqualTo(DateTime.Now).Within(TimeSpan.FromSeconds(1)));
        }
    }
}
