// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Glow" {
	Properties {
		_Color ("Main Color", Color) = (.5,.5,.5,1)
		_OutlineColor ("Outline Color", Color) = (1,1,1,1)
		_Outline ("Outline width", Range (0.0, 0.03)) = .1
		_MainTex ("Base (RGB)", 2D) = "white" { }
	}
 
    CGINCLUDE
    #include "UnityCG.cginc"
     
    struct v2in {
        float4 vertex : POSITION;
        float3 normal : NORMAL;
    };
     
    struct v2f {
        float4 pos : POSITION;
        float4 color : COLOR;
    };
     
    uniform float _Outline;
    uniform float4 _OutlineColor;
     
    // Make a scaled copy of the original v2in.
    v2f vert(v2in v) {
        
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
     
        float3 norm   = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
        float2 offset = TransformViewToProjection(norm.xy);
     
        o.pos.xy += offset * o.pos.z * _Outline;
        o.color = _OutlineColor;
        return o;
    }
    
    ENDCG
 
	SubShader {
		Tags { "Queue" = "Transparent" }
 
		// 
		Pass {
			Name "OUTLINE"
			// As suggested on the unity forums.
			Tags { "LightMode" = "Always" }
			Cull Off
			ZWrite Off
			ZTest Always
			ColorMask RGB // alpha not used
 
		    // Normal blending mode for the lighting. 
			Blend SrcAlpha OneMinusSrcAlpha
			
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
             
            half4 frag(v2f i) :COLOR {
                return i.color;
            }
            ENDCG
	    }
 
		Pass {
			Name "BASE"
			ZWrite On
			ZTest LEqual
			Blend SrcAlpha OneMinusSrcAlpha
			Material {
				Diffuse [_Color]
				Ambient [_Color]
			}
			Lighting On
			SetTexture [_MainTex] {
				ConstantColor [_Color]
				Combine texture * constant
			}
			SetTexture [_MainTex] {
				Combine previous * primary DOUBLE
			}
		}
	}
 
	SubShader {
		Tags { "Queue" = "Transparent" }
 
		Pass {
			Name "OUTLINE"
			Tags { "LightMode" = "Always" }
			Cull Front
			ZWrite Off
			ZTest Always
			ColorMask RGB
 
		
			Blend SrcAlpha OneMinusSrcAlpha
			
 
			CGPROGRAM
			#pragma vertex vert
			ENDCG
			SetTexture [_MainTex] { combine primary }
		}
 
		Pass {
			Name "BASE"
			ZWrite On
			ZTest LEqual
			Blend SrcAlpha OneMinusSrcAlpha
			Material {
				Diffuse [_Color]
				Ambient [_Color]
			}
			Lighting On
			SetTexture [_MainTex] {
				ConstantColor [_Color]
				Combine texture * constant
			}
			SetTexture [_MainTex] {
				Combine previous * primary DOUBLE
			}
		}
	}
 
	Fallback "Diffuse"
}