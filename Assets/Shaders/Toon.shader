// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Toon" {
    // Do not use default colours! DO NOT CHANGE SPEC COLOURS.
    Properties {
        // Close to white
        _Color ("Diffused Colour", Color) = (0.9,0.9,0.9,0.9)
        // Grey
        _UnlitColor ("Dark Color", Color) = (0.5,0.5,0.5,0.9)
        // Threshold to go from diffused to dark
        _DiffuseThreshold ("Lighting Threshold", Range(-1.1,1)) = 0.1
        // Colour of specular highlight, should mostly be white. Do not change
        _SpecColor ("Specular Material Color", Color) = (1,1,1,1)
        // Shininess of the object
        _Shininess ("Shininess", Range(0.5,1)) = 1 
        // Thickness of the outline.
        _OutlineThickness ("Outline Thickness", Range(0,1)) = 0.1
         
    }
   

    SubShader {
     Pass {
        Tags{ "LightMode" = "ForwardBase" }
        
        CGPROGRAM
       
        #pragma vertex vert
        #pragma fragment frag
        // My own variables.
        uniform float4 _Color;
        uniform float4 _UnlitColor;
        uniform float _DiffuseThreshold;
        uniform float4 _SpecColor;
        uniform float _Shininess;
        uniform float _OutlineThickness;
     
        // unity variables
        uniform float4 _LightColor0;
        uniform sampler2D _MainTex;
        uniform float4 _MainTex_ST;        
        // Vertex info for the vertex taken as output
        struct vertexInput {
           
        float4 vertex : POSITION;
        float3 normal : NORMAL;
        float4 texcoord : TEXCOORD0;
        };
       
        struct vertexOutput {
            float4 pos : SV_POSITION;
            float3 normalDir : TEXCOORD1;
            float4 lightDir : TEXCOORD2;
            float3 viewDir : TEXCOORD3;
            float2 uv : TEXCOORD0;
        };
       
        // Surface shader
        vertexOutput vert(vertexInput input) {
            // Output vector 
            vertexOutput output;
            //normalDirection - multiply the input normal and the unity position wrt to the world
            output.normalDir = normalize ( mul( float4( input.normal, 0.0 ), unity_WorldToObject).xyz );
           
            // Position in the world
            float4 posWorld = mul(unity_ObjectToWorld, input.vertex);
           
            // Direction of view, unit vector from camera view to the position of our output vector.
            output.viewDir = normalize( _WorldSpaceCameraPos.xyz - posWorld.xyz );
           
            // Light source appeats to be coming from the camera so that as the camera moves wrt to an
            // enemy it sourt of reflects on it.
            float3 fragmentToLightSource = ( _WorldSpaceCameraPos.xyz - posWorld.xyz);
            
            // Direction of light as a unit vector.
            output.lightDir = float4(
                // Use interpolation to make stuff smooth, original solution did not have interpolation
                // making it look bad, idea for interpolation taken from somehwhere on youtube. 
                // Use the w of the world space as the interpolation value between the camera and
                // the current output.
                normalize( lerp(_WorldSpaceLightPos0.xyz , fragmentToLightSource, _WorldSpaceLightPos0.w) ),
                lerp(1.0 , 1.0/length(fragmentToLightSource), _WorldSpaceLightPos0.w)
            );
            output.pos = UnityObjectToClipPos( input.vertex );  
           
            //UV-Map
            output.uv =input.texcoord;
            return output;
         
        }
        
        // Fragment shader
        float4 frag(vertexOutput input) : COLOR {
            // Normal direction odtted with light direction, see how much light we need.
            float nDotL = saturate(dot(input.normalDir, input.lightDir.xyz));
            // Show much much diffusion will happen.
            float diffuseCutoff = saturate( ( max(_DiffuseThreshold, nDotL) - _DiffuseThreshold ) *1000 );
            // How much Shininess is displayed.
            float specularCutoff = saturate( max(_Shininess, dot(reflect(-input.lightDir.xyz, input.normalDir), input.viewDir))-_Shininess ) * 1000;    
            // Same thing for outline.
            float outlineStrength = saturate( (dot(input.normalDir, input.viewDir ) - _OutlineThickness) * 1000 );
            // Normal lighting
            float3 ambientLight = (1-diffuseCutoff) * _UnlitColor.xyz;
            float3 diffuseReflection = (1-specularCutoff) * _Color.xyz * diffuseCutoff;
            // Highlight
            float3 specularReflection = _SpecColor.xyz * specularCutoff;
            // Combine everything
            float3 combinedLight = (ambientLight + diffuseReflection) * outlineStrength + specularReflection;
                   
            return float4(combinedLight, 1.0);
        }
                    
        ENDCG       
     }  
   }
   Fallback "Diffuse"
}