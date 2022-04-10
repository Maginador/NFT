Shader "Ricardo/Magic" {
    Properties {
      _MainTex ("Texture", 2D) = "white" {}
      _ColorGrad ("Texture", 2D) = "white" {}
      _AnimTexture ("Animation Texture", 2D) = "white" {}

      _ScreenMapping ("Vector for Tiling", VECTOR)=(1,1,0,0)
      _MaskVector ("Axis Mask for Movement", VECTOR)=(1,1,1)

      //_Color("Color", COLOR) = (0,0,0)
      _LerpEmission("Lerp", float) = 1

      _RimColor("Rim Color", COLOR) = (0,0,0)
      _RimPower ("Rim Power", float) = 1
      _EmissionIntensity("Emission", float) = 1
      _EmissionAnimationSpeed("Emission Animation Speed", float) = 0
      _EmissionMin("Emission Minimum Value", float) = 0
      _EmissionMax("Emission Maximun Value", float) = 1
      _EmissionColor("Emission COlor", COLOR) = (0,0,0)
      _EmissionColorIntensity("Emission Color Intensity", float) = 1
      _Amount ("Extrusion Amount", Range(-1,1)) = 0.5
      _Intensity ("Intensity", float) = 1
      _Speed ("Speed", float) = 1
		[Toggle(USE_NORMAL)] _UseNormal("Use Normal", Float) = 0

    }
    SubShader {
      Tags { "RenderType" = "Transparent" }
      Cull back
      CGPROGRAM
      #pragma surface surf Lambert vertex:vert 
      #pragma shader_feature USE_NORMAL

      struct Input {
          float2 uv_MainTex;
          float3 worldPos;
          float4 screenPos;
          float3 viewDir;

      };

      float3 _RimColor;
      float _RimPower;
      float _Amount,_Intensity,_EmissionIntensity,
      _Speed, _EmissionColorIntensity, _LerpEmission;
      float3 _EmissionColor, _Color;
      sampler2D _MainTex,_ColorGrad, _AnimTexture;
      float4 _ScreenMapping;
      float3 _MaskVector;
      float _EmissionAnimationSpeed, _EmissionMin, _EmissionMax;

      void vert (inout appdata_full v) {
          float4 col = tex2Dlod (_MainTex, float4(v.texcoord.xy,0,0));
            float4 anim = tex2Dlod (_AnimTexture, float4(sin(_Time.x),cos(_Time.x),0,0));

          _MaskVector *= anim.rgb;
      #ifdef USE_NORMAL
          v.vertex.x += v.normal.x * max(0,sin(col.r * _Time.y*_Speed))*_Intensity * _MaskVector.x; 
          v.vertex.y += v.normal.y *max(0,sin(col.g * _Time.y*_Speed))*_Intensity * _MaskVector.y; 
          v.vertex.z += v.normal.z *max(0,sin(col.b * _Time.y*_Speed))*_Intensity * _MaskVector.z; 
          v.vertex.xyz -= v.normal * _Amount;

      #else
          v.vertex.x *= max(0,sin(col.r * _Time.y*_Speed))*_Intensity * _MaskVector.x; 
          v.vertex.y *= max(0,sin(col.g * _Time.y*_Speed))*_Intensity * _MaskVector.y; 
          v.vertex.z *= max(0,sin(col.b * _Time.y*_Speed))*_Intensity * _MaskVector.z; 
          v.vertex.xyz += v.normal * _Amount;

      #endif

      }
      void surf (Input IN, inout SurfaceOutput o) {
          float2 newUV = (IN.worldPos.xy * _ScreenMapping.xy) + _ScreenMapping.zw + float2(_Time.x,0); 
        float2 tilledUV = (IN.screenPos.xy * _ScreenMapping.xy) + _ScreenMapping.zw; 

          float3 color =  tex2D (_MainTex, (newUV)).rgb;
          float3 colorGrad = tex2D (_ColorGrad, (tilledUV)).rgb;

          o.Albedo = lerp(IN.worldPos.zyx+1 * _EmissionIntensity,colorGrad*_EmissionIntensity,_LerpEmission); //_Color;//tex2D (_ColorGrad, IN.uv_MainTex).rgb;
          
           half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
            float3 rimCol = _RimColor.rgb * pow (rim, _RimPower);
          o.Emission =  rimCol + colorGrad * _EmissionColorIntensity * clamp(1+sin(_Time.x*_EmissionAnimationSpeed),_EmissionMin, _EmissionMax);
          o.Alpha = 1;
      }
      ENDCG
    } 
    Fallback "Diffuse"
  }