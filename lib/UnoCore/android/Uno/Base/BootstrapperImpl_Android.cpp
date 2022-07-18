#include <BootstrapperImpl_Android.h>
#include <@{Android.Base.JNI:Include}>
#include <@{Android.Base.Types.Bridge:Include}>
@(Android.Bindings.Bootstrapper.Include:Join())
void BootstrapperImpl() {
    @(Android.Bindings.Bootstrapper.Statement:Join())
}
