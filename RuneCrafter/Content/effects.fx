struct VertexToPixel
{
    float4 Position   	: POSITION;    
    float4 Color		: COLOR0;
    float3 Normal		: TEXCOORD0;
    float2 TextureCoords: TEXCOORD1;
};

struct PixelToFrame
{
    float4 Color : COLOR0;
};

//------- Constants --------
float4x4 xView;
float4x4 xProjection;
float4x4 xWorld;
float3 xLightDirection;
float xAmbient;
bool xEnableLighting;
bool xShowNormals;

//------- Texture Samplers --------

Texture xTexture;
sampler TextureSampler = sampler_state { texture = <xTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

//------- Technique: Pretransformed --------

VertexToPixel PretransformedVS( float4 inPos : POSITION, float4 inColor: COLOR)
{	
	VertexToPixel Output = (VertexToPixel)0;
	
	Output.Position = inPos;
	Output.Color = inColor;
    
	return Output;    
}

PixelToFrame PretransformedPS(VertexToPixel PSIn) 
{
	PixelToFrame Output = (PixelToFrame)0;		
	
	Output.Color = PSIn.Color;

	return Output;
}

technique Pretransformed
{
	pass Pass0
    {   
    	VertexShader = compile vs_1_1 PretransformedVS();
        PixelShader  = compile ps_1_1 PretransformedPS();
    }
}

//------- Technique: Colored --------

VertexToPixel ColoredVS( float4 inPos : POSITION, float4 inColor: COLOR, float3 inNormal: NORMAL)
{	
	VertexToPixel Output = (VertexToPixel)0;
	float4x4 preViewProjection = mul (xView, xProjection);
	float4x4 preWorldViewProjection = mul (xWorld, preViewProjection);
    
	Output.Position = mul(inPos, preWorldViewProjection);
	Output.Color = inColor;
	Output.Normal = normalize(mul(normalize(inNormal), xWorld));
    
	return Output;    
}

PixelToFrame ColoredPS(VertexToPixel PSIn) 
{
	PixelToFrame Output = (PixelToFrame)0;		
    
	float lightingFactor;
	if (xEnableLighting == 1)
	{
		lightingFactor = saturate(saturate(dot(PSIn.Normal, -xLightDirection)) + xAmbient);
	}
	else
	{
		lightingFactor = 1;
	}
	
	Output.Color = PSIn.Color*lightingFactor;
	
	if (xShowNormals == 1) Output.Color.rgb = PSIn.Normal;	

	return Output;
}

technique Colored
{
	pass Pass0
    {   
    	VertexShader = compile vs_1_1 ColoredVS();
        PixelShader  = compile ps_2_0 ColoredPS();
    }
}

//------- Technique: Textured --------

VertexToPixel TexturedVS( float4 inPos : POSITION, float3 inNormal: NORMAL, float2 inTexCoords: TEXCOORD0)
{	
	VertexToPixel Output = (VertexToPixel)0;
	float4x4 preViewProjection = mul (xView, xProjection);
	float4x4 preWorldViewProjection = mul (xWorld, preViewProjection);
    
	Output.Position = mul(inPos, preWorldViewProjection);
	Output.Normal = mul(normalize(inNormal), xWorld);
	Output.TextureCoords = inTexCoords;
    
	return Output;    
}

PixelToFrame TexturedPS(VertexToPixel PSIn) 
{
	PixelToFrame Output = (PixelToFrame)0;		
    
	float lightingFactor;
	if (xEnableLighting == 1)
	{
		lightingFactor = saturate(saturate(dot(PSIn.Normal, -xLightDirection)) + xAmbient);
	}
	else
	{
		lightingFactor = 1;
	}
	
	Output.Color = tex2D(TextureSampler, PSIn.TextureCoords)*lightingFactor;
	
	if (xShowNormals == 1) Output.Color.rgb = PSIn.Normal;

	return Output;
}

technique Textured
{
	pass Pass0
    {   
    	VertexShader = compile vs_1_1 TexturedVS();
        PixelShader  = compile ps_2_0 TexturedPS();
    }
}

//------- Technique: PointSprites --------

struct SpritesVertexToPixel
{
    float4 Position   	: POSITION;
    float4 Color    	: COLOR0;
    float1 Size 		: PSIZE;
};

SpritesVertexToPixel PointSpritesVS (float4 Position : POSITION, float4 Color : COLOR0, float1 Size : PSIZE)
{
    SpritesVertexToPixel Output = (SpritesVertexToPixel)0;
     
    float4x4 preViewProjection = mul (xView, xProjection);
	float4x4 preWorldViewProjection = mul (xWorld, preViewProjection); 
    Output.Position = mul(Position, preWorldViewProjection);    
    Output.Color = Color;
    Output.Size = 1/(pow(Output.Position.z,2)+1	) * Size;
    
    return Output;    
}

PixelToFrame PointSpritesPS(SpritesVertexToPixel PSIn, float2 TexCoords  : TEXCOORD0)
{ 
    PixelToFrame Output = (PixelToFrame)0;

    Output.Color = tex2D(TextureSampler, TexCoords)*PSIn.Color;
    
    return Output;
}

technique PointSprites
{
	pass Pass0
    {   
    	PointSpriteEnable = true;
    	VertexShader = compile vs_1_1 PointSpritesVS();
        PixelShader  = compile ps_2_0 PointSpritesPS();
    }
}