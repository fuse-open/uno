class Main
{
    public static void Main()
    {
        int a = new int[5][,][]; // $E Expected ']' following '[' -- found ',' (Comma)
        int[,] b = new int[3]; // $E Expected expression following '[' -- found ',' (Comma)

        int[][] c = new int[10][];
        c[0] = new int[5][5]; // $E Expected ']' following '[' -- found 5 (DecimalLiteral)
        c[0] = new int[2];
        c[1] = new int[2]{1,2};
    }
}
