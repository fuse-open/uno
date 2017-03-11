using Uno.Compiler.ExportTargetInterop;
using Uno.Math;
using Uno.Vector;

namespace Uno
{
    public static class Quaternion
    {
        const float ZeroTolerance = 1e-05f;

        public static float4 Mul(float4 left, float4 right)
        {
            float lx = left.X;
            float ly = left.Y;
            float lz = left.Z;
            float lw = left.W;
            float rx = right.X;
            float ry = right.Y;
            float rz = right.Z;
            float rw = right.W;

            return float4(
                (rx * lw + lx * rw + ry * lz) - (rz * ly),
                (ry * lw + ly * rw + rz * lx) - (rx * lz),
                (rz * lw + lz * rw + rx * ly) - (ry * lx),
                (rw * lw) - (rx * lx + ry * ly + rz * lz));
        }

        public static float4 Slerp(float4 start, float4 end, float amount)
        {
            float opposite;
            float inverse;
            float dot = Dot(start, end);

            if (Abs(dot) > 1.0f - ZeroTolerance)
            {
                inverse = 1.0f - amount;
                opposite = amount * Sign(dot);
            }
            else
            {
                float acos = Acos(Abs(dot));
                float invSin = 1.0f / Sin(acos);

                inverse = Sin((1.0f - amount) * acos) * invSin;
                opposite = Sin(amount * acos) * invSin * Sign(dot);
            }

            return float4(
                inverse * start.X + opposite * end.X,
                inverse * start.Y + opposite * end.Y,
                inverse * start.Z + opposite * end.Z,
                inverse * start.W + opposite * end.W);
        }

        public static float4 RotationAxis(float3 axisNormalized, float angleRadians)
        {
            axisNormalized = Normalize(axisNormalized);
            float h = angleRadians * 0.5f;
            float s = Sin(h);
            float c = Cos(h);

            return float4(
                axisNormalized.X * s,
                axisNormalized.Y * s,
                axisNormalized.Z * s,
                c);
        }

        public static float3 GetAxis(float4 q)
        {
            float length = (q.X * q.X) + (q.Y * q.Y) + (q.Z * q.Z);

            if (length < ZeroTolerance)
                return float3(1, 0, 0);

            float inv = 1.0f / length;
            return float3(q.X * inv, q.Y * inv, q.Z * inv);
        }

        public static float GetAngle(float4 q)
        {
            return 2.0f * Acos(q.W);
        }
/*
        public static void ToAxisAngle(float4 q, out float3 axis, out float angleRadians)
        {
            axis = GetAxis(q);
            angleRadians = GetAngle(q);
        }
*/
        public static float4 RotationX(float angleRadians)
        {
            float h = angleRadians * 0.5f;
            float s = Sin(h);
            float c = Cos(h);
            return float4(s, 0, 0, c);
        }

        public static float4 RotationY(float angleRadians)
        {
            float h = angleRadians * 0.5f;
            float s = Sin(h);
            float c = Cos(h);
            return float4(0, s, 0, c);
        }

        public static float4 RotationZ(float angleRadians)
        {
            float h = angleRadians * 0.5f;
            float s = Sin(h);
            float c = Cos(h);
            return float4(0, 0, s, c);
        }

        public static float4 RotationAlignForward(float3 forwardDirection, float3 upDirection)
        {
            forwardDirection = -forwardDirection;
            OrthoNormalize(ref forwardDirection, ref upDirection);
            float3 right = Cross(upDirection, forwardDirection);
            float W = Sqrt(((1.0f + right.X) + upDirection.Y) + forwardDirection.Z) * 0.5f;
            float w4_recip = 1.0f / (4.0f * W);
            float X = (forwardDirection.Y - upDirection.Z) * w4_recip;
            float Y = (right.Z - forwardDirection.X) * w4_recip;
            float Z = (upDirection.X - right.Y) * w4_recip;
            return float4(-X, -Y, -Z, W);
        }

        // TODO: Rename to RotationEulerAngle() to be consistent with rest of class ?

        ///
        /// X = Pitch, Y = Yaw, Z = Roll
        ///
        public static float4 FromEulerAngle(float3 v)
        {
            return FromEulerAngle(v.X, v.Y, v.Z);
        }

