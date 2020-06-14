// @(MSG_ORIGIN)
// @(MSG_EDIT_WARNING)

#pragma once
#include <uBase/String.h>
@{int2:IncludeDirective}
@{float2:IncludeDirective}

namespace uImage { class Texture; }
struct uString;

/**
    \addtogroup ThreadUtils
    @{
*/
struct uThreadLocal;

uThreadLocal* uCreateThreadLocal(void (*destructor)(void*));
void uDeleteThreadLocal(uThreadLocal* tls);

void uSetThreadLocal(uThreadLocal* tls, void* value);
void* uGetThreadLocal(uThreadLocal* tls);

void uEnterCritical();
bool uEnterCriticalIfNull(void* addr);
void uExitCritical();
/** @} */

/**
    \addtogroup TextureUtils
    @{
*/
struct uGLTextureInfo
{
    unsigned int GLTarget;
    int Width;
    int Height;
    int Depth;
    int MipCount;
};

uImage::Texture* uLoadXliTexture(const uBase::String& filename, uArray* data);
unsigned int uCreateGLTexture(uImage::Texture* texData, bool generateMipmap = true, uGLTextureInfo* outInfo = 0);
/** @} */
