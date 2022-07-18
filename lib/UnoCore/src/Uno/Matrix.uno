using Uno.Compiler.ExportTargetInterop;
using Uno.Math;
using Uno.Vector;

namespace Uno
{
    /*
     * Contains static methods for manipulating float3x3 and float4x4 as matrices.
     */
    public static class Matrix
    {
        const float ZeroTolerance = 1e-05f;

        public static float4x4 LookAtLH(float3 eye, float3 target, float3 up)
        {
            float3 zaxis = Normalize(target - eye);
            float3 xaxis = Normalize(Cross(up, zaxis));
            float3 yaxis = Normalize(Cross(zaxis, xaxis));

            float4x4 result = float4x4.Identity;
            result.M11 = xaxis.X; result.M21 = xaxis.Y; result.M31 = xaxis.Z;
            result.M12 = yaxis.X; result.M22 = yaxis.Y; result.M32 = yaxis.Z;
            result.M13 = zaxis.X; result.M23 = zaxis.Y; result.M33 = zaxis.Z;

            result.M41 = Dot(xaxis, eye);
            result.M42 = Dot(yaxis, eye);
            result.M43 = Dot(zaxis, eye);

            result.M41 = -result.M41;
            result.M42 = -result.M42;
            result.M43 = -result.M43;

            return result;
        }

        public static float4x4 PerspectiveLH(float fovRadians, float aspect, float znear, float zfar)
        {
            float yScale = 1.0f / Tan(fovRadians * 0.5f);
            float xScale = yScale / aspect;

            float halfWidth = znear / xScale;
            float halfHeight = znear / yScale;

            float left = -halfWidth;
            float right = halfWidth;
            float bottom = -halfHeight;
            float top = halfHeight;

            float zRange = zfar / (zfar - znear);

            var result = default(float4x4);
            result.M11 = 2.0f * znear / (right - left);
            result.M22 = 2.0f * znear / (top - bottom);
            result.M31 = (left + right) / (left - right);
            result.M32 = (top + bottom) / (bottom - top);
            result.M33 = zRange;
            result.M34 = 1.0f;
            result.M43 = -znear * zRange;

            // TODO: Get rid of these extra Mul-s (see PerspectiveRH)
            return Mul(Mul(result, Scaling(1.0f, 1.0f, 2.0f)), Translation(0.0f, 0.0f, -1.0f));
        }

        public static float4x4 OrthoLH(float width, float height, float zNear, float zFar)
        {
            float halfWidth = width * 0.5f;
            float halfHeight = height * 0.5f;

            float left = -halfWidth;
            float right = halfWidth;
            float bottom = -halfHeight;
            float top = halfHeight;

            float zRange = 1.0f / (zFar - zNear);

            float4x4 result = float4x4.Identity;
            result.M11 = 2.0f / (right - left);
            result.M22 = 2.0f / (top - bottom);
            result.M33 = zRange;
            result.M41 = (left + right) / (left - right);
            result.M42 = (top + bottom) / (bottom - top);
            result.M43 = -zNear * zRange;

            return result;
        }

        public static float4x4 OrthoRH(float width, float height, float zNear, float zFar)
        {
            float4x4 result = OrthoLH(width, height, zNear, zFar);
            result.M33 *= -1.0f;
            return result;
        }

        public static float4x4 LookAtRH(float3 eye, float3 target, float3 up)
        {
            float3 zaxis = (eye - target);
            float3 xaxis = (Cross(up, zaxis));
            float3 yaxis = (Cross(zaxis, xaxis));
            return Look(eye, xaxis, yaxis, zaxis);
        }

        public static float4x4 Look(float3 eye, float3 xaxis, float3 yaxis, float3 zaxis)
        {
            zaxis = Normalize(zaxis);
            xaxis = Normalize(xaxis);
            yaxis = Normalize(yaxis);
            float4x4 result = float4x4.Identity;
            result.M11 = xaxis.X; result.M21 = xaxis.Y; result.M31 = xaxis.Z;
            result.M12 = yaxis.X; result.M22 = yaxis.Y; result.M32 = yaxis.Z;
            result.M13 = zaxis.X; result.M23 = zaxis.Y; result.M33 = zaxis.Z;
            result.M41 = -Dot(xaxis, eye);
            result.M42 = -Dot(yaxis, eye);
            result.M43 = -Dot(zaxis, eye);
            return result;
        }

