using System;
using System.Collections.Generic;

using OfficeOpenXml;
using System.IO;
using TournoiRugby;

namespace RugbyTournament
{
    class ExcelManager
    {
        
        public ExcelManager() { }
        public void CreateExcelFile(string fileName, Pool[] pTab)
        {
            using (ExcelPackage excel = new ExcelPackage())
            {
                var headerRow = new List<string[]>()
                {
                    new string[] { "NumMatch", "Équipe 1", "Équipe 2", "Score Équipe 1", "Score Équipe 2" }
                };

                // Determine the header range (e.g. A1:D1)
                string headerRange = "A1:" + Char.ConvertFromUtf32(headerRow[0].Length + 64) + "1";


                foreach (Pool p in pTab)
                {
                    excel.Workbook.Worksheets.Add("Pool #" + p.PoolId );
                    // Target a worksheet
                    var worksheet = excel.Workbook.Worksheets["Pool #" + p.PoolId];

                    // Popular header row data
                    worksheet.Cells[headerRange].LoadFromArrays(headerRow);
                    worksheet.Cells[headerRange].Style.Font.Bold = true;
                    worksheet.Cells[headerRange].Style.Font.Size = 14;
                    worksheet.Cells[headerRange].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                    worksheet.Cells[headerRange].AutoFitColumns();

                    for (int i = 0; i < p.GetNbOfGames(); i++)
                    {
                        worksheet.Cells[i + 2, 1].Value = i+1;
                        worksheet.Cells[i + 2, 2].Value = p.GameList[i].TeamA.Name;
                        worksheet.Cells[i + 2, 3].Value = p.GameList[i].TeamB.Name;
                    }
                }

                FileInfo excelFile = new FileInfo(fileName);
                excel.SaveAs(excelFile);
            }
        }

        public void ReadResultsFromExcelFile(string fileName, Pool[] pTab)
        {
            FileInfo existingFile = new FileInfo(fileName);
            using (ExcelPackage excel = new ExcelPackage(existingFile))
            {
                foreach (Pool p in pTab)
                {
                    // Target a worksheet
                    var worksheet = excel.Workbook.Worksheets["Pool #" + p.PoolId];

                    for (int i = 0; i < p.GetNbOfGames(); i++)
                    {
                        int scoreA = Convert.ToInt32(worksheet.Cells[i + 2, 4].Value.ToString());
                        int scoreB = Convert.ToInt32(worksheet.Cells[i + 2, 5].Value.ToString());
                        p.GameList[i].EnterScore(scoreA, scoreB);
                    }
                }
            }
        }
    }
}
