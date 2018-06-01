using Uno;
using Uno.Testing;

namespace UnoTest.General
{
    public class OperatorOverloading
    {
        class Bloop
        {
            public int val;
            public float3 color;

            public Bloop()
            {
                this.val=0;
                this.color = float3(0.0f);
            }
            public Bloop(Bloop b1)
            {
                this.val = b1.val;
                this.color = b1.color;
            }
            public Bloop(int aVal,float3 aColor)
            {
                this.val = aVal;
                this.color = aColor;
            }
            //UNARY
            public static Bloop operator +(Bloop b1)
            {
                return new Bloop(Uno.Math.Abs(b1.val),b1.color);
            }
            public static Bloop operator -(Bloop b1)
            {
                return new Bloop(-b1.val,b1.color);
            }
            public static Bloop operator !(Bloop b1)
            {
                //Typically used with true false
                return new Bloop(-b1.val,(float3(1.0f)-b1.color));
            }
            public static Bloop operator ~(Bloop b1)
            {
                return new Bloop(~b1.val,b1.color);
            }
            /*public static Bloop operator ++(Bloop b1)
            {
                bl.val++;
                return Bloop(b1.val,b1.color);
            }
            public static Bloop operator --(Bloop b1)
            {
                bl.val--;
                return Bloop(bl.val,b1.color);
            }*/

            /*public static bool operator false(Bloop b1)
            {
                return (bl.val <= 0);
            }
            public static bool operator true(Bloop b1)
            {
                return (bl.val > 0);
            }*/

            //BINARY
            public static Bloop operator +(Bloop b1,Bloop b2)
            {
                return new Bloop(b1.val + b2.val,float3((b1.color.X + b2.color.X)/2.0f,(b1.color.Y + b2.color.Y)/2.0f,(b1.color.Z +b2.color.Z)/2.0f));
            }
            public static Bloop operator -(Bloop b1,Bloop b2)
            {
                return new Bloop(b1.val - b2.val,float3(Uno.Math.Abs(b1.color.X - b2.color.X),Uno.Math.Abs(b1.color.Y - b2.color.Y),Uno.Math.Abs(b1.color.Z - b2.color.Z)));
            }
            public static Bloop operator *(Bloop b1,Bloop b2)
            {
                return new Bloop(b1.val * b2.val,b1.color * b2.color);
            }
            public static Bloop operator /(Bloop b1,Bloop b2)
            {
                if(b2.color == float3(0.0f)) return new Bloop(b1.val/b2.val,b1.color);

                else return new Bloop(b1.val/b2.val,b1.color / b2.color);
            }
            public static Bloop operator %(Bloop b1,Bloop b2)
            {
                return new Bloop(b1.val % b2.val,b1.color);
            }
            public static Bloop operator &(Bloop b1,Bloop b2)
            {
                return new Bloop((b1.val & b2.val),b1.color);
            }
            public static Bloop operator |(Bloop b1,Bloop b2)
            {
                return new Bloop(b1.val | b2.val,b1.color);
            }
            public static Bloop operator ^(Bloop b1,Bloop b2)
            {
                return new Bloop(b1.val ^ b2.val,b1.color);
            }
            public static bool operator ==(Bloop b1,Bloop b2)
            {
                return((b1.val == b2.val) && (b1.color == b2.color));
            }
            public static bool operator !=(Bloop b1,Bloop b2)
            {
                return((b1.val != b2.val) && (b1.color != b2.color));
            }
            public static bool operator >(Bloop b1,Bloop b2)
            {
                return(b1.val > b2.val);
            }
            public static bool operator <(Bloop b1,Bloop b2)
            {
                return(b1.val < b2.val);
            }
            public static bool operator >=(Bloop b1,Bloop b2)
            {
                return(b1.val >= b2.val);
            }
            public static bool operator <=(Bloop b1,Bloop b2)
            {
                return(b1.val <= b2.val);
            }
            public static Bloop operator >>(Bloop b1,Bloop b2)
            {
                //this is not typically what this op does
                return b2 = b1;
            }
            public static Bloop operator <<(Bloop b1,Bloop b2)
            {
                //this is not typically what this op does
                return b1 = b2;
            }
        }

