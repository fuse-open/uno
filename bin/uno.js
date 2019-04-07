#!/usr/bin/env node
const path = require('path');
const {spawn} = require('child_process');

function uno(args) {
    const filename = path.join(__dirname, 'uno');
    const options = {stdio: 'inherit'};

    if (path.sep == '\\')
        return spawn(filename + '.exe', args, options);
    else {
        args.unshift(filename);
        return spawn('bash', args, options);
    }
}

uno(process.argv.slice(2)).on('exit', function(code) {
    process.exit(code);
});
