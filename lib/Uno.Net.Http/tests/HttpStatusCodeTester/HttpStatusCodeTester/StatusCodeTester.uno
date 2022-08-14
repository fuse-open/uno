using Uno;
using Uno.Collections;
using Fuse;
using Fuse.Controls;

namespace HttpStatusCodeTester
{
    public class StatusCodeTester : GridData
    {
        int[] codes = new []
        {
            0,
            100, 101, 102,
            200, 201, 202, 203, 204, 205, 206, 207, 208, 226,
            300, 301, 302, 303, 304, 305, 307, 308,
            400, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410, 411, 412, 413, 414, 415, 416, 417, 422, 423, 424, 426, 428, 429, 431,
            500, 501, 502, 503, 504, 505, 506, 507, 508, 510, 511
        };

        public StatusCodeTester()
        {
            foreach(var code in codes)
                Items.Add(new HttpMessage(code));

            Run();
        }

        public void Run()
        {
            foreach(var item in Items)
                (item as HttpMessage).Run();
        }
    }
}