Shader "Unlit/GridHeatMapShader"
{
    Properties
    {
         _Color("Main Color", Color) = (1,1,1,1)
         _MainTex("Texture", 2D) = "Main Color" {}
        
         _Color0("Color 0",Color) = (0,0,0,0)
         _Color1("Color 1",Color) = (0,.9,.2,1)
         _Color2("Color 2",Color) = (.9,1,.3,1)
         _Color3("Color 3",Color) = (.9,.7,.1,1)
         _Color4("Color 4",Color) = (1,0,0,1)

    }
        SubShader
         {   Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
             LOD 100
            ZWrite Off
            Lighting Off
             Fog { Mode Off }
              Blend SrcAlpha OneMinusSrcAlpha


             Pass
             {
                 Color[_Color]
                 CGPROGRAM
                 #pragma vertex vert
                 #pragma fragment frag

                 #include "UnityCG.cginc"

                 struct appdata
                 {
                     float4 vertex : POSITION;
                     float2 uv : TEXCOORD0;
                 };

                 struct v2f
                 {
                     float2 uv : TEXCOORD0;

                     float4 vertex : SV_POSITION;
                 };

                 sampler2D _MainTex;
                 float4 _MainTex_ST;

                 float uvArray[128 * 3];
                 float3 colors[5];

                 int _hitCount = 0;
                 float pointRange[5];

                 float4 _Color0;
                 float4 _Color1;
                 float4 _Color2;
                 float4 _Color3;
                 float4 _Color4;

                 v2f vert(appdata v)
                 {
                     v2f o;
                     o.vertex = UnityObjectToClipPos(v.vertex);
                     o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                     // o.uv = v.uv;
                      return o;
                  }

                  void initialize()
                  {
                      colors[0] = _Color0;
                      colors[1] = _Color1;
                      colors[2] = _Color2;
                      colors[3] = _Color3;
                      colors[4] = _Color4;
                      pointRange[0] = 0;
                      pointRange[1] = 0.25;
                      pointRange[2] = 0.5;
                      pointRange[3] = 0.75;
                      pointRange[4] = 1;




                  }
                  float3 getHeat(float weight)
                  {
                      if (weight <= pointRange[0]) {
                          return colors[0];
                      }
                      if (weight >= pointRange[4]) {
                          return colors[4];
                      }

                      for (int i = 1; i < 5; i++) {
                          if (weight < pointRange[i])
                          {
                              float minimum = weight - pointRange[i - 1];
                              float pointRangeSize = pointRange[i] - pointRange[i - 1];


                              float ratio = minimum / pointRangeSize;

                              float3 color_range = colors[i] - colors[i - 1];

                              float contribution = color_range * ratio;
                              float3 newColor = colors[i - 1] + contribution;
                              return newColor;
                          }

                      }
                      return colors[0];
                  }

                  float distsq(float2 a, float2 b)
                  {
                      //float area_of_effect_size = .003f;
                  float area_of_effect_size = 0.05;
                         return  pow(max(0.0, 1.0 - distance(a, b) / area_of_effect_size), 2.0);
                     }


                     fixed4 frag(v2f i) : SV_Target
                     {
                         // sample the texture
                         fixed4 col = tex2D(_MainTex, i.uv);
                         initialize();

                         float2 uv = i.uv;
                         float totalWeight = 0.0;
                         for (float i = 0.0; i < _hitCount; i++)
                         {
                             float2 work_pt = float2(uvArray[i * 3 + 0],uvArray[i * 3 + 1]);
                             float intensity = uvArray[i * 3 + 2];
                             totalWeight += 0.5 * distsq(work_pt, uv) * intensity;
                         }

                         return col + float4(getHeat(totalWeight), .5);
                     }
                     ENDCG
                 }
         }
}
