#ifndef EASE_FUNCS_INCLUDED

#define EASE_FUNCS_INCLUDED

float EaseInQuad (float t) { return t*t; }
float EaseOutQuad (float t) { return t*(2-t); }
float EaseInCubic (float t) { return t*t*t; }
float EaseOutCubic (float t) { t -= 1; return t*t*t+1; }
float EaseInQuart (float t) { return t*t*t*t;}
float EaseOutQuart (float t) { t-=1; return 1-t*t*t*t; }
float EaseInQuint (float t) { return t*t*t*t*t; }
float EaseOutQuint (float t) { t-=1; return 1+t*t*t*t*t; }

#endif