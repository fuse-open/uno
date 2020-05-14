#!/usr/bin/env node
const path = require('path');
const run = require('dotnet-run');

run(path.join(__dirname, 'uno.exe'),
    process.argv.slice(2),
    process.exit);
