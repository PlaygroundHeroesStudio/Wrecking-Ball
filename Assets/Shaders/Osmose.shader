Shader "Custom/Osmose"
{
	Properties
	{
		_Color ("Main Color", Color) = (1, 1, 1, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
        _Sharpness ("Sharpness", Float) = 10.0
        _ShadingLevels ("Shading Levels", Float) = 10.0
        _Intensity ("Intensity", Float) = 1.0
		_Speed ("Speed", Float) = 0.05
		_Bend ("Bend", Float) = 1.0
	}
	
	SubShader
	{
		Tags { "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma surface surf Spec vertex:vert
		
		struct Input
		{
			float2 uv_MainTex;
		};
		
		fixed4 _Color;
		sampler2D _MainTex;
        float _Sharpness;
        float _ShadingLevels;
        float _Intensity;
        float _Speed;
        float _Bend;
		
        half4 LightingSpec(SurfaceOutput SO, half3 LightDir, half3 ViewDir, half Attenuation)
        {
        	half3 Norm = normalize(SO.Normal);
        	
        	half Dot = max(0, dot(Norm, LightDir)) * 2;
        	
			half3 Lamb = SO.Albedo * _LightColor0.rgb * Dot * Attenuation;
			
			half3 Reflect = normalize(Dot * Norm - LightDir);
			
			half3 Spec = saturate(_LightColor0.rgb * pow(max(0, dot(Reflect, ViewDir)), _Sharpness) * _Intensity * Attenuation);
			
            return float4(round(min(Lamb + Spec, 1.0f) * _ShadingLevels) / _ShadingLevels * _LightColor0.a, SO.Alpha);
        }
      
		void vert (inout appdata_full v)
		{
			v.vertex.xyz += v.normal * (1.0f - length(v.texcoord - 0.5f)) * _Bend;
		}
        
		void surf(Input In, inout SurfaceOutput SO)
		{
		    float2 Diff = 0.5f - In.uv_MainTex;
		    float Length = length(Diff) * 2;
		    float Move = Length + _Time.y;
		    float2 UV = lerp(0.5f + normalize(Diff) * 0.5f, 0.5f, Move - trunc(Move));
		    float4 Colour = tex2D(_MainTex, UV) * _Color;
		   
		    SO.Albedo = Colour.rgb * Colour.a;
		    SO.Alpha = Colour.a * (1.0f - Length);
		}
		
		ENDCG
	}
	
	FallBack "VertexLit"
}