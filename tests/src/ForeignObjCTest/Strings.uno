using Uno;
using Uno.Compiler.ExportTargetInterop;
using Uno.Testing;

namespace ForeignObjCTest
{
    public extern(FOREIGN_OBJC_SUPPORTED) class Strings
    {
        readonly string[] _strings = new string[]
        {
            "",
            "abc",
            "The quick brown fox jumps over the lazy dog",
            "ç, é, õ",
            "åååååååååååååææææææææøøøøøøøøøøøøø ç, é, õ aaaaaaaaaaaabbbbbbbbbbc ccccccc",
            "eeeeææææææææææaaaaaaaaa",
            "صِفْ Amiri3 صِفْ خَلْقَ Amiri2 صِفْ خَلْقَ خَوْدٍ Amiri1 صِفْ خَلْقَ خَوْدٍ صِفْ",
            "𐐷𐐷𐐷𐐷",
            "𐐷𐐷𐐷𐐷abc𤭢𤭢𤭢𤭢a𐐷𐐷𐐷𐐷abc𤭢𤭢𤭢𤭢a𐐷𐐷𐐷𐐷abc𤭢𤭢𤭢𤭢a𐐷𐐷𐐷𐐷abc𤭢𤭢𤭢𤭢a",
            "Emoji 😃  are such fun!",
            "देवनागरीदेवनागरीदेवनागरीदेवनागरीदेवनागरीदेवनागरीदेवनागरीदेवनागरीदेवनागरी",
            " א ב ג ד ה ו ז ח ט י\n כ ך ל מ ם נ ן ס ע פ\n ף צ ץ ק ר ש ת  •  ﭏ",
            "Testing «ταБЬℓσ»: 1<2 & 4+1>3, now 20% off!",
            "٩(-̮̮̃-̃)۶ ٩(●̮̮̃•̃)۶ ٩(͡๏̯͡๏)۶ ٩(-̮̮̃•̃).",
            "Quizdeltagerne spiste jordbær med fløde, mens cirkusklovnen Wolther spillede på xylofon.",
            "Falsches Üben von Xylophonmusik quält jeden größeren Zwerg",
            "Γαζέες καὶ μυρτιὲς δὲν θὰ βρῶ πιὰ στὸ χρυσαφὶ ξέφωτο",
            "Ξεσκεπάζω τὴν ψυχοφθόρα βδελυγμία",
            "El pingüino Wenceslao hizo kilómetros bajo exhaustiva lluvia y frío, añoraba a su querido cachorro.",
            "Le cœur déçu mais l'âme plutôt naïve, Louÿs rêva de crapaüter en canoë au delà des îles, près du mälström où brûlent les novæ.",
            "D'fhuascail Íosa, Úrmhac na hÓighe Beannaithe, pór Éava agus Ádhaimh",
            "Árvíztűrő tükörfúrógép",
            "Kæmi ný öxi hér ykist þjófum nú bæði víl og ádrepa",
            "Sævör grét áðan því úlpan var ónýt",
            "いろはにほへとちりぬるを\n わかよたれそつねならむ\n うゐのおくやまけふこえて\n あさきゆめみしゑひもせす\n",
            "イロハニホヘト チリヌルヲ ワカヨタレソ ツネナラム\n ウヰノオクヤマ ケフコエテ アサキユメミシ ヱヒモセスン",
            "? דג סקרן שט בים מאוכזב ולפתע מצא לו חברה איך הקליטה",
            "Pchnąć w tę łódź jeża lub ośm skrzyń fig",
            "В чащах юга жил бы цитрус? Да, но фальшивый экземпляр!",
            "Съешь же ещё этих мягких французских булок да выпей чаю",
            "๏ เป็นมนุษย์สุดประเสริฐเลิศคุณค่า  กว่าบรรดาฝูงสัตว์เดรัจฉาน",
            "Pijamalı hasta, yağız şoföre çabucak güvendi.",
        };

        [Foreign(Language.ObjC)] static string StringId(string x) @{ return x; @}

        [Test]
        public void StringIds()
        {
            foreach (var s in _strings)
                Assert.AreEqual(s, StringId(s));
        }

        [Foreign(Language.ObjC)] static string Append(string x, string y)
        @{
            /* I'm a comment */
            // Another comment
            return [x stringByAppendingString:y];
            // I'm another comment
        @}

        [Test]
        public void Appends()
        {
            foreach (var s1 in _strings)
            foreach (var s2 in _strings)
                Assert.AreEqual(s1 + s2, Append(s1, s2));
        }

        [Foreign(Language.ObjC)]
        string staticString()
        @{
            /* I'm a comment */
            // Another comment
            static NSString* str = @"hej hopp";
            return str;
        @}

        [Test]
        public void StaticString()
        {
            Assert.AreEqual("hej hopp", staticString());
        }

        [Foreign(Language.ObjC)]
        string newString()
        @{
            /* I'm a comment */
            // Another comment
            return [[NSString alloc] initWithUTF8String: "ooob"];
        @}

        [Test]
        public void NewString()
        {
            Assert.AreEqual("ooob", newString());
        }
    }
}
