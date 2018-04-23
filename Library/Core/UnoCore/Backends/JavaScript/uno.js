@(Main.Namespaces)

$AsyncCount = 0;
$BundleBuffers = {};
$BundleImages = {};
$BundleAudios = {};
$PressedKeys = [];
$PressedButtons = [];

function $PreloadImages(files)
{
    for (var i = 0; i < files.length; i++) $PreloadImage(files[i]);
}

function $PreloadImage(name)
{
    $AsyncCount++;
    var url = "@(DataPrefix)" + name;

    var img = new Image();
    img.onload = function () {
        $BundleImages[name] = img;
        $AsyncCount--;
    };
    img.crossOrigin = '@(CrossOrigin)';
    img.src = url;
}

function $PreloadSounds(files)
{
    for (var i = 0; i < files.length; i++) $PreloadSound(files[i]);
}

function $PreloadSound(name)
{
    $AsyncCount++;
    var url = "@(DataPrefix)" + name;

    var audio = new Audio();
    audio.controls = false;
    audio.autoplay = false;
    audio.src = url;
    $BundleAudios[name] = audio;
    $AsyncCount--;
}

function $PreloadFonts(files)
{
    for (var i = 0; i < files.length; i++) $PreloadFont(files[i]);
}

function $PreloadFont(name)
{
    $AsyncCount++;
    var url = "@(DataPrefix)" + name;

    if(document.fonts && (typeof FontFace != "undefined")) {
        function success(fontFace) {
            document.fonts.add(fontFace);
            $AsyncCount--;
        }
        function error(e) {
            console.log("Could not load fontface " + name);
        }
        function loadFont() {
            var face = new FontFace(name, 'url(' + url + ')', {});
            face.loaded.then(success, error);
            face.load();
        }
        setTimeout(loadFont, 4); // NOTE: I have no clue why I need to do this async, but it randomly doesn't work if I don't
    } else {
        var d = document,
            s = d.createElement('style'),
            n = d.createElement('span'),
            f = "'" + name + "'";

        n.innerHTML = 'giItT1WQy@!-/#';
        n.style.position      = 'absolute';
        n.style.left          = '-10000px';
        n.style.top           = '-10000px';
        n.style.fontSize      = '300px';
        n.style.fontFamily    = f;
        n.style.fontVariant   = 'normal';
        n.style.fontStyle     = 'normal';
        n.style.fontWeight    = 'normal';
        n.style.letterSpacing = '0';
        d.body.appendChild(n);

        var startWidth = n.offsetWidth,
            interval = 0;

        function checkFont() {
            if (n.offsetWidth != startWidth) {
                n.parentNode.removeChild(n);

                if (interval)
                    clearInterval(interval);

                $AsyncCount--;

                //return true;
            }
        };

        s.appendChild(d.createTextNode("@font-face{font-family:" + f + ";src:url('" + url + "')}"));
        d.head.appendChild(s);

        //if (!checkFont())
            interval = setInterval(checkFont, 50);
    }
}

function $PreloadBuffers(files)
{
    for (var i = 0; i < files.length; i++) $PreloadBuffer(files[i]);
}

function $PreloadBuffer(name)
{
    var suffix = ".ttf";
    if (name.indexOf(suffix, name.length - suffix.length) !== -1)
    {
        $PreloadFont(name);
        return;
    }

    $AsyncCount++;
    var url = "@(DataPrefix)" + name;
    var req = new XMLHttpRequest();
    req.onreadystatechange = function ()
    {
        if (req.readyState == 4)
        {
            $BundleBuffers[name] = new Uint8Array(req.response);
            $AsyncCount--;
        }
    };
    req.open("GET", url, true);
    req.responseType = "arraybuffer";
    req.send(null);
}


