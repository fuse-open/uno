namespace Uno.Compiler.API.Domain.IL.Members
{
    public static class Fields
    {
        public static Field[] Copy(this Field[] fields, CopyState state)
        {
            var result = new Field[fields.Length];

            for (int i = 0; i < fields.Length; i++)
                result[i] = state.GetMember(fields[i]);

            return result;
        }
    }
}