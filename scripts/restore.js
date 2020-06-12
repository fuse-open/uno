const fs = require("fs");
const path = require("path");

function findup(suffix) {
    for (let dir = __dirname, parent = undefined;;
             dir = path.dirname(dir)) {

        if (dir == parent)
            throw Error(`${suffix} was not found`);

        parent = dir;
        const file = path.join(dir, suffix);

        if (fs.existsSync(file))
            return file;
    }
}

function findNodeModule(name) {
    const package = findup(`node_modules/${name}/package.json`);
    return path.dirname(package);
}

function restoreFiles(src, dst) {
    if (!fs.existsSync(dst))
        return;

    for (file of fs.readdirSync(src)) {
        const srcf = path.join(src, file);
        const dstf = path.join(dst, file);
        const placeholder = dstf + ".restore";

        if (!fs.existsSync(placeholder))
            continue;

        const relative = path.relative(process.cwd(), dstf);
        console.log(`restoring ${relative}`);
        fs.copyFileSync(srcf, dstf);
        fs.unlinkSync(placeholder);
    }
}

// Restore Xamarin.Mac binaries.
const xamarin = findNodeModule("@fuse-open/xamarin-mac");
restoreFiles(xamarin, path.join(__dirname, "..", "bin"));
restoreFiles(xamarin, path.join(__dirname, "..", "bin", "mac"));

// Restore OpenTK and ANGLE (Windows only).
const opentk = findNodeModule("@fuse-open/opentk");
restoreFiles(opentk, path.join(__dirname, "..", "bin", "win"));