var UnoDataXml = {
    jsNodeTypes: {
        Element: 1,
        Attribute: 2,
        Text: 3,
        CDATA: 4,
        ProcessingInstruction: 7,
        Comment: 8,
        Document: 9,
    },

    $MapXmlNodeTypeToUnoType: function(nodeType)
    {
		switch (nodeType)
		{
			case UnoDataXml.jsNodeTypes.Document:
				return 0;
			case UnoDataXml.jsNodeTypes.Text:
				return 1;
			case UnoDataXml.jsNodeTypes.CDATA:
				return 2;
			case UnoDataXml.jsNodeTypes.Element:
				return 3;
			case UnoDataXml.jsNodeTypes.Comment:
				return 5;
			case UnoDataXml.jsNodeTypes.ProcessingInstruction:
				return 6;
			default:
				return -1;
		}
    }
}

function $ParseStringToNumeric(str, typeName, minValue, maxValue)
{
    if (str === null)
    {
        throw @{Uno.ArgumentNullException(string):New("String")};
    }
    if (!isFinite(str))
    {
        throw @{Uno.FormatException(string):New("Unable to convert string to " + typeName)};
    }

    var parsedValue = parseFloat(str);
    if (isNaN(parsedValue) ||
        ((typeName == 'int' || typeName == 'long' || typeName == 'ulong') && (parsedValue % 1 != 0 || str.indexOf('.') >= 0)))
    {
        throw @{Uno.FormatException(string):New("Unable to convert string to " + typeName)};
    }

    if (minValue > parsedValue || parsedValue > maxValue)
    {
        throw @{Uno.OverflowException(string):New("Value was either too large or too small for " + typeName)};
    }
    return parsedValue;
}

function $TryParseStringToNumeric(str, typeName, minValue, maxValue)
{
    if (str === null || !isFinite(str))
    {
        return {
            success: false,
            result: 0
        };
    }

    var parsedValue = parseFloat(str);
    if (isNaN(parsedValue) || (minValue > parsedValue || parsedValue > maxValue) ||
        ((typeName == 'int' || typeName == 'long' || typeName == 'ulong') && (parsedValue % 1 != 0 || str.indexOf('.') >= 0)))
    {
        return {
            success: false,
            result: 0
        };
    }

    return {
        success: true,
        result: parsedValue
    };
}

function $ParseStringToBool(str)
{
    if (str === null)
    {
        throw @{Uno.ArgumentNullException(string):New("String")};
    }
    if (str.trim().toLowerCase() === "true") {
        return true;
    }
    if (str.trim().toLowerCase() === "false") {
        return false;
    }
    throw @{Uno.FormatException(string):New("Unable to convert string to bool")};
}

function $Vendor(obj, mn)
{
    var m = obj[mn];
    if (m) return m;
    for(var p in {ms:0, moz:0, webkit:0, o:0})
    {
        m = obj[p + mn.charAt(0).toUpperCase() + mn.slice(1)];
        if (m !== undefined) return m;
    }
    return undefined;
}

function $GetFullscreen()
{
    return $Vendor(document, "isFullScreen");
}

function $SetFullscreen(b)
{
    if (b)
        $Vendor(document.body, "requestFullscreen").call(document.body);
    else
        $Vendor(document, "cancelFullScreen").call(document);
}

// the following is used in utf8 encode / decode!

String.FromArray = function (arr) {
        // TODO: make this work without "invalid argument type"
        //return String.fromCharCode.apply(null, arr);
        //return arr.map(function(code) { return String.fromCharCode(code);}).join("");
        var str = [];
        for (var i = 0, l = arr.length; i < l; i++) str.push(String.fromCharCode(arr[i]));
        return str.join("");
};

String.ToArray = function(str) {
        var arr = new Array(str.length);
        for (var i = 0, l = arr.length; i < l; i++) arr[i] = str.charCodeAt(i);
        return arr;
};

Number.prototype.ByteToSByte = function() {
    return this > 127 ? 127 - this : this;
}

Number.prototype.SByteToByte = function() {
    return this < 0 ? 127 - this : this;
}

