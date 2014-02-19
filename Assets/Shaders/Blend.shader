Shader "Custom/Blend"
{
	Properties
	{
		_Color ("Main Color", Color) = (1, 1, 1, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BlendTex ("Blend (RGB)", 2D) = "white" {}
		_Factor ("Factor", Float) = 0.5
	}
	
	SubShader
	{
		CGPROGRAM
		#pragma surface surf Lambert
		
		float4 _Color;
		sampler2D _MainTex;
		sampler2D _BlendTex;
		float _Factor;
		
		struct Input
		{
			float2 uv_MainTex;
			float2 uv_BlendTex;
		};
		
		void surf(Input In, inout SurfaceOutput SO)
		{
			half4 Colour = lerp(tex2D(_MainTex, In.uv_MainTex), tex2D(_BlendTex, In.uv_BlendTex), _Factor) * _Color;
			
			SO.Albedo = Colour.rgb;
			SO.Alpha = Colour.a;
		}
		
		ENDCG
	}
	
	FallBack "Diffuse"
}