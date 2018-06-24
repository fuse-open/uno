class Main
{
    static void Main() // $E 'Main': Member names cannot be the same as their enclosing types
    {
        Employee.PreparePayroll();
    }
}

class Person
{
    protected virtual void CalculatePay() {}
}

class Manager : Person
{
    protected override void CalculatePay() {}
}

class Employee : Person
{
    public static void PreparePayroll()
    {
        var emp1 = new Employee();
        Person emp2 = new Manager();
        Person emp3 = new Employee();

        // The compiler cannot determine at compile time what the run-time types of
        // emp2 and emp3 will be.
        emp1.CalculatePay();
        emp2.CalculatePay(); // $E [Ignore] Cannot access protected member Person.CalculatePay() via a qualifier of type 'Person'; the qualifier must be of type 'Employee' (or derived from it)
        emp3.CalculatePay(); // $E [Ignore] Cannot access protected member Person.CalculatePay() via a qualifier of type 'Person'; the qualifier must be of type 'Employee' (or derived from it)
    }
}
