Shader "Custom/Lava"
{
    Properties
    {
        _Color ("Emission Color", Color) = (1,1,1,1)
        _MainTex ("Gradient (RGB)", 2D) = "white" {}
        _SamplingGrad ("Sampling (RG)", 2D) = "white" {}
        _Sampling("Sampling Intensity", FLOAT) = 1

        _RimColor ("Rim Color", Color) = (1,1,1,1)
        _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0

        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Intensity("Intensity", FLOAT) = 1
        _Emission("Emission", FLOAT) = 1
        _Speed("Speed", FLOAT) = 1
        _Shininess("Shininess", FLOAT) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        Cull off
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex, _SamplingGrad;
        fixed _RimPower, _Sampling;
        fixed4 _RimColor;
        float random (float2 st) {
            return frac(sin(dot(st.xy,
                         fixed2(12.9898,78.233)))
                 * 43758.5453123);
        }

        float noise (float2 st) {
            fixed2 i = floor(st);
            fixed2 f = frac(st);

            // Four corners in 2D of a tile
            float a = random(i);
            float b = random(i + fixed2(1.0, 0.0));
            float c = random(i + fixed2(0.0, 1.0));
            float d = random(i + fixed2(1.0, 1.0));

            // Smooth Interpolation

            // Cubic Hermine Curve.  Same as SmoothStep()
            //fixed2 u = f*f*(3.0-2.0*f);
            fixed2 u = smoothstep(0.,1.,f);

            // Mix 4 coorners percentages
            return lerp(a, b, u.x) +
                    (c - a)* u.y * (1.0 - u.x) +
                    (d - b) * u.x * u.y;
        }
        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            float3 worldNormal;
            float3 viewDir;

        };

        half _Glossiness;
        half _Metallic, _Emission;
        float _Intensity,_Speed, _Shininess;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 s = tex2D (_SamplingGrad, IN.uv_MainTex * _Sampling);

            // Albedo comes from a texture tinted by color
            float value = (noise(IN.uv_MainTex * _Intensity)+_Speed)*_Shininess;
            float value2 = (noise(IN.worldPos * _Intensity)+ sin(_Time.y * _Speed))*_Shininess;
            float value3 = (noise(s.rg))*_Shininess;

            value = (value + value2) /2;
            value = lerp(value,value2, value3);
            fixed4 c = tex2D (_MainTex, fixed2(value,value));
            fixed3 dir = IN.worldNormal - _WorldSpaceCameraPos; 
           // float value2 = (noise(IN.uv_MainTex * _Intensity)+_Speed)*_Shininess;
            value = floor(value);
            //value = floor(value - value2)   ;
            o.Albedo = c;//length(normalize(dir));//dot(IN.worldNormal;//fixed3(value,value,value);
            // Metallic and smoothness come from slider variables
            half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
            o.Emission = _RimColor.rgb * pow (rim, _RimPower) +(_Emission* c * _Color);

           // o.Emission = c *_Color * value * _Emission;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
        }

       
        ENDCG
    }
    FallBack "Diffuse"
}
