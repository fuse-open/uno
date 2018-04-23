#if !@(Debug:Defined)
try {
#endif

if ($SetupWebGL(document.body)) {
    $PreloadImages([@(Main.BundleImages)]);
    $PreloadFonts([@(Main.BundleFonts)]);
    $PreloadSounds([@(Main.BundleSounds)]);
    $PreloadBuffers([@(Main.BundleBuffers)]);
    $PopulateClasses();
    $Initialize(function() {
        @(Main.StartupCode)
    });
}

#if !@(Debug:Defined)
}
catch (e) {
    alert("There was an error on this page.\n\nError: " + e.message + "\n\n");
}
#endif
