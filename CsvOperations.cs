using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using ProjectLaborBackend.Controllers;
using ProjectLaborBackend.Entities;
using System.ComponentModel;
using System.Data;
using ProjectLaborBackend.Services;
using System.IO;
namespace ProjectLaborBackend
{
    public class CsvOperations
    {
        public CsvOperations()
        {
        }

        public void ExportDataToExcel(AppDbContext.Tables table)
        {
            // Connection string to the MySQL database
            DotNetEnv.Env.Load();
            string connectionString = DotNetEnv.Env.GetString("DB_CONNECTION_STRING");

            // SQL query to fetch data
            string query = $"SELECT * FROM {table.ToString()}";

            // Create a connection to the MySQL database
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();

                // Fill the DataTable with the result set
                adapter.Fill(dataTable);

                // Specify license type as NonCommercial
                ExcelPackage.License.SetNonCommercialPersonal("ProjectLabor");

                // Create a new Excel package to work with Excel files
                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    // Add a new worksheet to the Excel package
                    var worksheet = excelPackage.Workbook.Worksheets.Add(table.ToString());

                    // Load the DataTable into the Excel worksheet
                    worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);

                    // Save the Excel file
                    try
                    {
                        excelPackage.SaveAs(new FileInfo($"{table.ToString()}_Export.xlsx"));
                    }
                    catch (IOException ex)
                    {
                        throw new IOException("The Excel file is currently open. Please close it and try again.", ex);
                    }
                }
            }
        }

        public List<List<string>> ImportDataFromExcel(AppDbContext.Tables table)
        {
            // Load connection string from environment variables
            DotNetEnv.Env.Load();
            string connectionString = DotNetEnv.Env.GetString("DB_CONNECTION_STRING");
            string excelFilePath = $"{table.ToString()}_Export.xlsx";
            // set the EPPlus license context
            ExcelPackage.License.SetNonCommercialPersonal("ProjectLabor");

            List<List<string>> data = new List<List<string>>();

            // Read data from Excel file
            if (!File.Exists(excelFilePath))
                throw new FileNotFoundException($"The file {excelFilePath} was not found.");
            try { 
                
                using (var package = new ExcelPackage(new FileInfo(excelFilePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // get the first worksheet

                    // Get the dimensions of the worksheet
                    int colCount = worksheet.Dimension.End.Column;
                    int rowCount = worksheet.Dimension.End.Row;

                    for (int i = 1; i < rowCount; i++)
                    {
                        List<string> row = new List<string>();
                        for (int j = 1; j < colCount; j++)
                        {
                            // Read each cell value and add to the row list
                            if (!string.IsNullOrWhiteSpace(worksheet.Cells[i, j].Text))
                                row.Add(worksheet.Cells[i, j].Text);
                        }
                        data.Add(row);
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException("Permission denied to access the Excel file.", ex);
            }
            catch (Exception ex) {
                throw new Exception("An error occurred while reading the Excel file.", ex);
            }
            return data;
        }
    }
}
