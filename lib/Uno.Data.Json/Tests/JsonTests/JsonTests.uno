using Uno;
using Uno.Collections;
using Uno.IO;
using Uno.Data.Json;
using Uno.Testing;

namespace JsonTests
{
    public class JsonTests
    {
        public const string jsonFile1 = "./TestData/test1.json";
        public const string jsonFile2 = "./TestData/test2.json";
        public const string jsonFile3 = "./TestData/test3.json";
        public const string jsonFile4 = "./TestData/test4.json";

        [Test]
        public void SimpleParse()
        {
            var json = JsonReader.Parse(import(jsonFile1).ReadAllText());
            Assert.IsTrue(json != null);
        }

        [Test]
        public void JsonObject()
        {
            var json = JsonReader.Parse(import(jsonFile1).ReadAllText());
            var user = json["user"];
            Assert.IsTrue(user != null);
            Assert.IsTrue(user.JsonDataType == JsonDataType.Object);
            Assert.AreEqual(7, user.Count);
            Assert.AreEqual("subsquaremusic", user["username"].AsString());
        }

        [Test]
        public void JsonArray()
        {
            var json = JsonReader.Parse(import(jsonFile2).ReadAllText());
            var interests = json["interests"];
            Assert.IsTrue(interests != null);
            Assert.IsTrue(interests.JsonDataType == JsonDataType.Array);
            Assert.AreEqual(3, interests.Count);
            Assert.AreEqual("Mountain Biking", interests[1].AsString());
        }

        [Test]
        public void JsonString()
        {
            var json = JsonReader.Parse(import(jsonFile1).ReadAllText());
            var kind = json["kind"];
            Assert.IsTrue(kind != null);
            Assert.IsTrue(kind.JsonDataType == JsonDataType.String);
            Assert.AreEqual("track", kind.AsString());
        }

        [Test]
        public void JsonNumber()
        {
            var json = JsonReader.Parse(import(jsonFile1).ReadAllText());
            var id = json["user_id"];
            Assert.IsTrue(id != null);
            Assert.IsTrue(id.JsonDataType == JsonDataType.Number);
            Assert.Throws<InvalidCastException>(() => json["user_id"].AsString());
        }

        [Test]
        public void JsonBoolean()
        {
            var json = JsonReader.Parse(import(jsonFile1).ReadAllText());
            var commentable = json["commentable"];
            Assert.IsTrue(commentable != null);
            Assert.IsTrue(commentable.JsonDataType == JsonDataType.Boolean);
            Assert.Throws<InvalidCastException>(() => json["commentable"].AsString());
            Assert.AreEqual(true, commentable.AsBool());
        }

        [Test]
        public void JsonNull()
        {
            var json = JsonReader.Parse(import(jsonFile1).ReadAllText());
            var previous = json["previous"];
            Assert.IsTrue(previous != null);
            Assert.IsTrue(previous.JsonDataType == JsonDataType.Null);
            Assert.Throws<InvalidCastException>(() => json["previous"].AsString());

            var next = json["next"];
            Assert.IsTrue(next != null);
            Assert.IsTrue(next.JsonDataType == JsonDataType.String);
            Assert.AreEqual("null", next.AsString());
        }

        [Test]
        public void JsonKeys()
        {
            var json = JsonReader.Parse(import(jsonFile1).ReadAllText());
            var user = json["user"];
            Assert.AreEqual(7, user.Keys.Length);
            Assert.IsTrue(user.HasKey("permalink"));
            Assert.IsFalse(user.HasKey("fake"));
        }

        [Test]
        public void JsonNotExisted()
        {
            var json = JsonReader.Parse(import(jsonFile1).ReadAllText());
            var notExisted = json["not_existed"];
            Assert.IsTrue(notExisted == null);
        }

        [Test]
        public void SameName()
        {
            var json = JsonReader.Parse(import(jsonFile3).ReadAllText());
            var elem = json["name"];
            Assert.IsTrue(elem != null);
            Assert.IsTrue(elem.JsonDataType == JsonDataType.String);
            Assert.AreEqual("text", elem.AsString());
        }

