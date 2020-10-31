#include <cmath>

#define DLLEXPORT extern "C" __declspec(dllexport) float

#pragma region MathF

#define PI 3.14159274f

class MathF {
public:
    static float Pow(float x, float y) {
        return std::powf(x, y);
    }
    static float Sin(float x) {
        return std::sinf(x);
    }
    static float Cos(float x) {
        return std::cosf(x);
    }
    static float Asin(float x) {
        return std::asin(x);
    }
    static float Abs(float x) {
        return std::abs(x);
    }
    static float Sqrt(float x) {
        return std::sqrt(x);
    }
private:

};

#pragma endregion

DLLEXPORT QuadIn(float t, float totaltime, float min, float max) {
    max -= min;
    t /= totaltime;
    return max * t * t + min;
}

DLLEXPORT QuadOut(float t, float totaltime, float min, float max) {
    max -= min;
    t /= totaltime;
    return -max * t * (t - 2) + min;
}

DLLEXPORT QuadInOut(float t, float totaltime, float min, float max) {
    max -= min;
    t /= totaltime / 2;
    if (t < 1) return max / 2 * t * t + min;

    t -= 1;
    return -max / 2 * (t * (t - 2) - 1) + min;
}

DLLEXPORT CubicIn(float t, float totaltime, float min, float max) {
    max -= min;
    t /= totaltime;
    return max * t * t * t + min;
}

DLLEXPORT CubicOut(float t, float totaltime, float min, float max) {
    max -= min;
    t = t / totaltime - 1;
    return max * (t * t * t + 1) + min;
}

DLLEXPORT CubicInOut(float t, float totaltime, float min, float max) {
    max -= min;
    t /= totaltime / 2;
    if (t < 1) return max / 2 * t * t * t + min;

    t -= 2;
    return max / 2 * (t * t * t + 2) + min;
}

DLLEXPORT QuartIn(float t, float totaltime, float min, float max) {
    max -= min;
    t /= totaltime;
    return max * t * t * t * t + min;
}

DLLEXPORT QuartOut(float t, float totaltime, float min, float max) {
    max -= min;
    t = t / totaltime - 1;
    return -max * (t * t * t * t - 1) + min;
}

DLLEXPORT QuartInOut(float t, float totaltime, float min, float max) {
    max -= min;
    t /= totaltime / 2;
    if (t < 1) return max / 2 * t * t * t * t + min;

    t -= 2;
    return -max / 2 * (t * t * t * t - 2) + min;
}

DLLEXPORT QuintIn(float t, float totaltime, float min, float max) {
    max -= min;
    t /= totaltime;
    return max * t * t * t * t * t + min;
}

DLLEXPORT QuintOut(float t, float totaltime, float min, float max) {
    max -= min;
    t = t / totaltime - 1;
    return max * (t * t * t * t * t + 1) + min;
}

DLLEXPORT QuintInOut(float t, float totaltime, float min, float max) {
    max -= min;
    t /= totaltime / 2;
    if (t < 1) return max / 2 * t * t * t * t * t + min;

    t -= 2;
    return max / 2 * (t * t * t * t * t + 2) + min;
}

DLLEXPORT SineIn(float t, float totaltime, float min, float max) {
    max -= min;
    return -max * MathF::Cos(t * (PI * 90 / 180) / totaltime) + max + min;
}

DLLEXPORT SineOut(float t, float totaltime, float min, float max) {
    max -= min;
    return max * MathF::Sin(t * (PI * 90 / 180) / totaltime) + min;
}

DLLEXPORT SineInOut(float t, float totaltime, float min, float max) {
    max -= min;
    return -max / 2 * (MathF::Cos(t * PI / totaltime) - 1) + min;
}

DLLEXPORT ExpIn(float t, float totaltime, float min, float max) {
    max -= min;
    return t == 0.0 ? min : max * MathF::Pow(2, 10 * (t / totaltime - 1)) + min;
}

DLLEXPORT ExpOut(float t, float totaltime, float min, float max) {
    max -= min;
    return t == totaltime ? max + min : max * (-MathF::Pow(2, -10 * t / totaltime) + 1) + min;
}

DLLEXPORT ExpInOut(float t, float totaltime, float min, float max) {
    if (t == 0.0f) return min;
    if (t == totaltime) return max;
    max -= min;
    t /= totaltime / 2;

    if (t < 1) return max / 2 * MathF::Pow(2, 10 * (t - 1)) + min;

    t -= 1;
    return max / 2 * (-MathF::Pow(2, -10 * t) + 2) + min;

}

DLLEXPORT CircIn(float t, float totaltime, float min, float max) {
    max -= min;
    t /= totaltime;
    return -max * (MathF::Sqrt(1 - t * t) - 1) + min;
}

