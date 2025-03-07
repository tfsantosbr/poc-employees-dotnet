namespace Application.Employees.Models.Responses
{
    public record AddressResponse(
        string Street,
        string Number,
        string Complement,
        string Neighborhood,
        string City,
        string State,
        string ZipCode,
        string Country,
        bool IsMain
    );
}
