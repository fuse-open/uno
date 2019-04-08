var babel = require("@babel/core");
var babel_preset_env = require("@babel/preset-env");
var babel_preset_typescript = require("@babel/preset-typescript");
var babel_plugin_proposal_class_properties = require("@babel/plugin-proposal-class-properties");
var babel_plugin_proposal_object_rest_spread = require("@babel/plugin-proposal-object-rest-spread");
var http = require("http");

function transpile(filename, code) {
    var ext = filename.split('.').pop().toLowerCase();
    
    if (ext == "ts") {
        return babel.transform(code, {
            filename: filename,
            presets: [
                babel_preset_typescript,
                babel_preset_env
            ],
            plugins: [
                babel_plugin_proposal_class_properties, 
                babel_plugin_proposal_object_rest_spread
            ],
            sourceMaps: "inline"
        });
    } else {
        return babel.transform(code, {
            filename: filename,
            presets: [
                babel_preset_env
            ],
            sourceMaps: "inline"
        });
    }
}

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

            response.end(JSON.stringify({
                code: transpile(input.filename, input.code).code
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