        public static float4x4 PerspectiveRH(float fovRadians, float aspect, float znear, float zfar)
        {
            float yHalfScale = 0.5f / Tan(fovRadians * 0.5f);
            float xHalfScale = yHalfScale / aspect;

            float width = znear / xHalfScale;
            float height = znear / yHalfScale;
            float length = zfar - znear;

            float znearDoubled = 2.0f * znear;

            float4x4 result = default(float4x4);
            result.M11 = znearDoubled / width;
            result.M22 = znearDoubled / height;
            result.M33 = (-zfar-znear) / length;
            result.M43 = -znearDoubled * zfar / length;
            result.M34 = -1.0f;
            return result;
        }

        public static float4x4 Scaling(float3 scale)
        {
            float4x4 result = float4x4.Identity;
            result.M11 = scale.X;
            result.M22 = scale.Y;
            result.M33 = scale.Z;
            return result;
        }

        public static float4x4 Scaling(float x, float y, float z)
        {
            float4x4 result = float4x4.Identity;
            result.M11 = x;
            result.M22 = y;
            result.M33 = z;
            return result;
        }

        public static float4x4 Scaling(float scale)
        {
            float4x4 result = float4x4.Identity;
            result.M11 = scale;
            result.M22 = scale;
            result.M33 = scale;
            return result;
        }

        public static float4x4 Shear(float2 angle)
        {
            //SVG definition of skew: http://www.w3.org/TR/SVG11/coords.html
            float4x4 result = float4x4.Identity;
            result.M12 = Tan(angle.Y);
            result.M21 = Tan(angle.X);
            return result;
        }

        public static float4x4 RotationAxis(float3 axisNormalized, float angleRadians)
        {
            axisNormalized = Normalize(axisNormalized);
            float x = axisNormalized.X;
            float y = axisNormalized.Y;
            float z = axisNormalized.Z;

            float c = Cos(angleRadians);
            float s = Sin(angleRadians);

            float xx = x * x;
            float yy = y * y;
            float zz = z * z;
            float xy = x * y;
            float xz = x * z;
            float yz = y * z;

            float4x4 result = float4x4.Identity;
            result.M11 = xx + (c * (1.0f - xx));
            result.M12 = (xy - (c * xy)) + (s * z);
            result.M13 = (xz - (c * xz)) - (s * y);
            result.M21 = (xy - (c * xy)) - (s * z);
            result.M22 = yy + (c * (1.0f - yy));
            result.M23 = (yz - (c * yz)) + (s * x);
            result.M31 = (xz - (c * xz)) + (s * y);
            result.M32 = (yz - (c * yz)) - (s * x);
            result.M33 = zz + (c * (1.0f - zz));

            return result;
        }

        public static float4x4 RotationX(float angleRadians)
        {
            return RotationAxis(float3(1,0,0), angleRadians);
        }

        public static float4x4 RotationY(float angleRadians)
        {
            return RotationAxis(float3(0,1,0), angleRadians);
        }

        public static float4x4 RotationZ(float angleRadians)
        {
            return RotationAxis(float3(0,0,1), angleRadians);
        }

        public static float4x4 RotationQuaternion(float4 rotation)
        {
            float xx = rotation.X * rotation.X;
            float yy = rotation.Y * rotation.Y;
            float zz = rotation.Z * rotation.Z;
            float xy = rotation.X * rotation.Y;
            float zw = rotation.Z * rotation.W;
            float zx = rotation.Z * rotation.X;
            float yw = rotation.Y * rotation.W;
            float yz = rotation.Y * rotation.Z;
            float xw = rotation.X * rotation.W;

            float4x4 result = float4x4.Identity;
            result.M11 = 1.0f - (2.0f * (yy + zz));
            result.M12 = 2.0f * (xy + zw);
            result.M13 = 2.0f * (zx - yw);
            result.M21 = 2.0f * (xy - zw);
            result.M22 = 1.0f - (2.0f * (zz + xx));
            result.M23 = 2.0f * (yz + xw);
            result.M31 = 2.0f * (zx + yw);
            result.M32 = 2.0f * (yz - xw);
            result.M33 = 1.0f - (2.0f * (yy + xx));

            return result;
        }

