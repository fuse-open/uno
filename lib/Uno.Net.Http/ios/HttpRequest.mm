#include <Uno/Uno.h>

#include <@{Uno.Byte:Include}>
#include <@{Uno.String:Include}>
#include <@{Uno.Net.Http.HttpMessageHandlerRequest:Include}>

#include <Foundation/Foundation.h>

#include "HttpRequest.h"

namespace Uno {
    namespace Net {
    namespace Http {
    namespace iOS {

namespace {

    NSString *uString2NSString(uString *str)
    {
        if (str == nullptr)
            return nil;

        return [NSString stringWithCharacters:(const unichar *)str->_ptr length:str->_length];
    }

    uString *NSString2uString(NSString *str)
    {
        if (str == nil)
            return nullptr;

        NSUInteger length = str.length;

        uString *result = uString::New((int) length);
        [str getCharacters:(unichar *)result->_ptr range:(NSRange){ 0, length }];

        return result;
    }

} // <anonymous> namespace


static NSURLCache *sharedCache = nil;
static NSURLSession *sharedSession = nil;

void SetupSharedCache(bool isCacheEnabled, size_t sizeInBytes)
{
    sharedCache = nil;
    sharedSession = nil;

    if (isCacheEnabled && sizeInBytes == 0)
        return;

    if (isCacheEnabled)
    {
        sharedCache = [[NSURLCache alloc] initWithMemoryCapacity:4 * 1024 * 1024
            diskCapacity:sizeInBytes diskPath:@"UnoCache"];
    }

    NSURLSessionConfiguration *config
        = [NSURLSessionConfiguration defaultSessionConfiguration];
    config.URLCache = sharedCache;

    sharedSession = [NSURLSession sessionWithConfiguration:config];
}

void PurgeSharedCache()
{
    NSURLCache *cache = sharedCache;
    if (!cache)
        cache = [NSURLCache sharedURLCache];

    [cache removeAllCachedResponses];
}


struct HttpRequest::Private
{
    Private(const HttpRequest *r) : this_(const_cast<HttpRequest *>(r)) {}

    NSMutableURLRequest *request() const
    {
        if (![this_->requestTaskOrResponse_
                isKindOfClass:[NSMutableURLRequest class]])
            return nil;
        return (NSMutableURLRequest *) this_->requestTaskOrResponse_;
    }

    NSURLSessionDataTask *task() const
    {
        if (![this_->requestTaskOrResponse_
                isKindOfClass:[NSURLSessionDataTask class]])
            return nil;
        return (NSURLSessionDataTask *) this_->requestTaskOrResponse_;
    }

    NSHTTPURLResponse *response() const
    {
        if (![this_->requestTaskOrResponse_
                isKindOfClass:[NSHTTPURLResponse class]])
            return nil;
        return (NSHTTPURLResponse *) this_->requestTaskOrResponse_;
    }

    void Abort()
    {
        [task() cancel];

        uAutoReleasePool pool;
        @{Uno.Net.Http.HttpMessageHandlerRequest:Of(this_->unoRequest_).OnAborted():Call()};
    }

    void Completed(NSData *data, NSHTTPURLResponse *response, NSError *error)
    {
        this_->requestTaskOrResponse_ = response;

        uAutoReleasePool pool;

        if (data && data.length)
        {
            switch (@{Uno.Net.Http.HttpMessageHandlerRequest:Of(this_->unoRequest_).HttpResponseType:Get()})
            {
                case @{Uno.Net.Http.HttpResponseType.String}:       // String
                    this_->responseContent_ = uString::Utf8(
                        (const char *) data.bytes, (int) data.length);
                    break;

                case @{Uno.Net.Http.HttpResponseType.ByteArray}:    // ByteArray
                    this_->responseContent_ = uArray::New(
                        @{byte:TypeOf}->Array(), (int) data.length, data.bytes);
                    break;

                default:
                    break;
            }
        }

        if (error)
        {
            if (error.code == NSURLErrorTimedOut
                    && [error.domain isEqualToString:NSURLErrorDomain])
            {
                @{Uno.Net.Http.HttpMessageHandlerRequest:Of(this_->unoRequest_).OnTimeout():Call()};
            }
            else
            {
                uString *message = NSString2uString(error.localizedDescription);
                @{Uno.Net.Http.HttpMessageHandlerRequest:Of(this_->unoRequest_).OnError(string):Call(message)};
            }
        }
        else
        {
            @{Uno.Net.Http.HttpMessageHandlerRequest:Of(this_->unoRequest_).OnDone():Call()};
        }
    }

