using Uno;

namespace UnoTest
{
    public block Cube
    {
        apply DefaultPrimitivesBlock;

        public float Size: 10.0f;
        public float3 Scale: float3(Size);

//        ModelMesh MeshData: MeshGenerator.CreateCube(float3(0.0f), .5f);
    }
}