        public static float2x2 Transpose(float2x2 m)
        {
            return float2x2(
                m.M11, m.M21,
                m.M12, m.M22);
        }

        public static float3x3 Transpose(float3x3 m)
        {
            return float3x3(
                m.M11, m.M21, m.M31,
                m.M12, m.M22, m.M32,
                m.M13, m.M23, m.M33);
        }

        public static float4x4 Transpose(float4x4 m)
        {
            return float4x4(
                m.M11, m.M21, m.M31, m.M41,
                m.M12, m.M22, m.M32, m.M42,
                m.M13, m.M23, m.M33, m.M43,
                m.M14, m.M24, m.M34, m.M44);
        }

        public static float4x4 Translation(float3 offset)
        {
            float4x4 result = float4x4.Identity;
            result.M41 = offset.X;
            result.M42 = offset.Y;
            result.M43 = offset.Z;
            return result;
        }

        public static float4x4 Translation(float x, float y, float z)
        {
            float4x4 result = float4x4.Identity;
            result.M41 = x;
            result.M42 = y;
            result.M43 = z;
            return result;
        }

        public static float2x2 Mul(float2x2 a, float2x2 b, float2x2 c, float2x2 d, float2x2 e) { return Mul(Mul(a,b,c,d),e); }
        public static float2x2 Mul(float2x2 a, float2x2 b, float2x2 c, float2x2 d) { return Mul(Mul(a,b,c),d); }
        public static float2x2 Mul(float2x2 a, float2x2 b, float2x2 c) { return Mul(Mul(a,b),c); }

        public static float3x3 Mul(float3x3 a, float3x3 b, float3x3 c, float3x3 d, float3x3 e) { return Mul(Mul(a,b,c,d),e); }
        public static float3x3 Mul(float3x3 a, float3x3 b, float3x3 c, float3x3 d) { return Mul(Mul(a,b,c),d); }
        public static float3x3 Mul(float3x3 a, float3x3 b, float3x3 c) { return Mul(Mul(a,b),c); }

        public static float4x4 Mul(float4x4 a, float4x4 b, float4x4 c, float4x4 d, float4x4 e) { return Mul(Mul(a,b,c,d),e); }
        public static float4x4 Mul(float4x4 a, float4x4 b, float4x4 c, float4x4 d) { return Mul(Mul(a,b,c),d); }
        public static float4x4 Mul(float4x4 a, float4x4 b, float4x4 c) { return Mul(Mul(a,b),c); }

        public static float2x2 Mul(float2x2 left, float2x2 right)
        {
            float2x2 result;
            result.M11 = (left.M11 * right.M11) + (left.M12 * right.M21);
            result.M12 = (left.M11 * right.M12) + (left.M12 * right.M22);
            result.M21 = (left.M21 * right.M11) + (left.M22 * right.M21);
            result.M22 = (left.M21 * right.M12) + (left.M22 * right.M22);
            return result;
        }

        public static float3x3 Mul(float3x3 left, float3x3 right)
        {
            float3x3 result;
            result.M11 = (left.M11 * right.M11) + (left.M12 * right.M21) + (left.M13 * right.M31);
            result.M12 = (left.M11 * right.M12) + (left.M12 * right.M22) + (left.M13 * right.M32);
            result.M13 = (left.M11 * right.M13) + (left.M12 * right.M23) + (left.M13 * right.M33);
            result.M21 = (left.M21 * right.M11) + (left.M22 * right.M21) + (left.M23 * right.M31);
            result.M22 = (left.M21 * right.M12) + (left.M22 * right.M22) + (left.M23 * right.M32);
            result.M23 = (left.M21 * right.M13) + (left.M22 * right.M23) + (left.M23 * right.M33);
            result.M31 = (left.M31 * right.M11) + (left.M32 * right.M21) + (left.M33 * right.M31);
            result.M32 = (left.M31 * right.M12) + (left.M32 * right.M22) + (left.M33 * right.M32);
            result.M33 = (left.M31 * right.M13) + (left.M32 * right.M23) + (left.M33 * right.M33);
            return result;
        }

