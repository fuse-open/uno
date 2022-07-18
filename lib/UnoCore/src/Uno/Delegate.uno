using Uno.Compiler.ExportTargetInterop;

namespace Uno
{
    [extern(DOTNET) DotNetType("System.Delegate")]
    [extern(CPLUSPLUS) Set("TypeName", "uDelegate*")]
    [extern(CPLUSPLUS) Set("BaseType", "uDelegate")]
    public class Delegate
    {
        private Delegate()
        {
        }

        public static Delegate Combine(Delegate source, Delegate value)
        {
            if defined(CPLUSPLUS)
            @{
                if (!$1)
                    return $0;
                if (!$0)
                    return $1;
                if ($0->__type != $1->__type)
                    U_THROW_ICE();

                uDelegate copy;
                uDelegate *prev = &copy;

                for (uDelegate* d = $1; d != nullptr; d = d->_prev)
                    prev = prev->_prev = d->Copy();

                prev->_prev = $0;
                return copy._prev;
            @}
            else
                build_error;
        }

        public static Delegate Remove(Delegate source, Delegate value)
        {
            if defined(CPLUSPLUS)
            @{
                if ($0 == nullptr || $1 == nullptr)
                    return $0;
                if ($1->__type != $0->__type)
                    U_THROW_ICE();

                for (uDelegate *first = $0; first != nullptr; first = first->_prev)
                {
                    bool match = true;
                    uDelegate *last = first;

                    for (uDelegate *d = $1; d != nullptr; d = d->_prev, last = last->_prev)
                    {
                        if (last == nullptr ||
                            d->_func != last->_func ||
                            d->_this != last->_this)
                        {
                            match = false;
                            break;
                        }
                    }

                    if (match)
                    {
                        uDelegate temp;
                        uDelegate *prev = &temp;

                        for (uDelegate *e = $0; e != first; e = e->_prev)
                            prev = prev->_prev = e->Copy();

                        prev->_prev = last;
                        return temp._prev;
                    }
                }

                return $0;
            @}
            else
                build_error;
        }

        static bool EqualsImpl(Delegate left, Delegate right)
        {
            if defined(CPLUSPLUS)
            @{
                return $0 == $1 || (
                        $0 != nullptr && $1 != nullptr &&
                        $0->__type == $1->__type &&
                        $0->_func == $1->_func &&
                        $0->_this == $1->_this &&
                        @{object.Equals(object,object):Call($0->_prev, $1->_prev)}
                    );
            @}
            else
                build_error;
        }

        public static bool operator == (Delegate left, Delegate right)
        {
            return EqualsImpl(left, right);
        }

        public static bool operator != (Delegate left, Delegate right)
        {
            return !EqualsImpl(left, right);
        }

        public override bool Equals(object other)
        {
            return other is Delegate && EqualsImpl(this, other as Delegate);
        }

        public override int GetHashCode()
        {
            return 0; //TODO needs target specific implementation
        }
    }
}
