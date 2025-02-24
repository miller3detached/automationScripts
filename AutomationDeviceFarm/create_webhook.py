import asana
from asana.rest import ApiException


configuration = asana.Configuration()
configuration.access_token = "1/1"
api_client = asana.ApiClient(configuration)
webhooks_api = asana.WebhooksApi(api_client)


resource_id = "1"
target_endpoint = "a/asana-webhook-endpoint"

try:
    new_webhook = webhooks_api.create_webhook({
        "resource": resource_id,
        "target": target_endpoint
    })
except ApiException as e:
    print("Ошибка при создании вебхука:", e)
