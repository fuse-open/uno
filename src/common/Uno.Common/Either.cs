using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uno
{
    public class Either<TL, TR>
    {
        private readonly TL _left;
        private readonly TR _right;

        private readonly bool _isLeft;

        public Either(TL left)
        {
            _isLeft = true;
            _left = left;
        }

        public Either(TR right)
        {
            _isLeft = false;
            _right = right;
        }

        public T Match<T>(Func<TL, T> onLeft, Func<TR, T> onRight)
        {
            return _isLeft ? onLeft(_left) : onRight(_right);
        }

        public void Match(Action<TL> onLeft, Action<TR> onRight)
        {
            if (_isLeft)
                onLeft(_left);
            else
                onRight(_right);
        }
    }
}