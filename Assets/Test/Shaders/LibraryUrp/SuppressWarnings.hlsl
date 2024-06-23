#ifndef DUSTYROOM_SUPPRESS_WARNINGS
#define DUSTYROOM_SUPPRESS_WARNINGS

#pragma warning (disable : 3571)  // pow(f, e) will not work for negative f, use abs(f) or conditionally handle negative values if you expect them
#pragma warning (disable : 3577)  // value cannot be infinity, isfinite() may not be necessary. 

#pragma warning (disable : 4008)  //  floating point division by zero - https://issuetracker.unity3d.com/issues/floating-point-division-by-zero-warning-is-thrown-when-a-sample-gradient-node-is-connected-to-albedo-slot

#pragma warning (disable : 3206)  //  implicit truncation of vector type

void DoesNothing_float(out float _Out) {
    _Out = 1;
}

#endif  // DUSTYROOM_SUPPRESS_WARNINGS
