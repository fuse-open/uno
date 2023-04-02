#!/usr/bin/env node
const path = require("path")
const run = require("dotnet-run")

run(path.join(__dirname, "uno.dll"),
    process.argv.slice(2))
    .then(process.exit)
