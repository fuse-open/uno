#ifndef __UNO_XLI_HTTP_EVENT_HANDLER_H__
#define __UNO_XLI_HTTP_EVENT_HANDLER_H__

#include <XliHttpClient/HttpClient.h>
#include <@{Uno.Net.Http.HttpMessageHandlerRequest:include}>

class uXliHttpEventHandler: public Xli::HttpEventHandler
{
    static @{Uno.Net.Http.HttpMessageHandlerRequest} GetUnoRequest(
        Xli::HttpRequest *request)
    {
        return (@{Uno.Net.Http.HttpMessageHandlerRequest})
            request->GetUserData();
    }

    static void CompleteUnoRequest(Xli::HttpRequest *request)
    {
        request->SetUserData(nullptr);
    }

public:
    virtual void OnRequestStateChanged(Xli::HttpRequest* request)
    {
        switch (request->GetState())
        {
            case Xli::HttpRequestStateDone:
            {
                @{Uno.Net.Http.HttpMessageHandlerRequest} unoRequest
                    = GetUnoRequest(request);

                if (unoRequest == nullptr)
                    return;

                uAutoReleasePool pool;
                @{Uno.Net.Http.HttpMessageHandlerRequest:of(unoRequest).OnDone():call()};
                CompleteUnoRequest(request);
                break;
            }

            case Xli::HttpRequestStateHeadersReceived:
            {
                @{Uno.Net.Http.HttpMessageHandlerRequest} unoRequest
                    = GetUnoRequest(request);

                if (unoRequest == nullptr)
                    return;

                uAutoReleasePool pool;
                @{Uno.Net.Http.HttpMessageHandlerRequest:of(unoRequest).OnHeadersReceived():call()};
                break;
            }

            default:
                break;
        }
    }

    virtual void OnRequestProgress(
        Xli::HttpRequest* request, int position, int total, bool totalKnown)
    {
        @{Uno.Net.Http.HttpMessageHandlerRequest} unoRequest
            = GetUnoRequest(request);

        if (unoRequest == nullptr)
            return;

        uAutoReleasePool pool;
        @{Uno.Net.Http.HttpMessageHandlerRequest:of(unoRequest).OnProgress(int,int,bool):call(position, total, totalKnown)};
    }

    virtual void OnRequestAborted(Xli::HttpRequest* request)
    {
        @{Uno.Net.Http.HttpMessageHandlerRequest} unoRequest
            = GetUnoRequest(request);

        if (unoRequest == nullptr)
            return;

        uAutoReleasePool pool;
        @{Uno.Net.Http.HttpMessageHandlerRequest:of(unoRequest).OnAborted():call()};
        CompleteUnoRequest(request);
    }

    virtual void OnRequestTimeout(Xli::HttpRequest* request)
    {
        @{Uno.Net.Http.HttpMessageHandlerRequest} unoRequest
            = GetUnoRequest(request);

        if (unoRequest == nullptr)
            return;

        uAutoReleasePool pool;
        @{Uno.Net.Http.HttpMessageHandlerRequest:of(unoRequest).OnTimeout():call()};
        CompleteUnoRequest(request);
    }

    virtual void OnRequestError(Xli::HttpRequest* request, uBase::String message)
    {
        @{Uno.Net.Http.HttpMessageHandlerRequest} unoRequest
            = GetUnoRequest(request);

        if (unoRequest == nullptr)
            return;

        uAutoReleasePool pool;
        @{Uno.Net.Http.HttpMessageHandlerRequest:of(unoRequest).OnError(string):call(uString::Utf8(message.Ptr()))};
        CompleteUnoRequest(request);
    }
};

#endif
