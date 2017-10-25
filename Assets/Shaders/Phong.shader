Shader "Phong"
{
	// 
	Properties
	{
		_PointLightColor("Point Light Color", Color) = (0, 0, 0)
		_PointLightPosition("Point Light Position", Vector) = (0.0, 0.0, 0.0)

		_LowTex ("Texture", 2D) = "white" {}
		_MidTex("Texutre",2D) = "white" {}
		_HighTex("Texture",2D) = "black" {}
		_MaxHeight("Maximum Height",float)= 1
		_MinHeight("Minimum Height",float)=-1
	

	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Opaque" }
		// Level of Detail
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma surface surf Lambert alpha
			
			#include "UnityCG.cginc"


			// Input and output vertices. 
			struct vertexInput
			{
				float4 vertex: POSITION;
				float3 normal: NORMAL;
				float4 col : COLOR;
				float2 uv: TEXCOORD1;
			};

			struct vertexOutput
			{
				float4 vertex : SV_POSITION;
				float4 col : COLOR;

				float3 worldNormal: NORMAL;
				float4 worldVertex: TEXCOORD0;
				float2 uv: TEXCOORD1;
			};
			// Different textures. 
			uniform sampler2D _LowTex;
			uniform sampler2D _HighTex;	
			uniform sampler2D _MidTex;
			// Heights
			float _MaxHeight;
			float _MinHeight;

			// Light colors
			float4 _LightColor0;
			float3 _PointLightColor;
			float3 _PointLightPosition;

			// Output vertex
			vertexOutput vert (vertexInput v)
			{
				vertexOutput o;

				o.uv = v.uv;
				o.worldVertex = mul(unity_ObjectToWorld, v.vertex);
				o.worldNormal= normalize( mul( unity_WorldToObject,float4(v.normal,1.0) ).xyz );
				o.col = v.col;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}


			// fragment shader
			fixed4 frag (vertexOutput v) : COLOR
			{

			//C' = C(a/255) + 255(1-(a/255));
				// Calculating height of the blend
				float percent = 0.4;
				percent= 1-(1-percent)/2; 
				float totalHeight = _MaxHeight - _MinHeight;
				float blendHeight = 0.1*totalHeight;

				// 
				float upThresh = _MaxHeight- (totalHeight*(1-percent));
				float upLerpU = upThresh+ blendHeight/2;
				float upLerpD = upThresh-blendHeight/2;

				float downThresh = _MinHeight + (totalHeight*(1-percent));
				float downLerpU = downThresh + blendHeight/2;
				float downLerpD = downThresh - blendHeight/2;

				// Different textures
				float4 texMid = tex2D(_MidTex,v.uv);
				float4 texHigh = tex2D(_HighTex,v.uv);
				float4 texLow = tex2D(_LowTex,v.uv);


				// Assign appropriate texture
				if(v.worldVertex.y >= downThresh && v.worldVertex.y<=upThresh){
					v.col = texMid;
				}
				if(v.worldVertex.y>=upThresh){
					//texMid.a= 100*( (v.worldVertex.y-upThresh)/(_MxHeight-upThresh));
					v.col = texHigh;
				}
				if (v.worldVertex.y<=downThresh) {
						
					//texMid.a = 100*( (downThresh-v.worldVertex.y)/(downThresh-_MinHeight));
					v.col= texLow;
				}
				// Interpolate if not pure. 
				if(v.worldVertex.y >= upLerpD && v.worldVertex.y<= upLerpU){
					float amountBlend = ((v.worldVertex.y- upLerpD)/blendHeight)>=1 ? 1:  ((v.worldVertex.y- upLerpD)/blendHeight);
					v.col = lerp(texMid,texHigh,amountBlend);
				}

				float amountBlend;
				if(v.worldVertex.y >= upLerpD && v.worldVertex.y<= upLerpU){
					amountBlend = ((v.worldVertex.y- upLerpD)/blendHeight);
					v.col = lerp(texMid,texHigh,amountBlend);
				}

				if(v.worldVertex.y >= downLerpD && v.worldVertex.y<= downLerpU){
					amountBlend = ((v.worldVertex.y- downLerpD)/blendHeight);
					v.col = lerp(texLow,texMid,amountBlend);
				}

				float3 interpNormal = normalize(v.worldNormal);
				float Ka = 1;
				float3 amb = _LightColor0.xyz* v.col.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb * Ka;
				float fAtt = 1;
				float Kd = 1;
				// _WorldSpaceLightPos0.xyz - v.worldVertex.xyz
				float3 lightDirection = (normalize(_PointLightPosition.xyz - v.worldVertex.xyz));
				float LdotN = max(0, dot(lightDirection,interpNormal));
				//saturated
				float3 lambertReflection =  fAtt*Kd* _LightColor0.xyz*v.col.rgb *LdotN;


				float Ks = 1;
				float specN = 5;
				// Where a perfect (non-diffused) reflection would go. 
				float3 perfectReflect = normalize((2.0 * LdotN * interpNormal) - lightDirection);

				// Direction of view
				float3 viewDirection = normalize(float3(_WorldSpaceCameraPos.xyz-v.worldVertex));

				//float3 spe = fAtt *  _LightColor0.xyz* Ks * pow(saturate(dot(viewDirection, perfectReflect )), specN);

				specN = 5; // We usually need a higher specular power when using Blinn-Phong

				//float3 spe = fAtt * _LightColor0.xyz * Ks * pow(max(0,dot(viewDirection,perfectReflectd)), specN);
				float3 H = normalize(viewDirection + lightDirection);
				float3 spe = fAtt * _LightColor0.xyz * Ks * pow(max(0,dot(interpNormal, H)), specN);
				v.col = v.col*float4((amb+lambertReflection+spe),v.col.a);
				if(LdotN==0){
					v.col = float4(amb,0);
				}

				return v.col;
			}
			ENDCG
		}
	}
}
