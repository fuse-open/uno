class Main : Uno.Application {

    public void Method() {}

    public Main() {
        Method() = 2; // $E2027
    }
}