DLLEXPORT CircOut(float t, float totaltime, float min, float max) {
    max -= min;
    t = t / totaltime - 1;
    return max * MathF::Sqrt(1 - t * t) + min;
}

DLLEXPORT CircInOut(float t, float totaltime, float min, float max) {
    max -= min;
    t /= totaltime / 2;
    if (t < 1) return -max / 2 * (MathF::Sqrt(1 - t * t) - 1) + min;

    t -= 2;
    return max / 2 * (MathF::Sqrt(1 - t * t) + 1) + min;
}

DLLEXPORT ElasticIn(float t, float totaltime, float min, float max) {
    max -= min;
    t /= totaltime;
    float p = totaltime * 0.3f;
    float a = max;

    if (t == 0) return min;
    if (t == 1) return min + max;


    float s;
    if (a < MathF::Abs(max)) {
        a = max;
        s = p / 4;
    }
    else {
        s = p / (2 * PI) * MathF::Asin(max / a);
    }

    t -= 1;
    return -(a * MathF::Pow(2, 10 * t) * MathF::Sin((t * totaltime - s) * (2 * PI) / p)) + min;
}

DLLEXPORT ElasticOut(float t, float totaltime, float min, float max) {
    max -= min;
    t /= totaltime;
    float p = totaltime * 0.3f; ;
    float a = max;

    if (t == 0) return min;
    if (t == 1) return min + max;


    float s;
    if (a < MathF::Abs(max)) {
        a = max;
        s = p / 4;
    }
    else {
        s = p / (2 * PI) * MathF::Asin(max / a);
    }

    return a * MathF::Pow(2, -10 * t) * MathF::Sin((t * totaltime - s) * (2 * PI) / p) + max + min;
}

DLLEXPORT ElasticInOut(float t, float totaltime, float min, float max) {
    max -= min;
    t /= totaltime / 2;
    float p = totaltime * (0.3f * 1.5f);
    float a = max;

    if (t == 0) return min;
    if (t == 2) return min + max;


    float s;
    if (a < MathF::Abs(max)) {
        a = max;
        s = p / 4;
    }
    else {
        s = p / (2 * PI) * MathF::Asin(max / a);
    }

    if (t < 1) {
        return -0.5f * (a * MathF::Pow(2, 10 * (t -= 1)) * MathF::Sin((t * totaltime - s) * (2 * PI) / p)) + min;
    }

    t -= 1;
    
    return a * MathF::Pow(2, -10 * t) * MathF::Sin((t * totaltime - s) * (2 * PI) / p) * 0.5f + max + min;
}

DLLEXPORT BackIn(float t, float totaltime, float min, float max) {
    float val = max - min;
    float s = (float)(val * 0.01);

    max -= min;
    t /= totaltime;
    return max * t * t * ((s + 1) * t - s) + min;
}

DLLEXPORT BackOut(float t, float totaltime, float min, float max) {
    float val = max - min;
    float s = (float)(val * 0.001);

    max -= min;
    t = t / totaltime - 1;
    return max * (t * t * ((s + 1) * t + s) + 1) + min;
}

DLLEXPORT BackInOut(float t, float totaltime, float min, float max) {
    float val = max - min;
    float s = (float)(val * 0.01);

    max -= min;
    s *= 1.525f;
    t /= totaltime / 2;
    if (t < 1) return max / 2 * (t * t * ((s + 1) * t - s)) + min;

    t -= 2;
    return max / 2 * (t * t * ((s + 1) * t + s) + 2) + min;
}

DLLEXPORT BounceOut(float t, float totaltime, float min, float max) {
    max -= min;
    t /= totaltime;

    if (t < 1.0f / 2.75f) {
        return max * (7.5625f * t * t) + min;
    }
    else if (t < 2.0f / 2.75f) {
        t -= 1.5f / 2.75f;
        return max * (7.5625f * t * t + 0.75f) + min;
    }
    else if (t < 2.5f / 2.75f) {
        t -= 2.25f / 2.75f;
        return max * (7.5625f * t * t + 0.9375f) + min;
    }
    else {
        t -= 2.625f / 2.75f;
        return max * (7.5625f * t * t + 0.984375f) + min;
    }
}

DLLEXPORT BounceIn(float t, float totaltime, float min, float max) {
    max -= min;
    return max - BounceOut(totaltime - t, totaltime, 0, max) + min;
}

DLLEXPORT BounceInOut(float t, float totaltime, float min, float max) {
    if (t < totaltime / 2) {
        return BounceIn(t * 2, totaltime, 0, max - min) * 0.5f + min;
    }
    else {
        return BounceOut(t * 2 - totaltime, totaltime, 0, max - min) * 0.5f + min + (max - min) * 0.5f;
    }
}

DLLEXPORT Linear(float t, float totaltime, float min, float max) {
    return (max - min) * t / totaltime + min;
}
DLLEXPORT None(float t, float totaltime, float min, float max) {
    return min;
}