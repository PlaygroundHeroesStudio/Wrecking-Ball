Shader "Custom/Toon"
{
	Properties
	{
		_Color ("Main Color", Color) = (1, 1, 1, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
        _Sharpness ("Sharpness", Float) = 10.0
        _ShadingLevels ("Shading Levels", Float) = 10.0
        _Intensity ("Intensity", Float) = 1.0
		_Outline ("Outline Color", Color) = (0, 0, 0, 1)
		_Thickness ("Thickness", Float) = 0.05
	}
	
	SubShader
	{
		CGPROGRAM
		#pragma surface surf Spec
		
		struct Input
		{
			float2 uv_MainTex;
		};
		
		fixed4 _Color;
		sampler2D _MainTex;
        float _Sharpness;
        float _ShadingLevels;
        float _Intensity;
		
        half4 LightingSpec(SurfaceOutput SO, half3 LightDir, half3 ViewDir, half Attenuation)
        {
        	half3 Norm = normalize(SO.Normal);
        	
        	half Dot = max(0, dot(Norm, LightDir)) * 2;
        	
			half3 Lamb = SO.Albedo * _LightColor0.rgb * Dot * Attenuation;
			
			half3 Reflect = normalize(Dot * Norm - LightDir);
			
			half3 Spec = saturate(_LightColor0.rgb * pow(max(0, dot(Reflect, ViewDir)), _Sharpness) * _Intensity * Attenuation);
			
            return float4(round(min(Lamb + Spec, 1.0f) * _ShadingLevels) / _ShadingLevels * _LightColor0.a, SO.Alpha);
        }
		
		void surf(Input In, inout SurfaceOutput SO)
		{
			half4 Colour = tex2D(_MainTex, In.uv_MainTex) * _Color;
			
			SO.Albedo = Colour.rgb;
			SO.Alpha = Colour.a;
		}
		
		ENDCG
		
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha 
			Cull Front
			
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			
			struct Vert
			{
				float4 Pos : POSITION;
			};
			
			float4 _Outline;
			float _Thickness;

			Vert vert(appdata_full In)
			{
				Vert V;
				
				V.Pos = mul(UNITY_MATRIX_MVP, In.vertex + float4(In.normal * unity_Scale.w * _Thickness, 0.0f));
				
				return V;
			}
			
			half4 frag(Vert V) : COLOR
			{
				return _Outline;
			}
			
			ENDCG
		}
	}
	
	FallBack "VertexLit"
}