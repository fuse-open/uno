const path = require("path")
const run = require("dotnet-run")

module.exports = function(args) {
    const filename = path.join(__dirname, "bin", "net6.0", "uno.dll")
    return run(filename, args)
}
