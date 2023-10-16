using Blackbird.Applications.Sdk.Common;

namespace Apps.Storyblok.Models.Entities;

public class UserEntity
{
    [Display("User ID")] public string Id { get; set; }

    [Display("Name")] public string FriendlyName { get; set; }
}