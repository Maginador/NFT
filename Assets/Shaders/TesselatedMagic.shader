Shader "Custom/Tesselated Magic" {
        Properties {
            _Tess ("Tessellation", Range(1,32)) = 4
            _MainTex ("Base (RGB)", 2D) = "white" {}
            _Displacement ("Displacement", Range(0, 1.0)) = 0.3
            _Color ("Color", color) = (1,1,1,0)
            _ColorGrad ("Grad", 2D) = "white" {}

            _ScreenMapping ("Vector for Tiling", VECTOR)=(1,1,0,0)
            //_Color("Color", COLOR) = (0,0,0)
            _LerpEmission("Lerp", float) = 1

            _EmissionIntensity("Emission", float) = 1
            _EmissionColor("Emission COlor", COLOR) = (0,0,0)
            _EmissionColorIntensity("Emission Color Intensity", float) = 1
            _Amount ("Extrusion Amount", Range(-1,1)) = 0.5
            _Intensity ("Intensity", float) = 1
            _Speed ("Speed", float) = 1
            [Toggle(USE_NORMAL)] _UseNormal("Use Normal", Float) = 0

        }
        SubShader {
            Tags { "RenderType"="Opaque" }
            LOD 300
            
            CGPROGRAM
            #pragma surface surf BlinnPhong addshadow fullforwardshadows tessellate:tessFixed vertex:disp nolightmap
            #pragma target 4.6
            #pragma shader_feature USE_NORMAL

            struct appdata {
                float4 vertex : POSITION;
                float4 tangent : TANGENT;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
            };

            float _Tess;

            float4 tessFixed()
            {
                return _Tess;
            }

            float _Displacement;
            
            float _Amount,_Intensity,_EmissionIntensity,
            _Speed, _EmissionColorIntensity, _LerpEmission;
            float4 _EmissionColor, _Color;
            sampler2D _MainTex,_ColorGrad;
            float4 _ScreenMapping;

            void disp (inout appdata_full v)
            {
               float4 col = tex2Dlod (_MainTex, float4(v.texcoord.xy,0,0));
          
                #ifdef USE_NORMAL
                    v.vertex.x += v.normal.x * max(0,sin(col.r * _Time.y*_Speed))*_Intensity; 
                    v.vertex.y += v.normal.y *max(0,sin(col.g * _Time.y*_Speed))*_Intensity; 
                    v.vertex.z += v.normal.z *max(0,sin(col.b * _Time.y*_Speed))*_Intensity; 
                    v.vertex.xyz -= v.normal * _Amount;

                #else   
                    v.vertex.x *= max(0.5f,sin(col.r* _Time.y*_Speed))*_Intensity; 
                    v.vertex.y *= max(0,sin(col.g * _Time.y*_Speed))*_Intensity; 
                    v.vertex.z *= max(0,sin(col.b * _Time.y*_Speed))*_Intensity; 
                   // v.vertex.xyz += v.normal * _Amount;

                #endif
            }

            struct Input {
                float2 uv_MainTex;
                float3 worldPos;

            };

            void surf (Input IN, inout SurfaceOutput o) {
               float2 newUV = (IN.worldPos.xy * _ScreenMapping.xy) + _ScreenMapping.zw + float2(_Time.x,0); 
          float3 color =  tex2D (_MainTex, (newUV)).rgb;
          float3 colorGrad = tex2D (_ColorGrad, (IN.uv_MainTex)).rgb;
          o.Albedo = lerp(IN.worldPos.zyx+1 * _EmissionIntensity,colorGrad*_EmissionIntensity,_LerpEmission); //_Color;//tex2D (_ColorGrad, IN.uv_MainTex).rgb;
          o.Emission =  colorGrad * _EmissionColorIntensity;
            }
            ENDCG
        }
        FallBack "Diffuse"
    }