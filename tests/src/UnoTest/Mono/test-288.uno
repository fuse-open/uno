namespace Mono.test_288
{
    using Uno;
                                 
    namespace Test
    {
            public interface IBook
            {
                    string GetItem (int i);
                    string this [int i] { get; }
            }
                                                                              
                                                                              
                                 
            public interface IMovie
            {
                    string GetItem (int i);
                    string this [int i] { get; }
            }
                                                                              
                                                                              
                                 
            public class BookAboutMovie : IBook, IMovie
            {
                    private string title = "";
                    public BookAboutMovie (string title)
                    {
                            this.title = title;
                    }
                                                                              
                                                                              
                                 
                    public string GetItem (int i)
                    {
                            return title;
                    }
                                                                              
                                                                              
                                 
                    public string this [int i]
                    {
                            get { return title; }
                    }
    
                    [Uno.Testing.Test] public static void test_288() { Uno.Testing.Assert.AreEqual(0, Main(new string[0])); }
        public static int Main(string[] args)
                    {
                            BookAboutMovie jurassicPark = new BookAboutMovie("Jurassic Park");
                            Console.WriteLine ("Book Title : " + jurassicPark.GetItem (2));
                            Console.WriteLine ("Book Title : " + ((IBook)jurassicPark)[2] );
                            return 0;
                    }
            }
    }
}
