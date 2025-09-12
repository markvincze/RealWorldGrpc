namespace MyCompany.CustomerService.Model.V1;

public partial class Customer
{
    public string FullName => $"{FirstName} {LastName}";
}
