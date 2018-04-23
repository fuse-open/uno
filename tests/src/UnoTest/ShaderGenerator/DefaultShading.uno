using Uno;
using Uno.Math;
using Uno.Vector;
using Uno.Matrix;

namespace UnoTest
{
    public block DefaultShading
    {
        //public Entity : null;

        public texture2D NormalMap: prev;
        public texture2D DiffuseMap: prev;
        public texture2D SpecularMap: prev;

        public float3 VertexPosition: prev;
        public float2 TexCoord: prev;
        public float2 TexCoord0: prev;
        public float2 TexCoord1: prev;
        public float2 TexCoord2: prev;
        public float2 TexCoord3: prev;
        public float2 TexCoord4: prev;
        public float2 TexCoord5: prev;
        public float2 TexCoord6: prev;
        public float2 TexCoord7: prev;

        public float3 VertexNormal: prev;
        public float3 VertexBinormal: prev;
        public float3 VertexTangent: prev;

        public float4 VertexColor: prev;

        public double Time: 0;

        public float3 CameraPosition: float3(100,100,100);
        public float3 CameraTarget: float3(0,0,0);
        public float3 CameraUp: float3(0,0,1);

        public float ZNear: 1.0f;
        public float ZFar: 10000.0f;
        public float FovDegrees: 45.0f;
        public float FovRadians: FovDegrees / 180.0f * (float)PI;
        public float Aspect: 1.4f;

        public float4x4 View:
            LookAtRH(CameraPosition, CameraTarget, CameraUp);

        public float4x4 Projection:
            PerspectiveRH(FovRadians, Aspect, ZNear, ZFar);

        public float4x4 ViewProjection:
            Mul(View, Projection);

        public float3x3 View3x3: float3x3(View[0].XYZ, View[1].XYZ, View[2].XYZ);



        public float LinearDepth: (-ViewPosition.Z - ZNear) / (ZFar - ZNear); //pixel((2.0f * ZNear) / ((ZNear + ZFar) - (ClipPosition.Z / ClipPosition.W) * (ZFar - ZNear)));

        public float3 Translation:
            tag("Identity") float3(0,0,0);

        public float4x4 TranslationMatrix:
            tag("Identity") req(Translation tag "Identity") float4x4.Identity,
            Matrix.Translation(Translation);

        public float3 RotationAxis: tag("Identity") float3(0,0,1);
        public float RotationRadians: tag("Identity") 0;

        public float4 Rotation:
            tag("Identity") req(RotationAxis tag "Identity", RotationRadians tag "Identity") float4(0,0,0,1),
            Quaternion.RotationAxis(RotationAxis, RotationRadians);

        public float4x4 RotationMatrix:
            tag("Identity") req(Rotation tag "Identity") float4x4.Identity,
            RotationQuaternion(Rotation);

        public float UniformScale: tag("Identity") 1;

        public float3 Scale:
            tag("Identity", "UniformScale") req(UniformScale tag "Identity") float3(1,1,1),
            tag("UniformScale") float3(UniformScale);

        public float3 Scaling:
            tag("Identity", "UniformScale") req(Scale tag "Identity") float3(1,1,1),
            tag("UniformScale") req(Scale tag "UniformScale") Scale,
            Scale;

        public float4x4 ScalingMatrix:
            tag("Identity") req(Scaling tag "Identity") float4x4.Identity,
            tag("UniformScale") req(Scaling tag "UniformScale") Scaling(Scaling),
            Scaling(Scaling);

        public float4x4 ScalingRotationMatrix:
            tag("Identity") req(ScalingMatrix tag "Identity", RotationMatrix tag "Identity") float4x4.Identity,
            tag("UniformScale") req(ScalingMatrix tag "Identity") RotationMatrix,
            tag("UniformScale") req(ScalingMatrix tag "UniformScale", RotationMatrix tag "Identity") ScalingMatrix,
            req(RotationMatrix tag "Identity") ScalingMatrix,
            tag ("UniformScale") req(ScalingMatrix tag "UniformScale") Mul(ScalingMatrix, RotationMatrix),
            Mul(ScalingMatrix, RotationMatrix);

        public float4x4 WorldSrtMatrix:
            tag("Identity") req(ScalingRotationMatrix tag "Identity", TranslationMatrix tag "Identity") float4x4.Identity,
            tag("UniformScale") req(ScalingRotationMatrix tag "UniformScale", TranslationMatrix tag "Identity") ScalingRotationMatrix,
            req(TranslationMatrix tag "Identity") ScalingRotationMatrix,
            tag("UniformScale") req(ScalingRotationMatrix tag "UniformScale") Mul(ScalingRotationMatrix, TranslationMatrix),
            Mul(ScalingRotationMatrix, TranslationMatrix);

        public float4x4 World:
            WorldSrtMatrix;

        World:
            prev;

        public float4x4 WorldInverse:
            tag("Identity") req(World tag "Identity")
            float4x4.Identity, Invert(World);

        public float4x4 WorldInverseTranspose:
            tag("Identity") req(WorldInverse tag "Identity") float4x4.Identity,
            Transpose(WorldInverse);

        public float4x4 WorldView:
            tag("Default") req(World tag "Identity") View,
            tag("Default") Mul(World, View);

        public float4x4 WorldViewProjection:
            tag("Default") req(World tag "Identity") ViewProjection,
            tag("Default") Mul(World, ViewProjection);

        public float3x3 WorldRotation:
            tag("Identity") req(World tag "Identity") float3x3.Identity,
            req(World tag "UniformScale") float3x3(World[0].XYZ, World[1].XYZ, World[2].XYZ),
            float3x3(WorldInverseTranspose[0].XYZ, WorldInverseTranspose[1].XYZ, WorldInverseTranspose[2].XYZ);

        /*
        public float3x3 WorldRotationInverse:
            tag("Identity") req(World3x3 tag "Identity") float3x3.Identity,
            Transpose(WorldRotation);
        */

        public float3 WorldNormal:
            req(WorldRotation tag "Identity") Normalize(pixel VertexNormal),
            Normalize(pixel Transform(VertexNormal, WorldRotation));

        public float3 WorldBinormal:
            req(WorldRotation tag "Identity") Normalize(pixel VertexBinormal),
            Normalize(pixel Transform(VertexBinormal, WorldRotation));

        public float3 WorldTangent:
            req(WorldRotation tag "Identity") Normalize(pixel VertexTangent),
            Normalize(pixel Transform(VertexTangent, WorldRotation));

        public float3 WorldLightPosition: undefined;
        public float3 WorldLightDirection: WorldLightPosition - (pixel WorldPosition), float3(100, 0, 100);
        public float3 WorldViewDirection: (pixel WorldPosition) - CameraPosition;

        public float3 ViewNormal: Normalize(Transform(WorldNormal, View3x3));
        public float3 ViewBinormal: Normalize(Transform(WorldBinormal, View3x3));
        public float3 ViewTangent: Normalize(Transform(WorldTangent, View3x3));

        public float3 ViewLightDirection: Transform(Normalize(WorldLightDirection), View3x3);

        float3x3 Tangent3x3: float3x3(WorldTangent, WorldBinormal, WorldNormal);

        public float3 TangentLightDirection: Transform(WorldLightDirection, Tangent3x3);
        public float3 TangentNormal: NormalMap != null ? Normalize(sample(NormalMap, TexCoord).XYZ * 2.0f - 1.0f) : float3(0,0,1);
        public float3 TangentViewDirection: Transform(WorldViewDirection, Tangent3x3);

        public float3 Normal:
            Transform(TangentNormal, Tangent3x3),
            WorldNormal;

        public float3 LightDirection: WorldLightDirection;
        public float3 ViewDirection: WorldViewDirection;

        // TODO: Isn't the following more correct?
        //public float3 LightDirection: Transform(WorldLightDirection, World3x3Inverse);
        //public float3 ViewDirection: Transform(WorldViewDirection, World3x3Inverse);

        public float3 ReflectedViewDirection: Reflect(ViewDirection, Normal);

        public float3 AmbientLightColor: float3(0.5f);
        public float3 DiffuseLightColor: float3(1);
        public float3 SpecularLightColor: float3(1);

        // Material diffuse terms

        public float3 DiffuseColor: float3(1);
        public float4 DiffuseMapColor: DiffuseMap != null ? sample(DiffuseMap, TexCoord) : float4(1);
        public float3 MaterialDiffuse :
            DiffuseColor * DiffuseMapColor.XYZ,
            DiffuseColor;

        // Material specular terms

        public float3 SpecularColor: float3(1);
        public float4 SpecularMapColor: SpecularMap != null ? sample(SpecularMap, TexCoord) : float4(1);
        public float3 MaterialSpecular :
            SpecularColor * SpecularMapColor.XYZ,
            SpecularColor;

        public float Shininess: 22.0f;

        // Diffuse and specular light calculations

        float3 pixelNormal : Normalize(pixel Normal);
        float3 pixelLightDir : Normalize(pixel LightDirection);
        float3 pixelRefViewDir : Normalize(pixel ReflectedViewDirection);
        float pixelDot : Dot(pixelRefViewDir, pixelLightDir);

        public float DiffuseLight: Max(0.0f, Dot(pixelNormal, pixelLightDir));
        public float SpecularLight: Pow(Max(0.0f, pixelDot), Shininess);

        // Material X Light = Pixel color

        public float3 Specular :
            MaterialSpecular * SpecularLight * SpecularLightColor,
            MaterialSpecular;

        public float3 Diffuse :
            MaterialDiffuse * DiffuseLight * DiffuseLightColor,
            MaterialDiffuse;

        public float3 Ambient :
            MaterialDiffuse * AmbientLightColor;

        public float Opacity: 1.0f;

        PixelColor: float4(Ambient + Diffuse + Specular, Opacity);

        // Vertex transformations

        public float3 WorldPosition:
            tag("Default") req(World tag "Identity") VertexPosition,
            tag("Default") TransformAffine(VertexPosition, World);

        public float3 ViewPosition:
            tag("Default") req(WorldPosition tag "Default") TransformAffine(VertexPosition, WorldView),
            tag("Default") TransformAffine(WorldPosition, View);

        ClipPosition:
            tag("Default") req(ViewPosition tag "Default", WorldPosition tag "Default") Transform(VertexPosition, WorldViewProjection),
            tag("Default") req(ViewPosition tag "Default") Transform(WorldPosition, ViewProjection),
            tag("Default") Transform(ViewPosition, Projection);
    }
}