        [Test]
        public void StringCharacters()
        {
            var json = JsonReader.Parse(import(jsonFile4).ReadAllText());
            var elem = json["comment"];
            Assert.AreEqual("co\\mm!e/n/t\t te\"xt", elem.AsString());
        }

        [Test]
        public void ConvertToBool()
        {
            var json = JsonReader.Parse(import(jsonFile4).ReadAllText());
            Assert.AreEqual(true, json["show"].AsBool());
            Assert.AreEqual(false, json["edited"].AsBool());
        }

        [Test]
        public void ConvertToNumber()
        {
            var json = JsonReader.Parse(import(jsonFile4).ReadAllText());
            Assert.AreEqual(0.412e4, json["rating"].AsNumber());
        }

        [Test]
        public void ConvertNodeToString()
        {
            var json = JsonReader.Parse(import(jsonFile1).ReadAllText());
            Assert.Throws<InvalidCastException>(() => json.AsString());
        }

        [Test]
        public void ConvertNodeToNumber()
        {
            var json = JsonReader.Parse(import(jsonFile1).ReadAllText());
            Assert.Throws<InvalidCastException>(() => json.AsNumber());
        }

        [Test]
        public void ConvertStringToBool()
        {
            var json = JsonReader.Parse(import(jsonFile3).ReadAllText());
            var elem = json["name"];
            Assert.Throws<InvalidCastException>(() => elem.AsBool());
        }

        [Test]
        public void ConvertStringToNumber()
        {
            var json = JsonReader.Parse(import(jsonFile3).ReadAllText());
            var elem = json["name"];
            Assert.Throws<InvalidCastException>(() => elem.AsNumber());
        }

        [Test]
        public void ConvertNullToString()
        {
            var json = JsonReader.Parse(import(jsonFile3).ReadAllText());
            var elem = json["previous"];
            Assert.Throws<NullReferenceException>(() => elem.AsString());
        }

        [Test]
        public void ConvertNullToNumber()
        {
            var json = JsonReader.Parse(import(jsonFile3).ReadAllText());
            var elem = json["previous"];
            Assert.Throws<NullReferenceException>(() => elem.AsNumber());
        }

        [Test]
        public void ConvertNullToBool()
        {
            var json = JsonReader.Parse(import(jsonFile3).ReadAllText());
            var elem = json["previous"];
            Assert.Throws<NullReferenceException>(() => elem.AsBool());
        }

        [Test]
        public void ParseInvalidJson()
        {
            Assert.Throws<JsonException>(() => JsonReader.Parse("\"name\":\"text\""));
        }

        [Test]
        public void NotClosedTag()
        {
            Assert.Throws<JsonException>(() => JsonReader.Parse("{\"ttt\":null,\"name\":\"text\""));
        }

        [Test]
        public void NoValue()
        {
            Assert.Throws<JsonException>(() => JsonReader.Parse("{\"name\":}"));
        }

        [Test]
        public void NoName()
        {
            Assert.Throws<JsonException>(() => JsonReader.Parse("{:\"value\"}"));
        }

        [Test]
        public void TooManyClosingBrackets()
        {
            Assert.Throws<JsonException>(() => JsonReader.Parse("{\"name\":\"text\"}}"));
        }

        void VerifyParseInvalid(string json, string expectedMessage,
            [Uno.Compiler.CallerFilePath] string filePath = "", [Uno.Compiler.CallerLineNumber] int lineNumber = 0, [Uno.Compiler.CallerMemberName] string memberName = "")
        {
            try
            {
                JsonReader.Parse(json);
                Assert.Fail("JsonReader.Parse should have thrown an exception", filePath, lineNumber, memberName);
            }
            catch (JsonException e)
            {
                Assert.Contains(expectedMessage, e.Message, filePath, lineNumber, memberName);
            }
        }

