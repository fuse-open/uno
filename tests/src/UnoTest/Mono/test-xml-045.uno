namespace Mono.test_xml_045
{
    // Compiler options: -doc:xml-045.xml -warnaserror
    /// <summary>
    /// <see cref="Create" />
    /// <see cref="Define" />
    /// <see cref="Undefine" />
    /// <see cref="Undefine(bool)" />
    /// <see cref="Remove" />
    /// <see cref="Destroy" />
    /// </summary>
    public class EntryPoint {
        [Uno.Testing.Test] public static void test_xml_045() { Main(); }
        public static void Main() {
        }

        /// dummy comments
        protected void Create (bool test) {
            Define (true);
        }

        private void Define (bool test) {
        }

        /// dummy comments
        protected void Undefine (bool test) {
        }

        /// dummy comments
        protected void Remove () {
        }

        /// dummy comments
        public virtual void Destroy (bool test) {
        }
    }
}
