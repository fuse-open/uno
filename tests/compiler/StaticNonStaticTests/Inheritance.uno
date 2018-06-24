static class Bar {}

static class Foo : Bar // $E3015
{}

class Bar2 {}
static class Foo2 : Bar2 // $E3015
{}

static class Foo3 : int // $E3018
{}