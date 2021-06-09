struct structtest
{
	float3 position;
	float radius;
};

float testfunction(float input : SEMANTIC, structtest test)
{
	return input * test.radius;
}

