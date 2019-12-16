#include "UnityCG.cginc"
public void TestFunc(float _ScaleX, float _ScaleY, float4 position, out output)
{
	output = UnityObjectToClipPos(float4((_ScaleX * position.x *  UNITY_MATRIX_V[0].xyz) + (_ScaleY * position.y * UNITY_MATRIX_V[1].xyz), 1));
}
