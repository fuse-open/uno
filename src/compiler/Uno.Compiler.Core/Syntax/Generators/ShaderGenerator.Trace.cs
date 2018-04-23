using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API.Domain.Graphics;

namespace Uno.Compiler.Core.Syntax.Generators
{
    partial class ShaderGenerator
    {
        bool TraceMetaPropertyLocation(MetaLocation loc, TraceData trace)
        {
            if (trace.VisitedLocations.Contains(loc)) return true;
            trace.VisitedLocations.Add(loc);

            var mp = GetProperty(loc);

            // Detect circular references (should not happen)
            foreach (var ploc in trace.Stack)
                if (ploc == loc)
                    return false;

            if (mp.Definitions.Length == 0)
            {
                trace.Stack.Add(loc);
                trace.Errors.Add(new TraceError(mp.Source, ErrorCode.E2508, "Meta property " + mp.Name.Quote() + " has no definitions in " + Path.Quote(), null, trace.Stack.ToArray()));
                trace.Stack.RemoveLast();
                return true;
            }

            trace.Stack.Add(loc);
            bool errorsFound = false;

            for (int i = 0; i < mp.Definitions.Length; i++)
            {
                var def = mp.Definitions[i];

                foreach (var req in def.Requirements)
                {
                    if (Path.FailedReqStatements.Contains(Tuple.Create(loc, def, req)))
                    {
                        errorsFound = true;

                        switch (req.Type)
                        {
                            case ReqStatementType.Object:
                            {
                                var ri = req as ReqObject;
                                trace.Errors.Add(new TraceError(req.Source, ErrorCode.E2500, "An instance of " + ri.ObjectType.Quote() + " is not available in " + Path.Quote(), req, trace.Stack.ToArray()));
                                break;
                            }
                            case ReqStatementType.File:
                            {
                                var rf = req as ReqFile;
                                trace.Errors.Add(new TraceError(req.Source, ErrorCode.E2501, "Required file " + rf.Filename.Quote() + " does not exist", req, trace.Stack.ToArray()));
                                break;
                            }
                            case ReqStatementType.Property:
                            {
                                var rmp = req as ReqProperty;
                                var rloc = TryGetLocation(loc, rmp.PropertyName, rmp.Offset);

                                if (rloc == null)
                                {
                                    trace.Errors.Add(new TraceError(req.Source, ErrorCode.E2502, "Meta property " + rmp.PropertyName.Quote() + " was not found in " + Path.Quote(), req, trace.Stack.ToArray()));
                                    break;
                                }

                                var rdt = GetProperty(rloc.Value).ReturnType;
                                if (rmp.PropertyType != null && !rmp.PropertyType.Equals(rdt))
                                {
                                    trace.Errors.Add(new TraceError(req.Source, ErrorCode.E2503, "Meta property " + rmp.PropertyName.Quote() + " did not have expected type " + rmp.PropertyType.Quote() + " in " + Path.Quote(), req, trace.Stack.ToArray()));
                                    break;
                                }

                                if (rmp.Tag != null)
                                {
                                    var rdef = GetValidDefinition(rloc.Value);
                                    if (rdef != null && !rdef.Tags.Contains(rmp.Tag))
                                    {
                                        trace.Errors.Add(new TraceError(req.Source, ErrorCode.E2504, "Meta property " + rmp.PropertyName.Quote() + " did not specify required tag " + rmp.Tag.Quote() + " in " + Path.Quote(), req, trace.Stack.ToArray()));
                                        break;
                                    }
                                }

                                if (!TraceMetaPropertyLocation(rloc.Value, trace))
                                {
                                    trace.Errors.Add(new TraceError(req.Source, ErrorCode.E2505, "Meta property " + rmp.PropertyName.Quote() + " could not be used in " + Path.Quote(), req, trace.Stack.ToArray()));
                                    break;
                                }

                                break;
                            }

                            default:
                                trace.Errors.Add(new TraceError(req.Source, ErrorCode.E2506, "Unsupported req statement", req, trace.Stack.ToArray()));
                                break;
                        }

                        continue;
                    }
                }
            }

            trace.Stack.RemoveLast();
            return errorsFound;
        }

        TraceError[] FindErrors(MetaLocation loc, List<MetaLocation> circularRefStack)
        {
            var trace = new TraceData();

            if (circularRefStack != null)
            {
                var mp = GetProperty(loc);
                circularRefStack.Add(loc);
                trace.Errors.Add(new TraceError(mp.Source, ErrorCode.E2507, "Circular reference to " + mp.Name.Quote() + " detected in " + Path.Quote(), null, circularRefStack.ToArray()));
                circularRefStack.RemoveLast();
            }

            TraceMetaPropertyLocation(loc, trace);
            return trace.Errors.ToArray();
        }

        public Source CreateTrace(MetaProperty mp, MetaLocation loc, List<MetaLocation> circularRefStack)
        {
            var errors = FindErrors(loc, circularRefStack);

            foreach (var e in errors)
                Log.Error(Path.Source, e.ErrorCode, e.Message);

            var filename = System.IO.Path.Combine(Environment.CacheDirectory, "Traces", Path.DrawBlock.Method.DeclaringType.ToString().ToIdentifier() + "." + CreateLocalName(mp, loc).ToIdentifier() + ".unotrace");

            using (var f = Compiler.Disk.CreateText(filename))
            {
                f.WriteLine("Error trace generated on " + DateTime.Now);
                f.WriteLine();
                f.WriteLine("Terminal:  " + mp.Name + " in " + mp.Source);
                f.WriteLine("Drawable:  " + Path.DrawableBlock + " in " + Path.DrawableBlock.Source);
                f.WriteLine("Root:      " + Path.DrawBlock + " in " + Path.DrawBlock.Source);
                f.WriteLine();
                f.WriteLine("----------");

                for (int i = 0; i < errors.Length; i++)
                {
                    f.WriteLine();
                    f.WriteLine(errors[i].ErrorCode + ": " + errors[i].Message);
                    f.WriteLine();

                    if (errors[i].ReqStatement != null)
                    {
                        f.WriteLine("Req Statement:");
                        f.WriteLine("    " + errors[i].ReqStatement);
                        f.WriteLine();
                    }

                    f.WriteLine("Stack Trace:");

                    for (int j = errors[i].Stack.Length - 1; j >= 0; j--)
                    {
                        var pmp = GetProperty(errors[i].Stack[j]);
                        f.WriteLine("    [" + j + "]: " + pmp.Name + " in " + pmp.Source);
                    }

                    f.WriteLine();
                    f.WriteLine("----------");
                }
            }

            return new Source(FrameScope.Source.Package, filename, 1);
        }
    }
}