        public static float4x4 Mul(float4x4 left, float4x4 right)
        {
            float4x4 result;
            result.M11 = (left.M11 * right.M11) + (left.M12 * right.M21) + (left.M13 * right.M31) + (left.M14 * right.M41);
            result.M12 = (left.M11 * right.M12) + (left.M12 * right.M22) + (left.M13 * right.M32) + (left.M14 * right.M42);
            result.M13 = (left.M11 * right.M13) + (left.M12 * right.M23) + (left.M13 * right.M33) + (left.M14 * right.M43);
            result.M14 = (left.M11 * right.M14) + (left.M12 * right.M24) + (left.M13 * right.M34) + (left.M14 * right.M44);
            result.M21 = (left.M21 * right.M11) + (left.M22 * right.M21) + (left.M23 * right.M31) + (left.M24 * right.M41);
            result.M22 = (left.M21 * right.M12) + (left.M22 * right.M22) + (left.M23 * right.M32) + (left.M24 * right.M42);
            result.M23 = (left.M21 * right.M13) + (left.M22 * right.M23) + (left.M23 * right.M33) + (left.M24 * right.M43);
            result.M24 = (left.M21 * right.M14) + (left.M22 * right.M24) + (left.M23 * right.M34) + (left.M24 * right.M44);
            result.M31 = (left.M31 * right.M11) + (left.M32 * right.M21) + (left.M33 * right.M31) + (left.M34 * right.M41);
            result.M32 = (left.M31 * right.M12) + (left.M32 * right.M22) + (left.M33 * right.M32) + (left.M34 * right.M42);
            result.M33 = (left.M31 * right.M13) + (left.M32 * right.M23) + (left.M33 * right.M33) + (left.M34 * right.M43);
            result.M34 = (left.M31 * right.M14) + (left.M32 * right.M24) + (left.M33 * right.M34) + (left.M34 * right.M44);
            result.M41 = (left.M41 * right.M11) + (left.M42 * right.M21) + (left.M43 * right.M31) + (left.M44 * right.M41);
            result.M42 = (left.M41 * right.M12) + (left.M42 * right.M22) + (left.M43 * right.M32) + (left.M44 * right.M42);
            result.M43 = (left.M41 * right.M13) + (left.M42 * right.M23) + (left.M43 * right.M33) + (left.M44 * right.M43);
            result.M44 = (left.M41 * right.M14) + (left.M42 * right.M24) + (left.M43 * right.M34) + (left.M44 * right.M44);
            return result;
        }

        public static float Determinant(float4x4 m)
        {
            float temp1 = (m.M33 * m.M44) - (m.M34 * m.M43);
            float temp2 = (m.M32 * m.M44) - (m.M34 * m.M42);
            float temp3 = (m.M32 * m.M43) - (m.M33 * m.M42);
            float temp4 = (m.M31 * m.M44) - (m.M34 * m.M41);
            float temp5 = (m.M31 * m.M43) - (m.M33 * m.M41);
            float temp6 = (m.M31 * m.M42) - (m.M32 * m.M41);

            return ((((m.M11 * (((m.M22 * temp1) - (m.M23 * temp2)) + (m.M24 * temp3))) - (m.M12 * (((m.M21 * temp1) -
                (m.M23 * temp4)) + (m.M24 * temp5)))) + (m.M13 * (((m.M21 * temp2) - (m.M22 * temp4)) + (m.M24 * temp6)))) -
                (m.M14 * (((m.M21 * temp3) - (m.M22 * temp5)) + (m.M23 * temp6))));
        }

        public static float Determinant(float3x3 m)
        {
            return m.M11 * (m.M22 * m.M33 - m.M32 * m.M23) -
                   m.M21 * (m.M12 * m.M33 - m.M32 * m.M13) +
                   m.M31 * (m.M12 * m.M23 - m.M22 * m.M13);
        }

        public static float Determinant(float2x2 m)
        {
            return m.M11 * m.M22 -
                   m.M21 * m.M12;
        }

        public static float4x4 Invert(float4x4 value)
        {
            float4x4 result;
            if (!TryInvert(value, out result))
                return float4x4.Identity;
            return result;
        }

