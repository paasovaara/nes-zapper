
Shader "ColorSwapper" {
	Properties {
		_MyColor ("Color (RGBA)", Color) = (1.0, 0.0, 0.0, 1.0)
	}
	SubShader {
		Pass {
            CGPROGRAM

            #pragma vertex vert             
            #pragma fragment frag
            
            uniform half4 _MyColor;
            
            struct vertInput {
                float4 pos : POSITION;
            };  

            struct vertOutput {
                float4 pos : SV_POSITION;
            };

            vertOutput vert(vertInput input) {
                vertOutput o;
                o.pos = mul(UNITY_MATRIX_MVP, input.pos);
                return o;
            }

            half4 frag(vertOutput output) : COLOR {
                return _MyColor; 
            }
            ENDCG
		}
	}
}
