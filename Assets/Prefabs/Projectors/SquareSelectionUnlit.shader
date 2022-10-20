Shader "Unlit/SquareSelectionUnlit"
{
    Properties
    {
        _ShapeColour("Square Colour", Color) = (1, 0, 1)
        _Corner1("First Corner", Vector) = (0, 0, 0)
        _Corner2("Second Corner", Vector) = (50, 50, 50)
    }
    SubShader
    {
        Tags { "RenderType"="Overlay" "Queue"="Overlay"}
        LOD 100
        Blend Zero One
        ZTest Always

            ColorMask RGB
    Cull Off Lighting Off ZWrite Off

        CGPROGRAM

        #pragma surface surfaceFunc Lambert

        #pragma target 3.0

        struct Input
        {
            float3 worldPos;
        };

        fixed3 _ShapeColour;

        float3 _Corner1;
        float3 _Corner2;

        void surfaceFunc(Input IN, inout SurfaceOutput o) {
            if (
                (IN.worldPos.x > _Corner1.x && IN.worldPos.x > _Corner2.x)
                || (IN.worldPos.z > _Corner1.z && IN.worldPos.z > _Corner2.z)
                || (IN.worldPos.x < _Corner1.x && IN.worldPos.x < _Corner2.x)
                || (IN.worldPos.z < _Corner1.z && IN.worldPos.z < _Corner2.z)
                )
            {

            }
            else {
                o.Albedo = _ShapeColour;
            }
        }
        ENDCG
    }
}
