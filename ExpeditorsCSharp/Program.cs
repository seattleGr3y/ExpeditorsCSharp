// See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");

using System.Data;

public class GetTheInput
{
    public static void Main()
    {
        // not using static class or method so create instance of class
        var GTI = new GetTheInput();
        GTI.GetUserDataFromFile();
    }

    public string defaultfilePath = "C:\\TEMP_TEST\\userData.csv"; 
    public Dictionary<string, string> userDataAll = new Dictionary<string, string>();
    public string rawData = string.Empty;
    public DataTable rawDataTable = new DataTable();

    public DataTable GetDTfromCSV(string file)
    {
        using (StreamReader sr = new StreamReader(file))
        {
            string[] headers = sr.ReadLine()!.Split(',');
            int hdrCnt = headers.Length;
            foreach (string header in headers)
            {
                rawDataTable.Columns.Add(header.Trim('"'));
            }
            while (!sr.EndOfStream)
            {
                int colCount = rawDataTable.Columns.Count;
                string[] rows = sr.ReadLine()!.Split(',');
                DataRow dr = rawDataTable.NewRow();
                for (int i = 0; i < colCount; i++)
                {
                    dr[i] = rows[i].Trim('"');
                }
                rawDataTable.Rows.Add(dr);
            }

        }
        return rawDataTable;
    }

    public void GetUserDataFromFile()
    {

        // not using static class or method so create instance of class
        var GIN = new GetIsNull();
        var GFP = new GetFilePath();
        var GV = new GetValue();
        var SQ = new SqlQuery();
        var streetList = new List<string>();
        var cityList = new List<string>();

        // ask user to provide a custom file for the input or use a known default data file location
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Hit Enter to use default location to import user data or ");
        Console.WriteLine("     Enter full path to new file we need to import : ");
        Console.ForegroundColor = ConsoleColor.White;

        // depending on whether a new path is chosen or default decide where to look for the file
        GFP.FilePath = Console.ReadLine();
        if (GFP.FilePath != null && GFP.FilePath != String.Empty)
        {
            rawDataTable = GetDTfromCSV(GFP.FilePath);
        }
        else
        {
            GFP.FilePath = defaultfilePath;
            rawDataTable = GetDTfromCSV(GFP.FilePath);
        }

        int colCount = rawDataTable.Columns.Count;
        var colNames = new List<string>();
        Console.ForegroundColor = ConsoleColor.Cyan;
        var colNamesDict = new Dictionary<string, int>();

        for (int i = 0; i < colCount; i++)
        {
            var colNameTmp = rawDataTable.Columns[i].ColumnName;
            colNames.Add(colNameTmp);
            colNamesDict.Add(colNameTmp, i);
        }

        var tmpAddDict = new Dictionary<string, string>();
        var fullDict = new Dictionary<string, string>();
        var dataRow = rawDataTable.AsEnumerable();
        int indexIAlastName = 0;

        foreach (var item in dataRow)
        {
            colNamesDict.TryGetValue("userAddressStreet", out int indexIAstreet);
            string? tmpStreet = item?.ItemArray[indexIAstreet]?.ToString()?.Trim(',');

            colNamesDict.TryGetValue("userAddressCity", out int indexIAcity);
            string? tmpCity = item?.ItemArray[indexIAcity]?.ToString();

            colNamesDict.TryGetValue("userAddressState", out int indexIAstate);
            string? tmpState = item?.ItemArray[indexIAstate]?.ToString();

            colNamesDict.TryGetValue("userFirstName", out int indexIAfirstName);
            string? tmpFirstName = item?.ItemArray[indexIAfirstName]?.ToString();

            colNamesDict.TryGetValue("userLastName", out indexIAlastName);
            string? tmpLastName = item?.ItemArray[indexIAlastName]?.ToString();

            colNamesDict.TryGetValue("userAge", out int indexIAuserAge);
            string? tmpUserAge = item?.ItemArray[indexIAuserAge]?.ToString();


            if (tmpCity != null && tmpStreet != null && tmpFirstName != null && tmpLastName != null)
            {
                fullDict?.Add($"{tmpFirstName.ToUpper()}, {tmpLastName.ToUpper()}", $"{tmpCity.ToUpper()}, {tmpStreet.Trim().Trim(',').Trim('.').ToUpper()}, {tmpState?.Trim().Trim(',').Trim('.').ToUpper()}, Userage: {tmpUserAge}");
                tmpAddDict?.Add(tmpFirstName.ToUpper(), $"{tmpCity.ToUpper()},{tmpStreet.Trim().Trim(',').Trim('.').ToUpper()}");
            }
        }

        tmpAddDict.GroupBy(p => p.Value).Where(p => p.Count() > 1);    
        var lookup = tmpAddDict.ToLookup(p => p.Value, p => p.Key);

        foreach (var item in lookup)
        {
            string keys = item.Aggregate("", (s, v) => s + ", " + v).ToString().Trim(',');
            int keysCount = keys.Split(',').Count();
            var message = $"This address {item.Key} has {keysCount} occpants";
            Console.WriteLine(message);
        }
        Console.ForegroundColor = ConsoleColor.Yellow;

        foreach (var item in fullDict)
        {
            Console.WriteLine($"{item.Key}");
            Console.WriteLine($"{item.Value}");
        }
                
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("sorted by last name:");
        SQ.GetThatOrdered(rawDataTable);


        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("END OF NEW DATA IMPORT");
        Console.ReadLine();
    }
}

public class GetIsNull
{
    public bool CheckNow(string inputtotest)
    {
        bool valueisnull = false;
        if (inputtotest != null) { valueisnull = true; }
        return valueisnull;
    }
}
public class GetFilePath
{
    public string? FilePath { get; set; }
}
public class GetValue
{
    public string? UserAddressStreetValue { get; set; }
    public string? UserAddressCityValue { get; set; }
    public string? UserAddressStateValue { get; set; }
    public string? UserFirstNameValue { get; set; }
    public string? UserLastNameValue { get; set; }
    public string? UserAgeValue { get; set; }
}

public class SqlQuery
{
    string sortOrder = "userLastName ASC";
    
    public DataTable? FoundDataTable = new();

    internal void GetThatOrdered(DataTable table)
    {
        //// Use the Select method to find all rows matching the filter.
        var FoundRows = table.Select("","userLastName ASC");

        //// Print column 0 of each returned row.
        for (int i = 0; i < FoundRows.Length; i++)
            Console.WriteLine(FoundRows[i][1]);
    }
}