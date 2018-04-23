/* The Computer Language Benchmarks Game
   http://benchmarksgame.alioth.debian.org/

   contributed by Robert F. Tobler
   optimized based on java & C# by Enotus, Isaac Gouy, and Alp Toker
*/

using Uno;
using Uno.IO;
using Uno.Testing;
using Uno.Text;

public class FastaReduxTest
{
    [Test]
    public void Main () {
        FastaRedux.AccumulateAndScale(FastaRedux.HomoSapiens);
        FastaRedux.AccumulateAndScale(FastaRedux.IUB);
        int n = 250000;
        using (MemoryStream m = new MemoryStream ()) {
            using (Stream s = m/*Console.OpenStandardOutput ()*/) {
                s.WriteRepeatFasta ("ONE", "Homo sapiens alu",
                    Utf8.GetBytes (FastaRedux.ALU), n * 2);
                s.WriteRandomFasta ("TWO", "IUB ambiguity codes",
                    FastaRedux.IUB, n * 3);
                s.WriteRandomFasta ("THREE", "Homo sapiens frequency",
                    FastaRedux.HomoSapiens, n * 5);
            }
        }
    }
}

public static class FastaRedux
{
   const int LINE_LEN = 60;
   const int BUF_LEN = 64 * 1024;
   const byte LF = (byte)'\n';

   const int LOOKUP_LEN = 1024;
   const double LOOKUP_SCALE = LOOKUP_LEN - 1;

    public static readonly string ALU =
        "GGCCGGGCGCGGTGGCTCACGCCTGTAATCCCAGCACTTTGG" +
        "GAGGCCGAGGCGGGCGGATCACCTGAGGTCAGGAGTTCGAGA" +
        "CCAGCCTGGCCAACATGGTGAAACCCCGTCTCTACTAAAAAT" +
        "ACAAAAATTAGCCGGGCGTGGTGGCGCGCGCCTGTAATCCCA" +
        "GCTACTCGGGAGGCTGAGGCAGGAGAATCGCTTGAACCCGGG" +
        "AGGCGGAGGTTGCAGTGAGCCGAGATCGCGCCACTGCACTCC" +
        "AGCCTGGGCGACAGAGCGAGACTCCGTCTCAAAAA";

   struct Freq {
      public double P;
      public byte C;

      public Freq (char c, double p) { C = (byte)c; P = p; }
    }

    public static Freq[] IUB = new[] {
      new Freq('a', 0.27), new Freq('c', 0.12), new Freq('g', 0.12),
      new Freq('t', 0.27), new Freq('B', 0.02), new Freq('D', 0.02),
      new Freq('H', 0.02), new Freq('K', 0.02), new Freq('M', 0.02),
      new Freq('N', 0.02), new Freq('R', 0.02), new Freq('S', 0.02),
      new Freq('V', 0.02), new Freq('W', 0.02), new Freq('Y', 0.02),
    };

    public static Freq[] HomoSapiens = new[] {
      new Freq ('a', 0.3029549426680), new Freq ('c', 0.1979883004921),
      new Freq ('g', 0.1975473066391), new Freq ('t', 0.3015094502008),
    };

    public static void AccumulateAndScale(Freq[] a) {
        double cp = 0.0;
        for (int i = 0; i < a.Length; i++)
         a[i].P = (cp += a[i].P) * LOOKUP_SCALE;
      a[a.Length - 1].P = LOOKUP_SCALE;
    }

   static byte[] buf = new byte[BUF_LEN];

   public static int WriteDesc(this byte[] buf, string id, string desc)
   {
        var ds = Utf8.GetBytes (">" + id + " " + desc + "\n");
      for (int i = 0; i < ds.Length; i++) buf[i] = ds[i];
      return BUF_LEN - ds.Length;
   }

   public static int Min(int a, int b) { return a < b ? a : b; }

   public static void WriteRepeatFasta(
         this Stream s, string id, string desc, byte[] alu, int nr)
   {
      int alen = alu.Length;
      int ar = alen, br = buf.WriteDesc(id, desc), lr = LINE_LEN;
      while (nr > 0)
      {
         int r = Min(Min(nr, lr), Min(ar, br));
         for (int ai = alen - ar, bi = BUF_LEN - br, be = bi + r;
             bi < be; bi++) { buf[bi] = alu[ai]; ai++; }
         nr -= r; lr -= r; br -= r; ar -= r;
         if (ar == 0) ar = alen;
         if (br == 0) { s.Write(buf, 0, BUF_LEN); br = BUF_LEN; }
         if (lr == 0) { buf[BUF_LEN - (br--)] = LF; lr = LINE_LEN; }
         if (br == 0) { s.Write(buf, 0, BUF_LEN); br = BUF_LEN; }
      }
      if (lr < LINE_LEN) buf[BUF_LEN - (br--)] = LF;
      if (br < BUF_LEN) s.Write(buf, 0, BUF_LEN - br);
   }

   static Freq[] lookup = new Freq[LOOKUP_LEN];

   static void CreateLookup(Freq[] fr) {
      for (int i = 0, j = 0; i < LOOKUP_LEN; i++) {
         while (fr[j].P < i) j++;
         lookup[i] = fr[j];
      }
   }

    const int IM = 139968;
    const int IA = 3877;
    const int IC = 29573;
   const double SCALE = LOOKUP_SCALE / IM;

    static int last = 42;

   public static void WriteRandomFasta(
         this Stream s, string id, string desc, Freq[] fr, int nr)
   {
      CreateLookup(fr);
      int br = buf.WriteDesc(id, desc), lr = LINE_LEN;
      while (nr > 0)
      {
         int r = Min(Min(nr, lr), br);
         for (int bi = BUF_LEN - br, be = bi + r; bi < be; bi++)
         {
            double p = SCALE * (last = (last * IA + IC) % IM);
            int ai = (int)p; if (lookup[ai].P < p) ai++;
            buf[bi] = lookup[ai].C;
         }
         nr -= r; lr -= r; br -= r;
         if (br == 0) { s.Write(buf, 0, BUF_LEN); br = BUF_LEN; }
         if (lr == 0) { buf[BUF_LEN - (br--)] = LF; lr = LINE_LEN; }
         if (br == 0) { s.Write(buf, 0, BUF_LEN); br = BUF_LEN; }
      }
      if (lr < LINE_LEN) buf[BUF_LEN - (br--)] = LF;
      if (br < BUF_LEN) s.Write(buf, 0, BUF_LEN - br);
   }

}