        [Test]
        public void ParseKeywords()
        {
            Assert.AreEqual(JsonDataType.Null, JsonReader.Parse("null").JsonDataType);
            Assert.AreEqual(false, JsonReader.Parse("false").AsBool());
            Assert.AreEqual(true, JsonReader.Parse("true").AsBool());
        }

        [Test]
        public void InvalidKeywords()
        {
            VerifyParseInvalid("trui", "Unexpected character: i");
            VerifyParseInvalid("falss", "Unexpected character: s");
            VerifyParseInvalid("nULL", "Unexpected character: U");
        }

        [Test]
        public void ParseNumber()
        {
            Assert.AreEqual(1337, JsonReader.Parse("1337.0").AsNumber());
            Assert.AreEqual(1337.0, JsonReader.Parse("1337").AsNumber());
            Assert.AreEqual(1e+10, JsonReader.Parse("1e+10").AsNumber());
            Assert.AreEqual(1E-10, JsonReader.Parse("1E-10").AsNumber());
        }

        [Test]
        public void InvalidNumber()
        {
            VerifyParseInvalid("@3", "Unexpected character: @");
            VerifyParseInvalid("+3", "Unexpected character: +");
            VerifyParseInvalid(".0", "Unexpected character: .");
            VerifyParseInvalid("0.", "Unexpected end of file");
            VerifyParseInvalid("0.e", "Unexpected character: e");
            VerifyParseInvalid("0e+", "Unexpected end of file");
            VerifyParseInvalid("00.0", "Expected end of file");
            VerifyParseInvalid("0..0", "Unexpected character: .");
            VerifyParseInvalid("-eee", "Unexpected character: e");
            VerifyParseInvalid("1e@", "Unexpected character: @");
            VerifyParseInvalid("1.0e-X", "Unexpected character: X");
        }

        [Test]
        public void ParseString()
        {
            Assert.AreEqual("foo", JsonReader.Parse("\"foo\"").AsString());
            Assert.AreEqual("bar", JsonReader.Parse("\"bar\"").AsString());
            Assert.AreEqual("/", JsonReader.Parse("\"\\u002F\"").AsString());
            Assert.AreEqual("/", JsonReader.Parse("\"\\u002f\"").AsString());
            Assert.AreEqual("/", JsonReader.Parse("\"\\/\"").AsString());
            Assert.AreEqual("\u00B1", JsonReader.Parse("\"\\u00B1\"").AsString());
            Assert.AreEqual("\uD834\uDD1E", JsonReader.Parse("\"\\uD834\\uDD1E\"").AsString());
            Assert.AreEqual("\\@", JsonReader.Parse("\"\\@\"").AsString());
        }

        [Test]
        public void InvalidString()
        {
            VerifyParseInvalid("\"foo", "Unexpected end of file");
            VerifyParseInvalid("\"\\", "Unexpected end of file");
            VerifyParseInvalid("\"\\u000@\"", "Unexpected character: @");
            VerifyParseInvalid("\"\\u000", "Unexpected end of file");
        }

        [Test]
        public void ParseArray()
        {
            Assert.AreEqual(JsonDataType.Array, JsonReader.Parse("[]").JsonDataType);
            Assert.AreEqual(0, JsonReader.Parse("[]").Count);

            Assert.AreEqual(3, JsonReader.Parse("[1,2,3]").Count);
            Assert.AreEqual(3, JsonReader.Parse("[ 1, 2, 3]").Count);
            Assert.AreEqual(3, JsonReader.Parse("[1 ,2 ,3]").Count);
            Assert.AreEqual(3, JsonReader.Parse("[1, 2, 3 ]").Count);
        }

        [Test]
        public void InvalidArray()
        {
            VerifyParseInvalid("[", "Unexpected end of file");
            VerifyParseInvalid("[1", "Unexpected end of file");
            VerifyParseInvalid("[1,", "Unexpected end of file");
        }