function $Error(ex) {
    this.exception = ex;
    arguments[0] = @{Uno.Exception:Of(ex).Message:Get()}
    var tmp = Error.apply(this, arguments);

    tmp.name = this.name = 'Uno.Exception'
    this.message = tmp.message

    Object.defineProperty(this, 'stack', {
        get: function() {
            return tmp.stack
        }
    })

    return this
}

var IntermediateInheritor = function() {}
IntermediateInheritor.prototype = Error.prototype;
$Error.prototype = new IntermediateInheritor()

$ConvertNativeException = function(ex)
{
    if (ex instanceof TypeError)
        return @{Uno.NullReferenceException():New()};

    if (ex && ex.exception)
        return ex.exception;

    return ex;
};

$SetupWebGL = function(rootDomElement)
{
    var success = true;

    rootDomElement.appendChild((function() {
        canvas = document.createElement("canvas");
        canvas.style.position = "absolute";
        canvas.style.outline = 'none';
        canvas.setAttribute("tabindex", "0");

        var message = document.createElement("div");
        message.style.padding = "10px";

        if (!window.WebGLRenderingContext) {
            success = false;
            message.innerHTML = "Hum, your browser does not support WebGL.<br /><a href='http://get.webgl.org'>Get a new one</a>, it's easy!";
            //document.location = "http://get.webgl.org/";
            return message;
        }

        var glAttrs = {
            alpha: false
        };

        gl = canvas.getContext("webgl", glAttrs) || canvas.getContext("experimental-webgl", glAttrs);

        if (!gl)
        {
            success = false;
            message.innerHTML = "Could not initialize WebGL.<br />Don't worry, <a href='http://get.webgl.org/troubleshooting'>help is on the way</a>!";
            //document.location = "http://get.webgl.org/troubleshooting";
            return message;
        }

        // TODO: Implement way to see if this extension is used
        gl.getExtension('OES_standard_derivatives');

        return canvas;
    })());

    return success;
};


(function()
{
    var constructors = [];
    var populizers = [];

    $CreateClass = function(constructor, populizer)
    {
        constructors.push(constructor);
        populizers.push(populizer);
        return constructor;
    };

    $PopulateClasses = function()
    {
        for (i = 0; i < constructors.length; i++)
            populizers[i](constructors[i]);
    };
})();

Array.CreateId = function(id) {
    return id + (1 << 16);
};

Array.Init = function (array, elmId) {
    if (elmId == @{byte:TypeOf})
        array = new Uint8Array(array);

    array.GetType = function() {
        return Array.CreateId(elmId);
    }

    return array;
};

Array.Sized = function (size, elmId) {
    return Array.Init(new Array(size), elmId);
};

Array.Fill = function (size, cb, elmId) {
    var array = new Array(size);
    for (var i = 0; i < size; i++)
        array[i] = cb();
    return Array.Init(array, elmId);
};

Array.Structs = function (size, ctor, elmId, elmArgIds) {
    return Array.Fill(size, function(){return new ctor(elmArgIds);}, elmId);
};

Array.Zeros = function (s, elmId) {
    for (var i = 0, a = new Array(s); i < s;) a[i++] = 0;
    return Array.Init(a, elmId);
};

#if @(Debug:Defined)
@{object} = Object;
#endif


I = Object.prototype;

I.@{object.GetType()} = function() {
    return @{object:TypeOf};
};

I.$II = function() {
    return false;
};

#if !@{object():IsStripped}
@{object()} = function () {
    return new Object();
};
#endif
#if !@{object.GetHashCode():IsStripped}

//$globalHashCounter = 0;

I.@{object.GetHashCode()} = function () {

    if (this instanceof Number)
        return this | 0;

    return 0;

/*
    if (this.$hashCode != undefined)
        return this.$hashCode;

    this.$hashCode = $globalHashCounter++;
    return this.$hashCode;
*/
};

#endif
#if !@{object.Equals(object):IsStripped}

