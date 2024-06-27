#ifndef FOLIAGE_LIB_INCLUDED
#define FOLIAGE_LIB_INCLUDED

float2 GradientNoise_Dir(float2 p)
{
    // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
    p = p % 289;
    float x = (34 * p.x + 1) * p.x % 289 + p.y;
    x = (34 * x + 1) * x % 289;
    x = frac(x / 41) * 2 - 1;
    return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
}

float GradientNoise(float2 UV, float Scale)
{
    const float2 p = UV * Scale;
    const float2 ip = floor(p);
    float2 fp = frac(p);
    const float d00 = dot(GradientNoise_Dir(ip), fp);
    const float d01 = dot(GradientNoise_Dir(ip + float2(0, 1)), fp - float2(0, 1));
    const float d10 = dot(GradientNoise_Dir(ip + float2(1, 0)), fp - float2(1, 0));
    const float d11 = dot(GradientNoise_Dir(ip + float2(1, 1)), fp - float2(1, 1));
    fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
    return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
}

inline float SineWave(float3 pos, float offset, float speed, float frequency)
{
    const float angle = offset + _WindDirection * PI;
    const float s = sin(angle);
    const float c = cos(angle);
    return sin(offset + _Time.z * speed + (pos.x * s + pos.z * c) * frequency);
}

#endif  // FOLIAGE_LIB_INCLUDED