        public static float4 FromEulerAngle(float pitch, float yaw, float roll)
        {
            var tmp = pitch;
            pitch = yaw;
            yaw = tmp;

            float rollOver2 = roll * 0.5f;
            float sinRollOver2 = Sin(rollOver2);
            float cosRollOver2 = Cos(rollOver2);
            float yawOver2 = yaw * 0.5f;
            float sinYawOver2 = Sin(yawOver2);
            float cosYawOver2 = Cos(yawOver2);
            float pitchOver2 = pitch * 0.5f;
            float sinPitchOver2 = Sin(pitchOver2);
            float cosPitchOver2 = Cos(pitchOver2);
            float4 result;
            result.W = ((cosPitchOver2 * cosYawOver2) * cosRollOver2) + ((sinPitchOver2 * sinYawOver2) * sinRollOver2);
            result.X = ((cosPitchOver2 * sinYawOver2) * cosRollOver2) + ((sinPitchOver2 * cosYawOver2) * sinRollOver2);
            result.Y = ((sinPitchOver2 * cosYawOver2) * cosRollOver2) - ((cosPitchOver2 * sinYawOver2) * sinRollOver2);
            result.Z = ((cosPitchOver2 * cosYawOver2) * sinRollOver2) - ((sinPitchOver2 * sinYawOver2) * cosRollOver2);
            return result;
        }

        public static float4 FromEulerAngleDegrees(float3 v)
        {
            return FromEulerAngleDegrees(v.X, v.Y, v.Z);
        }

        public static float4 FromEulerAngleDegrees(float pitch, float yaw, float roll)
        {
            return FromEulerAngle(DegreesToRadians(pitch), DegreesToRadians(yaw), DegreesToRadians(roll));
        }

        ///
        /// X = Pitch, Y = Yaw, Z = Roll
        ///
        public static float3 ToEulerAngle(float4 q1)
        {
            float sqw = q1.W * q1.W;
            float sqx = q1.X * q1.X;
            float sqy = q1.Y * q1.Y;
            float sqz = q1.Z * q1.Z;
            float unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
            float test = q1.X * q1.W - q1.Y * q1.Z;
            float3 v;

            if (test > 0.4995f * unit)
            { // singularity at north pole
                v.Y = 2f * Atan2(q1.Y, q1.X);
                v.X = (float)PI / 2;
                v.Z = 0;
                return v;
            }
            if (test < -0.4995f * unit)
            { // singularity at south pole
                v.Y = -2f * Atan2(q1.Y, q1.X);
                v.X = -(float)PI / 2;
                v.Z = 0;
                return v;
            }
            float4 q = float4(q1.W, q1.Z, q1.X, q1.Y);
            v.Y = (float)Atan2(2f * q.X * q.W + 2f * q.Y * q.Z, 1 - 2f * (q.Z * q.Z + q.W * q.W));     // Yaw
            v.X = (float)Asin(2f * (q.X * q.Z - q.W * q.Y));                             // Pitch
            v.Z = (float)Atan2(2f * q.X * q.Y + 2f * q.Z * q.W, 1 - 2f * (q.Y * q.Y + q.Z * q.Z));      // Roll
            return v;
        }

        public static float3 ToEulerAngleDegrees(float4 q1)
        {
            // TODO: Why is this one wrapping the angles while the other one (ToEulerAngle) does not?
            // TODO: Why do we even provide these degree overloads (consistency?)
            var v = ToEulerAngle(q1);
            return NormalizeAnglesDegrees(float3(RadiansToDegrees(v.X), RadiansToDegrees(v.Y), RadiansToDegrees(v.Z)));
        }

        static float3 NormalizeAnglesDegrees(float3 angles)
        {
            angles.X = NormalizeAngleDegrees(angles.X);
            angles.Y = NormalizeAngleDegrees(angles.Y);
            angles.Z = NormalizeAngleDegrees(angles.Z);
            return angles;
        }

        static float NormalizeAngleDegrees(float angle)
        {
            while (angle > 360)
                angle -= 360;
            while (angle < 0)
                angle += 360;
            return angle;
        }

