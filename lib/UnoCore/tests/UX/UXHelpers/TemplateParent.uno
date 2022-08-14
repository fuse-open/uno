using Uno;
using Uno.Collections;
using Uno.UX;

namespace UXHelpers
{
    public class TemplateParent : RootObject
    {
        public override void Root()
        {
            // Instantiate template object
            var t = FindTemplate("temp");
            var instance = t.New();
            Children.Add(instance);

            // Root children as usual
            base.Root();
        }

        Template FindTemplate(string key)
        {
            foreach (var child in Children)
            {
                var t = child as Template;
                if (t == null)
                    continue;

                if (t.Key == key)
                    return t;
            }

            throw new Exception("Unable to find template for key: " + key);
        }
    }
}