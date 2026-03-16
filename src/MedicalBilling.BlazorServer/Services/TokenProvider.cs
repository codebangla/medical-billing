namespace MedicalBilling.BlazorServer.Services;

public class TokenProvider
{
    private string? _accessToken;
    public string? AccessToken 
    { 
        get => _accessToken;
        set 
        {
            Console.WriteLine($"[Blazor DEBUG] TokenProvider Set: {(value == null ? "NULL" : "Value Present")}");
            _accessToken = value;
        }
    }
}
