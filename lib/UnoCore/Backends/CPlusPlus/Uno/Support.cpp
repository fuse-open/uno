// @(MSG_ORIGIN)
// @(MSG_EDIT_WARNING)

#include <Uno/Support.h>
#include <uBase/Buffer.h>
#include <uBase/BufferStream.h>
#include <uBase/Path.h>
#include <uImage/Bitmap.h>
#include <uImage/Jpeg.h>
#include <uImage/Png.h>
#include <uImage/Texture.h>
#include <XliPlatform/GL.h>
#include <XliPlatform/MessageBox.h>
#include <mutex>
@{byte:IncludeDirective}

#if ANDROID
#include <android/log.h>
#elif IOS
void uLogApple(const char* prefix, const char* format, va_list args);
#else
#include <cstdio>
#endif

#ifdef WIN32
#include <Uno/WinAPIHelper.h>
#else
#include <pthread.h>
#endif

static std::recursive_mutex _Critical;

// Synchronized logging
void uLogv(int level, const char* format, va_list args)
{
    U_ASSERT(uLogLevelDebug == 0 &&
             uLogLevelInformation == 1 &&
             uLogLevelWarning == 2 &&
             uLogLevelError == 3 &&
             uLogLevelFatal == 4);

    if (!format)
        format = "";

    if (level < 0)
        level = 0;
    else if (level > uLogLevelFatal)
        level = uLogLevelFatal;

#if ANDROID
    int logs[] = {
        ANDROID_LOG_DEBUG,  // uLogLevelDebug
        ANDROID_LOG_INFO,   // uLogLevelInformation
        ANDROID_LOG_WARN,   // uLogLevelWarning
        ANDROID_LOG_ERROR,  // uLogLevelError
        ANDROID_LOG_FATAL   // uLogLevelFatal
    };
    __android_log_vprint(logs[level], "@(Project.Name)", format, args);
#else
    static const char* strings[] = {
        "",             // uLogLevelDebug
        "Info: ",       // uLogLevelInformation
        "Warning: ",    // uLogLevelWarning
        "Error: ",      // uLogLevelError
        "Fatal: "       // uLogLevelFatal
    };
#if IOS
    // Defined in ObjC file to call NSLog()
    uLogApple(strings[level], format, args);
#else
    FILE* fp = level >= uLogLevelWarning
            ? stderr
            : stdout;
    _Critical.lock();
    fputs(strings[level], fp);
    vfprintf(fp, format, args);
    fputc('\n', fp);
    fflush(fp);
    _Critical.unlock();
#endif
#endif
}

void uLog(int level, const char* format, ...)
{
    va_list args;
    va_start(args, format);
    uLogv(level, format, args);
    va_end(args);
}

void uFatal(const char* src, const char* msg)
{
    uLog(uLogLevelFatal, "Runtime Error in %s: %s",
        src && strlen(src) ? src : "(unknown)",
        msg && strlen(msg) ? msg : "(no message)");
    Xli::MessageBox::Show(NULL, "The application has crashed.", "Fatal Error", Xli::DialogButtonsOK, Xli::DialogHintsError);
    abort();
}

uThreadLocal* uCreateThreadLocal(void (*destructor)(void*))
{
#ifdef WIN32
    // TODO: Handle destructor...
    return (uThreadLocal*)(intptr_t)::TlsAlloc();
#else
    pthread_key_t handle;
    if (pthread_key_create(&handle, destructor))
        U_THROW_IOE("pthread_key_create() failed");

    return (uThreadLocal*)(intptr_t)handle;
#endif
}

void uDeleteThreadLocal(uThreadLocal* handle)
{
#ifdef WIN32
    ::TlsFree((DWORD)(intptr_t)handle);
#else
    pthread_key_delete((pthread_key_t)(intptr_t)handle);
#endif
}

void uSetThreadLocal(uThreadLocal* handle, void* data)
{
#ifdef WIN32
    ::TlsSetValue((DWORD)(intptr_t)handle, data);
#else
    pthread_setspecific((pthread_key_t)(intptr_t)handle, data);
#endif
}

void* uGetThreadLocal(uThreadLocal* handle)
{
#ifdef WIN32
    return ::TlsGetValue((DWORD)(intptr_t)handle);
#else
    return pthread_getspecific((pthread_key_t)(intptr_t)handle);
#endif
}

bool uEnterCriticalIfNull(void* addr)
{
    if (*(void**)addr)
        return false;

    _Critical.lock();

    if (!*(void**)addr)
        return true;

    _Critical.unlock();
    return false;
}

void uEnterCritical()
{
    _Critical.lock();
}

void uExitCritical()
{
    _Critical.unlock();
}

uBase::String uStringToXliString(uString* ustr)
{
    uCString cstr(ustr);
    return uBase::String(cstr.Ptr, (int) cstr.Length);
}

uString* uStringFromXliString(const uBase::String& str)
{
    return uString::Utf8(str.Ptr(), str.Length());
}

@{int2} uInt2FromXliVector2i(const uBase::Vector2i& vec)
{
    return *(@{int2}*)&vec;
}

uBase::Vector2i uInt2ToXliVector2i(const @{int2}& vec)
{
    return *(uBase::Vector2i*)&vec;
}

@{float2} uFloat2FromXliVector2(const uBase::Vector2& vec)
{
    return *(@{float2}*)&vec;
}

