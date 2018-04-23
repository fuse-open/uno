using Uno;

namespace UnoTest.ShaderGenerator
{
    class Lights : DefaultShading
    {
        const int max_lights = 2;

        public bool[] Light_On = new bool[max_lights];
        public float3[] Light_Position = new float3[max_lights];

        public float[] Light_Distance = new float[max_lights];
        public float[] Light_FlatFactor = new float[max_lights];

        public float[] Light_DiffuseAttenuation = new float[max_lights];
        public float3[] Light_DiffuseColor = new float3[max_lights];

        public float[] Light_SpecularExponent = new float[max_lights];
        public float3[] Light_SpecularColor = new float3[max_lights];
        public float[] Light_SpecularAttenuation = new float[max_lights];

        public float3[] Light_AmbientColor = new float3[max_lights];
        public float[] Light_AmbientAttenuation = new float[max_lights];

        public Lights() {
            for( int i=0; i < max_lights; ++i ) {
                Light_On[i] = false;
                Light_Position[i] = float3(0,0,0);
                Light_Distance[i] = 10f;
                Light_FlatFactor[i] = 1.2f;
                Light_DiffuseAttenuation[i] = 0.9f;
                Light_DiffuseColor[i] = float3(1,1,1) * 1.8f;
                Light_SpecularExponent[i] = 35f;
                Light_SpecularColor[i] = float3(0.7f,0.85f,1f) * 1.1f;
                Light_SpecularAttenuation[i] = 2.5f;
                Light_AmbientColor[i] = float3(1,1,1) * 0.8f;
                Light_AmbientAttenuation[i] = 0.65f;
            }
        }

        //Diffuse Lighting
        float3 sLightDir: req(WorldPosition as float3)
            Vector.Normalize( pixel Light_Position[0] - WorldPosition );
        float sDistance: req(WorldPosition as float3)
            Math.Max( Light_FlatFactor[0],
            Vector.Length( pixel Light_Position[0] - WorldPosition ) / Light_Distance[0] );
        float sIntensityDiffuse: req(VertexNormal as float3)
            Math.Clamp( Vector.Dot( VertexNormal, sLightDir ), 0, 1 );

        float3 sDiffuseOut: req(DiffuseColor as float3)
            sIntensityDiffuse * DiffuseColor * Light_DiffuseColor[0] /
                Math.Pow( sDistance, Light_DiffuseAttenuation[0] );

        //Specular Ligthing
        float3 CameraFacing: req(CameraTarget as float3) req(CameraPosition as float3)
            Vector.Normalize( CameraTarget - CameraPosition );
        float3 H : Vector.Normalize( CameraFacing + sLightDir );
        float sIntensitySpecular: req(VertexNormal as float3)
            Math.Pow( Math.Clamp( Vector.Dot( VertexNormal, H ), 0, 1 ), Light_SpecularExponent[0] );

        float3 sSpecularOut: req(SpecularColor as float3)
            sIntensitySpecular * SpecularColor * Light_SpecularColor[0] /
                Math.Pow( sDistance, Light_SpecularAttenuation[0] );

        //Ambient Ligthing
        //float3 sAmbientOut: req(AmbientColor as float3)
        //  AmbientColor * Light_AmbientColor[0] / Math.Pow( sDistance, Light_AmbientAttenuation[0] );
        //
        //PixelColor: {
        //  return float4( sAmbientOut + sDiffuseOut + sSpecularOut, 1 );
        //};

        PixelColor: req(AmbientColor as float3) {
            //Ambient Ligthing
            var ambient = AmbientColor * Light_AmbientColor[0] /
                Math.Pow( sDistance, Light_AmbientAttenuation[0] );

            return float4( ambient + sDiffuseOut + sSpecularOut, 1 );
        };

        public void Draw()
        {
            draw DefaultShading, Cube, this
            {
                float3 AmbientColor: float3(0);
                PixelColor : float4();
                VertexCount: 0;
                VertexPosition: float3();
            };
        }
    }
}
