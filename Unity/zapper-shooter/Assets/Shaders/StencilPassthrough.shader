// Unlit shader. Simplest possible colored shader.
// - no lighting
// - no lightmap support
// - no texture

Shader "Zapper/Passthrough" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,0)
}

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 100
    
    ZWrite Off
    ColorMask 0

	Stencil
    {
      Ref 1
      Comp Always
      Pass Replace
    }

	Pass {
        
        
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				UNITY_FOG_COORDS(0)
			};

			half4 _Color;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			half4 frag (v2f i) : COLOR
			{
				half4 col = _Color;
                col.a = 0.0;
				return col;
			}
		ENDCG
	}
}

}
