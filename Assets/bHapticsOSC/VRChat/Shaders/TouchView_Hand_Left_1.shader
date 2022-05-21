Shader "bHapticsOSC/TouchView_Hand_Left_1"
{
	Properties
	{
		_DefaultColor("Default Color", Color) = (0, 0, 0, 0)
		_TouchColor("Touch Color", Color) = (0, 1, 1, 0.5)

		[HideInInspector] _MainTex("Albedo (RGB)", 2D) = "white" {}
		[HideInInspector] _Device("Device", Float) = 4.1
		[HideInInspector][MaterialToggle] _SingularNode("Singular Node", Int) = 1

		[HideInInspector][MaterialToggle] _Node1("Node 1", Int) = 0
		[HideInInspector][MaterialToggle] _Node2("Node 2", Int) = 0
		[HideInInspector][MaterialToggle] _Node3("Node 3", Int) = 0
		[HideInInspector][MaterialToggle] _Node4("Node 4", Int) = 0
		[HideInInspector][MaterialToggle] _Node5("Node 5", Int) = 0
		[HideInInspector][MaterialToggle] _Node6("Node 6", Int) = 0
		[HideInInspector][MaterialToggle] _Node7("Node 7", Int) = 0
		[HideInInspector][MaterialToggle] _Node8("Node 8", Int) = 0
		[HideInInspector][MaterialToggle] _Node9("Node 9", Int) = 0
		[HideInInspector][MaterialToggle] _Node10("Node 10", Int) = 0
		[HideInInspector][MaterialToggle] _Node11("Node 11", Int) = 0
		[HideInInspector][MaterialToggle] _Node12("Node 12", Int) = 0
		[HideInInspector][MaterialToggle] _Node13("Node 13", Int) = 0
		[HideInInspector][MaterialToggle] _Node14("Node 14", Int) = 0
		[HideInInspector][MaterialToggle] _Node15("Node 15", Int) = 0
		[HideInInspector][MaterialToggle] _Node16("Node 16", Int) = 0
		[HideInInspector][MaterialToggle] _Node17("Node 17", Int) = 0
		[HideInInspector][MaterialToggle] _Node18("Node 18", Int) = 0
		[HideInInspector][MaterialToggle] _Node19("Node 19", Int) = 0
		[HideInInspector][MaterialToggle] _Node20("Node 20", Int) = 0
	}

	CustomEditor "bHapticsOSC.VRChat.bTouchViewEditor"

	SubShader
	{
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }

		CGPROGRAM

		#include "TouchView.cginc"
		#pragma surface surf Standard alpha
		#pragma target 3.0

		uniform sampler2D _MainTex;
		uniform fixed _IsFlip;

		uniform float _Device;
		uniform float4 _DefaultColor;
		uniform float4 _TouchColor;
		uniform int _SingularNode;

		uniform int _Node1;
		uniform int _Node2;
		uniform int _Node3;
		uniform int _Node4;
		uniform int _Node5;
		uniform int _Node6;
		uniform int _Node7;
		uniform int _Node8;
		uniform int _Node9;
		uniform int _Node10;
		uniform int _Node11;
		uniform int _Node12;
		uniform int _Node13;
		uniform int _Node14;
		uniform int _Node15;
		uniform int _Node16;
		uniform int _Node17;
		uniform int _Node18;
		uniform int _Node19;
		uniform int _Node20;

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			TouchViewSurf(
				IN,
				o,
				_Device,
				_MainTex,
				_IsFlip,
				_SingularNode,
				_DefaultColor,
				_TouchColor,
				_Node1,
				_Node2,
				_Node3,
				_Node4,
				_Node5,
				_Node6,
				_Node7,
				_Node8,
				_Node9,
				_Node10,
				_Node11,
				_Node12,
				_Node13,
				_Node14,
				_Node15,
				_Node16,
				_Node17,
				_Node18,
				_Node19,
				_Node20);
		}

		ENDCG
	}

	FallBack "Diffuse"
}