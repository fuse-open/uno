var http = require("http");
var fs = require("fs")

if (process.argv.length != 4) {
    console.error("Usage: client.js <port> <filename>");
    process.exit(1);
}

var port = parseInt(process.argv[2]);
var filename = process.argv[3];

var request = http.request({
    host: "127.0.0.1",
    port: port,
    method: "POST"
}, function(response) {
    var chunks = [];

    response.on("data", function(data) {
        chunks.push(data);
    });

    response.on("end", function() {
        var result = JSON.parse(Buffer.concat(chunks));
        
        if (result.code) {
            console.log(result.code);
            return;
        }

        console.error(result.message);
        if (result.codeFrame)
            console.error(result.codeFrame);
        process.exit(1);
    });
});

fs.readFile(filename, "utf8", function(err, data) {
    if (err) {
        console.error(err);
        process.exit(1);
    }

    request.end(JSON.stringify({
        filename: filename,
        code: data
    }));
});
