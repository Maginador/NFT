Shader "Unlit/StylizedPicture"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _newRColor("R Color", 2D) = "white" {}
        _colorRIntensity("Color R Intensity", Range(0,1)) = 1

        _newGColor("G Color", 2D) = "white" {}
        _colorGIntensity("Color G Intensity", Range(0,1)) = 1

        _newBColor("B Color", 2D) = "white" {}
        _colorBIntensity("Color B Intensity", Range(0,1)) = 1

        _displaceRamp("Displacement Ramp", 2D) = "white" {}
        _displacementIntensity("Displacement Intensity", Range(0,1)) = 0

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
            float _displacementIntensity, _colorRIntensity, _colorGIntensity, _colorBIntensity;
            sampler2D _newRColor, _newGColor, _newBColor,_displaceRamp;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 disp = tex2D(_displaceRamp, col.r) * _displacementIntensity;
                fixed4 disp2 = tex2D(_displaceRamp, col.g) * _displacementIntensity;

                 col = tex2D(_MainTex, i.uv + disp + disp2);

                col.r = lerp(col.r, tex2D(_newRColor, float2( col.r, col.r)), _colorRIntensity);
                col.g = lerp(col.g, tex2D(_newGColor, float2( col.g, col.g)), _colorGIntensity);
                col.b = lerp(col.b, tex2D(_newBColor, float2( col.b, col.b)), _colorBIntensity);

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
