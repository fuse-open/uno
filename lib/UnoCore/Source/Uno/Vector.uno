using Uno.Compiler.ExportTargetInterop;
using Uno.Math;

namespace Uno
{
    /*
     * Contains static methods for manipulating tuple-values as linear algebra vectors
     */
    public static class Vector
    {
        public static float LengthSquared(float2 v) { return v.X * v.X + v.Y * v.Y; }
        public static float LengthSquared(float3 v) { return v.X * v.X + v.Y * v.Y + v.Z * v.Z; }
        public static float LengthSquared(float4 v) { return v.X * v.X + v.Y * v.Y + v.Z * v.Z + v.W * v.W; }

        [GlslIntrinsic("length")] public static float Length(float2 v) { return Sqrt(LengthSquared(v)); }
        [GlslIntrinsic("length")] public static float Length(float3 v) { return Sqrt(LengthSquared(v)); }
        [GlslIntrinsic("length")] public static float Length(float4 v) { return Sqrt(LengthSquared(v)); }

        [GlslIntrinsic("distance")] public static float Distance(float2 p0, float2 p1) { return Length(p1 - p0); }
        [GlslIntrinsic("distance")] public static float Distance(float3 p0, float3 p1) { return Length(p1 - p0); }
        [GlslIntrinsic("distance")] public static float Distance(float4 p0, float4 p1) { return Length(p1 - p0); }

        [GlslIntrinsic("normalize")] public static float2 Normalize(float2 v) { return v / Length(v); }
        [GlslIntrinsic("normalize")] public static float3 Normalize(float3 v) { return v / Length(v); }
        [GlslIntrinsic("normalize")] public static float4 Normalize(float4 v) { return v / Length(v); }

        [GlslIntrinsic("dot")] public static float Dot(float2 a, float2 b) { return a.X * b.X + a.Y * b.Y; }
        [GlslIntrinsic("dot")] public static float Dot(float3 a, float3 b) { return a.X * b.X + a.Y * b.Y + a.Z * b.Z; }
        [GlslIntrinsic("dot")] public static float Dot(float4 a, float4 b) { return a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W; }

        [GlslIntrinsic("cross")]
        public static float3 Cross(float3 left, float3 right)
        {
            return float3(
                (left.Y * right.Z) - (left.Z * right.Y),
                (left.Z * right.X) - (left.X * right.Z),
                (left.X * right.Y) - (left.Y * right.X));
        }

        [GlslIntrinsic("reflect")]
        public static float2 Reflect(float2 vector, float2 normal)
        {
            float dot = Dot(vector, normal);
            return vector - (2.0f * dot) * normal;
        }

        [GlslIntrinsic("reflect")]
        public static float3 Reflect(float3 vector, float3 normal)
        {
            float dot = Dot(vector, normal);
            return vector - (2.0f * dot) * normal;
        }

        [GlslIntrinsic("reflect")]
        public static float4 Reflect(float4 vector, float4 normal)
        {
            float dot = Dot(vector, normal);
            return vector - (2.0f * dot) * normal;
        }

        [GlslIntrinsic("refract")]
        public static float2 Refract(float2 vector, float2 normal, float eta)
        {
            float dot = Dot(normal, vector);
            float k = 1.0f - eta * eta * (1.0f - dot * dot);
            return k < 0.0f ? float2(0.0f) : eta * vector - (eta * dot + Sqrt(k)) * normal;
        }

        [GlslIntrinsic("refract")]
        public static float3 Refract(float3 vector, float3 normal, float eta)
        {
            float dot = Dot(normal, vector);
            float k = 1.0f - eta * eta * (1.0f - dot * dot);
            return k < 0.0f ? float3(0.0f) : eta * vector - (eta * dot + Sqrt(k)) * normal;
        }

        [GlslIntrinsic("refract")]
        public static float4 Refract(float4 vector, float4 normal, float eta)
        {
            float dot = Dot(normal, vector);
            float k = 1.0f - eta * eta * (1.0f - dot * dot);
            return k < 0.0f ? float4(0.0f) : eta * vector - (eta * dot + Sqrt(k)) * normal;
        }

        public static void OrthoNormalize(ref float2 orthonormalTo, ref float2 v) { orthonormalTo = Normalize(orthonormalTo); v = Normalize(v - Project(v, orthonormalTo)); }
        public static void OrthoNormalize(ref float3 orthonormalTo, ref float3 v) { orthonormalTo = Normalize(orthonormalTo); v = Normalize(v - Project(v, orthonormalTo)); }
        public static void OrthoNormalize(ref float4 orthonormalTo, ref float4 v) { orthonormalTo = Normalize(orthonormalTo); v = Normalize(v - Project(v, orthonormalTo)); }

        public static float2 Project(float2 v, float2 projectOn) { return projectOn * Dot(projectOn, v) / Dot(projectOn, projectOn); }
        public static float3 Project(float3 v, float3 projectOn) { return projectOn * Dot(projectOn, v) / Dot(projectOn, projectOn); }
        public static float4 Project(float4 v, float4 projectOn) { return projectOn * Dot(projectOn, v) / Dot(projectOn, projectOn); }

        public static float2 Rotate(float2 v, float angleRadians)
        {
            return float2(v.X * Cos(angleRadians) - v.Y * Sin(angleRadians), v.Y * Cos(angleRadians) + v.X * Sin(angleRadians));
        }

