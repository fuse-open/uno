using System.Linq;

namespace Uno.UX.Markup
{
    public struct TypeNameHelper
    {
        public string FullName { get; }

        public string Surname
        {
            get
            {
                if (FullName.Contains('.'))
                {
                    var i = FullName.LastIndexOf('.');
                    return FullName.Substring(i + 1, FullName.Length - i - 1);
                }
                return FullName;
            }
        }

        // TODO: introduce NamespaceName-type

        public string Namespace
        {
            get
            {
                if (FullName.Contains('.'))
                {
                    var i = FullName.LastIndexOf('.');
                    return FullName.Substring(0, i);
                }
                return "";
            }
        }

        public TypeNameHelper(string typeName)
        {
            FullName = typeName;
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}