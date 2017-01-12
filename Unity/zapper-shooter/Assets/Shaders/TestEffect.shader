
Shader "TestEffect" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_DeltaX ("Delta X", Float) = 0.0010
		_DeltaY ("Delta Y", Float) = 0.0006
		_Blur ("Blur", Int) = 0
	}
	SubShader {
		Pass {
            CGPROGRAM

            #pragma vertex vert             
            #pragma fragment frag

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
                return half4(1.0, 0.0, 0.0, 1.0); 
            }
            ENDCG
		}
	}
}