        /**
            Calculates the inverse of a matrix

            @param result the inverse of the `value` matrix. The values are undefined if this function returns `false`
            @return true if the inverse matrix is valid, false if it could not be calculated correctly
        */
        public static bool TryInvert(float4x4 value, out float4x4 result)
        {
            float b0 = (value.M31 * value.M42) - (value.M32 * value.M41);
            float b1 = (value.M31 * value.M43) - (value.M33 * value.M41);
            float b2 = (value.M34 * value.M41) - (value.M31 * value.M44);
            float b3 = (value.M32 * value.M43) - (value.M33 * value.M42);
            float b4 = (value.M34 * value.M42) - (value.M32 * value.M44);
            float b5 = (value.M33 * value.M44) - (value.M34 * value.M43);

            float d11 = value.M22 * b5 + value.M23 * b4 + value.M24 * b3;
            float d12 = value.M21 * b5 + value.M23 * b2 + value.M24 * b1;
            float d13 = value.M21 * -b4 + value.M22 * b2 + value.M24 * b0;
            float d14 = value.M21 * b3 + value.M22 * -b1 + value.M23 * b0;

            float det = value.M11 * d11 - value.M12 * d12 + value.M13 * d13 - value.M14 * d14;

            float detInv = 1.0f / det;

            float a0 = (value.M11 * value.M22) - (value.M12 * value.M21);
            float a1 = (value.M11 * value.M23) - (value.M13 * value.M21);
            float a2 = (value.M14 * value.M21) - (value.M11 * value.M24);
            float a3 = (value.M12 * value.M23) - (value.M13 * value.M22);
            float a4 = (value.M14 * value.M22) - (value.M12 * value.M24);
            float a5 = (value.M13 * value.M24) - (value.M14 * value.M23);

            float d21 = value.M12 * b5 + value.M13 * b4 + value.M14 * b3;
            float d22 = value.M11 * b5 + value.M13 * b2 + value.M14 * b1;
            float d23 = value.M11 * -b4 + value.M12 * b2 + value.M14 * b0;
            float d24 = value.M11 * b3 + value.M12 * -b1 + value.M13 * b0;

            float d31 = value.M42 * a5 + value.M43 * a4 + value.M44 * a3;
            float d32 = value.M41 * a5 + value.M43 * a2 + value.M44 * a1;
            float d33 = value.M41 * -a4 + value.M42 * a2 + value.M44 * a0;
            float d34 = value.M41 * a3 + value.M42 * -a1 + value.M43 * a0;

            float d41 = value.M32 * a5 + value.M33 * a4 + value.M34 * a3;
            float d42 = value.M31 * a5 + value.M33 * a2 + value.M34 * a1;
            float d43 = value.M31 * -a4 + value.M32 * a2 + value.M34 * a0;
            float d44 = value.M31 * a3 + value.M32 * -a1 + value.M33 * a0;

            result.M11 =  d11 * detInv; result.M12 = -d21 * detInv; result.M13 =  d31 * detInv; result.M14 = -d41 * detInv;
            result.M21 = -d12 * detInv; result.M22 =  d22 * detInv; result.M23 = -d32 * detInv; result.M24 =  d42 * detInv;
            result.M31 =  d13 * detInv; result.M32 = -d23 * detInv; result.M33 =  d33 * detInv; result.M34 = -d43 * detInv;
            result.M41 = -d14 * detInv; result.M42 =  d24 * detInv; result.M43 = -d34 * detInv; result.M44 =  d44 * detInv;

            return Abs(det) > ZeroTolerance;
        }

        public static float3x3 Invert(float3x3 value)
        {
            float3x3 result;
            if (!TryInvert(value, out result))
                return float3x3.Identity;
            return result;
        }
        
