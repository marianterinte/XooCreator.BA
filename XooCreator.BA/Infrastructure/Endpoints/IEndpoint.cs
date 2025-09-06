namespace XooCreator.BA.Infrastructure.Endpoints;

public interface IEndpointDefinition
{
    void Register(IEndpointRouteBuilder app);
}