uBase::Vector2 uFloat2ToXliVector2(const @{float2}& vec)
{
    return *(uBase::Vector2*)&vec;
}

uImage::Texture* uLoadXliTexture(const uBase::String& filename, uArray* data)
{
    uBase::String fnUpper = filename.ToUpper();
    uBase::BufferPtr buffer(data->Ptr(), data->Length(), false);
    uBase::BufferStream stream(&buffer, true, false);
    uBase::Auto<uImage::ImageReader> ir;

    if (fnUpper.EndsWith(".PNG"))
        ir = uImage::Png::CreateReader(&stream);
    else if (fnUpper.EndsWith(".JPG") || fnUpper.EndsWith(".JPEG"))
        ir = uImage::Jpeg::CreateReader(&stream);
    else
        throw uBase::Exception("Unsupported texture extension '" + uBase::Path::GetExtension(filename) + "'");

    uBase::Auto<uImage::Bitmap> bmp = ir->ReadBitmap();
    return uImage::Texture::Create(bmp);
}

static bool TryGetGLFormat(uImage::Format format, GLenum& glFormat, GLenum& glType)
{
    switch (format)
    {
    case uImage::FormatRGBA_8_8_8_8_UInt_Normalize:
        glFormat = GL_RGBA;
        glType = GL_UNSIGNED_BYTE;
        return true;

    case uImage::FormatRGB_8_8_8_UInt_Normalize:
        glFormat = GL_RGB;
        glType = GL_UNSIGNED_BYTE;
        return true;

    case uImage::FormatR_8_UInt_Normalize:
    case uImage::FormatL_8_UInt_Normalize:
        glFormat = GL_LUMINANCE;
        glType = GL_UNSIGNED_BYTE;
        return true;

    case uImage::FormatRG_8_8_UInt_Normalize:
    case uImage::FormatLA_8_8_UInt_Normalize:
        glFormat = GL_LUMINANCE_ALPHA;
        glType = GL_UNSIGNED_BYTE;
        return true;

    default:
        return false;
    }
}

unsigned int uCreateGLTexture(uImage::Texture* texData, bool generateMips, uGLTextureInfo* outInfo)
{
    GLuint texHandle;
    glGenTextures(1, &texHandle);

    int width = texData->Faces[0].MipLevels[0]->GetWidth();
    int height = texData->Faces[0].MipLevels[0]->GetHeight();
    int mipCount = texData->Faces[0].MipLevels.Length();

    GLenum texTarget =
        texData->Type == uImage::TextureTypeCube ?
            GL_TEXTURE_CUBE_MAP :
            GL_TEXTURE_2D;

    glBindTexture(texTarget, texHandle);
    glPixelStorei(GL_UNPACK_ALIGNMENT, 1);
    glPixelStorei(GL_PACK_ALIGNMENT, 1);

    for (int i = 0; i < texData->Faces.Length(); i++)
    {
        GLenum texFace =
            texTarget == GL_TEXTURE_CUBE_MAP ?
                GL_TEXTURE_CUBE_MAP_POSITIVE_X + i :
                GL_TEXTURE_2D;

        for (int j = 0; j < texData->Faces[i].MipLevels.Length(); j++)
        {
            uImage::Image* mip = texData->Faces[i].MipLevels[j];
            uBase::Auto<uImage::Bitmap> bmp = mip->ToBitmap();

            GLenum glFormat, glType;
            if (!TryGetGLFormat(bmp->GetFormat(), glFormat, glType))
                throw uBase::Exception("Unsupported texture format: " + uImage::FormatInfo::ToString(bmp->GetFormat()));

            glTexImage2D(texFace, j, glFormat, bmp->GetWidth(), bmp->GetHeight(), 0, glFormat, glType, bmp->GetPtr());
        }
    }

    if (generateMips)
    {
        glGenerateMipmap(texTarget);
        GLenum err = glGetError();

        if (err == GL_NO_ERROR)
        {
            int w = width, h = height;

            while (w > 1 || h > 1)
            {
                w /= 2;
                h /= 2;
                mipCount++;
            }
        }
    }

    if (outInfo)
    {
        outInfo->GLTarget = texTarget;
        outInfo->Width = width;
        outInfo->Height = height;
        outInfo->Depth = 1;
        outInfo->MipCount = mipCount;
    }

    return texHandle;
}

void uReverseBytes(uint8_t* ptr, size_t size)
{
    uint8_t tmp;

    switch (size)
    {
    case 2:
        tmp = ptr[0];
        ptr[0] = ptr[1];
        ptr[1] = tmp;
        break;
    case 4:
        tmp = ptr[0];
        ptr[0] = ptr[3];
        ptr[3] = tmp;
        tmp = ptr[1];
        ptr[1] = ptr[2];
        ptr[2] = tmp;
        break;
    case 8:
        tmp = ptr[0];
        ptr[0] = ptr[7];
        ptr[7] = tmp;
        tmp = ptr[1];
        ptr[1] = ptr[6];
        ptr[6] = tmp;
        tmp = ptr[2];
        ptr[2] = ptr[5];
        ptr[5] = tmp;
        tmp = ptr[3];
        ptr[3] = ptr[4];
        ptr[4] = tmp;
        break;
    default:
        U_FATAL();
    }
}
