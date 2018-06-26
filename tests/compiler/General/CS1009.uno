class Main
{
    public Main()
    {
        string a = "\m"; // $E1007
        string a = "\t";

        //The following line should not yield multiple E1007 errors
        string filename1 = "c:\myFolder\myFile.txt"; // $E1007 $E1007
        string filename2 = "c:\\myFolder\\myFile.txt";

        // FIXME: Verbatim strings (@"") are not yet supported
        //string filename3 = @"c:\myFolder\myFile.txt";
    } // $E Expected statement or '}' following ';' -- found '}' (RightCurlyBrace)
}