I.@{object.Equals(object)} = function (obj) {
    if (obj === undefined || obj === null)
        return false;

    if (obj.$box)
        return @{object:Of(this).Equals(object):Call(obj.$)};

    if (this.$struct) {
        for (p in this)
            if (!$EqOp(this[p], obj[p]))
                return false;

        return true;
    }

    return this == obj;
};

#endif
#if !@{object.ToString():IsStripped}

I.@{object.ToString()} = function (id) {
    return id == @{char:TypeOf}
        ? String.fromCharCode(this)
        : (this instanceof Boolean)
            ? (this==true)
                ? "True"
                : "False"
            : this.toString();
};

#endif

function $CreateRef(getFunc, setFunc, objInst, $) {
    $ = {
        G: getFunc,
        S: setFunc,
        $: objInst
    };
    return function (v) {
        return v !== undefined ? $.S(v) : $.G();
    }
}

function $CopyStruct(obj) {
    var copy = {};
    copy.prototype = obj.prototype;

    for (p in obj) {
        copy[p] = obj[p] && obj[p].$struct ?
            $CopyStruct(obj[p]) :
            obj[p];
    }

    return copy;
}

function $CreateBox(obj, id) {
    return obj === undefined || obj === null ? null :
            obj.constructor == Number || obj.constructor == Boolean ? {
#if !@{object.Equals(object):IsStripped}
                @{object.Equals(object)}: function(obj) {
                    return @{object:Of(this.$).Equals(object):Call(obj)};
                },
#endif
#if !@{object.GetHashCode():IsStripped}
                @{object.GetHashCode()}: function() {
                    return @{object:Of(this.$).GetHashCode():Call()};
                },
#endif
#if !@{object.ToString():IsStripped}
                @{object.ToString()}: function() {
                    return @{object:Of(this.$).ToString():Call(this.id)};
                },
#endif
                GetType: function() {
                    return this.id;
                },
                $box: true,
                $: obj,
                id: id
            } :
            obj.$struct ?
                $CopyStruct(obj) :
                obj;
}

function $CreateDelegate(inst, func, id) {
#if @(DEBUG:Defined)
    if (!func)
        throw @{Uno.NullReferenceException():New()};
#endif
    return {
        $: inst,
        F: func,
        P: null,
        GetType: function () {
            return id;
        },
        @{Uno.Delegate.Equals(object)}: function(obj) {
            return obj && this.GetType() == obj.GetType() && this.$ == obj.$ && this.F == obj.F;
        },
        Invoke: function () {
            var t = this, p = t.P, a = arguments;
            if (p) p.Invoke.apply(p, a);
            return t.F.apply(t.$, a);
        }
    };
};

function $EqOp(left, right) {
    return left == right || left && @{object:Of(left).Equals(object):Call(right)};
}

var $BaseTypeIds = [@(BaseTypeIds)];

function $IsOp(obj, id, objId) {
    if (obj) {

        ti = objId || obj.GetType();

        // Interface bit
        if (id & (1 << 15))
            return obj.$II(id ^ (1 << 15));

        do {
            if (ti == id) return true;
            ti = ti < $BaseTypeIds.length ? $BaseTypeIds[ti] : @{object:TypeOf};
        } while (ti);
    }

    return false;
}

function $AsOp(obj, id, objId) {
    return $IsOp(obj, id, objId) ? obj : null;
}

function $DownCast(obj, id) {
    if (!obj)
        return obj;

    if ($IsOp(obj, id))
        return obj.$box ? obj.$ : obj;

    throw @{Uno.InvalidCastException():New()};
}

