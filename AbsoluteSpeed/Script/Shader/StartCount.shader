//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.3                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/StartCount"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
_HdrCreate_Value_1("_HdrCreate_Value_1", Range(-2, 2)) = 3.832
_Add_Fade_1("_Add_Fade_1", Range(0, 4)) = 0.682
_OutlineEmpty_Size_1("_OutlineEmpty_Size_1", Range(1, 16)) = 16.0
_OutlineEmpty_Color_1("_OutlineEmpty_Color_1", COLOR) = (1,1,1,1)
_HdrCreate_Value_2("_HdrCreate_Value_2", Range(-2, 2)) = 2.447
_Add_Fade_2("_Add_Fade_2", Range(0, 4)) = 0.425
_SpriteFade("SpriteFade", Range(0, 1)) = 1.0

// required for UI.Mask
[HideInInspector]_StencilComp("Stencil Comparison", Float) = 8
[HideInInspector]_Stencil("Stencil ID", Float) = 0
[HideInInspector]_StencilOp("Stencil Operation", Float) = 0
[HideInInspector]_StencilWriteMask("Stencil Write Mask", Float) = 255
[HideInInspector]_StencilReadMask("Stencil Read Mask", Float) = 255
[HideInInspector]_ColorMask("Color Mask", Float) = 15

}

SubShader
{

Tags {"Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off

// required for UI.Mask
Stencil
{
Ref [_Stencil]
Comp [_StencilComp]
Pass [_StencilOp]
ReadMask [_StencilReadMask]
WriteMask [_StencilWriteMask]
}

Pass
{

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

struct appdata_t{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 texcoord : TEXCOORD0;
};

struct v2f
{
float2 texcoord  : TEXCOORD0;
float4 vertex   : SV_POSITION;
float4 color    : COLOR;
};

sampler2D _MainTex;
float _SpriteFade;
float _HdrCreate_Value_1;
float _Add_Fade_1;
float _OutlineEmpty_Size_1;
float4 _OutlineEmpty_Color_1;
float _HdrCreate_Value_2;
float _Add_Fade_2;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}


float2 RotationUV(float2 uv, float rot, float posx, float posy, float speed)
{
rot=rot+(_Time*speed*360);
uv = uv - float2(posx, posy);
float angle = rot * 0.01744444;
float sinX = sin(angle);
float cosX = cos(angle);
float2x2 rotationMatrix = float2x2(cosX, -sinX, sinX, cosX);
uv = mul(uv, rotationMatrix) + float2(posx, posy);
return uv;
}
float4 OutLineEmpty(float2 uv,sampler2D source, float value, float4 color)
{

value*=0.01;
float4 mainColor = tex2D(source, uv + float2(-value, value))
+ tex2D(source, uv + float2(value, -value))
+ tex2D(source, uv + float2(value, value))
+ tex2D(source, uv - float2(value, value));

mainColor.rgb = color;
float4 addcolor = tex2D(source, uv);
if (mainColor.a > 0.40) { mainColor = color; }
if (addcolor.a > 0.40) { mainColor.a = 0; }
return mainColor;
}
float4 Threshold(float4 txt, float value)
{
float l = (txt.x + txt.y + txt.z) * 0.33;
txt.rgb = smoothstep(value, value +0.0001, l);
return txt;
}
float2 ZoomUV(float2 uv, float zoom, float posx, float posy)
{
float2 center = float2(posx, posy);
uv -= center;
uv = uv * zoom;
uv += center;
return uv;
}
float Lightning_Hash(float2 p)
{
float3 p2 = float3(p.xy, 1.0);
return frac(sin(dot(p2, float3(37.1, 61.7, 12.4)))*3758.5453123);
}

float Lightning_noise(in float2 p)
{
float2 i = floor(p);
float2 f = frac(p);
f *= f * (1.5 - .5*f);
return lerp(lerp(Lightning_Hash(i + float2(0., 0.)), Lightning_Hash(i + float2(1., 0.)), f.x),
lerp(Lightning_Hash(i + float2(0., 1.)), Lightning_Hash(i + float2(1., 1.)), f.x),
f.y);
}

float Lightning_fbm(float2 p)
{
float v = 0.0;
v += Lightning_noise(p*1.0)*.5;
v += Lightning_noise(p*2.)*.25;
v += Lightning_noise(p*4.)*.125;
v += Lightning_noise(p*8.)*.0625;
return v;
}

float4 Generate_Lightning(float2 uv, float2 uvx, float posx, float posy, float size, float number, float speed, float black)
{
uv -= float2(posx, posy);
uv *= size;
uv -= float2(posx, posy);
float rot = (uv.x*uvx.x + uv.y*uvx.y);
float time = _Time * 20 * speed;
float4 r = float4(0, 0, 0, 0);
for (int i = 1; i < number; ++i)
{
float t = abs(.750 / ((rot + Lightning_fbm(uv + (time*5.75) / float(i)))*65.));
r += t *0.5;
}
r = saturate(r);
r.a = saturate(r.r + black);
return r;

}
float4 HdrCreate(float4 txt,float value)
{
if (txt.r>0.98) txt.r=2;
if (txt.g>0.98) txt.g=2;
if (txt.b>0.98) txt.b=2;
return lerp(saturate(txt),txt, value);
}
float4 frag (v2f i) : COLOR
{
float4 _MainTex_1 = tex2D(_MainTex, i.texcoord);
float2 ZoomUV_1 = ZoomUV(i.texcoord,0.929,0.5,0.5);
float2 RotationUV_1 = RotationUV(ZoomUV_1,12.857,1.214,0.286,0);
float4 _GenerateLightning_1 = Generate_Lightning(RotationUV_1,float2(0,1),0,0.607,1.981,7.9,4.207,0);
_GenerateLightning_1.a = lerp(_MainTex_1.a, 1 - _MainTex_1.a ,0);
float4 HdrCreate_1 = HdrCreate(_GenerateLightning_1,_HdrCreate_Value_1);
_MainTex_1 = lerp(_MainTex_1,_MainTex_1*_MainTex_1.a + HdrCreate_1*HdrCreate_1.a,_Add_Fade_1);
float4 Threshold_1 = Threshold(_MainTex_1,0.5);
_MainTex_1.a = lerp(Threshold_1.a, 1 - Threshold_1.a ,0);
float4 _OutlineEmpty_1 = OutLineEmpty(i.texcoord,_MainTex,_OutlineEmpty_Size_1,_OutlineEmpty_Color_1);
float4 HdrCreate_2 = HdrCreate(_OutlineEmpty_1,_HdrCreate_Value_2);
_MainTex_1 = lerp(_MainTex_1,_MainTex_1*_MainTex_1.a + HdrCreate_2*HdrCreate_2.a,_Add_Fade_2);
float4 FinalResult = _MainTex_1;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
FinalResult.rgb *= FinalResult.a;
FinalResult.a = saturate(FinalResult.a);
return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
