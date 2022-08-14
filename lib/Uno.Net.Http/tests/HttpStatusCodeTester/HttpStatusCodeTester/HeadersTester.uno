using Uno;
using Uno.Collections;
using Fuse;
using Fuse.Controls;

namespace HttpStatusCodeTester
{
    public class HeaderTester : GridData
    {
        IDictionary<string, string> _generalHeaders = new Dictionary<string, string>
        {
            { "Cache-Control", "" },
            { "Connection", "" },
            { "Date", "" },
            { "Pragma", "" },
            { "Trailer", "" },
            { "Transfer-Encoding", "" },
            { "Upgrade", "" },
            { "Via", "" },
            { "Warning", "" }
        };
        
        IDictionary<string, string> _requestHeaders = new Dictionary<string, string>
        {
            { "Accept", "" },
            { "Accept-Charset", "" },
            { "Accept-Encoding", "" },
            { "Accept-Language", "" },
            { "Authorization", "" },
            { "Expect", "" },
            { "From", "" },
            { "Host", "" },
            { "If-Match", "" },
            { "If-Modified-Since", "" },
            { "If-None-Match", "" },
            { "If-Range", "" },
            { "If-Unmodified-Since", "" },
            { "Max-Forwards", "" },
            { "Proxy-Authorization", "" },
            { "Range", "" },
            { "Referer", "" },
            { "TE", "" },
            { "User-Agent", "" }
        };
        
        IDictionary<string, string> _responseHeaders = new Dictionary<string, string>
        {
            { "Accept-Ranges", "" },
            { "Age", "" },
            { "ETag", "" },
            { "Location", "" },
            { "Proxy-Authenticate", "" },
            { "Retry-After", "" },
            { "Server", "" },
            { "Vary", "" },
            { "WWW-Authenticate", "" }
        };
        
        IDictionary<string, string> _entityHeaders = new Dictionary<string, string>
        {
            { "Allow", "" },
            { "Content-Encoding", "" },
            { "Content-Language", "" },
            { "Content-Length", "" },
            { "Content-Location", "" },
            { "Content-MD5", "" },
            { "Content-Range", "" },
            { "Content-Type", "" },
            { "Expires", "" },
            { "Last-Modified", "" }
        };
        
        public HeaderTester()
        {
            foreach(var header in _generalHeaders)
                Items.Add(new HeaderTest(header.Key, header.Value));
            
            foreach(var header in _requestHeaders)
                Items.Add(new HeaderTest(header.Key, header.Value));
            
            foreach(var header in _entityHeaders)
                Items.Add(new HeaderTest(header.Key, header.Value));

            Run();
        }

        public void Run()
        {
            foreach(var item in Items)
                (item as HeaderTest).Run();
        }
    }
    
    public class HeaderTest : Grid
    {
        readonly string _key;
        readonly string _value;
        readonly HttpWrapper _client;
        readonly Cell _state;
        readonly Cell _status;
        readonly Cell _event;
        readonly Cell _header;
        readonly Cell _body;
        
        public HeaderTest(string key, string value)
        {
            _key = key;
            _value = value;
            
            _client = new HttpWrapper(Callback);
            
            _state = new Cell("State");
            _status = new Cell("Status");
            _event = new Cell("Event");
            
            _header = new Cell("Header");
            _header.Text = _key;
            _body = new Cell("Body");
            
            this.RowCount = 1;
            this.ColumnCount = 5;
            
            this.Children.Add(_header);
            this.Children.Add(_status);
            this.Children.Add(_state);
            this.Children.Add(_event);
            this.Children.Add(_body);
        }
        
        void Callback(HttpEvent data)
        {
            _status.Text = data.StatusCode;
            
            if("200" == data.StatusCode)
                _status.Color = float4(0.5f, 1f, 0.5f, 1f);
            else
                _status.Color = float4(1f, 0.5f, 0.5f, 1f);

            _state.Text = data.State;
            _event.Text = data.Type;
            //this.headers.Text = data.Headers;
            
            _body.Text = (data.Exception != null) ? data.Exception : "false";
            
            if(data.Content != null) 
            {
                var list = data.Content.Split(',');
                foreach(string it in list) 
                {
                    if(it.Trim() == _key)
                    {
                        _body.Text = "true";
                        break;
                    }
                }
            }
        }
    
        public void Run()
        {
            _client.Run("http://httpbin.org/response-headers?" + _key + "=" + _value);
        }
    }
}
