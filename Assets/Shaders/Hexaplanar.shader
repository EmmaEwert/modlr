Shader "Custom/Hexaplanar" {
	Properties {
		_WestTex   ("West",   2D) = "white" {}
		_EastTex   ("East",   2D) = "white" {}
		_BottomTex ("Bottom", 2D) = "white" {}
		_TopTex    ("Top",    2D) = "white" {}
		_SouthTex  ("South",  2D) = "white" {}
		_MainTex   ("North",  2D) = "white" {}
		[HideInInspector]
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert

		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _SouthTex;
		sampler2D _TopTex;
		sampler2D _BottomTex;
		sampler2D _WestTex;
		sampler2D _EastTex;

		struct Input {
			float3 worldNormal;
			float3 position;
		};

		fixed4 _Color;
		
		void vert (inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.position = v.vertex.xyz;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			IN.position /= 16;
			
			fixed4 c = _Color * (
				tex2D(_MainTex,   IN.position.xy) * max(0, -IN.worldNormal.z) +
				tex2D(_SouthTex,  IN.position.xy) * max(0,  IN.worldNormal.z) +
				tex2D(_TopTex,    IN.position.xz) * max(0,  IN.worldNormal.y) +
				tex2D(_BottomTex, IN.position.xz) * max(0, -IN.worldNormal.y) +
				tex2D(_WestTex,   IN.position.zy) * max(0, -IN.worldNormal.x) +
				tex2D(_EastTex,   IN.position.zy) * max(0,  IN.worldNormal.x)
			);
			
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
