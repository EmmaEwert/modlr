Shader "Custom/Trixel" {
  Properties {
    _MainTex ("Albedo (RGB) Transparency (A)", 2D) = "white" {}
  }
  
  SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 200
    Cull Back
    
    CGPROGRAM
    #pragma surface surf Standard fullforwardshadows vertex:vert
    #pragma target 3.0
    
    sampler2D _MainTex;
    
    struct Input {
      float3 worldNormal;
      float3 position;
	  float3 minuspos;
    };
    
    void vert(inout appdata_full v, out Input o) {
      UNITY_INITIALIZE_OUTPUT(Input, o);
      o.position = mul(_Object2World, v.vertex.xyz) / 16.0f;
	  o.minuspos = o.position * float2(-1,1).xyx;
    }
    
    void surf(Input i, inout SurfaceOutputStandard o) {
      fixed4 c =
        tex2D(_MainTex, i.position.zy) * max(0.0f, -i.worldNormal.x) +
        tex2D(_MainTex, i.minuspos.zy) * max(0.0f,  i.worldNormal.x) +
        tex2D(_MainTex, i.position.xz) * max(0.0f, -i.worldNormal.y) +
        tex2D(_MainTex, i.minuspos.xz) * max(0.0f,  i.worldNormal.y) +
        tex2D(_MainTex, i.position.xy) * max(0.0f, -i.worldNormal.z) +
        tex2D(_MainTex, i.minuspos.xy) * max(0.0f,  i.worldNormal.z);
      o.Albedo = c.rgb;
      o.Alpha = c.a;
    }
    
    ENDCG
  }
  
  FallBack "Diffuse"
}
