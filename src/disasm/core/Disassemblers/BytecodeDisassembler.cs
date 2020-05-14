using System;
using System.Collections.Generic;
using System.IO;
using Uno.Compiler.API;
using Uno.Compiler.API.Domain.Bytecode;
using Uno.Compiler.API.Domain.IL;

namespace Uno.Disasm.Disassemblers
{
    public class BytecodeDisassembler : Disassembler
    {
        public override string ToString()
        {
            return "Bytecode";
        }

        protected override void AppendBody(Function function)
        {
            var bytecode = function.CreateBytecodeCompiler();
            bytecode.Compile();
            AppendInstructions(bytecode.Code.ToArray());
        }

        void AppendInstructions(Instruction[] code)
        {
            var lines = new Dictionary<string, string[]>();

            for (int i = 0; i < code.Length; i++)
            {
                var linenum = i.ToString("D4") + "    ";
                var instr = code[i].Opcode.ToString();

                if (code[i].Opcode == Opcodes.MarkSource)
                {
                    var src = (Source)code[i].Argument;

                    if (!src.IsUnknown)
                    {
                        string[] l;
                        if (!lines.TryGetValue(src.FullPath, out l))
                        {
                            l = File.ReadAllLines(src.FullPath);
                            lines.Add(src.FullPath, l);
                        }

                        AppendLine("[" + i.ToString("D4") + "]   ; " + l[src.Line - 1].Trim() + " // " + src);
                    }
                }
                else
                {
                    var spacing = new string(' ', Math.Max(0, 28 - instr.Length));

                    if (code[i].Opcode == Opcodes.MarkLabel)
                        AppendLine("[" + i.ToString("D4") + "]");
                    else
                        AppendLine(" " + linenum + instr + spacing + (
                            code[i].Argument != null
                                ? code[i].Argument.ToLiteral()
                                : null));
                }
            }
        }
    }
}
