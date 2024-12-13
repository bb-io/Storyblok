using Apps.Storyblok.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace Apps.Storyblok.Connection;

public class ConnectionDefinition : IConnectionDefinition
{
    public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups => new List<ConnectionPropertyGroup>()
    {
        new()
        {
            Name = "Developer API token",
            AuthenticationType = ConnectionAuthenticationType.Undefined,
            ConnectionUsage = ConnectionUsage.Actions,
            ConnectionProperties = new List<ConnectionProperty>
            {
                new(CredsNames.ApiToken) { DisplayName = "API Key", Sensitive = true },
                new(CredsNames.LocalizationLevel) { 
                    DisplayName = "Localization level",
                    DataItems = [
                        new(CredsNames.FieldLocalizationLevel, "Field level"),
                        new(CredsNames.FolderLocalizationLevel, "Folder level")
                    ]
                }
            }
        }
    };

    public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(
        Dictionary<string, string> values)
    {
        yield return new(AuthenticationCredentialsRequestLocation.None, CredsNames.ApiToken,
            values[CredsNames.ApiToken]);
    }
}