        [Test]
        public void Run()
        {
            var bloop1 = new Bloop(5,float3(1.0f));
            var bloop2 = new Bloop(-5,float3(0.0f));

            var tempBloop = new Bloop();
            /*tempBloop = +bloop1;
            var tempBloop = Bloop(+bloop1);
            Assert.AreEqual(5, tempBloop.val);
            Assert.IsTrue(tempBloop.color == bloop1.color);

            tempBloop = +bloop2;
            Assert.AreEqual(5, tempBloop.val);
            Assert.IsTrue(tempBloop.color == bloop2.color);

            tempBloop = Bloop(-bloop1);
            Assert.AreEqual(-5, tempBloop.val);
            Assert.IsTrue(tempBloop.color == bloop1.color);

            tempBloop = Bloop(-bloop2);
            Assert.AreEqual(5, tempBloop.val);
            Assert.IsTrue(tempBloop.color == bloop2.color);*/

            tempBloop = ~bloop1;
            Assert.AreEqual(-6, tempBloop.val);
            Assert.IsTrue(tempBloop.color == bloop1.color);

            tempBloop = new Bloop(~bloop2);
            Assert.AreEqual(4, tempBloop.val);
            Assert.IsTrue(tempBloop.color == bloop2.color);

            tempBloop = !bloop1;
            Assert.AreEqual(-5, tempBloop.val);
            Assert.AreEqual(float3(0.0f), tempBloop.color);

            tempBloop = new Bloop(!bloop1);
            Assert.AreEqual(-5, tempBloop.val);
            Assert.AreEqual(float3(0.0f), tempBloop.color);

            tempBloop = bloop1 + bloop2;
            Assert.AreEqual(0, tempBloop.val);
            Assert.AreEqual(float3(0.5f), tempBloop.color);

            tempBloop = bloop1 - bloop2;
            Assert.AreEqual(10, tempBloop.val);
            Assert.AreEqual(float3(1.0f), tempBloop.color);

            tempBloop = bloop1 * bloop2;
            Assert.AreEqual(-25, tempBloop.val);
            Assert.AreEqual(float3(0.0f), tempBloop.color);

            tempBloop = bloop1 / bloop2;
            Assert.AreEqual(-1, tempBloop.val);
            Assert.AreEqual(float3(1.0f), tempBloop.color);

            tempBloop = bloop1 & bloop2;
            Assert.AreEqual(1, tempBloop.val);
            Assert.IsTrue(tempBloop.color == bloop1.color);

            tempBloop = bloop1 | bloop2;
            Assert.AreEqual(-1, tempBloop.val);
            Assert.IsTrue(tempBloop.color == bloop1.color);

            tempBloop = bloop1 ^ bloop2;
            Assert.AreEqual(-2, tempBloop.val);
            Assert.IsTrue(tempBloop.color == bloop1.color);

            if(bloop1 == bloop2)Assert.IsTrue(false);
            if(bloop1 == bloop1)Assert.IsTrue(true);

            if(bloop1 != bloop2)Assert.IsTrue(true);
            if(bloop1 != bloop1)Assert.IsTrue(false);

            if(bloop1 > bloop2)Assert.IsTrue(true);
            if(bloop1 < bloop2)Assert.IsTrue(false);
            if(bloop2 < bloop1)Assert.IsTrue(true);
            if(bloop2 > bloop1)Assert.IsTrue(false);

            if(bloop1 >= bloop2)Assert.IsTrue(true);
            if(bloop1 >= bloop1)Assert.IsTrue(true);
            if(bloop2 >= bloop1)Assert.IsTrue(false);

            if(bloop2 <= bloop1)Assert.IsTrue(true);
            if(bloop2 <= bloop2)Assert.IsTrue(true);
            if(bloop1 <= bloop2)Assert.IsTrue(false);

            tempBloop = bloop1 >> bloop2;
            Assert.AreEqual(5, tempBloop.val);
            Assert.IsTrue(tempBloop.color == bloop1.color);

            tempBloop = bloop1 << bloop2;
            Assert.AreEqual(-5, tempBloop.val);
            Assert.IsTrue(tempBloop.color == bloop2.color);
        }
    }
}
