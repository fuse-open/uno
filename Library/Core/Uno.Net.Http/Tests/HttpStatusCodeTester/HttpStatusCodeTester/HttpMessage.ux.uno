using Uno;
using Uno.Collections;
using Fuse;
using Fuse.Controls;
using HttpStatusCodeTester;

public partial class HttpMessage
{
    int _statusCode;
    HttpWrapper _client;

    public HttpMessage(int statusCode)
    {
        InitializeUX();
        StatusCode = statusCode;
        _client = new HttpWrapper(Callback);
    }

    void Callback(HttpEvent data)
    {
        if(this.statusCode.Text == data.StatusCode)
            this.statusCode.Color = float4(0.5f, 1f, 0.5f, 1f);
        else
            this.statusCode.Color = float4(1f, 0.5f, 0.5f, 1f);

        this.httpState.Text = data.State;
        this.a.Text = data.Type;
        this.headers.Text = data.Headers;
        this.body.Text = data.Content;
    }

    int StatusCode
    {
        get { return _statusCode; }
        set
        {
            statusCode.Text = value.ToString();
            _statusCode = value;
        }
    }

    public void Run()
    {
        _client.Run("http://httpbin.org/status/" + _statusCode.ToString());
    }
}

