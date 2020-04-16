// @(MSG_ORIGIN)
// @(MSG_EDIT_WARNING)

#pragma once
#include <XliPlatform/GLContext.h>
#include <XliPlatform/GL.h>
#include <Uno/Support.h>

extern ::Xli::GLContext *_XliGLContextPtr;

struct uGraphicsContext
{
    static uGraphicsContext GetInstance()
    {
        return uGraphicsContext(_XliGLContextPtr);
    }

    uGraphicsContext()
    {
        this->context = nullptr;
    }

    GLuint GetBackbufferGLHandle()
    {
#if IOS
        return 1;
#else
        return 0;
#endif
    }

    @{int2} GetBackbufferSize()
    {
        return uInt2FromXliVector2i(context->GetDrawableSize());
    }

private:
    uGraphicsContext(Xli::GLContext *context)
    {
        this->context = context;
    }

    ::Xli::GLContext *context;
};
