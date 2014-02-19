Shader "Custom/Field"
{
	Properties
	{
		_Color ("Main Color", Color) = (1, 1, 1, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_DetailTex ("Detail (RGB)", 2D) = "white" {}
	}
	
	SubShader
	{
		Tags { "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		
		#pragma surface surf Lambert alpha
		
		float4 _Color;
		sampler2D _MainTex;
		sampler2D _DetailTex;
		
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input In, inout SurfaceOutput SO)
		{
			float Dist = abs(In.uv_MainTex.x - 0.5f) + abs(In.uv_MainTex.y - 0.5f);
			
			half4 Colour = tex2D(_MainTex, In.uv_MainTex) * tex2D(_DetailTex, In.uv_MainTex) * _Color * (1.0f - Dist * Dist);
			
			SO.Albedo = Colour.rgb;
			SO.Alpha = Colour.a;
		}
		
		ENDCG
	}
	
	FallBack "Diffuse"
}