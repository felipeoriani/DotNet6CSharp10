namespace Features.Clients;

internal class ApiClient
{
	private const string BaseUrl = "https://localhost";
	private const string CustomerEndpoint = "customer";
	private const string GetCustomer = $"{BaseUrl}/{CustomerEndpoint}";
	private const string CreateCustomer = $"{BaseUrl}/{CustomerEndpoint}";

	public object Get() => new object();
	public object Post() => new object();
}

