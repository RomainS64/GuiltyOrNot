Shader "Custom/DegradeResolutionShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" { }
        _ResolutionFactor ("Resolution Factor", Range(1, 8)) = 2
    }

    SubShader
    {
        Tags { "Queue" = "Overlay" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma exclude_renderers gles xbox360 ps3
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float _ResolutionFactor;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = float4(1, 1, 1, 1); // White color for simplicity
                return o;
            }

            fixed4 frag(v2f i) : COLOR
            {
                // Calculate the UV coordinates for the texture
                float2 uv = i.pos.xy / i.pos.w;

                // Downsample the texture
                float2 downsampledUV = floor(uv * _ResolutionFactor) / _ResolutionFactor;

                // Sample the texture
                fixed4 col = tex2D(_MainTex, downsampledUV);

                return col;
            }
            ENDCG
        }
    }
}