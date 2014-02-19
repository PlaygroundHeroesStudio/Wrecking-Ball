Shader "Custom/Outline"
{
	Properties
	{
		_Color ("Main Color", Color) = (1, 1, 1, 1)
		_Thickness ("Thickness", Float) = 0.05
	}
	
	SubShader
	{
		Tags { "Queue" = "Transparent+1" }
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		
		#pragma surface surf Lambert

		struct Input
		{
			float Dummy;
		};

		void surf(Input In, inout SurfaceOutput SO)
		{
			SO.Albedo = 0.0f;
			SO.Alpha = 0.0f;
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
			
			float4 _Color;
			float _Thickness;

			Vert vert(appdata_full In)
			{
				Vert V;
				
				V.Pos = mul(UNITY_MATRIX_MVP, In.vertex + float4(In.normal * _Thickness, 0.0f));
				
				return V;
			}
			
			half4 frag(Vert V) : COLOR
			{
				return _Color;
			}
			
			ENDCG
		}
	}
	
	FallBack "Diffuse"
}
