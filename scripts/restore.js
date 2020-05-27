const fs = require("fs");
const path = require("path");
const findup = require("findup-sync");

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
