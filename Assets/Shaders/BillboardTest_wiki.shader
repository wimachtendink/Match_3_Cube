
/*
	Based on billboarding shader tutorial on http://www.opengl-tutorial.org
*/

Shader "OGAF_Shader" {
	Properties{
	
		_MainTex("Texture Image", 2D) = "white" {}
		_Cutoff("Alpha Cutoff", Float) = 0.5	
		_ScaleX("Scale X", Float) = 1.0	
		_ScaleY("Scale Y", Float) = 1.0	
		_Radius("Radius", Float) = 0.5	
		_Shape("Shape", Int) = 0	
		_Color("Main Color", Color) = (0,0,0,0)

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

				//output.pos = UnityObjectToClipPos(float4( (UNITY_MATRIX_V[0].xyz) + (UNITY_MATRIX_V[1].xyz), 1));

				output.pos = UnityObjectToClipPos(float4((_ScaleX * i.v.x *  UNITY_MATRIX_V[0].xyz) + (_ScaleY * i.v.y * UNITY_MATRIX_V[1].xyz), 1));
				
				output.tex = i.tex;

				return output;
			  }


			  float4 frag(vertexOutput input) : COLOR
			  {
				 float4 textureColor = tex2D(_MainTex, input.tex.xy) * _Color;
				 
				 //0 = circle

				 if (_Shape < .9)
				 {
					 if (distance(input.tex.xy, float2(0.5f, 0.5f)) > _Radius)
					 {
						 discard;
					 }
				 }
				 else if (_Shape < 1.9)
				 {
					 float3 col = float3(0.0f, 0.0f, 0.0f);
					 float2 uv = float2(0.50f, 0.50f) - input.tex; //this should offset to the center... I don't think this works out...

					 uv = abs(uv);

					 float c = dot(uv, normalize(float2(1, 1.73)));
					 c = max(c, uv.x);

					 if (c > _Radius)
					 {
						 discard;
					 }
				 }
				 else if (_Shape < 2.9)
				 {
					 float2 uv = float2(0.50f, 0.50f) - input.tex;

					 uv = abs(uv);
					 if (uv.x > _Radius || uv.y > _Radius)
					 {
						 discard;
					 }
				 }
				 else if (_Shape < 3.9)
				 {
					 //Diamond
					 float3 col = float3(0.0f, 0.0f, 0.0f);
					 float2 uv = float2(0.50f, 0.50f) - input.tex;

					 uv = abs(uv);

					 float c = dot(uv, normalize(float2(1,1)));

					 if (c > _Radius - .06f)
					 {
						 discard;
					 }
				 }				 
				 return float4(textureColor.xyz, 1.0f);
			  }

			  ENDCG
		   }
	   }
}
