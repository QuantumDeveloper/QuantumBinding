#pragma once
#include <stdint.h>

typedef struct mylib_context_T* mylib_context;

typedef enum mylib_mode
{
    MYLIB_MODE_FAST = 0,
    MYLIB_MODE_SAFE = 1
} mylib_mode;

typedef struct mylib_settings
{
    mylib_mode mode;
    uint32_t threads;
} mylib_settings;

typedef void (*mylib_callback)(void* userData, int32_t value);

int32_t mylib_create(const mylib_settings* settings, mylib_context* outContext);
int32_t mylib_watch(mylib_context context, mylib_callback callback, void* userData);
void mylib_release(mylib_context context);
