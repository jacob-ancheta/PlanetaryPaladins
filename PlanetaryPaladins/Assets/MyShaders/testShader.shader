Shader "Unlit/BlueLightsaber"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _EmissionColor("Emission Color", Color) = (0, 0, 1, 1) // Default emission color is blue
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }
            LOD 100

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                // make fog work
                #pragma multi_compile_fog

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    UNITY_FOG_COORDS(1)
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                fixed4 _EmissionColor;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    UNITY_TRANSFER_FOG(o,o.vertex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // Center the coordinates
                    float2 uv = i.uv - 0.5;

                    // Aspect ratio correction
                    uv.x *= _MainTex_ST.x / _MainTex_ST.y;

                    // Angle of the current pixel relative to the center
                    float angle = atan2(uv.y, uv.x);

                    // Radius of the cylinder
                    float radius = 0.2;

                    // Creating the cylindrical shape
                    float cylinder = smoothstep(radius - 0.01, radius, length(uv));

                    // Create glowing core
                    float core = smoothstep(0.02, 0.1, abs(cylinder - 0.5));

                    // Add glowing effect
                    float glow = smoothstep(0.1, 0.2, abs(cylinder - 0.5));

                    // Add some flickering effect
                    float flicker = abs(sin(_Time.y * 5.0 + i.vertex.x * 0.1));

                    // Final color
                    fixed3 color = fixed3(0.0, 0.3, 1.0) * (core * (1.0 - flicker * 0.1));

                    // Emission
                    fixed3 emission = _EmissionColor * glow;

                    // Combine color with emission
                    color += emission;

                    // apply fog
                    UNITY_APPLY_FOG(i.fogCoord, color);

                    return fixed4(color, 1.0);
                }
                ENDCG
            }
        }
}
