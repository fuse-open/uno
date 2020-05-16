using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Uno.Compiler.Backends.CIL
{
    class AppLoader
    {
        readonly AssemblyDefinition _assembly;
        readonly ModuleDefinition _module;

        public string Architecture => _module.Architecture.ToString().ToLowerInvariant();

        public AppLoader(string inputFile)
        {
            _assembly = AssemblyDefinition.ReadAssembly(inputFile);
            _module = _assembly.MainModule;
        }

        public void Save(string outputFile)
        {
            _assembly.Write(outputFile);
        }

        public void ClearPublicKey()
        {
            _assembly.Name.HasPublicKey = false;
            _assembly.Name.PublicKey = new byte[0];
            _module.Attributes &= ~ModuleAttributes.StrongNameSigned;
        }

        public void SetX86()
        {
            _module.Architecture = TargetArchitecture.I386;
            _module.Attributes |= ModuleAttributes.Preferred32Bit;
            _module.Attributes |= ModuleAttributes.Required32Bit;
        }

        public void SetX64()
        {
            _module.Architecture = TargetArchitecture.AMD64;
            _module.Attributes &= ~ModuleAttributes.Preferred32Bit;
            _module.Attributes &= ~ModuleAttributes.Required32Bit;
        }

        public void SetAssemblyInfo(string name, Version version, Func<string, bool, string> getAttribute)
        {
            _assembly.Name.Name = name;
            _assembly.Name.Version = version;

            foreach (var a in _assembly.CustomAttributes)
            {
                switch (a.AttributeType.Name)
                {
                    case "AssemblyProductAttribute":
                    case "AssemblyTitleAttribute":
                    case "AssemblyFileVersionAttribute":
                    case "AssemblyInformationalVersionAttribute":
                    case "AssemblyDescriptionAttribute":
                    case "AssemblyConfigurationAttribute":
                    case "AssemblyCompanyAttribute":
                    case "AssemblyCopyrightAttribute":
                    case "AssemblyTrademarkAttribute":
                        a.ConstructorArguments[0] = new CustomAttributeArgument(
                            a.ConstructorArguments[0].Type,
                            getAttribute(a.AttributeType.Name.Replace("Attribute", ""), false));
                        break;
                }
            }
        }

        public void SetMainClass(string type, string file, string loadClass, string loadMethod)
        {
            var ctor = _module.ImportReference(AssemblyDefinition.ReadAssembly(file).MainModule
                        .GetType(type).Methods.First(m => m.Name == ".ctor" && m.Parameters.Count == 0));
            var load = _module.GetType(loadClass).Methods.First(m => m.Name == loadMethod);

            load.Body.Instructions.Clear();
            load.Body.Instructions.Add(Instruction.Create(OpCodes.Newobj, ctor));
            load.Body.Instructions.Add(Instruction.Create(OpCodes.Pop));
            load.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
        }
    }
}
