#ifndef UNIVERSAL_LIT_DISSOLVE_INPUT_INCLUDED
#define UNIVERSAL_LIT_DISSOLVE_INPUT_INCLUDED

#define InitializeStandardLitSurfaceData InitializeStandardLitSurfaceData_Base
#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
#undef InitializeStandardLitSurfaceData

#include "LitDissolve.hlsl"

inline void InitializeStandardLitSurfaceData(float2 uv, out SurfaceData outSurfaceData)
{
    InitializeStandardLitSurfaceData_Base(uv, outSurfaceData);
    ApplyDissolve(uv, outSurfaceData.emission);
}

#endif
