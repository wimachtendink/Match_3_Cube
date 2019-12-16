
/*
	Based on billboarding shader tutorial on http://www.opengl-tutorial.org
*/

Shader "FromSheet_Billboard" {
	Properties{
	
		_MainTex("Texture Image", 2D) = "white" {}
		_Cutoff("Alpha Cutoff", Float) = 0.5	
		_ScaleX("Scale X", Float) = 1.0	
		_ScaleY("Scale Y", Float) = 1.0	

	}
		SubShader{
		   Pass {

			  CGPROGRAM

			  #pragma vertex vert  
			  #pragma fragment frag
			  #include "UnityCG.cginc"


			  // User-specified uniforms            
			  uniform sampler2D _MainTex;
			  uniform float _Cutoff;
			  uniform float _ScaleX;
			  uniform float _ScaleY;
			  uniform float _Radius;
			  uniform int _Shape;
			  uniform float4 _Color;

			  struct vertexInput {
				 float4 v : POSITION;
				 float4 tex : TEXCOORD0;
			  };
			  struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 tex : TEXCOORD0;
			  };
				
			  //simple billboard
			  vertexOutput vert(vertexInput i)
			  {
				vertexOutput output;				  

				output.pos = UnityObjectToClipPos(float4((_ScaleX * i.v.x *  UNITY_MATRIX_V[0].xyz) + (_ScaleY * i.v.y * UNITY_MATRIX_V[1].xyz), 1));
				
				output.tex = i.tex;

				return output;
			  }


			  float4 frag(vertexOutput input) : COLOR
			  {
				 float4 textureColor = tex2D(_MainTex, input.tex.xy + input.pos.xy);// * _Color;

				 if(textureColor.a < .5)
				 {
				 	 discard;
				 }
				 
				 return float4(textureColor.x, textureColor.y, textureColor.z, 1.0);
			  }

			  ENDCG
		   }
	   }
}
