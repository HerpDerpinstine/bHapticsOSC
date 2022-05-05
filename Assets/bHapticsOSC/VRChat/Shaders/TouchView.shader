Shader "bHapticsOSC/TouchView"
{
	Properties
	{
		_MainTex("Albedo (RGB)", 2D) = "white" {}

		_Device("Device", Float) = 0
		_DefaultColor("Default Color", Color) = (0,0,0,0)
		_TouchColor("Touch Color", Color) = (0,1,1,0.5)
		//_AudioLink("AudioLink", Int) = 0
		[MaterialToggle] _SingularNode("Singular Node", Int) = 0
		
		[MaterialToggle] _Node1("Node 1", Int) = 0
		[MaterialToggle] _Node2("Node 2", Int) = 0
		[MaterialToggle] _Node3("Node 3", Int) = 0
		[MaterialToggle] _Node4("Node 4", Int) = 0
		[MaterialToggle] _Node5("Node 5", Int) = 0
		[MaterialToggle] _Node6("Node 6", Int) = 0
		[MaterialToggle] _Node7("Node 7", Int) = 0
		[MaterialToggle] _Node8("Node 8", Int) = 0
		[MaterialToggle] _Node9("Node 9", Int) = 0
		[MaterialToggle] _Node10("Node 10", Int) = 0
		[MaterialToggle] _Node11("Node 11", Int) = 0
		[MaterialToggle] _Node12("Node 12", Int) = 0
		[MaterialToggle] _Node13("Node 13", Int) = 0
		[MaterialToggle] _Node14("Node 14", Int) = 0
		[MaterialToggle] _Node15("Node 15", Int) = 0
		[MaterialToggle] _Node16("Node 16", Int) = 0
		[MaterialToggle] _Node17("Node 17", Int) = 0
		[MaterialToggle] _Node18("Node 18", Int) = 0
		[MaterialToggle] _Node19("Node 19", Int) = 0
		[MaterialToggle] _Node20("Node 20", Int) = 0
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }

		CGPROGRAM

		#pragma surface surf Standard alpha
		#pragma target 3.0

		uniform sampler2D _MainTex;
		uniform fixed _IsFlip;

		uniform float _Device;
		uniform float4 _DefaultColor;
		uniform float4 _TouchColor;
		//uniform int _AudioLink;
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

		struct Input
		{
			float2 uv_MainTex;
		};

		int GetCurrentNodeDefault(inout Input IN)
		{
			int CurrentNode = 0;

			if (IN.uv_MainTex.y >= 0.8)
			{
				if (IN.uv_MainTex.x >= 0.75)
					CurrentNode = 4;
				else if (IN.uv_MainTex.x >= 0.50)
					CurrentNode = 3;
				else if (IN.uv_MainTex.x >= 0.25)
					CurrentNode = 2;
				else if (IN.uv_MainTex.x >= 0)
					CurrentNode = 1;
			}
			else if (IN.uv_MainTex.y >= 0.6)
			{
				if (IN.uv_MainTex.x >= 0.75)
					CurrentNode = 8;
				else if (IN.uv_MainTex.x >= 0.50)
					CurrentNode = 7;
				else if (IN.uv_MainTex.x >= 0.25)
					CurrentNode = 6;
				else if (IN.uv_MainTex.x >= 0)
					CurrentNode = 5;
			}
			else if (IN.uv_MainTex.y >= 0.4)
			{
				if (IN.uv_MainTex.x >= 0.75)
					CurrentNode = 12;
				else if (IN.uv_MainTex.x >= 0.50)
					CurrentNode = 11;
				else if (IN.uv_MainTex.x >= 0.25)
					CurrentNode = 10;
				else if (IN.uv_MainTex.x >= 0)
					CurrentNode = 9;
			}
			else if (IN.uv_MainTex.y >= 0.2)
			{
				if (IN.uv_MainTex.x >= 0.75)
					CurrentNode = 16;
				else if (IN.uv_MainTex.x >= 0.50)
					CurrentNode = 15;
				else if (IN.uv_MainTex.x >= 0.25)
					CurrentNode = 14;
				else if (IN.uv_MainTex.x >= 0)
					CurrentNode = 13;
			}
			else if (IN.uv_MainTex.y >= 0)
			{
				if (IN.uv_MainTex.x >= 0.75)
					CurrentNode = 20;
				else if (IN.uv_MainTex.x >= 0.50)
					CurrentNode = 19;
				else if (IN.uv_MainTex.x >= 0.25)
					CurrentNode = 18;
				else if (IN.uv_MainTex.x >= 0)
					CurrentNode = 17;
			}

			return CurrentNode;
		}

		int GetCurrentNodeArms(inout Input IN)
		{
			int CurrentNode = 0;

			if (IN.uv_MainTex.y >= 0.5)
			{
				if (IN.uv_MainTex.x >= 0.66)
					CurrentNode = 3;
				else if (IN.uv_MainTex.x >= 0.33)
					CurrentNode = 2;
				else if (IN.uv_MainTex.x >= 0)
					CurrentNode = 1;
			}
			else if (IN.uv_MainTex.y >= 0)
			{
				if (IN.uv_MainTex.x >= 0.66)
					CurrentNode = 6;
				else if (IN.uv_MainTex.x >= 0.33)
					CurrentNode = 5;
				else if (IN.uv_MainTex.x >= 0)
					CurrentNode = 4;
			}

			return CurrentNode;
		}

		int GetCurrentNodeFeet(inout Input IN)
		{
			int CurrentNode = 0;

			if (IN.uv_MainTex.x >= 0.66)
				CurrentNode = 3;
			else if (IN.uv_MainTex.x >= 0.33)
				CurrentNode = 2;
			else if (IN.uv_MainTex.x >= 0)
				CurrentNode = 1;
			
			return CurrentNode;
		}

		int GetCurrentNodeHead(inout Input IN)
		{
			int CurrentNode = 0;

			if (IN.uv_MainTex.x >= 0.84)
				CurrentNode = 1;
			else if (IN.uv_MainTex.x >= 0.669)
				CurrentNode = 2;
			else if (IN.uv_MainTex.x >= 0.498)
				CurrentNode = 3;
			else if (IN.uv_MainTex.x >= 0.327)
				CurrentNode = 4;
			else if (IN.uv_MainTex.x >= 0.156)
				CurrentNode = 5;
			else if (IN.uv_MainTex.x >= 0)
				CurrentNode = 6;

			return CurrentNode;
		}

		int GetCurrentNode(inout Input IN)
		{
			if (_Device == 0)
				return GetCurrentNodeHead(IN);
			else if ((_Device == 2) || (_Device == 3))
				return GetCurrentNodeArms(IN);
			else if ((_Device == 8) || (_Device == 9))
				return GetCurrentNodeFeet(IN);
			return GetCurrentNodeDefault(IN);
		}

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			float4 _MainTex_var = tex2D(_MainTex, _IsFlip);

			if (_SingularNode == 1)
			{
				if (_Node1 == 1)
				{
					o.Albedo = _TouchColor.rgb;
					o.Alpha = _TouchColor.a;
				}
				else
				{
					o.Albedo = _DefaultColor.rgb;
					o.Alpha = _DefaultColor.a;
				}
				return;
			}

			int CurrentNode = GetCurrentNode(IN);
			if (((CurrentNode == 1) && (_Node1 == 1))
				|| ((CurrentNode == 2) && (_Node2 == 1))
				|| ((CurrentNode == 3) && (_Node3 == 1))
				|| ((CurrentNode == 4) && (_Node4 == 1))
				|| ((CurrentNode == 5) && (_Node5 == 1))
				|| ((CurrentNode == 6) && (_Node6 == 1))
				|| ((CurrentNode == 7) && (_Node7 == 1))
				|| ((CurrentNode == 8) && (_Node8 == 1))
				|| ((CurrentNode == 9) && (_Node9 == 1))
				|| ((CurrentNode == 10) && (_Node10 == 1))
				|| ((CurrentNode == 11) && (_Node11 == 1))
				|| ((CurrentNode == 12) && (_Node12 == 1))
				|| ((CurrentNode == 13) && (_Node13 == 1))
				|| ((CurrentNode == 14) && (_Node14 == 1))
				|| ((CurrentNode == 15) && (_Node15 == 1))
				|| ((CurrentNode == 16) && (_Node16 == 1))
				|| ((CurrentNode == 17) && (_Node17 == 1))
				|| ((CurrentNode == 18) && (_Node18 == 1))
				|| ((CurrentNode == 19) && (_Node19 == 1))
				|| ((CurrentNode == 20) && (_Node20 == 1)))
			{
				o.Albedo = _TouchColor.rgb;
				o.Alpha = _TouchColor.a;
			}
			else
			{
				o.Albedo = _DefaultColor.rgb;
				o.Alpha = _DefaultColor.a;
			}
		}

		ENDCG
	}
	FallBack "Diffuse"
}