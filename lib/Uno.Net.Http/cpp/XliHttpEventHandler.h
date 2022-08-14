#ifndef __UNO_XLI_HTTP_EVENT_HANDLER_H__
#define __UNO_XLI_HTTP_EVENT_HANDLER_H__

#include <XliHttpClient/HttpClient.h>
#include <@{Uno.Net.Http.HttpMessageHandlerRequest:Include}>

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
                @{Uno.Net.Http.HttpMessageHandlerRequest:Of(unoRequest).OnDone():Call()};
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
                @{Uno.Net.Http.HttpMessageHandlerRequest:Of(unoRequest).OnHeadersReceived():Call()};
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
        @{Uno.Net.Http.HttpMessageHandlerRequest:Of(unoRequest).OnProgress(int,int,bool):Call(position, total, totalKnown)};
    }

    virtual void OnRequestAborted(Xli::HttpRequest* request)
    {
        @{Uno.Net.Http.HttpMessageHandlerRequest} unoRequest
            = GetUnoRequest(request);

        if (unoRequest == nullptr)
            return;

        uAutoReleasePool pool;
        @{Uno.Net.Http.HttpMessageHandlerRequest:Of(unoRequest).OnAborted():Call()};
        CompleteUnoRequest(request);
    }

    virtual void OnRequestTimeout(Xli::HttpRequest* request)
    {
        @{Uno.Net.Http.HttpMessageHandlerRequest} unoRequest
            = GetUnoRequest(request);

        if (unoRequest == nullptr)
            return;

        uAutoReleasePool pool;
        @{Uno.Net.Http.HttpMessageHandlerRequest:Of(unoRequest).OnTimeout():Call()};
        CompleteUnoRequest(request);
    }

    virtual void OnRequestError(Xli::HttpRequest* request, uBase::String message)
    {
        @{Uno.Net.Http.HttpMessageHandlerRequest} unoRequest
            = GetUnoRequest(request);

        if (unoRequest == nullptr)
            return;

        uAutoReleasePool pool;
        @{Uno.Net.Http.HttpMessageHandlerRequest:Of(unoRequest).OnError(string):Call(uString::Utf8(message.Ptr()))};
        CompleteUnoRequest(request);
    }
};

#endif
