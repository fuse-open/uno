public class Main
{
    public Main()
    {
        int myarray[2]; // $E Expected ',' or ';' following 'myarray' -- found '[' (LeftSquareBrace)

        // Correct declaration.
        int[] myarray2;

        // Declaration and initialization in one statement
        int[] myArray3= new int[2] { 1,2 }

        // Access an array element.
        myarray3[0] = 0; // $E Expected ',' or ';' following '}' -- found 'myarray3' (Identifier)
    } // $E Expected statement or '}' following ';' -- found '}' (RightCurlyBrace)
}
