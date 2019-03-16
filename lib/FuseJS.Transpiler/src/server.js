var babel = require("@babel/core");
var babel_preset_env = require("@babel/preset-env");
var http = require("http");

var server = http.createServer(function(request, response) {
    if (request.method !== "POST") {
        response.writeHead(404);
        response.end();
        return;
    }

    var chunks = [];

    request.on("data", function(data) {
        chunks.push(data);
    });

    request.on("end", function() {
        try {
            var input = JSON.parse(Buffer.concat(chunks));
            if (input.quit) {
                response.end();
                process.exit(0);
            }

            var result = babel.transform(input.code, {
                filename: input.filename,
                presets: [babel_preset_env],
                sourceMaps: "inline",
            });
            response.end(JSON.stringify({
                code: result.code
            }));
        } catch (err) {
            response.end(JSON.stringify({
                message: err.message,
                codeFrame: err.codeFrame
            }));
        }
    });
})

server.listen(0, "127.0.0.1", function() {
    process.stdout.write("port:" + server.address().port + "\n");
});