function $Initialize(main) {

    if (!window.cancelAnimationFrame)
        window.cancelAnimationFrame = $Vendor(window, "cancelAnimationFrame");

    if (!window.requestAnimationFrame)
        window.requestAnimationFrame = $Vendor(window, "requestAnimationFrame");

    if (!window.requestAnimationFrame)
        window.requestAnimationFrame = function (callback) {
            var currTime = new Date().getTime();
            var timeToCall = Math.max(0, 16 - (currTime - lastTime));
            var id = window.setTimeout(function () { callback(timeToCall); },
              timeToCall);
            lastTime = currTime + timeToCall;
            return id;
        };

    window.oncontextmenu = function () {
        return false;
    }

    getMouseButton = function(e) {
        return e.which == 1
                ? @{Uno.Platform.MouseButton.Left}
                : e.which == 2
                    ? @{Uno.Platform.MouseButton.Middle}
                    : e.which == 3
                        ? @{Uno.Platform.MouseButton.Right}
                        : 0;
    }

    addEvent = window.addEventListener ? function (elem, type, method) {
            elem.addEventListener(type, method, false);
        } : function (elem, type, method) {
            elem.attachEvent('on' + type, method);
        };

/*
    Note: Methods below does not seem to be used

    removeEvent = window.removeEventListener ? function (elem, type, method) {
            elem.removeEventListener(type, method, false);
        } : function (elem, type, method) {
            elem.detachEvent('on' + type, method);
        };

    addWindowEvent = function (type, method) {
            window.addEventListener(type, method, false);
        };

    removeWindowEvent = function (type, method) {
            window.removeEventListener(type, method, false);
        };
*/

    var started = false;

    // start rendering
    function tick() {
        if (!started) {
            if ($AsyncCount > 0) { // Wait until everything in the app bundle is loaded
                window.requestAnimationFrame(tick);
                return;
            }

            started = true;
            canvas.width = window.innerWidth;
            canvas.height = window.innerHeight;

            main();

            var stopPropagationIfHandled = function(e, isHandled) {
                if (isHandled)
                    e.preventDefault();
            };

            addEvent(canvas, "touchstart", function(e) {
                var touch = e.changedTouches[0];
                @{Uno.Runtime.Implementation.Internal.Bootstrapper.OnTouchDown(Uno.Runtime.Implementation.PlatformWindowHandle,float,float,int):Call(canvas, touch.clientX, touch.clientY, touch.identifier)};
            });

            addEvent(canvas, "touchmove", function(e) {
                var touch = e.changedTouches[0];
                @{Uno.Runtime.Implementation.Internal.Bootstrapper.OnTouchMove(Uno.Runtime.Implementation.PlatformWindowHandle,float,float,int):Call(canvas, touch.clientX, touch.clientY, touch.identifier)};
            });

            addEvent(canvas, "touchend", function(e) {
                var touch = e.changedTouches[0];
                @{Uno.Runtime.Implementation.Internal.Bootstrapper.OnTouchUp(Uno.Runtime.Implementation.PlatformWindowHandle,float,float,int):Call(canvas, touch.clientX, touch.clientY, touch.identifier)};
            });

#if !@(FirefoxOS:Defined)

            addEvent(canvas, "mousedown", function(e) {
                var button = getMouseButton(e);
                $PressedButtons[button] = true;
                @{Uno.Runtime.Implementation.Internal.Bootstrapper.OnMouseDown(Uno.Runtime.Implementation.PlatformWindowHandle,int,int,Uno.Platform.MouseButton):Call(canvas, e.clientX, e.clientY, button)};
            });

            addEvent(canvas, "mousemove", function(e) {
                @{Uno.Runtime.Implementation.Internal.Bootstrapper.OnMouseMove(Uno.Runtime.Implementation.PlatformWindowHandle,int,int):Call(canvas, e.clientX, e.clientY)};
            });

            addEvent(canvas, "mouseout", function(e) {
                e = e ? e : window.event;
                var from = e.relatedTarget || e.toElement;
                if (!from || from.nodeName == "HTML") {
                    @{Uno.Runtime.Implementation.Internal.Bootstrapper.OnMouseOut(Uno.Runtime.Implementation.PlatformWindowHandle):Call(canvas)};
                }
            });

            addEvent(canvas, "mouseup", function(e) {
                var button = getMouseButton(e);
                $PressedButtons[button] = false;
                @{Uno.Runtime.Implementation.Internal.Bootstrapper.OnMouseUp(Uno.Runtime.Implementation.PlatformWindowHandle,int,int,Uno.Platform.MouseButton):Call(canvas, e.clientX, e.clientY, button)};
            });

#endif

            addEvent(canvas, "wheel", function(e) {
                var deltaX = e.deltaX;
                var deltaY = e.deltaY;

                // e.deltaMode as of 16.01.2015 is the same as our WheelDeltaMode enum.
                @{Uno.Runtime.Implementation.Internal.Bootstrapper.OnMouseWheel(Uno.Runtime.Implementation.PlatformWindowHandle,float,float,int):Call(canvas, deltaX, deltaY, e.deltaMode)};
            });

            addEvent(canvas, "keydown", function(e) {
                $PressedKeys[e.keyCode] = true;

                stopPropagationIfHandled(e,
                    @{Uno.Runtime.Implementation.Internal.Bootstrapper.OnKeyDown(Uno.Runtime.Implementation.PlatformWindowHandle,Uno.Platform.Key):Call(canvas, e.keyCode)});
            });

            addEvent(canvas, "keyup", function(e) {
                $PressedKeys[e.keyCode] = false;
                stopPropagationIfHandled(e,
                    @{Uno.Runtime.Implementation.Internal.Bootstrapper.OnKeyUp(Uno.Runtime.Implementation.PlatformWindowHandle,Uno.Platform.Key):Call(canvas, e.keyCode)});
            });

            addEvent(canvas, "keypress", function(e) {
                stopPropagationIfHandled(e,
                    @{Uno.Runtime.Implementation.Internal.Bootstrapper.OnTextInput(Uno.Runtime.Implementation.PlatformWindowHandle,string):Call(canvas, String.fromCharCode(e.keyCode || e.charCode))});
                // TODO: Probably needs an invisible input text box to recieve input string and also to handle onscreen keyboard on mobile
            });

            addEvent(window, "beforeunload", function(e) {
                var cancel = @{Uno.Runtime.Implementation.Internal.Bootstrapper.OnAppTerminating(Uno.Runtime.Implementation.PlatformWindowHandle):Call(canvas)};
                if (cancel) {
                    var m = "Are you sure you want to leave this page? Data you have entered may not be saved.";
                    (e || window.event).returnValue = m;
                    return m;
                }
            });

            addEvent(window, "resize", function(e) {
                canvas.width = window.innerWidth;
                canvas.height = window.innerHeight;
                @{Uno.Runtime.Implementation.Internal.Bootstrapper.OnWindowSizeChanged(Uno.Runtime.Implementation.PlatformWindowHandle):Call(canvas)};
            });

            addEvent(window, "focus", function(e) {
                @{Uno.Platform.CoreApp.EnterInteractive():Call()};
            });

            addEvent(window, "blur", function(e) {
                @{Uno.Platform.CoreApp.ExitInteractive():Call()};
            });

            @{Uno.Platform.CoreApp.Start():Call()};
        }

        @{Uno.Runtime.Implementation.Internal.Bootstrapper.OnUpdate():Call()};

        if (@{Uno.Application.Current:Get().NeedsRedraw:Get()})
            @{Uno.Runtime.Implementation.Internal.Bootstrapper.OnDraw():Call()};

        window.requestAnimationFrame(tick);
    }

#if !@{string(char[]):IsStripped}
    @{string(char[])} = function(arr) {
        return String.fromCharCode.apply(window, arr);
    }
#endif

#if !@{object.Equals(object,object):IsStripped}
    @{object.Equals(object,object)} = function (left, right) {
        return left == right || left && @{object:Of(left).Equals(object):Call(right)};
    }
#endif

    tick();
}
