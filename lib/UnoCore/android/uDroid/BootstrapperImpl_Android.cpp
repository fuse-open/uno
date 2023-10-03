#include <BootstrapperImpl_Android.h>
#include <@{Android.Base.JNI:include}>
#include <@{Android.Base.Types.Bridge:include}>
@(android.bindings.bootstrapper.include:join())
void BootstrapperImpl() {
    @(android.bindings.bootstrapper.statement:join())
}