    HttpRequest *this_;
};

HttpRequest::HttpRequest(
        @{Uno.Net.Http.HttpMessageHandlerRequest} unoRequest,
        uString *method, uString *url)
    : unoRequest_(unoRequest)
    , requestTaskOrResponse_(nil)
{
    NSMutableURLRequest *request = [[NSMutableURLRequest alloc]
        initWithURL:[NSURL
            URLWithString:uString2NSString(url)]];
    request.HTTPMethod = uString2NSString(method);

    requestTaskOrResponse_ = request;
}

void HttpRequest::SetTimeout(int ms)
{
    Private(this).request().timeoutInterval = ms / 1000.;
}

void HttpRequest::SetCacheEnabled(bool isCacheEnabled)
{
    Private(this).request().cachePolicy = isCacheEnabled
        ? NSURLRequestUseProtocolCachePolicy
        : NSURLRequestReloadIgnoringLocalCacheData;
}

void HttpRequest::SetHeader(uString *key, uString *value)
{
    NSString *headerField = uString2NSString(key);
    NSString *headerContent = uString2NSString(value);

    if ([@"Range" caseInsensitiveCompare:headerField])
    {
        // Caching is broken for HTTP Range requests
        SetCacheEnabled(false);
    }

    [Private(this).request() addValue:headerContent
        forHTTPHeaderField:headerField];
}

void HttpRequest::SendAsync(uString *content)
{
    uCString data(content);
    SendAsync(data.Ptr, data.Length);
}

void HttpRequest::SendAsync(const void *data, size_t length)
{
    if (data && length)
    {
        Private(this).request().HTTPBody
            = [NSData dataWithBytes:data length:length];
    }

    NSURLSession *session = sharedSession;
    if (!session)
        session = [NSURLSession sharedSession];

    NSURLSessionDataTask *task = [session
        dataTaskWithRequest:Private(this).request()
        completionHandler:^void (NSData *d, NSURLResponse *r, NSError *e)
        {
            dispatch_async(dispatch_get_main_queue(), ^{
                Private(this).Completed(d, (NSHTTPURLResponse *) r, e);
            });
        }];

    requestTaskOrResponse_ = task;
    [task resume];
}

void HttpRequest::Abort()
{
    Private(this).Abort();
}

int HttpRequest::GetResponseStatus() const
{
    return (int) Private(this).response().statusCode;
}

uString *HttpRequest::GetResponseHeader(uString *key) const
{
    NSString *value = [Private(this).response().allHeaderFields
        objectForKey:uString2NSString(key)];
    return NSString2uString(value);
}

uString *HttpRequest::GetResponseHeaders() const
{
    NSDictionary *headers = Private(this).response().allHeaderFields;

    __block size_t resultLength = 0;
    [headers enumerateKeysAndObjectsUsingBlock:
            ^(NSString *key, NSString *obj, BOOL *stop)
    {
        resultLength = resultLength + key.length + obj.length + 2;
    }];

    if (resultLength == 0)
        return nullptr;

    uString *result = uString::New((int) (resultLength - 1));
    __block char16_t *ptr = result->_ptr;
    char16_t *ptrEnd = result->_ptr + result->_length;

    [headers enumerateKeysAndObjectsUsingBlock:
            ^(NSString *key, NSString *obj, BOOL *stop)
    {
        assert(ptrEnd > ptr);

        NSUInteger keyLength = key.length;
        assert(ptrEnd - ptr > keyLength);

        [key getCharacters:(unichar *)ptr range:(NSRange){ 0, keyLength }];
        ptr += keyLength;

        *ptr++ = (char16_t) ':';

        NSUInteger objLength = obj.length;
        assert(ptrEnd - ptr >= objLength);

        [obj getCharacters:(unichar *)ptr range:(NSRange){ 0, objLength }];
        ptr += objLength;

        // NOTE: Overwrites terminating NULL on last iteration
        *ptr++ = (char16_t) '\n';
    }];

    assert(--ptr == ptrEnd);
    *ptr = '\0';

    return result;
}

uString *HttpRequest::GetResponseContentString() const
{
    return uAs< uString *>(
        (uObject *&)responseContent_, @{string:TypeOf});
}

uArray *HttpRequest::GetResponseContentByteArray() const
{
    return uAs< uArray *>(
        (uObject *&)responseContent_, @{byte[]:TypeOf});
}

}}}} // namespace Uno::Net::Http::Implementation::iOS
