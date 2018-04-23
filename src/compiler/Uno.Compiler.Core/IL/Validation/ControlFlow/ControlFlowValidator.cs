using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Logging;

namespace Uno.Compiler.Core.IL.Validation.ControlFlow
{
    public partial class ControlFlowValidator
    {
        public readonly Function Function;

        public ControlFlowValidator(Function func)
        {
            Function = func;
            Function.Stats &= ~EntityStats.ImplicitReturn;
        }

        public BasicBlock EntryPoint { get; private set; }

        class MemoryNode
        {
            public readonly string Name;
            public readonly MemoryNode Parent;
            public readonly Dictionary<Field, MemoryNode> Children;

            public MemoryNode(MemoryNode parent, string name, DataType dt)
            {
                Parent = parent;
                Name = name;

                if (!dt.IsStruct)
                    return;

                foreach (var f in dt.Fields)
                {
                    if (f.IsStatic)
                        continue;

                    if (Children == null)
                        Children = new Dictionary<Field, MemoryNode>();

                    Children.Add(f, new MemoryNode(this, f.UnoName, f.ReturnType));
                }

                foreach (var f in dt.Properties)
                {
                    if (f.IsStatic)
                        continue;

                    if (f.ImplicitField != null)
                    {
                        if (Children == null)
                            Children = new Dictionary<Field, MemoryNode>();

                        Children.Add(f.ImplicitField, new MemoryNode(this, "Generated backing field for property " + f.UnoName, f.ImplicitField.ReturnType));
                    }
                }

                foreach (var f in dt.Events)
                {
                    if (f.IsStatic)
                        continue;

                    if (f.ImplicitField != null)
                    {
                        if (Children == null)
                            Children = new Dictionary<Field, MemoryNode>();

                        Children.Add(f.ImplicitField, new MemoryNode(this, "Generated backing field for event " + f.UnoName, f.ImplicitField.ReturnType));
                    }
                }
            }

            public override string ToString()
            {
                return Name;
            }
        }

        class MemoryRoots
        {
            public MemoryNode This;
            public Dictionary<Variable, MemoryNode> Locals;
            public Dictionary<int, MemoryNode> OutParameters;

            public MemoryNode AddLocal(Variable variable)
            {
                if (Locals == null)
                    Locals = new Dictionary<Variable, MemoryNode>();

                if (Locals.ContainsKey(variable))
                    return Locals[variable];

                var node = new MemoryNode(null, variable.Name, variable.ValueType);
                Locals.Add(variable, node);
                return node;
            }
        }

        readonly MemoryRoots memory = new MemoryRoots();
        Log Log;

        public void Validate(Log log)
        {
            if (!Function.HasBody)
                return;

            Log = log;
            EntryPoint = NewBlock(Function.Source);
            _current = EntryPoint;

            if (Function.IsConstructor && Function.DeclaringType.IsStruct)
            {
                // TODO: Fix this
                // Structs constructors must initialize all nonstatic fields of 'this' before 'this' can be used.
                // Register memory node for 'this'
                //memory.This = new MemoryNode(null, "this", TypeProcessor.GetParameterizedType(Function.DeclaringType));
            }

            // Register memory nodes for all out-parameters
            for (int i = 0; i < Function.Parameters.Length; i++)
            {
                var p = Function.Parameters[i];

                if (p.Modifier != ParameterModifier.Out)
                    continue;

                if (memory.OutParameters == null)
                    memory.OutParameters = new Dictionary<int, MemoryNode>();

                memory.OutParameters.Add(i, new MemoryNode(null, p.Name, p.Type));
            }

            CompileStatement(Function.Body);

            var reachableBlocks = new HashSet<BasicBlock>();

            FindReachableBlocksAndValidateReturnPaths(EntryPoint, reachableBlocks);

            // TODO: This reports false positives
            /*
            // Give warnings on unreachable code
            foreach (var b in blocks)
                if (!reachableBlocks.Contains(b) && (b.Instructions.Count > 0 || b.Ending == BlockEnding.CondBr))
                    UnreachableCodeDetected(b.Source);
            */
            foreach (var b in reachableBlocks)
            {
                // Verify that all memory is written before read in reachable code
                for (int k = 0; k < b.Instructions.Count; k++)
                {
                    var i = b.Instructions[k];
                    if (i.Opcode == Opcodes.ReadNode || i.Opcode == Opcodes.ValidateNodeIsInitialized)
                    {
                        // Hack: this is reporting false positives
                        if (!IsNodeInitializedBefore(b, i.Argument as MemoryNode, b, k))
                            Log.Error(i.Source, ErrorCode.E4511, "Potential use of " + (i.Argument as MemoryNode).Name.Quote() + " before it is initialized");
                    }
                }

                // Verify that all required memory is initialized before return
                if (b.Ending == BlockEnding.RetNonVoid || b.Ending == BlockEnding.RetVoid)
                {
                    if (memory.This != null)
                    {
                        if (!IsNodeInitializedBefore(b, memory.This, b, b.Instructions.Count-1))
                        {
                            string hints = "";

                            if (memory.This.Children != null)
                            {
                                foreach (var f in memory.This.Children)
                                {
                                    if (!IsNodeInitializedBefore(b, f.Value, b, b.Instructions.Count - 1))
                                    {
                                        if (f.Key.IsGenerated)
                                            hints += f.Value.Name.Quote() + " is not initialized. ";
                                        else
                                            hints += "The field " + f.Key.Quote() + " is not initialized. ";
                                    }
                                }

                                Log.Error(b.Source, ErrorCode.E4512, "All non-static fields must be initialized before returning from struct constructor (in all code paths). " + hints);
                            }
                        }
                    }

                    if (memory.OutParameters != null)
                        foreach (var p in memory.OutParameters)
                            if (!IsNodeInitializedBefore(b, p.Value, b, b.Instructions.Count - 1))
                                Log.Error(b.Source, ErrorCode.E4513, "Out-parameter " + Function.Parameters[p.Key].Name.Quote() + " must be initialized before returning (in all code paths)");
                }
            }
        }

