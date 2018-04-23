#!/usr/bin/python
import os
blacklist = open('_blacklist.txt').read()
ignore = open('_ignore.txt').read()
skipped = 0
ported = 0

for subdir, dirs, files in os.walk('C:/mono/mcs/tests'):
    for file in files:
        if '.cs' not in file or 'Main.cs' in file or '.csproj' in file:
            continue

        src_name = os.path.join(subdir, file)
        dst_name = file.replace('.cs', '.uno')

        if '\n' + dst_name in blacklist:
            print 'SKIP ' + src_name
            skipped += 1
            continue

        src = open(src_name)
        src_code = src.read()
        print src_name + ' -> ' + dst_name

        name = file.replace('.cs', '').replace('-lib', '').replace('-', '_')
        src_code = '    ' + src_code.replace('\t', '    ').replace('\n', '\n    ').strip()
        src_code = src_code.replace('using System;', 'using Uno;')
        src_code = src_code.replace('System.Collections.Generic', 'Uno.Collections')
        src_code = src_code.replace('System.Collections', 'Uno.Collections')
        src_code = src_code.replace('System.Threading', 'Uno.Threading')
        src_code = src_code.replace('System.Text', 'Uno.Text')
        src_code = src_code.replace('System.Console', 'Console')
        src_code = src_code.replace('System.', 'Uno.')
        src_code = src_code.replace('apply', '@apply')
        src_code = src_code.replace('UInt64', 'ULong')
        src_code = src_code.replace('UInt32', 'UInt')
        src_code = src_code.replace('UInt16', 'UShort')
        src_code = src_code.replace('Int64', 'Long')
        src_code = src_code.replace('Int32', 'Int')
        src_code = src_code.replace('Int16', 'Short')
        src_code = src_code.replace('Single', 'Float')
        src_code = src_code.replace('String []', 'string[]')
        src_code = src_code.replace('String[]', 'string[]')
        src_code = src_code.replace('string []', 'string[]')
        src_code = src_code.replace('( string[]', '(string[]')
        src_code = src_code.replace('Uno.string[]', 'Uno.String[]')
        src_code = src_code.replace('namespace Uno.', 'namespace System.')
        src_code = src_code.replace("\\tpublic static void Main ()\\n", "\\tpublic static void Brain ()\\n")
        src_code = src_code.replace(' Main (', ' Main(')
        src_code = src_code.replace(' Main(params string[]', ' Main(string[]')
        src_code = src_code.replace('    static void Main(', '    public static void Main(')
        src_code = src_code.replace('    static int Main(', '    public static int Main(')
        src_code = src_code.replace('static public void Main(', 'public static void Main(')
        src_code = src_code.replace('static public int Main(', 'public static int Main(')
        src_code = src_code.replace('public static int Main()', '[Uno.Testing.Test] public static void ' + name + '() { Uno.Testing.Assert.AreEqual(0, Main()); }\n        public static int Main()')
        src_code = src_code.replace('public static int Main(string[]', '[Uno.Testing.Test] public static void ' + name + '() { Uno.Testing.Assert.AreEqual(0, Main(new string[0])); }\n        public static int Main(string[]')
        src_code = src_code.replace('public static void Main()', '[Uno.Testing.Test] public static void ' + name + '() { Main(); }\n        public static void Main()')
        src_code = src_code.replace('public static void Main(string[]', '[Uno.Testing.Test] public static void ' + name + '() { Main(new string[0]); }\n        public static void Main(string[]')

        if '\n' + dst_name in ignore:
            src_code = src_code.replace('[Uno.Testing.Test]', '[Uno.Testing.Ignore, Uno.Testing.Test]')

        if ' Main(' in src_code and 'Uno.Testing.Test' not in src_code:
            print 'WARNING: test not added in ' + dst_name

        dst = open(dst_name, 'w')
        dst.write('namespace Mono.' + name)
        dst.write('\n{\n' + src_code + '\n}\n')
        ported += 1

print 'DONE! -- skipped: ' + str(skipped) + ' ported: ' + str(ported)
