class Main {
    public Main() {
        var validExpression = 1 < 1;
        if(validExpression) {}
        while(validExpression) {}
        do{} while(validExpression);
        for(;validExpression;) {}

        var notValidExpression = "string";
        if(notValidExpression) {} // $E2047
        while(notValidExpression) {} // $E2047
        do{} while(notValidExpression); // $E2047
        for(;notValidExpression;) {} // $E2047

        var notValidExpression1 = 1;
        if(notValidExpression1) {} // $E2047
        while(notValidExpression1) {} // $E2047
        do{} while(notValidExpression1); // $E2047
        for(;notValidExpression1;) {} // $E2047

        var notValidExpression2 = this;
        if(notValidExpression2) {} // $E2047
        while(notValidExpression2) {} // $E2047
        do{} while(notValidExpression2); // $E2047
        for(;notValidExpression2;) {} // $E2047

        var notValidExpression3 = null;
        if(notValidExpression3) {} // $E2043
        while(notValidExpression3) {} // $E2043
        do{} while(notValidExpression3); // $E2043
        for(;notValidExpression3;) {} // $E2043
    }
}
