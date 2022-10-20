Shader "Unlit/SquareSelectionUnlit"
{
    Properties
    {
        _ShapeColour("Square Colour", Color) = (1, 0, 1)
        _Center("Center", Vector) = (0, 0, 0)
        _Radius("Radius", Range(0, 100)) = 10
        _Thickness("Thickness", Range(0, 100)) = 5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Blend Zero One
        CGPROGRAM

        #pragma surface surfaceFunc Lambert

        #pragma target 3.0

        struct Input
        {
            float3 worldPos;
            float4 color;
            float2 uv_MainText;
            float2 uv_BumpMap;
        };

        float3 _Center;
        float _Border;
        float _Radius;
        fixed3 _ShapeColour;
        float _Thickness;

        void surfaceFunc(Input IN, inout SurfaceOutput o) {
            float dist = distance(_Center, IN.worldPos);
            if (dist > _Radius && dist < (_Radius + _Thickness)) {
                o.Albedo = _ShapeColour;
            }
        }
        ENDCG
    }
}