        /**
            Calculates the inverse of a matrix

            @param result the inverse of the `value` matrix. The values are undefined if this function returns `false`
            @return true if the inverse matrix is valid, false if it could not be calculated correctly
        */
        public static bool TryInvert(float3x3 value, out float3x3 result)
        {
            var det = Determinant(value);

            var detInv = 1.0f / det;

            result.M11 = detInv * (value.M22 * value.M33 - value.M32 * value.M23);
            result.M12 = detInv * (value.M32 * value.M13 - value.M12 * value.M33);
            result.M13 = detInv * (value.M12 * value.M23 - value.M22 * value.M13);
            result.M21 = detInv * (value.M23 * value.M31 - value.M21 * value.M33);
            result.M22 = detInv * (value.M11 * value.M33 - value.M31 * value.M13);
            result.M23 = detInv * (value.M21 * value.M13 - value.M11 * value.M23);
            result.M31 = detInv * (value.M21 * value.M32 - value.M31 * value.M22);
            result.M32 = detInv * (value.M31 * value.M12 - value.M11 * value.M32);
            result.M33 = detInv * (value.M11 * value.M22 - value.M12 * value.M21);

            return Abs(det) > ZeroTolerance;
        }

        public static float2x2 Invert(float2x2 value)
        {
            float2x2 result;
            if (!TryInvert(value, out result))
                return float2x2.Identity;
            return result;
        }
        
        /**
            Calculates the inverse of a matrix

            @param result the inverse of the `value` matrix. The values are undefined if this function returns `false`
            @return true if the inverse matrix is valid, false if it could not be calculated correctly
        */
        public static bool TryInvert(float2x2 value, out float2x2 result)
        {
            var det = Determinant(value);

            var detInv = 1.0f / det;

            result.M11 = detInv * value.M22;
            result.M12 = detInv * -value.M12;
            result.M21 = detInv * -value.M21;
            result.M22 = detInv * value.M11;

            return Abs(det) > ZeroTolerance;
        }

        public static float4x4 Compose(float3 scale, float4 rotationQuaternion, float3 translation)
        {
           return Mul(Mul(
                    Scaling(scale),
                    RotationQuaternion(rotationQuaternion)),
                    Translation(translation));
        }

        public static bool Decompose(float4x4 value, out float3 scale, out float4 rotationQuaternion, out float3 translation)
        {
            //Source: Unknown
            //References: http://www.gamedev.net/community/forums/topic.asp?topic_id=441695

            //Get the translation.
            translation.X = value.M41;
            translation.Y = value.M42;
            translation.Z = value.M43;

            //Scaling is the length of the rows.
            scale.X = Sqrt((value.M11 * value.M11) + (value.M12 * value.M12) + (value.M13 * value.M13));
            scale.Y = Sqrt((value.M21 * value.M21) + (value.M22 * value.M22) + (value.M23 * value.M23));
            scale.Z = Sqrt((value.M31 * value.M31) + (value.M32 * value.M32) + (value.M33 * value.M33));

            //If any of the scaling factors are zero, than the rotation matrix can not exist.
            if (Abs(scale.X) < ZeroTolerance ||
                Abs(scale.Y) < ZeroTolerance ||
                Abs(scale.Z) < ZeroTolerance)
            {
                rotationQuaternion = float4.Identity;
                return false;
            }

            //The rotation is the left over matrix after dividing out the scaling.
            float3x3 rotationMatrix = float3x3(
                value.M11 / scale.X, value.M12 / scale.X, value.M13 / scale.X,
                value.M21 / scale.Y, value.M22 / scale.Y, value.M23 / scale.Y,
                value.M31 / scale.Z, value.M32 / scale.Z, value.M33 / scale.Z);

            rotationQuaternion = Quaternion.RotationMatrix(rotationMatrix);
            return true;
        }

        public static float3 GetTranslation(float4x4 value)
        {
            return float3(value.M41, value.M42, value.M43);
        }

        public static float3 GetScaling(float4x4 value)
        {
            return float3(
                Sqrt((value.M11 * value.M11) + (value.M12 * value.M12) + (value.M13 * value.M13)),
                Sqrt((value.M21 * value.M21) + (value.M22 * value.M22) + (value.M23 * value.M23)),
                Sqrt((value.M31 * value.M31) + (value.M32 * value.M32) + (value.M33 * value.M33)));
        }

        public static float4 GetRotationQuaternion(float4x4 value)
        {
            float3 scale, offset;
            float4 rotation;
            Decompose(value, out scale, out rotation, out offset);
            return rotation;
        }
    }
}
