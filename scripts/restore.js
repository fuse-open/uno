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

function restoreXamarinMac(dst) {
    if (!fs.existsSync(dst))
        return;

    const src = findNodeModule("@fuse-open/xamarin-mac");

    for (file of fs.readdirSync(src)) {
        switch (file.split('.').pop()) {
            case "dll":
            case "dylib":
                break;
            default:
                continue;
        }

        const srcf = path.join(src, file);
        const dstf = path.join(dst, file);

        if (fs.existsSync(dstf))
            continue;

        const relative = path.relative(process.cwd(), dstf);
        console.log(`restoring ${relative}`);
        fs.copyFileSync(srcf, dstf);
    }
}

// Restore Xamarin.Mac binaries.
restoreXamarinMac(path.join(__dirname, "..", "bin"));
restoreXamarinMac(path.join(__dirname, "..", "bin", "mac"));
