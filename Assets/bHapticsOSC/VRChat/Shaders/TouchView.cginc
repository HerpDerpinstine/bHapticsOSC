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

int GetCurrentNode(inout Input IN, int device)
{
	if (device == 0)
		return GetCurrentNodeHead(IN);
	else if ((device == 2) || (device == 3))
		return GetCurrentNodeArms(IN);
	else if ((device == 8) || (device == 9))
		return GetCurrentNodeFeet(IN);
	return GetCurrentNodeDefault(IN);
}

void TouchViewSurf(Input IN, 
	inout SurfaceOutputStandard o, 
	int device,
	sampler2D mainTex, 
	fixed isFlip, 
	int isSingularNode,
	float4 defaultColor,
	float4 touchColor,
	int node1,
	int node2,
	int node3,
	int node4,
	int node5,
	int node6,
	int node7,
	int node8,
	int node9,
	int node10,
	int node11,
	int node12,
	int node13,
	int node14,
	int node15,
	int node16,
	int node17,
	int node18,
	int node19,
	int node20)
{
	float4 _MainTex_var = tex2D(mainTex, isFlip);

	if (isSingularNode == 1)
	{
		if ((node1 == 1)
			|| (node2 == 1)
			|| (node3 == 1))
		{
			o.Albedo = touchColor.rgb;
			o.Alpha = touchColor.a;
		}
		else
		{
			o.Albedo = defaultColor.rgb;
			o.Alpha = defaultColor.a;
		}
		return;
	}

	int CurrentNode = GetCurrentNode(IN, device);
	if (((CurrentNode == 1) && (node1 == 1))
		|| ((CurrentNode == 2) && (node2 == 1))
		|| ((CurrentNode == 3) && (node3 == 1))
		|| ((CurrentNode == 4) && (node4 == 1))
		|| ((CurrentNode == 5) && (node5 == 1))
		|| ((CurrentNode == 6) && (node6 == 1))
		|| ((CurrentNode == 7) && (node7 == 1))
		|| ((CurrentNode == 8) && (node8 == 1))
		|| ((CurrentNode == 9) && (node9 == 1))
		|| ((CurrentNode == 10) && (node10 == 1))
		|| ((CurrentNode == 11) && (node11 == 1))
		|| ((CurrentNode == 12) && (node12 == 1))
		|| ((CurrentNode == 13) && (node13 == 1))
		|| ((CurrentNode == 14) && (node14 == 1))
		|| ((CurrentNode == 15) && (node15 == 1))
		|| ((CurrentNode == 16) && (node16 == 1))
		|| ((CurrentNode == 17) && (node17 == 1))
		|| ((CurrentNode == 18) && (node18 == 1))
		|| ((CurrentNode == 19) && (node19 == 1))
		|| ((CurrentNode == 20) && (node20 == 1)))
	{
		o.Albedo = touchColor.rgb;
		o.Alpha = touchColor.a;
	}
	else
	{
		o.Albedo = defaultColor.rgb;
		o.Alpha = defaultColor.a;
	}
}