        bool IsNodeInitializedBefore(BasicBlock origin, MemoryNode node, BasicBlock b, int pos)
        {
            // If the node is written directly, it is initialized
            if (IsNodeWrittenBefore(origin, node, b, pos, new HashSet<BasicBlock>()))
                return true;

            // If the node has ancesors, and any ancestor is written before, the node is initialized
            var p = node.Parent;
            while (p != null)
            {
                if (IsNodeWrittenBefore(origin, p, b, pos, new HashSet<BasicBlock>()))
                    return true;

                p = p.Parent;
            }

            // Otherwise, check recursively that all children are initialized
            if (node.Children != null && node.Children.Count > 0)
            {
                foreach (var c in node.Children)
                    if (!IsNodeInitializedBefore(origin, c.Value, b, pos))
                        return false;

                // If there are children and they are all initialized, the node is initialized
                return true;
            }

            // If the node has no children and is not written to, it is not initialized
            return false;
        }

        bool IsNodeWrittenBefore(BasicBlock origin, MemoryNode node, BasicBlock b, int pos, HashSet<BasicBlock> reachedSet)
        {
            if (b == null)
                return false;

            if (reachedSet.Contains(b))
                return true;

            reachedSet.Add(b);

            for (int i = pos; i >= 0; i--)
            {
                // Ignore writes within other try-blocks
                if (b.TryDepth >= origin.TryDepth && b.TryBlock != origin.TryBlock)
                    continue;

                if (b.Instructions[i].Opcode == Opcodes.WriteNode &&
                    b.Instructions[i].Argument == node)
                    return true;
            }

            if (predecessors.ContainsKey(b) && predecessors[b].Any())
            {
                foreach (var p in predecessors[b])
                    if (!IsNodeWrittenBefore(origin, node, p, p.Instructions.Count - 1, reachedSet))
                        return false;

                return true;
            }

            return false;
        }

        readonly Dictionary<BasicBlock, List<BasicBlock>> predecessors = new Dictionary<BasicBlock, List<BasicBlock>>();

        void FindReachableBlocksAndValidateReturnPaths(BasicBlock block, HashSet<BasicBlock> reachedSet)
        {
            if (reachedSet.Contains(block))
                return;

            reachedSet.Add(block);

            // register predecessors
            if (block.Successors != null)
            {
                foreach (var s in block.Successors)
                {
                    List<BasicBlock> pr;
                    if (!predecessors.TryGetValue(s, out pr))
                    {
                        pr = new List<BasicBlock>();
                        predecessors.Add(s, pr);
                    }

                    pr.Add(block);
                }
            }

            if (block.Ending == BlockEnding.Open)
            {
                if (Function.ReturnType.IsVoid)
                {
                    block.Ending = BlockEnding.RetVoid;

                    // Signal bytecode backends that this method has a fall-through and needs an implicit return instruction emitted at the end
                    Function.Stats |= EntityStats.ImplicitReturn;
                }
                else if (!(Function.Body.Statements.Count == 1 && Function.Body.Statements[0] is ExternScope))
                    Log.Error(block.Source, ErrorCode.E4516, "Not all code paths return a value");
            }

            if (block.Ending == BlockEnding.RetVoid)
            {
                if (!(Function.ReturnType.IsVoid))
                    Log.Error(block.Source, ErrorCode.E4514, "Not all code paths return a value");
            }
            else if (block.Ending == BlockEnding.RetNonVoid)
            {
                if ((Function.ReturnType.IsVoid))
                    Log.Error(block.Source, ErrorCode.E4515, "Void function returning a value");
            }

            if (block.Successors != null)
                foreach (var b in block.Successors)
                    FindReachableBlocksAndValidateReturnPaths(b, reachedSet);
        }
    }
}
