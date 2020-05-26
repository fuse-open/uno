// @(MSG_ORIGIN)
// @(MSG_EDIT_WARNING)

#pragma once
#include <uBase/String.h>
#include <uBase/Vector2.h>
@{int2:IncludeDirective}
@{float2:IncludeDirective}
@{Uno.Buffer:ForwardDeclaration}

namespace uBase { class DataAccessor; }
namespace uBase { class Stream; }
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
    \addtogroup XliUtils
    @{
*/
uBase::String uStringToXliString(uString* ustr);
uString* uStringFromXliString(const uBase::String& str);

@{int2} uInt2FromXliVector2i(const uBase::Vector2i& vec);
uBase::Vector2i uInt2ToXliVector2i(const @{int2}& vec);

@{float2} uFloat2FromXliVector2(const uBase::Vector2& vec);
uBase::Vector2 uFloat2ToXliVector2(const @{float2}& vec);

@{Uno.Buffer} uBufferFromXliDataAccessor(const uBase::DataAccessor* data);
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
