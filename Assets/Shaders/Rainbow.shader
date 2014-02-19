Shader "Custom/Rainbow"
{
	Properties
	{
		_Color ("Main Color", Color) = (1, 1, 1, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_TintTex ("Tint (RGB)", 2D) = "white" {}
		_Duration ("Duration", Float) = 7.0
	}
	
	SubShader
	{
		CGPROGRAM
		#pragma surface surf Lambert
		
		float4 _Color;
		sampler2D _MainTex;
		sampler2D _TintTex;
		float _Duration;
		
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input In, inout SurfaceOutput SO)
		{
			half3 Tint = tex2D(_TintTex, float2(0.0f, _Time.y / _Duration + length(In.uv_MainTex - 0.5f) * 2));
			
			half4 Colour = tex2D(_MainTex, In.uv_MainTex) * _Color;
			
			SO.Albedo = Colour.rgb * Tint;
			SO.Alpha = Colour.a;
		}
		
		ENDCG
	}
	
	FallBack "Diffuse"
}