        public static float3 Transform(float3 vector, float4 rotationQuaternion)
        {
            float x = rotationQuaternion.X + rotationQuaternion.X;
            float y = rotationQuaternion.Y + rotationQuaternion.Y;
            float z = rotationQuaternion.Z + rotationQuaternion.Z;
            float wx = rotationQuaternion.W * x;
            float wy = rotationQuaternion.W * y;
            float wz = rotationQuaternion.W * z;
            float xx = rotationQuaternion.X * x;
            float xy = rotationQuaternion.X * y;
            float xz = rotationQuaternion.X * z;
            float yy = rotationQuaternion.Y * y;
            float yz = rotationQuaternion.Y * z;
            float zz = rotationQuaternion.Z * z;

            float num1 = ((1.0f - yy) - zz);
            float num2 = (xy - wz);
            float num3 = (xz + wy);
            float num4 = (xy + wz);
            float num5 = ((1.0f - xx) - zz);
            float num6 = (yz - wx);
            float num7 = (xz - wy);
            float num8 = (yz + wx);
            float num9 = ((1.0f - xx) - yy);

            return float3(
                ((vector.X * num1) + (vector.Y * num2)) + (vector.Z * num3),
                ((vector.X * num4) + (vector.Y * num5)) + (vector.Z * num6),
                ((vector.X * num7) + (vector.Y * num8)) + (vector.Z * num9));
        }

        public static float3 Transform(float3 vector, float3x3 matrix)
        {
            return float3(
                (vector.X * matrix.M11) + (vector.Y * matrix.M21) + (vector.Z * matrix.M31),
                (vector.X * matrix.M12) + (vector.Y * matrix.M22) + (vector.Z * matrix.M32),
                (vector.X * matrix.M13) + (vector.Y * matrix.M23) + (vector.Z * matrix.M33));
        }

        public static float2 Transform(float2 vector, float2x2 matrix)
        {
            return float2(
                (vector.X * matrix.M11) + (vector.Y * matrix.M21),
                (vector.X * matrix.M12) + (vector.Y * matrix.M22));
        }

        public static float4 Transform(float4 vector, float4x4 matrix)
        {
            return float4(
                (vector.X * matrix.M11) + (vector.Y * matrix.M21) + (vector.Z * matrix.M31) + (vector.W * matrix.M41),
                (vector.X * matrix.M12) + (vector.Y * matrix.M22) + (vector.Z * matrix.M32) + (vector.W * matrix.M42),
                (vector.X * matrix.M13) + (vector.Y * matrix.M23) + (vector.Z * matrix.M33) + (vector.W * matrix.M43),
                (vector.X * matrix.M14) + (vector.Y * matrix.M24) + (vector.Z * matrix.M34) + (vector.W * matrix.M44));
        }

        public static float4 Transform(float2 vector, float4x4 matrix)
        {
            return float4(
                (vector.X * matrix.M11) + (vector.Y * matrix.M21) + matrix.M41,
                (vector.X * matrix.M12) + (vector.Y * matrix.M22) + matrix.M42,
                (vector.X * matrix.M13) + (vector.Y * matrix.M23) + matrix.M43,
                (vector.X * matrix.M14) + (vector.Y * matrix.M24) + matrix.M44);
        }

        public static float4 Transform(float3 vector, float4x4 matrix)
        {
            return float4(
                (vector.X * matrix.M11) + (vector.Y * matrix.M21) + (vector.Z * matrix.M31) + matrix.M41,
                (vector.X * matrix.M12) + (vector.Y * matrix.M22) + (vector.Z * matrix.M32) + matrix.M42,
                (vector.X * matrix.M13) + (vector.Y * matrix.M23) + (vector.Z * matrix.M33) + matrix.M43,
                (vector.X * matrix.M14) + (vector.Y * matrix.M24) + (vector.Z * matrix.M34) + matrix.M44);
        }


        public static float3 TransformAffine(float3 vector, float4x4 matrix)
        {
            // Assumes W is one, assumes transformed W is one.
            return float3(
                (vector.X * matrix.M11) + (vector.Y * matrix.M21) + (vector.Z * matrix.M31) + matrix.M41,
                (vector.X * matrix.M12) + (vector.Y * matrix.M22) + (vector.Z * matrix.M32) + matrix.M42,
                (vector.X * matrix.M13) + (vector.Y * matrix.M23) + (vector.Z * matrix.M33) + matrix.M43);
        }

        public static float3 TransformCoordinate(float3 vector, float4x4 matrix)
        {
            // Assumes W is one, result XYZ is divided by transformed W.
            float4 tmp = Transform(vector, matrix);
            return tmp.XYZ / tmp.W;
        }

        public static float3 TransformNormal(float3 vector, float4x4 matrix)
        {
            // Assumes W is zero, and thus only applies scale and rotation.
            return float3(
                (vector.X * matrix.M11) + (vector.Y * matrix.M21) + (vector.Z * matrix.M31),
                (vector.X * matrix.M12) + (vector.Y * matrix.M22) + (vector.Z * matrix.M32),
                (vector.X * matrix.M13) + (vector.Y * matrix.M23) + (vector.Z * matrix.M33));
        }

        public static float2 TransformCoordinate(float2 vector, float4x4 matrix)
        {
            // Assumes W is one, result XYZ is divided by transformed W.
            float4 tmp = Transform(vector, matrix);
            return tmp.XY / tmp.W;
        }

        public static float2 TransformNormal(float2 vector, float4x4 matrix)
        {
            // Assumes W is zero, and thus only applies scale and rotation.
            return float2(
                (vector.X * matrix.M11) + (vector.Y * matrix.M21),
                (vector.X * matrix.M12) + (vector.Y * matrix.M22));
        }
    }
}