        public static float4 RotationMatrix(float3x3 matrix)
        {
            float sqrt;
            float halff;
            float scale = matrix.M11 + matrix.M22 + matrix.M33;
            float4 result;

            if (scale > 0.0f)
            {
                sqrt = Sqrt(scale + 1.0f);
                result.W = sqrt * 0.5f;
                sqrt = 0.5f / sqrt;

                result.X = (matrix.M23 - matrix.M32) * sqrt;
                result.Y = (matrix.M31 - matrix.M13) * sqrt;
                result.Z = (matrix.M12 - matrix.M21) * sqrt;
            }
            else if ((matrix.M11 >= matrix.M22) && (matrix.M11 >= matrix.M33))
            {
                sqrt = Sqrt(1.0f + matrix.M11 - matrix.M22 - matrix.M33);
                halff = 0.5f / sqrt;

                result.X = 0.5f * sqrt;
                result.Y = (matrix.M12 + matrix.M21) * halff;
                result.Z = (matrix.M13 + matrix.M31) * halff;
                result.W = (matrix.M23 - matrix.M32) * halff;
            }
            else if (matrix.M22 > matrix.M33)
            {
                sqrt = Sqrt(1.0f + matrix.M22 - matrix.M11 - matrix.M33);
                halff = 0.5f / sqrt;

                result.X = (matrix.M21 + matrix.M12) * halff;
                result.Y = 0.5f * sqrt;
                result.Z = (matrix.M32 + matrix.M23) * halff;
                result.W = (matrix.M31 - matrix.M13) * halff;
            }
            else
            {
                sqrt = Sqrt(1.0f + matrix.M33 - matrix.M11 - matrix.M22);
                halff = 0.5f / sqrt;

                result.X = (matrix.M31 + matrix.M13) * halff;
                result.Y = (matrix.M32 + matrix.M23) * halff;
                result.Z = 0.5f * sqrt;
                result.W = (matrix.M12 - matrix.M21) * halff;
            }

            return result;
        }

        public static float3x3 ToMatrix(float4 q)
        {
            float xx = q.X * q.X;
            float yy = q.Y * q.Y;
            float zz = q.Z * q.Z;
            float xy = q.X * q.Y;
            float zw = q.Z * q.W;
            float zx = q.Z * q.X;
            float yw = q.Y * q.W;
            float yz = q.Y * q.Z;
            float xw = q.X * q.W;

            return float3x3(
                1.0f - (2.0f * (yy + zz)),
                2.0f * (xy + zw),
                2.0f * (zx - yw),
                2.0f * (xy - zw),
                1.0f - (2.0f * (zz + xx)),
                2.0f * (yz + xw),
                2.0f * (zx + yw),
                2.0f * (yz - xw),
                1.0f - (2.0f * (yy + xx)));
        }

        //http://lolengine.net/blog/2013/09/18/beautiful-maths-quaternion-from-vectors
        static public float4 RotationBetween( float3 from, float3 to )
        {
            /*from = Normalize(from);
            to = Normalize(to);
            var angle = Acos( Dot( from, to ) );
            var vector = Normalize( Cross( from, to ) );
            return Quaternion.RotationAxis( vector, angle );*/
            float m = Sqrt(2f + 2f * Dot(from, to));
            float3 w = (1 / m) * Cross(from, to);
            return float4(w.X, w.Y, w.Z, 0.5f * m);
        }

/*
        public static float4 RotationAlign(float3 from, float3 to)
        {
            float4 q;
            Vector3 a = Cross(from, to);
            q.X = a.X;
            q.Y = a.Y;
            q.Z = a.Z;
            q.W = (float)Sqrt(from.LengthSquared() * to.LengthSquared()) + Dot(from, to);
            q.Normalize();
            return q;
        }

        public static float4 RotationAlignAroundAxis(float3 axis, float3 from, float3 to)
        {
            var u0 = axis;
            u0.Normalize();
            var v1 = from;
            var u1 = v1 - Dot(v1, u0) * u0;
            u1.Normalize();
            var v2 = to;
            var u2 = v2 - Dot(v2, u0) * u0 - Dot(v2, u1) * u1;
            u2.Normalize();

            float a = (float)Atan2(Dot(to, u1), Dot(to, u2));
            float b = (float)Atan2(Dot(from, u1), Dot(from, u2));
            float angle = Dot(axis, Cross(to, from)) < 0.0 ? a - b : b - a;

            return Quaternion.RotationAxis(u0, angle);
        }

*/
        public static float4 Invert(float4 q)
        {
            float lengthSq = LengthSquared(q);

            if (lengthSq < ZeroTolerance)
                return float4(0);

            lengthSq = 1.0f / lengthSq;

            float4 result;
            result.X = -q.X * lengthSq;
            result.Y = -q.Y * lengthSq;
            result.Z = -q.Z * lengthSq;
            result.W = q.W * lengthSq;
            return result;
        }

    }
}
