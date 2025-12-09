using Apps.Storyblok.Webhooks.Lists;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Storyblok.Base;

namespace Tests.Storyblok
{
    [TestClass]
    public class WebhookListTests : TestBase
    {
        [TestMethod]
        public async Task HandleWebhook_InvalidPayload_ThrowsException()
        {
            var webhookList = new WebhookList(InvocationContext);

            var request = new WebhookRequest { Body = "" };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                webhookList.OnWorkflowChanged(request)
            );
        }
    }
}