        [Test]
        public void ParseObject()
        {
            Assert.AreEqual(JsonDataType.Object, JsonReader.Parse("{}").JsonDataType);
            Assert.AreEqual(0, JsonReader.Parse("{}").Count);

            Assert.AreEqual(2, JsonReader.Parse("{\"foo\":1,\"bar\":2}").Count);
            Assert.AreEqual(2, JsonReader.Parse("{ \"foo\":1, \"bar\":2}").Count);
            Assert.AreEqual(2, JsonReader.Parse("{\"foo\" :1,\"bar\" :2}").Count);
            Assert.AreEqual(2, JsonReader.Parse("{\"foo\": 1,\"bar\": 2}").Count);
            Assert.AreEqual(2, JsonReader.Parse("{\"foo\":1 ,\"bar\":2 }").Count);
            Assert.AreEqual(2, JsonReader.Parse("{\"foo\":1, \"bar\":2}").Count);
            Assert.AreEqual(1, JsonReader.Parse("{\"foo\":1, \"foo\":2}").Count);
        }

        [Test]
        public void InvalidObject()
        {
            VerifyParseInvalid("{", "Unexpected end of file");
            VerifyParseInvalid("{\"foo\"", "Unexpected end of file");
            VerifyParseInvalid("{\"foo\":", "Unexpected end of file");
            VerifyParseInvalid("{\"foo\":1", "Unexpected end of file");
            VerifyParseInvalid("{\"foo\":1,", "Unexpected end of file");
        }

        void VerifyQuoteString(string expected, string input,
            [Uno.Compiler.CallerFilePath] string filePath = "", [Uno.Compiler.CallerLineNumber] int lineNumber = 0, [Uno.Compiler.CallerMemberName] string memberName = "")
        {
            var quoted = JsonWriter.QuoteString(input);
            Assert.AreEqual(expected, quoted, filePath, lineNumber, memberName);
            Assert.AreEqual(input, JsonReader.Parse(quoted).AsString(), filePath, lineNumber, memberName);
        }

        [Test]
        public void QuoteString()
        {
            VerifyQuoteString("\"foo\"", "foo");
            VerifyQuoteString("\"\\\"\"", "\"");
            VerifyQuoteString("\"\\\\\"", "\\");
            VerifyQuoteString("\"/\"", "/");
            VerifyQuoteString("\"\\b\"", "\b");
            VerifyQuoteString("\"\\f\"", "\f");
            VerifyQuoteString("\"\\n\"", "\n");
            VerifyQuoteString("\"\\r\"", "\r");
            VerifyQuoteString("\"\\t\"", "\t");
            VerifyQuoteString("\"\\u0001\"", "\u0001");
            VerifyQuoteString("\"\u00b1\"", "\u00b1");
            VerifyQuoteString("\"\u0950\"", "\u0950");
        }

        void VerifyEscapeString(string expected, string input,
            [Uno.Compiler.CallerFilePath] string filePath = "", [Uno.Compiler.CallerLineNumber] int lineNumber = 0, [Uno.Compiler.CallerMemberName] string memberName = "")
        {
            var escaped = JsonWriter.EscapeString(input);
            Assert.AreEqual(expected, escaped, filePath, lineNumber, memberName);
            var quoted = "\"" + escaped + "\"";
            Assert.AreEqual(input, JsonReader.Parse(quoted).AsString(), filePath, lineNumber, memberName);
        }

        [Test]
        public void EscapeString()
        {
            VerifyEscapeString("foo", "foo");
            VerifyEscapeString("\\\"", "\"");
            VerifyEscapeString("\\\\", "\\");
            VerifyEscapeString("/", "/");
            VerifyEscapeString("\\b", "\b");
            VerifyEscapeString("\\f", "\f");
            VerifyEscapeString("\\n", "\n");
            VerifyEscapeString("\\r", "\r");
            VerifyEscapeString("\\t", "\t");
            VerifyEscapeString("\\u0001", "\u0001");
            VerifyEscapeString("\u00b1", "\u00b1");
            VerifyEscapeString("\u0950", "\u0950");
        }
    }
}
